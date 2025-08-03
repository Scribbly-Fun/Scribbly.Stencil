namespace Scribbly.Stencil;

/// <summary>
/// When applied to a partial static method, the delegate will become a handler for an HTTP Post endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PostEndpointAttribute : EndpointAttribute
{
    /// <summary>
    /// When applied to a partial static method, the delegate will become a handler for an HTTP Post endpoint.
    /// </summary>
    public PostEndpointAttribute(string route, string? name = null, string? description = null)
        : base("Post", route, name, description)
    {
    }
}