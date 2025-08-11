using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Builder.Factories;

namespace Scribbly.Stencil;

public partial class EndpointGenerator
{
    private static bool BuilderInvocationSyntacticPredicate(SyntaxNode node, CancellationToken cancellation)
    {
        return node is InvocationExpressionSyntax;
    }
    
    private static (IMethodSymbol symbol, BuilderCaptureContext builder)? BuilderInvocationSemanticTransform(
        GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol)
        {
            return null;
        }
        
        if (symbol.Name == "AddStencil" && symbol.ContainingNamespace.ToDisplayString() == "Scribbly.Stencil")
        {
            return (symbol, new BuilderCaptureContext
            {
                AddStencilWasInvoked = true
            }); 
        }

        return null;
    }
    
    private static BuilderCaptureContext TransformBuilderInvocationType((
        IMethodSymbol symbol,
        BuilderCaptureContext metadata) type)
    {
        return new BuilderCaptureContext
        {
            AddStencilWasInvoked = type.metadata.AddStencilWasInvoked,
        };
    }
}

