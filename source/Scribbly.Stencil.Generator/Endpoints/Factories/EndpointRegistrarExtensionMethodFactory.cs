using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Endpoints.Factories;

public static class EndpointRegistrarExtensionMethodFactory
{
    public static StringBuilder CreateEndpointRegistrationMethodSignature(this StringBuilder sb, BuilderCaptureContext? builderCtx)
    {

        if (builderCtx is not { AddStencilWasInvoked: true })
        {
            return sb.AppendLine("public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder MapScribblyEndpoints(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)");
        }

        return sb.AppendLine(
            "public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder MapScribblyEndpoints(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder, global::Microsoft.Extensions.DependencyInjection.IServiceScope scope)");
    }   
}