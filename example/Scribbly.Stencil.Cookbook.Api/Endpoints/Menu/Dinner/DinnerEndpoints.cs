using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Dinner;

[Configure] 
[EndpointGroup("/dinner/2", "Dinner Squared")]
[GroupMember<MenuGroup>]
public partial class DinnerEndpoints
{
    [GetEndpoint("/{id}", "Gets Dinner", "Queries a new Dinner Item")]
    private IResult GetDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("/{id}", "Create Dinner", "Creates a new Dinner Item")]
    private object PostDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("/{id}", "Edit Dinner", "Edits a Dinner Item")]
    private object PutDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
    
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder dinnerEndpointsBuilder)
    {
        
    }

    /// <inheritdoc />
    public void ConfigureGetDinnerMenu(IEndpointConventionBuilder getDinnerMenuBuilder)
    {
    }

    /// <inheritdoc />
    public void ConfigurePostDinnerMenu(IEndpointConventionBuilder postDinnerMenuBuilder)
    {
    }

    /// <inheritdoc />
    public void ConfigurePutDinnerMenu(IEndpointConventionBuilder putDinnerMenuBuilder)
    {
    }

}