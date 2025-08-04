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
//
// [EndpointGroup("/api", "Root API")]
// [Configure]
// public partial class AppRoot
// {
//     /// <inheritdoc />
//     public void Configure(IEndpointConventionBuilder appRootBuilder)
//     {
//         appRootBuilder.WithTags("Customer");
//     }
// }