using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Scribbly.Stencil.IntegrationTests.EndpointsInsideGroup;

[Collection(Collections.Api)]
public class PutRequest(ApplicationFactory application)
{
    [Fact]
    public async Task Put_Request_Inside_ParentGroup_Should_InvokeHandler()
    {
        using var client = application.CreateClient();
        using var response = await client.PutAsJsonAsync("/cookbook/indian-food/MY_ID", new {Id = Guid.NewGuid()});
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}