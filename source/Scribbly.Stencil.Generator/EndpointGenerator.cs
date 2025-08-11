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
        
        IncrementalValueProvider<BuilderCaptureContext?> stencilBuilderProvider =
            context.SyntaxProvider
                .CreateSyntaxProvider(BuilderInvocationSyntacticPredicate, BuilderInvocationSemanticTransform)
                .Where(static type => type.HasValue)
                .Select(static (type, _) => TransformBuilderInvocationType(type!.Value))
                .WithComparer(BuilderCaptureContextComparer.Instance)
                .Collect()
                .Select(static (list, _) => list.FirstOrDefault());
        
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        var endpointBuilderProvider = endpointProvider.Combine(stencilBuilderProvider);
        
        context.RegisterSourceOutput(endpointProvider, EndpointHandlerExecution.Generate);
        context.RegisterSourceOutput(endpointBuilderProvider, EndpointExtensionsExecution.Generate);
        
        var collectedEndpoints = endpointProvider.Collect();
        
        context.RegisterSourceOutput(collectedEndpoints, EndpointRegistrarExecution.Generate);
        
        context.RegisterSourceOutput(routeGroupProvider, GroupBuilderExecution.Generate);
        
        var groupedEndpoints = routeGroupProvider.Combine(collectedEndpoints);
        context.RegisterSourceOutput(groupedEndpoints, GroupExtensionsExecution.Generate);
        
        var collectedGroups = routeGroupProvider.Collect();
        var routeTree = collectedEndpoints.Combine(collectedGroups);
        context.RegisterSourceOutput(routeTree, GroupRegistrarExecution.Generate);
        
        var dependencyInjection = stencilBuilderProvider.Combine(routeTree);
        context.RegisterSourceOutput(dependencyInjection, BuilderRegistrarExecution.Generate);
    }
    
    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        var builder = new StringBuilder().CreateServiceRegistrar();
        context.AddSource($"Registrar.Scribbly.Stencil.ServiceExtensions.g.cs", builder.ToString());
    }
}

