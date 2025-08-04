// ReSharper disable SuggestVarOrType_Elsewhere

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Attributes.Endpoints;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Endpoints.Context;
using Scribbly.Stencil.Endpoints.Execution;
using Scribbly.Stencil.Groups;
using Scribbly.Stencil.Types.Attributes;

namespace Scribbly.Stencil;

[Generator(LanguageNames.CSharp)]
public class EndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TargetMethodCaptureContext> endpointProvider = context.SyntaxProvider
            .CreateSyntaxProvider(HandlerSyntacticPredicate, HandlerSemanticTransform)
            .Where(static (type) => type.HasValue)
            .Select(static (type, _) => TransformHandlerType(type!.Value))
            .WithComparer(TargetMethodCaptureContextComparer.Instance);
        
        IncrementalValuesProvider<TargetGroupCaptureContext> routeGroupProvider = context.SyntaxProvider
            .CreateSyntaxProvider(GroupSyntacticPredicate, GroupSemanticTransform)
            .Where(static (type) => type.HasValue)
            .Select(static (type, _) => TransformGroupType(type!.Value))
            .WithComparer(TargetGroupCaptureContextComparer.Instance);
        
        // TODO: Pass the collected endpoints and collected groups to a single generator that emits the mapped together tree.
        var collectedEndpoints = endpointProvider.Collect();
        var collectedGroups = routeGroupProvider.Collect();

        context.RegisterSourceOutput(endpointProvider, EndpointHandlerExecution.Generate);
        context.RegisterSourceOutput(collectedEndpoints, EndpointRegistrarExecution.Generate);
        
        context.RegisterSourceOutput(routeGroupProvider, GroupBuilderExecution.Generate);
        context.RegisterSourceOutput(routeGroupProvider, GroupExtensionsExecution.Generate);
        context.RegisterSourceOutput(collectedGroups, GroupRegistrarExecution.Generate);
    }
    
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
    
    private static bool GroupSyntacticPredicate(SyntaxNode node, CancellationToken cancellation)
    {
        if (node is not ClassDeclarationSyntax classDeclaration)
            return false;
        
        return classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr => attr.Name.ToString().Contains(EndpointGroupAttribute.UsageName));
    }

    private static (INamedTypeSymbol symbol, TargetMethodCaptureContext handler)? HandlerSemanticTransform(GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return null;

        var classDeclaration = Extensions.SyntaxNodeExtensions.GetParent<ClassDeclarationSyntax>(methodDeclaration);
        if (!ValidateHandlerCandidateModifiers(classDeclaration))
            return null;

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
            return null;

        var getEndpointAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == GetEndpointAttribute.TypeFullName);

        var postEndpointAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == PostEndpointAttribute.TypeFullName);

        var putEndpointAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == PutEndpointAttribute.TypeFullName);

        var deleteEndpointAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == DeleteEndpointAttribute.TypeFullName);

        var capture = (getEndpointAttr, postEndpointAttr, putEndpointAttr, deleteEndpointAttr) switch
        {
            (null, null, null, null) => null,
            (not null, null, null, null) => CaptureGetContext(classSymbol, methodSymbol, getEndpointAttr),
            (null, not null, null, null) => CapturePostContext(classSymbol, methodSymbol, postEndpointAttr),
            (null, null, not null, null) => CapturePutContext(classSymbol, methodSymbol, putEndpointAttr),
            (null, null, null, not null) => CaptureDeleteContext(classSymbol, methodSymbol, deleteEndpointAttr),
            _ => null
        };
        if (capture is null)
        {
            return null;
        }
        
        return (classSymbol, capture);
    }
    
    private static (INamedTypeSymbol symbol, TargetGroupCaptureContext handler)? GroupSemanticTransform(GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        if (!ValidateGroupCandidateModifiers(classDeclaration))
            return null;
        
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
            return null;

        var groupAtt = classSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == EndpointGroupAttribute.TypeFullName);
        
        if (groupAtt is null)
        {
            return null;
        }

        var prefix = groupAtt.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        var tag = groupAtt.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();

        if (prefix is null)
        {
            return null;
        }
        
        var configureAtt = classSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName);
        
        return (classSymbol, new TargetGroupCaptureContext(
            classSymbol.ContainingNamespace.ToDisplayString(),
            classSymbol.Name,
            prefix,
            tag,
            null,
            configureAtt is not null));
    }
    
    private static bool ValidateHandlerCandidateModifiers(ClassDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static bool ValidateGroupCandidateModifiers(ClassDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static (string? route, string? name, string? description) GetAttributeProperties(AttributeData getEndpointAttr)
    {
        return (
            getEndpointAttr.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString(),
            getEndpointAttr.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString(),
            getEndpointAttr.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString());
    }
    
    private static TargetMethodCaptureContext CapturePostContext(INamedTypeSymbol classSymbol, IMethodSymbol methodSymbol, AttributeData getEndpointAttr)
    {
        var (httpRoute, name, description) = GetAttributeProperties(getEndpointAttr);
        var methodName = methodSymbol.Name;

        return new TargetMethodCaptureContext(
                classSymbol.ContainingNamespace.ToDisplayString(),
                classSymbol.Name,
                methodName,
                "Post",
                httpRoute,
                name, 
                description);
    }
    
    private static TargetMethodCaptureContext CapturePutContext(INamedTypeSymbol classSymbol, IMethodSymbol methodSymbol, AttributeData getEndpointAttr)
    {
        var (httpRoute, name, description) = GetAttributeProperties(getEndpointAttr);
        var methodName = methodSymbol.Name;

        return new TargetMethodCaptureContext(
                classSymbol.ContainingNamespace.ToDisplayString(),
                classSymbol.Name,
                methodName,
                "Put",
                httpRoute,
                name, 
                description);
    }
    
    private static TargetMethodCaptureContext CaptureDeleteContext(INamedTypeSymbol classSymbol, IMethodSymbol methodSymbol, AttributeData getEndpointAttr)
    {
        var (httpRoute, name, description) = GetAttributeProperties(getEndpointAttr);
        var methodName = methodSymbol.Name;

        return new TargetMethodCaptureContext(
                classSymbol.ContainingNamespace.ToDisplayString(),
                classSymbol.Name,
                methodName,
                "Delete",
                httpRoute,
                name, 
                description);
    }
    
    private static TargetMethodCaptureContext CaptureGetContext(INamedTypeSymbol classSymbol, IMethodSymbol methodSymbol, AttributeData getEndpointAttr)
    {
        var (httpRoute, name, description) = GetAttributeProperties(getEndpointAttr);
        var methodName = methodSymbol.Name;

        return new TargetMethodCaptureContext(
            classSymbol.ContainingNamespace.ToDisplayString(),
            classSymbol.Name,
            methodName,
            "Get",
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

        return new TargetMethodCaptureContext(
            @namespace,
            name,
            type.metadata.MethodName,
            type.metadata.HttpMethod,
            type.metadata.HttpRoute,
            type.metadata.Name,
            type.metadata.Description);
    }
    
    /// <summary>
    /// Creates the partial class capture from the provided type, method, and args
    /// </summary>
    private static TargetGroupCaptureContext TransformGroupType((
        INamedTypeSymbol symbol,
        TargetGroupCaptureContext metadata) type)
    {
        var @namespace = type.symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.symbol.ContainingNamespace.ToDisplayString();

        var name = type.symbol.Name;

        return new TargetGroupCaptureContext(
            @namespace,
            name,
            type.metadata.RoutePrefix,
            type.metadata.Tag,
            type.metadata.MemberOf,
            type.metadata.IsConfigurable);
    }
}