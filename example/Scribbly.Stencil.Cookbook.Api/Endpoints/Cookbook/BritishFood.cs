using Microsoft.AspNetCore.Mvc;

namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Cookbook;

[EndpointGroup("/food")]
public partial class NotAGroup
{
    
}

[EndpointGroup("british-food", "British-Food")]
[GroupMember<Api>]
public partial class BritishGroup;

public partial class BritishFood
{
    private record EditRequest(string Id);
    
    [GroupMember<BritishGroup>]
    [GetEndpoint("/{id}", "GetBritishRecipe", "Gets an British Recipe")]
    private static IResult Get(HttpContext context, string id)
    {
        return Results.Ok(new { Receipe = "Bad Food" });
    }
    
    [GroupMember<BritishGroup>]
    [PostEndpoint("/{id}", "PostBritishRecipe", "Creates an British Recipe")]
    private static object Post(HttpContext context, string id)
    {
        return Results.CreatedAtRoute("GetBritishRecipe", new { id }, new { Receipe = "Bad Food"  });
    }
    
    [GroupMember<BritishGroup>]
    [PutEndpoint("/{id}", "PutBritishRecipe", "Edits an British Recipe")]
    private static object Put(HttpContext context, string id, EditRequest request)
    {
        return Results.Ok(new EditRequest(Id: "Bad Food"));
    }
    
    [GroupMember<BritishGroup>]
    [DeleteEndpoint("/{id}", "DeleteBritishRecipe", "Removes an British Recipe")]
    private static object Delete(HttpContext context, string id)
    {
        return new { id = "Bad Food" };
    }
}