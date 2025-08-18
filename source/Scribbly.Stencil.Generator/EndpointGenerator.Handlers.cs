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
        
        var memberAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(GroupMemberAttribute.TypeFullName);
        
        TargetMethodCaptureContext? capture = null;
        
        string? methodMembership = null;
        string? classMembership = null;

        var methodConfigurationMode = false;
        var classConfigurationMode = false;
        
        
        foreach (var methodAttribute in methodAttributes)
        {
            var attrClass = methodAttribute.AttributeClass;
            if (attrClass is null)
            {
                continue;
            }

            if (attrClass.ToDisplayString() == GetEndpointAttribute.TypeFullName)
            {
                capture = CaptureHttpVerbContext(classSymbol, methodSymbol, methodAttribute, "Get");
            }
            if (attrClass.ToDisplayString() == PostEndpointAttribute.TypeFullName)
            {
                capture = CaptureHttpVerbContext(classSymbol, methodSymbol, methodAttribute, "Post");
            }
            if (attrClass.ToDisplayString() == PutEndpointAttribute.TypeFullName)
            {
                capture = CaptureHttpVerbContext(classSymbol, methodSymbol, methodAttribute, "Put");
            }
            if (attrClass.ToDisplayString() == DeleteEndpointAttribute.TypeFullName)
            {
                capture = CaptureHttpVerbContext(classSymbol, methodSymbol, methodAttribute, "Delete");
            }
            
            if (SymbolEqualityComparer.Default.Equals(attrClass.OriginalDefinition, memberAttributeSymbol) && 
                attrClass is { TypeArguments.Length: 1 } attrSymbol &&
                attrSymbol.TypeArguments[0] is INamedTypeSymbol typeArg)
            {
                methodMembership = typeArg.ToDisplayString();
            }
            
            if(attrClass.ToDisplayString() == ConfigureAttribute.TypeFullName)
            {
                methodConfigurationMode = true;
            }
        }
        
        if (capture is null)
        {
            return null;
        }
        
        var classAttributes = classSymbol.GetAttributes();
        
        foreach (var classAttribute in classAttributes)
        {
            var attrClass = classAttribute.AttributeClass;
            if (attrClass is null)
            {
                continue;
            }
            if(attrClass.ToDisplayString() == EndpointGroupAttribute.TypeFullName)
            {
                capture.IsEndpointGroup =  true;
            }
            
            if(attrClass.ToDisplayString() == ConfigureAttribute.TypeFullName) 
            {
                classConfigurationMode = true;
            }
            
            if (SymbolEqualityComparer.Default.Equals(attrClass.OriginalDefinition, memberAttributeSymbol) && 
                attrClass is { TypeArguments.Length: 1 } attrSymbol &&
                attrSymbol.TypeArguments[0] is INamedTypeSymbol typeArg)
            {
                classMembership = typeArg.ToDisplayString();
            }
        }
        
        capture.ConfigurationMode = (methodConfigurationMode, classConfigurationMode) switch
        {
            (false, false) => TargetMethodCaptureContext.DeclarationMode.Na,
            (true, false) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
            (false, true) => TargetMethodCaptureContext.DeclarationMode.ClassDeclaration,
            (true, true) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
        };
        capture.MemberOf = methodMembership ?? classMembership;
        capture.GroupMode = (methodMembership, classMembership) switch
        {
            (null, null) => TargetMethodCaptureContext.DeclarationMode.Na,
            (not null, null) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
            (null, not null) => TargetMethodCaptureContext.DeclarationMode.ClassDeclaration,
            (not null, not null) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
        };
        
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