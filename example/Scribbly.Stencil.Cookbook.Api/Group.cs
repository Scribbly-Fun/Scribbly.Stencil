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

[EndpointGroup("/lunch", "Manage Lunch Menu"), Configure]
[GroupMember<ApplicationRoot>]
public partial class LunchGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder lunchGroupBuilder)
    {
        // throw new NotImplementedException();
    }
}

[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup
{
}
[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup2
{
}
[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup3
{
}
[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup4
{
}

[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup5
{
}

[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup7
{
}
[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup8
{
}
[EndpointGroup("/dinner", "Manage Dinner Menu")]
[GroupMember<ApplicationRoot>]
public partial class BreakfastGroup9
{
}


[EndpointGroup("/test")]
[GroupMember<ApplicationRoot>]
[Configure]
public partial class DinnerGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder dinnerGroupBuilder)
    {
        // throw new NotImplementedException();
    }
}

public static partial class LunchEndpoints
{

    // ----------------------------------------> THE CODE USER WILL WRITE
    
    [GetEndpoint("lunch/{id}", "Gets Lunch", "Queries a new Lunch Item")]
    [GroupMember<ApplicationRoot>]
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
    [GroupMember<ApplicationRoot>]
    private static object PostLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("lunchs/{id}", "Edit Lunch", "Edits a Lunch Item")]
    [GroupMember<ApplicationRoot>]
    private static object PutLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
}