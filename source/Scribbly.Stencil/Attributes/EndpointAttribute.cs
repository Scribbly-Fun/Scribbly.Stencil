namespace Scribbly.Stencil;

/// <summary>
/// When applied to a partial static method, the delegate will become a handler for an HTTP Request endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class EndpointAttribute : Attribute
{
    /// <summary>
    /// The route HTTP method.
    /// </summary>
    public string Method { get; private set; }

    /// <summary>
    /// The route for the endpoint.
    /// </summary>
    public string Route { get; private set; }

    /// <summary>
    /// A name for the HTTP endpoint used for API documentation and routing.
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// A description for the HTTP endpoint used for API documentation.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Constructs a new instance of the endpoint attribute.
    /// </summary>
    public EndpointAttribute(string method, string route, string? name = null, string? description = null)
    {
        Method = method;
        Route = route;
        Name = name;
        Description = description;
    }
}