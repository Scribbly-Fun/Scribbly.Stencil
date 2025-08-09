namespace Scribbly.Stencil.Types.Classes;

internal static class StencilOptionsClass
{
    public const string FileName = "StencilOptions.g.cs";
    public const string Value = """
                                namesapce Scribbly.Stencil;
                                
                                /// <summary>
                                /// Options for the scribbly stencil setup.
                                /// </summary>
                                public class StencilOptions
                                {
                                    /// <summary>
                                    /// When true all endpoints and Groups will be resolves from the DI container.
                                    /// </summary>
                                    public bool UseDependencyInjection { get; set; } = true;
                                    
                                    /// <summary>
                                    /// All services will be registered with the provided scope.
                                    /// <remarks>When using a scoped service lifetime stencil will create a scope inside the UseStencil</remarks>
                                    /// </summary>
                                    public ServiceLifetime ServicesScope { get; set; } = ServiceLifetime.Transient;
                                }
                                """;
}
