using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Dinner;

// [Configure]
[EndpointGroup("/dinner", "Dinner Squared")]
[GroupMember<MenuGroup>]
public partial class DinnerEndpoints
{
    // [Configure]
    [GetEndpoint("fffff/{id}", "Gets Dinner", "Queries a new Dinner Item")]
    private IResult GetDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("fffff/{id}", "Create Dinner", "Creates a new Dinner Item")]
    private object PostDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("fffff/{id}", "Edit Dinner", "Edits a Dinner Item")]
    private object PutDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
}