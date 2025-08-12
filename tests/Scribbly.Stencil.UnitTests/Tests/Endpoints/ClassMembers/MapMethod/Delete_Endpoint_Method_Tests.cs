using FluentAssertions;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.MapMethod;

// ReSharper disable once InconsistentNaming
public partial class Delete_Endpoint_Method_Tests
{
    [DeleteEndpoint("/class-test-del")]
    private static void Delete(){ }
    
    [Fact]
    public void Type_Should_Contain_EndpointMappingExtensionMethod_Name()
    {
        GetType()
            .GetMethods()
            .Where(m => m.Name == $"Map{nameof(Delete_Endpoint_Method_Tests)}{nameof(Delete)}")
            .Should()
            .ContainSingle();
    }
    
    [Fact]
    public void Method_Should_Contain_Builder_Parameter()
    {
        GetType()
            .GetMethods()
            .Single(m => m.Name == $"Map{nameof(Delete_Endpoint_Method_Tests)}{nameof(Delete)}")
            .GetParameters()
            .First()
            .Name
            .Should()
            .Be("builder");
    }
    
    [Fact]
    public void Method_Should_Contain_Builder_Parameter_Type()
    {
        GetType()
            .GetMethods()
            .Single(m => m.Name == $"Map{nameof(Delete_Endpoint_Method_Tests)}{nameof(Delete)}")
            .GetParameters()
            .First()
            .ParameterType
            .Name
            .Should()
            .Be("IEndpointRouteBuilder");
    }
    
    [Fact]
    public void Method_Should_Return_Builder_Parameter_Type()
    {
        GetType()
            .GetMethods()
            .Single(m => m.Name == $"Map{nameof(Delete_Endpoint_Method_Tests)}{nameof(Delete)}")
            .ReturnParameter
            .ParameterType
            .Name
            .Should()
            .Be("RouteHandlerBuilder");
    }
}