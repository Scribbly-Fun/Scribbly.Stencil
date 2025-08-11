using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.ConfigureInterface;

// ReSharper disable once InconsistentNaming
public partial class Get_Endpoint_ConfigureInterface_Tests
{
    [GetEndpoint("/iconfig-test-get")]
    [Configure]
    private static void Get(){ }
    
    [Fact]
    public void Type_Should_Implement_ConfigureInterface()
    {
        GetType()
            .GetInterfaces()
            .Where(m => m.Name == $"I{nameof(Get)}Configure")
            .Should()
            .ContainSingle();
    }

    /// <inheritdoc />
#pragma warning disable xUnit1013
    public void ConfigureGet(IEndpointConventionBuilder getBuilder)
#pragma warning restore xUnit1013
    {
    }
}