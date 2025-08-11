using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Endpoints.Factories;

public static class EndpointMappingExtensionMethodFactory
{
    public static StringBuilder CreateHandleExtensionMethodDeclaration(this StringBuilder sb, TargetMethodCaptureContext subject, BuilderCaptureContext? builderCtx)
    {
        if (builderCtx is not { AddStencilWasInvoked: true })
        {
            return sb.Append("Map").Append(subject.TypeName).Append(subject.MethodName).Append('(')
                .Append("this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)");
        }

        return sb.Append("Map").Append(subject.TypeName).Append(subject.MethodName).Append('(')
            .Append("this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder, ")
            .Append("this global::Microsoft.Extensions.DependencyInjection.IServiceScope scope)");
    }
}