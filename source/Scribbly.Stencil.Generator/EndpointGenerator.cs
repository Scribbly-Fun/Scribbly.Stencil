// ReSharper disable SuggestVarOrType_Elsewhere

using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Endpoints.Context;
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
        
        context.RegisterSourceOutput(endpointProvider, EndpointHandlerExecution.Generate);
        context.RegisterSourceOutput(endpointProvider, EndpointExtensionsExecution.Generate);
        
        var collectedEndpoints = endpointProvider.Collect();
        context.RegisterSourceOutput(collectedEndpoints, EndpointRegistrarExecution.Generate);
        
        context.RegisterSourceOutput(routeGroupProvider, GroupBuilderExecution.Generate);
        
        var groupedEndpoints = routeGroupProvider.Combine(collectedEndpoints);
        context.RegisterSourceOutput(groupedEndpoints, GroupExtensionsExecution.Generate);
        
        var collectedGroups = routeGroupProvider.Collect();
        var routeTree = collectedEndpoints.Combine(collectedGroups);
        context.RegisterSourceOutput(routeTree, GroupRegistrarExecution.Generate);
    }
}