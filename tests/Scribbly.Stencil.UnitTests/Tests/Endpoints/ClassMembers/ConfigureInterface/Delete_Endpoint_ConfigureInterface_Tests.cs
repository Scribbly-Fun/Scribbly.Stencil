using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.ConfigureInterface;

// ReSharper disable once InconsistentNaming
public partial class Delete_Endpoint_ConfigureInterface_Tests
{
    [DeleteEndpoint("/iconfig-test-del")]
    [Configure]
    private static void Delete(){ }
    
    [Fact]
    public void Type_Should_Implement_ConfigureInterface()
    {
        GetType()
            .GetInterfaces()
            .Where(m => m.Name == $"I{nameof(Delete)}Configure")
            .Should()
            .ContainSingle();
    }

    /// <inheritdoc />
#pragma warning disable xUnit1013
    public void ConfigureDelete(RouteHandlerBuilder getBuilder)
#pragma warning restore xUnit1013
    {
    }
}