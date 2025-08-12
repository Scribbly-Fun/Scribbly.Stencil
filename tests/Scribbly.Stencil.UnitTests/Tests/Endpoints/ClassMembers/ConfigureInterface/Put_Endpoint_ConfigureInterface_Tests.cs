using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.ConfigureInterface;

// ReSharper disable once InconsistentNaming
public partial class Put_Endpoint_ConfigureInterface_Tests
{
    [PutEndpoint("/iconfig-test-put")]
    [Configure]
    private static void Put(){ }
    
    [Fact]
    public void Type_Should_Implement_ConfigureInterface()
    {
        GetType()
            .GetInterfaces()
            .Where(m => m.Name == $"I{nameof(Put)}Configure")
            .Should()
            .ContainSingle();
    }

    /// <inheritdoc />
#pragma warning disable xUnit1013
    public void ConfigurePut(RouteHandlerBuilder putBuilder)
#pragma warning restore xUnit1013
    {
    }
}