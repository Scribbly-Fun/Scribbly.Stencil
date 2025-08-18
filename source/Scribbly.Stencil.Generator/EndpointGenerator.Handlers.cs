using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Groups;
using Scribbly.Stencil.Types.Attributes;

namespace Scribbly.Stencil;

public partial class EndpointGenerator
{
    private static bool HandlerSyntacticPredicate(SyntaxNode node, CancellationToken cancellation)
    {
        if (node is not MethodDeclarationSyntax method)
            return false;

        return method.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr =>
                attr.Name.ToString().Contains(GetEndpointAttribute.UsageName) ||
                attr.Name.ToString().Contains(PostEndpointAttribute.UsageName) ||
                attr.Name.ToString().Contains(PutEndpointAttribute.UsageName) ||
                attr.Name.ToString().Contains(DeleteEndpointAttribute.UsageName));
    }

    private static (INamedTypeSymbol symbol, TargetMethodCaptureContext handler)? HandlerSemanticTransform(
        GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        if (!ValidateHandlerCandidateModifiers(methodDeclaration))
            return null;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return null;
        
        var classDeclaration = Extensions.SyntaxNodeExtensions.GetParent<ClassDeclarationSyntax>(methodDeclaration);
        if (!ValidateHandlerCandidateModifiers(classDeclaration))
            return null;

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
            return null;
        
        var methodAttributes = methodSymbol.GetAttributes();

        if (methodAttributes.Length == 0)
        {
            return null;
        }

        var httpVerbEndpointAttr = methodAttributes
            .FirstOrDefault(attr => 
                attr.AttributeClass?.ToDisplayString() == GetEndpointAttribute.TypeFullName ||
                attr.AttributeClass?.ToDisplayString() == PostEndpointAttribute.TypeFullName ||
                attr.AttributeClass?.ToDisplayString() == PutEndpointAttribute.TypeFullName ||
                attr.AttributeClass?.ToDisplayString() == DeleteEndpointAttribute.TypeFullName);
        
        var capture = httpVerbEndpointAttr switch
        {
            {  AttributeClass.Name: GetEndpointAttribute.TypeName } => CaptureHttpVerbContext(classSymbol, methodSymbol, httpVerbEndpointAttr, "Get"),
            {  AttributeClass.Name: PostEndpointAttribute.TypeName }  => CaptureHttpVerbContext(classSymbol, methodSymbol, httpVerbEndpointAttr, "Post"),
            {  AttributeClass.Name: PutEndpointAttribute.TypeName }  => CaptureHttpVerbContext(classSymbol, methodSymbol, httpVerbEndpointAttr, "Put"),
            {  AttributeClass.Name: DeleteEndpointAttribute.TypeName }  => CaptureHttpVerbContext(classSymbol, methodSymbol, httpVerbEndpointAttr, "Delete"),
            _ => null
        };
        
        if (capture is null)
        {
            return null;
        }

        var memberAttributeSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName(GroupMemberAttribute.TypeFullName);

        var classAttributes = classSymbol.GetAttributes();
        
        capture.IsEndpointGroup = classAttributes
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == EndpointGroupAttribute.TypeFullName) != null;

        if (!capture.IsEndpointGroup)
        {
            foreach (var attribute in classAttributes)
            {
                if (!SymbolEqualityComparer.Default.Equals(attribute?.AttributeClass?.OriginalDefinition,
                        memberAttributeSymbol))
                {
                    continue;
                }

                if (attribute?.AttributeClass is { TypeArguments.Length: 1 } attrSymbol &&
                    attrSymbol.TypeArguments[0] is INamedTypeSymbol typeArg)
                {
                    capture.MemberOf = typeArg.ToDisplayString();
                    capture.GroupMode = TargetMethodCaptureContext.DeclarationMode.ClassDeclaration;
                }
            }

            foreach (var attribute in methodAttributes)
            {
                if (!SymbolEqualityComparer.Default.Equals(attribute?.AttributeClass?.OriginalDefinition,
                        memberAttributeSymbol))
                {
                    continue;
                }

                if (attribute?.AttributeClass is { TypeArguments.Length: 1 } attrSymbol &&
                    attrSymbol.TypeArguments[0] is INamedTypeSymbol typeArg)
                {
                    capture.MemberOf = typeArg.ToDisplayString();
                    capture.GroupMode = TargetMethodCaptureContext.DeclarationMode.MethodDeclaration;
                }
            }
        }
        
        if (classAttributes.FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName) != null)
        {
            capture.ConfigurationMode = TargetMethodCaptureContext.DeclarationMode.ClassDeclaration;
        }
        if(methodAttributes.FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName) != null)
        {
            capture.ConfigurationMode = TargetMethodCaptureContext.DeclarationMode.MethodDeclaration;
        }

        return (classSymbol, capture);
    }

    private static bool ValidateHandlerCandidateModifiers(ClassDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static bool ValidateHandlerCandidateModifiers(MethodDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;
        
        if (!candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static (string? route, string? name, string? description) GetAttributeProperties(
        AttributeData getEndpointAttr)
    {
        return (
            getEndpointAttr.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString(),
            getEndpointAttr.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString(),
            getEndpointAttr.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString());
    }

    private static TargetMethodCaptureContext CaptureHttpVerbContext(INamedTypeSymbol classSymbol, IMethodSymbol methodSymbol, AttributeData getEndpointAttr, string verb)
    {
        var (httpRoute, name, description) = GetAttributeProperties(getEndpointAttr);
        var methodName = methodSymbol.Name;

        return new TargetMethodCaptureContext(
            classSymbol.ContainingNamespace.ToDisplayString(),
            classSymbol.Name,
            methodName,
            verb,
            httpRoute,
            name,
            description);
    }

    /// <summary>
    /// Creates the partial class capture from the provided type, method, and args
    /// </summary>
    private static TargetMethodCaptureContext TransformHandlerType((
        INamedTypeSymbol symbol,
        TargetMethodCaptureContext metadata) type)
    {
        var @namespace = type.symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.symbol.ContainingNamespace.ToDisplayString();

        var name = type.symbol.Name;

        var memberOf = type.metadata.IsEndpointGroup
            ? $"{@namespace}.{name}"
            : type.metadata.MemberOf;

        var configurationMode =
            type.metadata.ConfigurationMode == TargetMethodCaptureContext.DeclarationMode.ClassDeclaration &&
            type.metadata.IsEndpointGroup
                ? TargetMethodCaptureContext.DeclarationMode.Na
                : type.metadata.ConfigurationMode;
        
        return new TargetMethodCaptureContext(
            @namespace,
            name,
            type.metadata.MethodName,
            type.metadata.HttpMethod,
            type.metadata.HttpRoute,
            type.metadata.Name,
            type.metadata.Description,
            memberOf,
            configurationMode,
            type.metadata.IsEndpointGroup 
                ? TargetMethodCaptureContext.DeclarationMode.ClassDeclaration 
                : type.metadata.GroupMode,
            type.metadata.IsEndpointGroup);
    }
}