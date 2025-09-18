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
                static (ctx, ct) => CaptureEndpointHandler(ctx, "Get", ct))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .Collect();

        IncrementalValueProvider<ImmutableArray<CapturedHandler>> postHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                PostEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (ctx, ct) => CaptureEndpointHandler(ctx, "Post", ct))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .Collect();
        
        IncrementalValueProvider<ImmutableArray<CapturedHandler>> putHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                PutEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (ctx, ct) => CaptureEndpointHandler(ctx, "Put", ct))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
            .Collect();
        
        IncrementalValueProvider<ImmutableArray<CapturedHandler>> deleteHandlers = context.SyntaxProvider.ForAttributeWithMetadataName(
                DeleteEndpointAttribute.TypeFullName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (ctx, ct) => CaptureEndpointHandler(ctx, "Delete", ct))
            .Where(static h => h is not null)
            .Select(static (h, _) => h!)
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
            .Select(static (captured, _) => TransformHandlerType(captured!));

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
}

