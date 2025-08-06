using System.Net;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsOutsideGroup;

[Collection(Collections.Api)]
public class DeleteRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Delete_Request_When_Parent_IsExternal_Class_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.DeleteAsync("/cookbook/greek-food/MY_ID");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}