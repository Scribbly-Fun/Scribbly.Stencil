namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Breakfast;

[EndpointGroup("/breakfast", "Breakfast Menu")]
[Configure]
[GroupMember<MenuGroup>]
public partial class BreakfastGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder breakfastGroupBuilder)
    {
        breakfastGroupBuilder.ProducesProblem(404);
    }
}