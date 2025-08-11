namespace Scribbly.Stencil.Cookbook.ApiService.Endpoints.Cookbook;

[EndpointGroup("greek-food", "Greek-Food")]
[GroupMember<Api>]
public partial class GreekGroup
{
    
}

[GroupMember<GreekGroup>]
public partial class GreekFood
{
    private record EditRequest(string Id);
    
    [GetEndpoint("/{id}", "GetGreekRecipe", "Gets an Greek Recipe")]
    private static IResult Get(HttpContext context, string id)
    {
        return Results.Ok(new { Receipe = id });
    }
    
    [PostEndpoint("/{id}", "PostGreekRecipe", "Creates an Greek Recipe")]
    private static object Post(HttpContext context, string id)
    {
        return Results.CreatedAtRoute("GetGreekRecipe", new { id = id }, new { Receipe = id });
    }
    
    [PutEndpoint("/{id}", "PutGreekRecipe", "Edits an Greek Recipe")]
    private static object Put(HttpContext context, string id, EditRequest request)
    {
        return Results.Ok(request);
    }
    
    [DeleteEndpoint("/{id}", "DeleteGreekRecipe", "Removes an Greek Recipe")]
    private static object Delete(HttpContext context, string id)
    {
        return new { id = id };
    }
}