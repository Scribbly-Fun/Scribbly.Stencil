using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Groups.Factories;

public static class GroupRegistrarExtensionMethodFactory
{
    public static StringBuilder CreateGroupRegistrationMethodSignature(this StringBuilder sb, TargetGroupCaptureContext group, BuilderCaptureContext? builderCtx)
    {
        if (builderCtx is not { AddStencilWasInvoked: true })
        {
            return sb.AppendLine("public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder Map").Append(group.TypeName).Append("(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)");
        }

        return sb
            .AppendLine("public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder Map")
            .Append(group.TypeName)
            .Append("(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder, global::Microsoft.Extensions.DependencyInjection.IServiceScope scope)");
    }   
}