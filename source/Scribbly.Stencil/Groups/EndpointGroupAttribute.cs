namespace Scribbly.Stencil;

/// <summary>
/// When applied to a partial static class the class will become a handle for a group of endpoints.
/// Groups allow users to map several endpoints to a parent route such as /api/receips
/// Groups will allow users to adapt common configuration such as auth and filters.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EndpointGroupAttribute : Attribute
{
    /// <summary>
    /// The route prefix for the group of endpoints
    /// </summary>
    public string RoutePrefix { get; }
    
    /// <summary>
    /// The group of endpoints tag name used for Open API
    /// </summary>
    public string? Tag { get; }

    /// <summary>
    /// When applied to a partial static class the class will become a handle for a group of endpoints.
    /// Groups allow users to map several endpoints to a parent route such as /api/receips
    /// Groups will allow users to adapt common configuration such as auth and filters.
    /// </summary>
    /// <param name="routePrefix">The route prefix for the group of endpoints</param>
    /// <param name="tag">The group of endpoints tag name used for Open API</param>
    public EndpointGroupAttribute(string routePrefix, string? tag = null)
    {
        RoutePrefix = routePrefix;
        Tag = tag;
    }
}