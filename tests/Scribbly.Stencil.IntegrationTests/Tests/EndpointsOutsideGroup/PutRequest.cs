using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsOutsideGroup;

[Collection(Collections.Api)]
public class PutRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Put_Request_When_Parent_IsExternal_Class_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.PutAsJsonAsync("/cookbook/greek-food/MY_ID", new {Id = Guid.NewGuid()});
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}