using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Breakfast;

public partial class BreakfastEndpoints
{
    [PutEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    [GroupMember<BreakfastGroup>]
    private static object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
}