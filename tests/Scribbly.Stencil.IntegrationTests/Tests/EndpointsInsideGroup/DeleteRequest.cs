using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsInsideGroup;

[Collection(Collections.Api)]
public class DeleteRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Delete_Request_Inside_ParentGroup_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.DeleteAsync("/cookbook/indian-food/MY_ID");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}