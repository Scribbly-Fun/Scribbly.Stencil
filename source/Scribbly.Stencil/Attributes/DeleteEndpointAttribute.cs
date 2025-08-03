namespace Scribbly.Stencil;

/// <summary>
/// When applied to a partial static method, the delegate will become a handler for an HTTP Delete endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class DeleteEndpointAttribute : EndpointAttribute
{
    /// <summary>
    /// When applied to a partial static method, the delegate will become a handler for an HTTP Delete endpoint.
    /// </summary>
    public DeleteEndpointAttribute(string route, string? name = null, string? description = null)
        : base("Delete", route, name, description)
    {
    }
}