namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;

[EndpointGroup("/menu", "Manage Menu Items")]
[Configure]
public partial class MenuGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder applicationRootBuilder)
    {
        applicationRootBuilder.WithTags("Customer");
    }
}