using System.Net;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsAttachedGroup;

[Collection(Collections.Api)]
public class DeleteRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Delete_Request_When_Method_Delegated_To_Group_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.DeleteAsync("/cookbook/british-food/MY_ID");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}