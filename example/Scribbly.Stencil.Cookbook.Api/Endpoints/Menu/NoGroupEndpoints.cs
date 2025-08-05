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
    static partial void ConfigureGetLunchMenu(IEndpointConventionBuilder builder)
    {
        builder
            .WithRequestTimeout(TimeSpan.FromSeconds(1))
            .ProducesProblem(400).WithDisplayName("DISPLAY NAME");
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
}