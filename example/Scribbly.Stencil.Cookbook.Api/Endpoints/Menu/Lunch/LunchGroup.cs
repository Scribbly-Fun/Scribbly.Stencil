namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Lunch;

[EndpointGroup("/lunch", "Lunch Menu")]
[Configure]
[GroupMember<MenuGroup>]
public partial class LunchGroup
{

    /// <inheritdoc />
    public void Configure(RouteGroupBuilder lunchGroupBuilder)
    {
    }
}