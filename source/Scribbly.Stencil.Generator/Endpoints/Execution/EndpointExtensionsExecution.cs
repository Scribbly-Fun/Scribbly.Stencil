using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Endpoints.Factories;
using Scribbly.Stencil.Factories;

namespace Scribbly.Stencil.Endpoints;

using Microsoft.CodeAnalysis;

public class EndpointExtensionsExecution
{
    /// <summary>
    /// Writes the captured information about the handle method as a Minimal API endpoint.
    /// </summary>
    /// <param name="context">Generator Context</param>
    /// <param name="captureContext">Captured endpoint combined with the endpoints Context</param>
    public static void Generate(SourceProductionContext context, (TargetMethodCaptureContext subject, BuilderCaptureContext? builderCtx) captureContext)
    {
        var (subject, builderCtx) = captureContext;
        
        var includeNamespace = subject.Namespace is not null;
        
        var builder = FileHeaderFactory.CreateFileHeader()
            .Append(
                """

                using Microsoft.AspNetCore.Builder;
                using Microsoft.AspNetCore.Http;
                using Microsoft.AspNetCore.Mvc;
                using Microsoft.AspNetCore.Routing;
                
                """)
            .AppendLine()
            .AppendCondition("namespace ", includeNamespace).AppendCondition(subject.Namespace, includeNamespace).AppendLine(";")
            .AppendLine()
            .Append("public static class ").Append(subject.TypeName).Append(subject.MethodName).AppendLine("Extensions")
            .Append("""
                    {
                        /// <summary>
                        /// Maps the endpoint 
                    """)
            .Append(subject.TypeName).Append(" to an endpoint builder ").Append(subject.MemberOf).Append(" with the route ").Append(subject.HttpMethod).Append(" ").AppendLine(subject.HttpRoute)
            .Append("""
                        /// </summary>
                        public static global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder 
                    """)
            .CreateHandleExtensionMethodDeclaration(subject, builderCtx)
            .AppendLine()
            .AppendLine("    {")
            .CreateNewEndpoint(subject, builderCtx)
            .AppendLine()
            .Append("        var endpointBuilder = scribblyEndpoint.").CreateEndpointMappingMethodName(subject).AppendLine("(builder);")
            .CreateConfigureInvocation(subject)
            .Append("""
                            
                            return endpointBuilder;
                        }
                    }
                    """);
        
        var endpointName = subject.Namespace is null ? $"{subject.TypeName}.{subject.MethodName}" : $"{subject.Namespace}.{subject.TypeName}.{subject.MethodName}";
        context.AddSource($"Endpoint.{endpointName}.Extensions.g.cs", builder.ToString());
    }
}