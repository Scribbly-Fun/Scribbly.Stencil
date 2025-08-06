using System.Net;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsAttachedGroup;

[Collection(Collections.Api)]
public class PostRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Post_Request_When_Method_Delegated_To_Group_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.PostAsync("/cookbook/british-food/MY_ID", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}