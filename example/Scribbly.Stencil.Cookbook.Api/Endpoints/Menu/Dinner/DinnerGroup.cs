namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Dinner;

[EndpointGroup("/dinner", "Dinner Menu")]
[Configure]
[GroupMember<MenuGroup>]
public partial class DinnerGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder dinnerGroupBuilder)
    {
        dinnerGroupBuilder.ProducesProblem(404);
    }
}