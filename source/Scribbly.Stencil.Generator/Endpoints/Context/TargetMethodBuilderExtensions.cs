namespace Scribbly.Stencil.Endpoints;

public static class TargetMethodBuilderExtensions
{
    public static string CreateEndpointMappingMethod(this TargetMethodCaptureContext subject)
    {
        if (subject.HasConfigurationHandler)
        {
            return $@"    
    /// <summary>
    /// Maps the method {subject.MethodName} to an Endpoint group with the Route {subject.HttpMethod?.ToUpper()} {subject.HttpRoute} and configures the endpoint with the provided options.
    /// </summary>
    public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder Map{subject.TypeName}{subject.MethodName}Endpoint(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)
    {{
        var endpoint = builder.Map{subject.HttpMethod}(""{subject.HttpRoute}"", {subject.MethodName});
        
        Configure{subject.TypeName}{subject.MethodName}Endpoint(endpoint);

        return builder;
    }}

        static partial void Configure{subject.TypeName}{subject.MethodName}Endpoint(IEndpointConventionBuilder builder);";
        }
        
        return $@"    
    /// <summary>
    /// Maps the method {subject.MethodName} to an Endpoint group with the Route {subject.HttpMethod?.ToUpper()} {subject.HttpRoute}.
    /// </summary>
    public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder Map{subject.TypeName}{subject.MethodName}Endpoint(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)
    {{
        builder.Map{subject.HttpMethod}(""{subject.HttpRoute}"", {subject.MethodName});
        return builder;
    }}";
    }
}