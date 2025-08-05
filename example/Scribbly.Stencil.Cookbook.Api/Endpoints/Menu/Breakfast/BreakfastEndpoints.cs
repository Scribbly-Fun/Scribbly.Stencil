using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Breakfast;

public partial class BreakfastEndpoints
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("/{id}", "Gets Breakfast", "Queries a new Breakfast Item")]
    [GroupMember<BreakfastGroup>]
    private static IResult GetBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("/{id}", "Create Breakfast", "Creates a new Breakfast Item")]
    [GroupMember<BreakfastGroup>]
    private static object PostBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    [GroupMember<BreakfastGroup>]
    private static object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }

    /// <inheritdoc />
    public void ConfigureGetBreakfastMenu(IEndpointConventionBuilder getBreakfastMenuBuilder)
    {
        Console.WriteLine();
    }

    /// <inheritdoc />
    public void ConfigurePutBreakfastMenu(IEndpointConventionBuilder putBreakfastMenuBuilder)
    {
        Console.WriteLine();
    }

    /// <inheritdoc />
    public void ConfigurePostBreakfastMenu(IEndpointConventionBuilder postBreakfastMenuBuilder)
    {
        Console.WriteLine();
    }
}