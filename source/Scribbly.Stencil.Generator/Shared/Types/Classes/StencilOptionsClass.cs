namespace Scribbly.Stencil.Types.Classes;

internal static class StencilOptionsClass
{
    public const string FileName = "StencilOptions.g.cs";
    public const string Value = """
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
                                    public global::Microsoft.Extensions.DependencyInjection.ServiceLifetime ServicesScope { get; set; } = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient;
                                }
                                """;
}
