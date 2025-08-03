using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Lunch;

// [EndpointGroup("/lunch", "Manage Lunch")]
static partial class Group
{
    
}

// [EndpointGroup("/lunch")]
public static partial class LunchGroup
{

    // ----------------------------------------> THE CODE USER WILL WRITE

    // [EndpointBuilder]
    [GetEndpoint("create/{id}", "Get Lunch", "Fetches the current lunch for the provided restaurant ID.")]
    private static IResult GetLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("create/{id}", "Get Lunch", "Fetches the current lunch for the provided restaurant ID.")]
    private static IResult PostLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
}