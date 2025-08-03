using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Lunch;

// [EndpointGroup("/lunch", "Manage Lunch")]
static partial class Group
{
    
}

[EndpointGroup("/lunch")]
public static partial class LunchGroup
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("lunch/{id}")]
    private static IResult GetLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    static partial void ConfigureGetLunchMenu(IEndpointConventionBuilder builder)
    {
        builder
            .WithRequestTimeout(TimeSpan.FromSeconds(1))
            .ProducesProblem(400).WithDisplayName("DISPLAY NAME");
    }

    [PostEndpoint("lunch/{id}", "Create Lunch", "Creates a new Lunch Item")]
    private static IResult PostLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("lunchs/{id}", "Edit Lunch", "Edits a Lunch Item")]
    private static object PutLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
}