namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Breakfast;

[EndpointGroup("/breakfast", "Breakfast Menu")]
[GroupMember<MenuGroup>]
[Configure]
public partial class BreakfastGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder breakfastGroupBuilder)
    {
        
    }
}