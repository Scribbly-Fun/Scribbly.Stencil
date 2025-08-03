using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Scribbly.Stencil;

public interface IGroupConfigurationHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    static IEndpointRouteBuilder Configure(IEndpointRouteBuilder builder)
    {
        return builder;   
    }
}

[EndpointGroup("/lunch")]
public static partial class LunchGroup 
{

    // ----------------------------------------> THE CODE USER WILL WRITE
   
    [GetEndpoint("create/{id}", "Get Lunch", "Fetches the current lunch for the provided restaurant ID.")]
    [EndpointBuilder]
    private static IResult GetLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    } 
    
    static partial void ConfigureMapGetLunchMenu(IEndpointConventionBuilder builder)
    {
        builder.WithDescription("Creates a new group");  // ---------------> Bad example because the endpoint Attributes should include this by default
    }

    // --------------> Generated Code Extension maps each endpoint.
    public static IEndpointRouteBuilder MapGetLunchMenu(this IEndpointRouteBuilder builder)
    {
        var endpoint = builder.MapPost("create/{id}", GetLunchMenu);
        
        // ------------------> If Endpoint Build is declared call the configure method
        ConfigureMapGetLunchMenu(endpoint);
        
        return builder;
    }
    
    // ------------------------> GET Generated Only when an Endpoint handler includes a EndpointBuilder att
    static partial void ConfigureMapGetLunchMenu(IEndpointConventionBuilder builder);
}

public static partial class LunchGroup
{
    public static IEndpointRouteBuilder MapLunchGroup(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("/lunch")
            .MapGetLunchMenu();
        
        return group;
    }
}