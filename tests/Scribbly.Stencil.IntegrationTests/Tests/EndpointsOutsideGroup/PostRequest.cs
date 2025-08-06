using System.Net;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsOutsideGroup;

[Collection(Collections.Api)]
public class PostRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Post_Request_When_Parent_IsExternal_Class_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.PostAsync("/cookbook/greek-food/MY_ID", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}