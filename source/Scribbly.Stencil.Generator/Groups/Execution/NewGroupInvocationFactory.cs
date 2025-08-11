using System.Text;
using Scribbly.Stencil.Builder.Context;

namespace Scribbly.Stencil.Groups;

public static class NewGroupInvocationFactory
{
    public static StringBuilder CreateNewGroup(this StringBuilder builder, TargetGroupCaptureContext group, BuilderCaptureContext? builderCtx)
    {
        builder.Append("var scribblyGroup = ");
        if (builderCtx is not { AddStencilWasInvoked: true })
        {
            return builder.Append("new global::").Append(group.Namespace).Append(".").Append(group.TypeName).AppendLine("();");
        }
        
        return builder.Append("scope.ServiceProvider.GetRequiredService<global::").Append(group.Namespace).Append(".").Append(group.TypeName).AppendLine(">();");
    }
}