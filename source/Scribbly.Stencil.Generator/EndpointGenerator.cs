// ReSharper disable SuggestVarOrType_Elsewhere

using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Builder;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Builder.Factories;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Groups;

namespace Scribbly.Stencil;

[Generator(LanguageNames.CSharp)]
public partial class EndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // ----------------------> Registered Initialization Types
        context.RegisterPostInitializationOutput(PostInitializationCallback);
        
         // ----------------------> Capture Providers
        var endpointProvider = context.SyntaxProvider
            .CreateSyntaxProvider(HandlerSyntacticPredicate, HandlerSemanticTransform)
            .Where(static type => type.HasValue)
            .Select(static (type, _) => TransformHandlerType(type!.Value))
            .WithComparer(TargetMethodCaptureContextComparer.Instance);

        var routeGroupProvider = context.SyntaxProvider
            .CreateSyntaxProvider(GroupSyntacticPredicate, GroupSemanticTransform)
            .Where(static type => type.HasValue)
            .Select(static (type, _) => TransformGroupType(type!.Value))
            .WithComparer(TargetGroupCaptureContextComparer.Instance);

        var stencilBuilderProvider = context.SyntaxProvider
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

