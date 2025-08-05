using Microsoft.AspNetCore.Mvc;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Dinner;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Dinner;

public partial class DinnerEndpoints
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("/{id}", "Gets Dinner", "Queries a new Dinner Item")]
    [GroupMember<DinnerGroup>]
    [Configure]
    private static IResult GetDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("/{id}", "Create Dinner", "Creates a new Dinner Item")]
    [GroupMember<DinnerGroup>]
    private static object PostDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("/{id}", "Edit Dinner", "Edits a Dinner Item")]
    [GroupMember<DinnerGroup>]
    private static object PutDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }

    /// <inheritdoc />
    public void ConfigureGetDinnerMenu(IEndpointConventionBuilder getDinnerMenuBuilder)
    {
        
    }
    
}