namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Cookbook;

[EndpointGroup("indian-food", "Indian-Food")]
[GroupMember<Api>]
public partial class IndianFood
{
    private record EditRequest(string Id);
    
    [GetEndpoint("/{id}", "GetIndianRecipe", "Gets an Indian Recipe")]
    private static IResult Get(HttpContext context, string id)
    {
        return Results.Ok(new { Receipe = id });
    }
    
    [PostEndpoint("/{id}", "PostIndianRecipe", "Creates an Indian Recipe")]
    private static object Post(HttpContext context, string id)
    {
        return Results.CreatedAtRoute("GetIndianRecipe", new { id = id }, new { Receipe = id });
    }
    
    [PutEndpoint("/{id}", "PutIndianRecipe", "Edits an Indian Recipe")]
    private static object Put(HttpContext context, string id, EditRequest request)
    {
        return Results.Ok(request);
    }
    
    [DeleteEndpoint("/{id}", "DeleteIndianRecipe", "Removes an Indian Recipe")]
    private static object Delete(HttpContext context, string id)
    {
        return new { id = id };
    }
}