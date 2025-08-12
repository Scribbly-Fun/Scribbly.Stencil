using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.ConfigureInterface;

// ReSharper disable once InconsistentNaming
public partial class Post_Endpoint_ConfigureInterface_Tests
{
    [PostEndpoint("/iconfig-test-post")]
    [Configure]
    private static void Post(){ }
    
    [Fact]
    public void Type_Should_Implement_ConfigureInterface()
    {
        GetType()
            .GetInterfaces()
            .Where(m => m.Name == $"I{nameof(Post)}Configure")
            .Should()
            .ContainSingle();
    }

    /// <inheritdoc />
#pragma warning disable xUnit1013
    public void ConfigurePost(RouteHandlerBuilder postBuilder)
#pragma warning restore xUnit1013
    {
    }
}