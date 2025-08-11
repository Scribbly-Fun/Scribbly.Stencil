using FluentAssertions;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.MapMethod;

// ReSharper disable once InconsistentNaming
public partial class Get_Endpoint_Method_Tests
{
    [GetEndpoint("/class-test-get")]
    private static void Get(){ }
    
    [Fact]
    public void Type_Should_Contain_EndpointMappingExtensionMethod_Name()
    {
        GetType()
            .GetMethods()
            .Where(m => m.Name == $"Map{nameof(Get_Endpoint_Method_Tests)}{nameof(Get)}")
            .Should()
            .ContainSingle();
    }
    
    [Fact]
    public void Method_Should_Contain_Builder_Parameter()
    {
        GetType()
            .GetMethods()
            .Single(m => m.Name == $"Map{nameof(Get_Endpoint_Method_Tests)}{nameof(Get)}")
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
            .Single(m => m.Name == $"Map{nameof(Get_Endpoint_Method_Tests)}{nameof(Get)}")
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
            .Single(m => m.Name == $"Map{nameof(Get_Endpoint_Method_Tests)}{nameof(Get)}")
            .ReturnParameter
            .ParameterType
            .Name
            .Should()
            .Be("IEndpointRouteBuilder");
    }
}