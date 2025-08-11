using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Builder.Factories;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Factories;
using Scribbly.Stencil.Groups;

namespace Scribbly.Stencil.Builder;

public class BuilderRegistrarExecution
{
    public static void Generate(
        SourceProductionContext context,
        (BuilderCaptureContext? Builder, (
            ImmutableArray<TargetMethodCaptureContext> Endpoints, 
            ImmutableArray<TargetGroupCaptureContext> Groups) 
            Tree) builderContext)
    {
        var builder = FileHeaderFactory.CreateFileHeader();
        
        if (builderContext.Builder is not {AddStencilWasInvoked: true })
        {
            builder.CreateServiceRegistration([], []);
            
            context.AddSource($"Registrar.Scribbly.Stencil.ServiceRegistration.g.cs", builder.ToString());
            return;
        }
        var (endpoints, groups) = builderContext.Tree;
        
        if (endpoints.IsDefaultOrEmpty && groups.IsDefaultOrEmpty)
        {
            return;
        }
        
        builder.CreateServiceRegistration(endpoints, groups);
        
        context.AddSource($"Registrar.Scribbly.Stencil.ServiceRegistration.g.cs", builder.ToString());
    }
}