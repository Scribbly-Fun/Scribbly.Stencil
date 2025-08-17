using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Endpoints.Factories;

public static class NewEndpointInvocationFactory
{
    public static StringBuilder CreateNewEndpoint(this StringBuilder builder, TargetMethodCaptureContext subject, BuilderCaptureContext? builderCtx)
    {
        if (builderCtx is not {AddStencilWasInvoked: true})
        {
            return builder
                .Append("        var scribblyEndpoint = new global::")
                .Append(subject.Namespace).Append('.').Append(subject.TypeName)
                .Append("();");
        }

        return builder
            .Append("        var scribblyEndpoint = scope.ServiceProvider.GetRequiredService<global::")
            .Append(subject.Namespace).Append('.').Append(subject.TypeName)
            .Append(">();");
    }
}