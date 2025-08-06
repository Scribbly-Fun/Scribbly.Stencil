using System.Net;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsAttachedGroup;

[Collection(Collections.Api)]
public class GetRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Get_Request_When_Method_Delegated_To_Group_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.GetAsync("/cookbook/british-food/MY_ID");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}