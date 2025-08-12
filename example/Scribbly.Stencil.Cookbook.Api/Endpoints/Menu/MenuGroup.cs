using Microsoft.Extensions.Options;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;

public class MyOptions
{
    public string[] Tags { get; init; } = [];
}

[EndpointGroup("/menu", "Manage Menu Items")]
[Configure]
public partial class MenuGroup(IOptions<MyOptions> options)
{
    /// <inheritdoc />
    public void Configure(RouteGroupBuilder menuGroupBuilder)
    {

    }
}