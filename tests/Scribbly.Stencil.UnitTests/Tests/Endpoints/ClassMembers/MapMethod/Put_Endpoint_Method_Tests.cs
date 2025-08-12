using FluentAssertions;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.MapMethod;

// ReSharper disable once InconsistentNaming
public partial class Put_Endpoint_Method_Tests
{
    [GetEndpoint("/class-test-put")]
    private static void Put(){ }
    
    [Fact]
    public void Type_Should_Contain_EndpointMappingExtensionMethod_Name()
    {
        GetType()
            .GetMethods()
            .Where(m => m.Name == $"Map{nameof(Put_Endpoint_Method_Tests)}{nameof(Put)}")
            .Should()
            .ContainSingle();
    }
    
    [Fact]
    public void Method_Should_Contain_Builder_Parameter()
    {
        GetType()
            .GetMethods()
            .Single(m => m.Name == $"Map{nameof(Put_Endpoint_Method_Tests)}{nameof(Put)}")
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
            .Single(m => m.Name == $"Map{nameof(Put_Endpoint_Method_Tests)}{nameof(Put)}")
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
            .Single(m => m.Name == $"Map{nameof(Put_Endpoint_Method_Tests)}{nameof(Put)}")
            .ReturnParameter
            .ParameterType
            .Name
            .Should()
            .Be("RouteHandlerBuilder");
    }
}