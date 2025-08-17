using System.Text;

namespace Scribbly.Stencil.Endpoints.Factories;

internal static class ConfigureEndpointFactories
{
    internal static StringBuilder CreateConfigureInvocation(this StringBuilder builder, TargetMethodCaptureContext subject)
    {
        if (!subject.IsConfigurable)
        {
            return builder;
        }
        return subject.IsConfigurable 
            ? builder.Append("        scribblyEndpoint.Configure").Append(subject.MethodName).Append("(endpointBuilder);") 
            : builder;
    } 
}