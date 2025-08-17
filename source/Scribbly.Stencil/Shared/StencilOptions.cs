using Microsoft.Extensions.DependencyInjection;

namespace Scribbly.Stencil;

/// <summary>
/// Options for the scribbly stencil setup.
/// </summary>
public class StencilOptions
{
    /// <summary>
    /// All services will be registered with the provided scope.
    /// <remarks>When using a scoped service lifetime stencil will create a scope inside the UseStencil</remarks>
    /// </summary>
    public ServiceLifetime ServicesScope { get; set; } = ServiceLifetime.Transient;
}