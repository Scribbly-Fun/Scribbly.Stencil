namespace Scribbly.Stencil.Attributes.Endpoints;

public static class EndpointAttributeData
{
    public class TypeInformation(string TypeName, string UsageName, string FullName);
    
    public static Dictionary<string, TypeInformation> EndpointAttributes = new ()
    {
        { GetEndpointAttribute.TypeFullName, new TypeInformation(GetEndpointAttribute.TypeName, GetEndpointAttribute.UsageName, GetEndpointAttribute.TypeFullName) },
        { PostEndpointAttribute.TypeFullName, new TypeInformation(PostEndpointAttribute.TypeName, PostEndpointAttribute.UsageName, PostEndpointAttribute.TypeFullName) },
        { PutEndpointAttribute.TypeFullName, new TypeInformation(PutEndpointAttribute.TypeName, PutEndpointAttribute.UsageName, PutEndpointAttribute.TypeFullName) },
        { DeleteEndpointAttribute.TypeFullName, new TypeInformation(DeleteEndpointAttribute.TypeName, DeleteEndpointAttribute.UsageName, DeleteEndpointAttribute.TypeFullName) }
    };
}