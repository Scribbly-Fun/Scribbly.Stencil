using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsInsideGroup;

[Collection(Collections.Api)]
public class PostRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Post_Request_Inside_ParentGroup_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.PostAsync("/cookbook/indian-food/MY_ID", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}