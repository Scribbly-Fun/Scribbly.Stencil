namespace Scribbly.Stencil;

/// <summary>
/// When applied to a partial static method, the delegate will become a handler for an HTTP GET endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GetEndpointAttribute : EndpointAttribute
{
    /// <summary>
    /// When applied to a partial static method, the delegate will become a handler for an HTTP GET endpoint.
    /// </summary>
    public GetEndpointAttribute(string route, string? name = null, string? description = null)
        : base("Get", route, name, description)
    {
    }
}