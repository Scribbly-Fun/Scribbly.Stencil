using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;


public partial class NoGroupEndpoints
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("no-group/{id}")]
    private static IResult GetLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }

    [PostEndpoint("no-group/{id}")]
    private static object PostLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("no-group/{id}")]
    private static object PutLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }

    /// <inheritdoc />
    public void ConfigureGetLunchMenu(IEndpointConventionBuilder getLunchMenuBuilder)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void ConfigurePostLunchMenu(IEndpointConventionBuilder postLunchMenuBuilder)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void ConfigurePutLunchMenu(IEndpointConventionBuilder putLunchMenuBuilder)
    {
        throw new NotImplementedException();
    }
}