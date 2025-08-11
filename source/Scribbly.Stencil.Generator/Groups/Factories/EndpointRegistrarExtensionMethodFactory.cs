using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Groups.Factories;

public static class GroupRegistrarExtensionMethodFactory
{
    public static StringBuilder CreateGroupRegistrationMethodSignature(this StringBuilder sb, TargetGroupCaptureContext group, BuilderCaptureContext? builderCtx)
    {
        if (builderCtx is not { AddStencilWasInvoked: true })
        {
            return sb.Append("public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder Map").Append(group.TypeName).AppendLine("(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)");
        }

        return sb
            .Append("public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder Map")
            .Append(group.TypeName)
            .AppendLine("(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder, global::Microsoft.Extensions.DependencyInjection.IServiceScope scope)");
    }   
}