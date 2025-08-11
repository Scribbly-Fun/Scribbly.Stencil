using System.Text;

namespace Scribbly.Stencil.Endpoints.Factories;

public static class EndpointMappingMethodNameFactory
{
    public static StringBuilder CreateEndpointMappingMethodName(this TargetMethodCaptureContext subject)
    {
        return new StringBuilder().CreateEndpointMappingMethodName(subject);
    }
    
    public static StringBuilder CreateEndpointMappingMethodName(this StringBuilder sb, TargetMethodCaptureContext subject)
    {
        return sb.Append("Map").Append(subject.TypeName).Append(subject.MethodName);
    }
    
    public static StringBuilder CreateEndpointMappingMethodInvocation(this StringBuilder sb, TargetMethodCaptureContext subject)
    {
        return sb.CreateEndpointMappingMethodName(subject).Append("();");
    }
}