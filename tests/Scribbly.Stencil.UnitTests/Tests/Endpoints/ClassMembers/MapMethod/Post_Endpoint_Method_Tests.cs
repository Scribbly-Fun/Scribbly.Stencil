using FluentAssertions;

namespace Scribbly.Stencil.UnitTests.Tests.Endpoints.ClassMembers.MapMethod;

// ReSharper disable once InconsistentNaming
public partial class Post_Endpoint_Method_Tests
{
    [PostEndpoint("/class-test-post")]
    private static void Post(){ }
    
    [Fact]
    public void Type_Should_Contain_EndpointMappingExtensionMethod_Name()
    {
        GetType()
            .GetMethods()
            .Where(m => m.Name == $"Map{nameof(Post_Endpoint_Method_Tests)}{nameof(Post)}")
            .Should()
            .ContainSingle();
    }
    
    [Fact]
    public void Method_Should_Contain_Builder_Parameter()
    {
        GetType()
            .GetMethods()
            .Single(m => m.Name == $"Map{nameof(Post_Endpoint_Method_Tests)}{nameof(Post)}")
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
            .Single(m => m.Name == $"Map{nameof(Post_Endpoint_Method_Tests)}{nameof(Post)}")
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
            .Single(m => m.Name == $"Map{nameof(Post_Endpoint_Method_Tests)}{nameof(Post)}")
            .ReturnParameter
            .ParameterType
            .Name
            .Should()
            .Be("IEndpointRouteBuilder");
    }
}