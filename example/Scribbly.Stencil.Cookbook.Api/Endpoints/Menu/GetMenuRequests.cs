using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;

public static partial class GetMenuRequests
{
    [GetEndpoint("/", "GetMenus", "Queries a new Lunch Item")]
    [GroupMember<MenuGroup>]
    private static IResult GetMenus(HttpContext context)
    {
        return Results.Ok();
    }
    
    [GetEndpoint("/{id}", "GetMenu", "Queries a new Lunch Item")]
    [GroupMember<MenuGroup>]
    private static IResult GetMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
}