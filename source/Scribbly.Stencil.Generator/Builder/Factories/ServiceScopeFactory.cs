using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Builder.Factories;

public static class ServiceScopeFactory
{
    public static StringBuilder CreateServiceScopeUsing(this StringBuilder sb, BuilderCaptureContext? builderCtx)
    {
        if (builderCtx is not { AddStencilWasInvoked: true })
        {
            return sb;
        }

        return sb.AppendLine("using global::Microsoft.Extensions.DependencyInjection.IServiceScope scope = webApplication.Services.CreateScope();");
    }
}