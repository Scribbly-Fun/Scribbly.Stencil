using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Builder.Factories;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Groups;

namespace Scribbly.Stencil.Builder;

public class BuilderRegistrarExecution
{
    public static void Generate(SourceProductionContext context,
        (ImmutableArray<TargetMethodCaptureContext> Endpoints, ImmutableArray<TargetGroupCaptureContext> Groups) tree)
    {
        if (tree.Endpoints.IsDefaultOrEmpty && tree.Groups.IsDefaultOrEmpty)
        {
            return;
        }
        
        var builder = new StringBuilder()
            .CreateServiceRegistrar(tree.Endpoints, tree.Groups);
        
        context.AddSource($"Registrar.Scribbly.Stencil.Builder.g.cs", builder.ToString());
    }
}