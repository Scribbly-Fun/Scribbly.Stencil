using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Builder.Factories;

public static class CreateServiceScopeFactory
{
    public static StringBuilder CreateServiceScope(this StringBuilder builder, BuilderCaptureContext context)
    {
        if (!context.AddStencilWasInvoked)
        {
            return builder;
        }

        return builder.AppendLine("      using global::Microsoft.Extensions.DependencyInjection.IServiceScope scope = webApplication.Services.CreateScope();");
    }
}