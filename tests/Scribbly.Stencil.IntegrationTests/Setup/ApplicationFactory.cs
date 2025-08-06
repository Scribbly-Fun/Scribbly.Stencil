using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Scribbly.Stencil.IntegrationTests;



/// <summary>
/// The application factory is the entry point for our testing environment.
/// The application factory creates a test container used for a temporary DB
/// and an HTTP client used to test our REST APIs
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public partial class ApplicationFactory : WebApplicationFactory<Scribbly.Stencil.Cookbook.ApiService.IAssemblyMarker>, IAsyncLifetime
{
    /// <summary>
    /// Gives a fixture an opportunity to configure the application before it gets built.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> for the application.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
    }
    
    /// <summary>
    /// Called immediately after the class has been created, before it is used.
    /// </summary>
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
        
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}