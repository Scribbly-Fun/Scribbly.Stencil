// ReSharper disable SuggestVarOrType_Elsewhere

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Builder;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Builder.Factories;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Groups;
using Scribbly.Stencil.Types.Attributes;
using System.Collections.Immutable;
using System.Text;

namespace Scribbly.Stencil;

[Generator(LanguageNames.CSharp)]
public partial class EndpointGenerator : IIncrementalGenerator
{
    

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // ----------------------> Registered Initialization Types
        context.RegisterPostInitializationOutput(PostInitializationCallback);
        
        IncrementalValueProvider<ImmutableArray<CapturedHandler>> getHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                GetEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax method && ValidateHandlerCandidateModifiers(method),
                static (ctx, ct) => Capture(ctx, "Get"))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .WithComparer(CapturedHandlerComparer.Instance)
            .Collect();

        IncrementalValueProvider<ImmutableArray<CapturedHandler>> postHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                PostEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (ctx, ct) => Capture(ctx, "Post"))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .WithComparer(CapturedHandlerComparer.Instance)
            .Collect();
        
        IncrementalValueProvider<ImmutableArray<CapturedHandler>> putHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                PutEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (ctx, ct) => Capture(ctx, "Put"))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .WithComparer(CapturedHandlerComparer.Instance)
            .Collect();
        
        IncrementalValueProvider<ImmutableArray<CapturedHandler>> deleteHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                DeleteEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (ctx, ct) => Capture(ctx, "Delete"))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .WithComparer(CapturedHandlerComparer.Instance)
            .Collect();
        
        var combinedEndpointArrays = getHandlers
            .Combine(postHandlers)
            .Combine(putHandlers)
            .Combine(deleteHandlers);

        // ----------------------> Capture Providers

        IncrementalValuesProvider<TargetMethodCaptureContext> endpointProvider = 
            combinedEndpointArrays.SelectMany(static (tuple, _) =>
                {
                    var (((gets, posts), puts), deletes) = tuple;
                    
                    var total = gets.Length + posts.Length + puts.Length + deletes.Length;
                    if (total == 0)
                    {
                        return ImmutableArray<CapturedHandler>.Empty;
                    }
                    
                    if (gets.IsEmpty && posts.IsEmpty)
                    {
                        return ImmutableArray<CapturedHandler>.Empty;
                    }
        
                    var builder = ImmutableArray.CreateBuilder<CapturedHandler>(total);
                    builder.AddRange(gets);
                    builder.AddRange(posts);
                    builder.AddRange(puts);
                    builder.AddRange(deletes);
                    return builder.MoveToImmutable();
                })
            .Select(static (captured, _) => TransformHandlerType2(captured!));

        IncrementalValuesProvider<TargetGroupCaptureContext> routeGroupProvider = context.SyntaxProvider
            .CreateSyntaxProvider(GroupSyntacticPredicate, GroupSemanticTransform)
            .Where(static type => type.HasValue)
            .Select(static (type, _) => TransformGroupType(type!.Value))
            .WithComparer(TargetGroupCaptureContextComparer.Instance);

        IncrementalValueProvider<BuilderCaptureContext?> stencilBuilderProvider = context.SyntaxProvider
            .CreateSyntaxProvider(BuilderInvocationSyntacticPredicate, BuilderInvocationSemanticTransform)
            .Where(static type => type.HasValue)
            .Select(static (type, _) => TransformBuilderInvocationType(type!.Value))
            .WithComparer(BuilderCaptureContextComparer.Instance)
            .Collect()
            .Select(static (list, _) => list.FirstOrDefault());

        // ----------------------> Collected Arrays
        var collectedEndpoints = endpointProvider.Collect();
        var collectedGroups   = routeGroupProvider.Collect();

        // ----------------------> Combined Providers
        var endpointBuilderProvider = endpointProvider
            .Combine(stencilBuilderProvider);
        
        var collectedEndpointsProvider = collectedEndpoints
            .Combine(stencilBuilderProvider);
        
        var groupedEndpoints = routeGroupProvider
            .Combine(stencilBuilderProvider)
            .Combine(collectedEndpoints);

        var routeTree = collectedEndpoints
            .Combine(collectedGroups);
        
        var routeTreeProvider = routeTree
            .Combine(stencilBuilderProvider);

        var dependencyInjection = stencilBuilderProvider
            .Combine(routeTree);

        // ----------------------> Registered Source Outputs
        context.RegisterSourceOutput(endpointProvider, EndpointHandlerExecution.Generate);
        context.RegisterSourceOutput(endpointBuilderProvider, EndpointExtensionsExecution.Generate);
        context.RegisterSourceOutput(collectedEndpointsProvider, EndpointRegistrarExecution.Generate);
        context.RegisterSourceOutput(routeGroupProvider, GroupBuilderExecution.Generate);
        context.RegisterSourceOutput(groupedEndpoints, GroupExtensionsExecution.Generate);
        context.RegisterSourceOutput(routeTreeProvider, GroupRegistrarExecution.Generate);
        context.RegisterSourceOutput(dependencyInjection, BuilderRegistrarExecution.Generate);
    }
    
    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        var registrar = new StringBuilder().CreateServiceRegistrar();
        var scopeMapping = new StringBuilder().CreateServiceScopeMapping();
        context.AddSource($"Registrar.Scribbly.Stencil.ServiceExtensions.g.cs", registrar.ToString());
        context.AddSource($"Registrar.Scribbly.Stencil.ScopeExtensions.g.cs", scopeMapping.ToString());
    }

    private static CapturedHandler? Capture(GeneratorAttributeSyntaxContext context, string httpVerb)
    {
        if (context.TargetSymbol is not IMethodSymbol methodSymbol)
            return null;

        var classSymbol = methodSymbol.ContainingType;
        if (classSymbol is null)
            return null;

        // Enforce candidate rules
        if (!ValidateHandlerCandidateModifiers(classSymbol.DeclaringSyntaxReferences.First().GetSyntax() as ClassDeclarationSyntax))
            return null;
        if (!ValidateHandlerCandidateModifiers(methodSymbol.DeclaringSyntaxReferences.First().GetSyntax() as MethodDeclarationSyntax))
            return null;

        // Extract info from the endpoint attribute
        var endpointAttr = context.Attributes.FirstOrDefault();
        var (httpRoute, name, description) = GetAttributeProperties(endpointAttr);

        // Membership/group flags
        string? methodMembership = null;
        string? classMembership = null;
        bool methodConfig = false;
        bool classConfig = false;
        bool isEndpointGroup = false;

        // Check method attributes
        foreach (var attr in methodSymbol.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName)
                methodConfig = true;

            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass?.OriginalDefinition,
                    context.SemanticModel.Compilation.GetTypeByMetadataName(GroupMemberAttribute.TypeFullName)) &&
                attr.AttributeClass is { TypeArguments.Length: 1 } symbol &&
                symbol.TypeArguments[0] is INamedTypeSymbol typeArg)
            {
                methodMembership = typeArg.ToDisplayString();
            }
        }

        // Check class attributes
        foreach (var attr in classSymbol.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() == EndpointGroupAttribute.TypeFullName)
                isEndpointGroup = true;

            if (attr.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName)
                classConfig = true;

            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass?.OriginalDefinition,
                    context.SemanticModel.Compilation.GetTypeByMetadataName(GroupMemberAttribute.TypeFullName)) &&
                attr.AttributeClass is { TypeArguments.Length: 1 } symbol &&
                symbol.TypeArguments[0] is INamedTypeSymbol typeArg)
            {
                classMembership = typeArg.ToDisplayString();
            }
        }

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();

        var memberOf = methodMembership ?? classMembership;

        var configurationMode = (methodConfig, classConfig) switch
        {
            (false, false) => TargetMethodCaptureContext.DeclarationMode.Na,
            (true, false) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
            (false, true) => TargetMethodCaptureContext.DeclarationMode.ClassDeclaration,
            (true, true) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
        };

        var groupMode = (methodMembership, classMembership) switch
        {
            (null, null) => TargetMethodCaptureContext.DeclarationMode.Na,
            (not null, null) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
            (null, not null) => TargetMethodCaptureContext.DeclarationMode.ClassDeclaration,
            (not null, not null) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
        };

        return new CapturedHandler(
            Namespace: ns,
            ClassName: classSymbol.Name,
            MethodName: methodSymbol.Name,
            HttpVerb: httpVerb,
            Route: httpRoute,
            Name: name,
            Description: description,
            MemberOf: memberOf,
            IsEndpointGroup: isEndpointGroup,
            ConfigurationMode: configurationMode,
            GroupMode: groupMode
        );
    }

    private static TargetMethodCaptureContext TransformHandlerType2(CapturedHandler captured)
    {
        var memberOf = captured.IsEndpointGroup
            ? $"{captured.Namespace}.{captured.ClassName}"
            : captured.MemberOf;

        var configurationMode =
            captured.ConfigurationMode == TargetMethodCaptureContext.DeclarationMode.ClassDeclaration &&
            captured.IsEndpointGroup
                ? TargetMethodCaptureContext.DeclarationMode.Na
                : captured.ConfigurationMode;

        return new TargetMethodCaptureContext(
            captured.Namespace,
            captured.ClassName,
            captured.MethodName,
            captured.HttpVerb,
            captured.Route,
            captured.Name,
            captured.Description,
            memberOf,
            configurationMode,
            captured.IsEndpointGroup
                ? TargetMethodCaptureContext.DeclarationMode.ClassDeclaration
                : captured.GroupMode,
            captured.IsEndpointGroup);
    }

    private static void Execute(SourceProductionContext context, TargetMethodCaptureContext target)
    {
        // This is where you generate code. You already have all the info you need.
        // Example: simple diagnostic/logging for now
        var hintName = $"{target.TypeName}_{target.MethodName}_Endpoint.g.cs";

        var source = $$"""
                       // <auto-generated />
                       namespace {{target.Namespace}}
                       {
                           public static partial class {{target.TypeName}}_Generated
                           {
                               public static void {{target.MethodName}}_Info()
                               {
                                   System.Console.WriteLine("Endpoint: [{{target.MethodName}}] {{target.HttpRoute}}");
                               }
                           }
                       }
                       """;

        context.AddSource(hintName, source);
    }
}

