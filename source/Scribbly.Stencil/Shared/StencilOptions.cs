namespace Scribbly.Stencil;

/// <summary>
/// Options for the scribbly stencil setup.
/// </summary>
public class StencilOptions
{
    /// <summary>
    /// All Stencil Group Classes will be registered with the provided scope.
    /// </summary>
    public ServiceScope GroupsScope { get; set; } = ServiceScope.Transient;
    
    /// <summary>
    /// All Stencil Endpoint Classes will be registered with the provided scope.
    /// </summary>
    public ServiceScope EndpointsScope { get; set; } = ServiceScope.Transient;
    
    /// <summary>
    /// The service lifetime.
    /// </summary>
    public enum ServiceScope
    {
        /// <summary>
        /// Instance per lifetime,
        /// </summary>
        Transient,
        /// <summary>
        /// Instance per scope.
        /// </summary>
        Scoped,
        /// <summary>
        /// Single instance per container.
        /// </summary>
        Singleton
    }
}