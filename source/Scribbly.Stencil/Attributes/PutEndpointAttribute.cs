namespace Scribbly.Stencil;

/// <summary>
/// When applied to a partial static method, the delegate will become a handler for an HTTP PUT endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PutEndpointAttribute : EndpointAttribute
{
    /// <summary>
    /// When applied to a partial static method, the delegate will become a handler for an HTTP PUT endpoint.
    /// </summary>
    public PutEndpointAttribute(string route, string? name = null, string? description = null)
        : base("Get", route, name, description)
    {
    }
}