using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Lunch;

public partial class LunchEndpoints
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("/{id}", "Gets Lunch", "Queries a new Lunch Item")]
    [GroupMember<LunchGroup>]
    [Configure]
    private static IResult GetLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("/{id}", "Create Lunch", "Creates a new Lunch Item")]
    [GroupMember<LunchGroup>]
    private static object PostLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("/{id}", "Edit Lunch", "Edits a Lunch Item")]
    [GroupMember<LunchGroup>]
    private static object PutLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }

    /// <inheritdoc />
    public void ConfigureGetLunchMenu(RouteHandlerBuilder getLunchMenuBuilder)
    {
    }
}