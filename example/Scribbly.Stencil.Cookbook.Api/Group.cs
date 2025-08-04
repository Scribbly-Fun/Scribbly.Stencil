using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService;

[EndpointGroup("/api", "Application Root")]
[Configure]
public partial class ApplicationRoot
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder applicationRootBuilder)
    {
        // applicationRootBuilder.AddEndpointFilter<MyFilter>();
        // -------------------> Add root configuration to all children
    }
}

[EndpointGroup("/lunch", "Manage Lunch Menu"), Configure, GroupMember<ApplicationRoot>]
public partial class LunchGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder lunchGroupBuilder)
    {
        throw new NotImplementedException();
    }
}

[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<LunchGroup>]
public partial class Dinner
{
}

public static partial class LunchEndpoints
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("lunch/{id}")]
    [GroupMember<LunchGroup>]
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
    [GroupMember<LunchGroup>]
    private static IResult PostLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("lunchs/{id}", "Edit Lunch", "Edits a Lunch Item")]
    [GroupMember<LunchGroup>]
    private static object PutLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
}