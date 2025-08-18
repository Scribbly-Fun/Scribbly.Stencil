using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Factories;

namespace Scribbly.Stencil.Endpoints;

public static class EndpointHandlerExecution
{
    /// <summary>
    /// Writes the captured information about the handle method as a Minimal API endpoint.
    /// </summary>
    /// <param name="context">Generator Context</param>
    /// <param name="subject">Captured Method Context</param>
    public static void Generate(SourceProductionContext context, TargetMethodCaptureContext subject)
    {
        
        var codeBuilder = FileHeaderFactory.CreateFileHeader();
            
        if (subject.IsConfigurable)
        {
            codeBuilder.GenerateConfiguredEndpoint(subject);
        }
        else
        {
            codeBuilder.GenerateSimpleEndpoint(subject);
        }
        
        var handlerName = subject.Namespace is null ? $"{subject.TypeName}.{subject.MethodName}" : $"{subject.Namespace}.{subject.TypeName}.{subject.MethodName}";
        context.AddSource($"Handler.{handlerName}.g.cs", codeBuilder.ToString());
    }

    private static void GenerateConfiguredEndpoint(this StringBuilder builder, TargetMethodCaptureContext subject)
    {
        var includeNamespace = subject.Namespace is not null;
        
        var span = new Span<char>(subject.MethodName!.ToCharArray());
        span[0] = char.ToLowerInvariant(span[0]);
        var builderParameter = new string(span.ToArray());

        builder
            .AppendUsingImports(subject)
            .AppendLine()
            .AppendCondition("namespace ", includeNamespace).AppendCondition(subject.Namespace, includeNamespace).AppendLine(";")
            .AppendLine()
            .Append("public partial class ").Append(subject.TypeName).Append(": global::").Append(subject.Namespace).Append(".").Append(subject.TypeName).Append(".").Append("I").Append(subject.MethodName).AppendLine("Configure")
            .Append("""
                    {
                        public interface I
                    """)
            .Append(subject.MethodName).Append("Configure: global::Scribbly.Stencil.").Append(ConfigureMarkerInterface.TypeName)
            .Append("""
                        { 
                            void Configure
                    """)
            .Append(subject.MethodName).Append("(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder ").Append(builderParameter).AppendLine("Builder);")
            .Append("""
                        }
                        /// <summary>
                        /// Maps the method 
                    """)
            .Append(subject.MethodName).Append(" to an Endpoint group with the Route ").Append(subject.HttpMethod?.ToUpper()).Append(" ").Append(subject.HttpRoute).AppendLine(".")
            .Append("""
                        /// </summary>
                        public global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder Map
                    """)
            .Append(subject.TypeName).Append(subject.MethodName).AppendLine("(global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)")
            .Append("""
                        {
                            var endpoint = builder.Map
                    """)
            .Append(subject.HttpMethod).Append("(\"").Append(subject.HttpRoute).Append("\"").Append(", ").Append(subject.MethodName).AppendLine(");")
            .AppendApiDocumentation(subject)
            .Append("""
                    
                            return endpoint;
                        }
                    }
                    """);
    }

    private static void GenerateSimpleEndpoint(this StringBuilder builder, TargetMethodCaptureContext subject)
    {
        var includeNamespace = subject.Namespace is not null;

        builder
            .AppendUsingImports(subject)
            .AppendLine()
            .AppendCondition("namespace ", includeNamespace).AppendCondition(subject.Namespace, includeNamespace) .AppendLine(";")
            .AppendLine()
            .Append("public partial class ").AppendLine(subject.TypeName)
            .Append("""
                    {
                        /// <summary>
                        /// Maps the method 
                    """)
            .Append(subject.MethodName).Append(" to an Endpoint group with the Route ").Append(subject.HttpMethod?.ToUpper()).Append(" ").Append(subject.HttpRoute).AppendLine(".")
            .Append("""
                        /// </summary>
                        public global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder Map
                    """)
            .Append(subject.TypeName).Append(subject.MethodName).AppendLine("(global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)")
            .Append("""
                        {
                            var endpoint = builder.Map
                    """)
            .Append(subject.HttpMethod).Append("(\"").Append(subject.HttpRoute).Append("\"").Append(", ").Append(subject.MethodName).AppendLine(");")
            .AppendApiDocumentation(subject)
            .Append("""

                            return endpoint;
                        }
                    }
                    """)
            ;
    }

    private static StringBuilder AppendApiDocumentation(this StringBuilder builder, TargetMethodCaptureContext subject)
    {
        return subject switch
        {
            { Name: not null, Description: null } => 
                builder.Append("        endpoint.WithName(\"").Append(subject.Name).AppendLine("\");"),
            { Name: null, Description: not null } => 
                builder.Append("        endpoint.WithDescription(\"").Append(subject.Description).AppendLine("\");"),
            { Name: not null, Description: not null } => 
                builder.AppendLine(
                            """
                                    endpoint
                            """)
                    .Append("            .WithName(\"").Append(subject.Name).AppendLine("\")")
                    .Append("            .WithDescription(\"").Append(subject.Description).AppendLine("\");"),
            _ => builder
        };
    }

    private static StringBuilder AppendUsingImports(this StringBuilder builder, TargetMethodCaptureContext subject)
    {
        return builder
            .Append(
                """

                using Microsoft.AspNetCore.Builder;
                using Microsoft.AspNetCore.Http;
                using Microsoft.AspNetCore.Mvc;
                using Microsoft.AspNetCore.Routing;
                """)
            .AppendLine()
            .Append("// -------------> PARENT: ").AppendLine(subject.MemberOf)
            .Append("// -------------> GROUP_MODE: ").AppendLine(subject.GroupMode.ToString())
            .Append("// -------------> IS_GROUP: ").AppendLine(subject.IsEndpointGroup.ToString())
            .Append("// -------------> CONFIG_MODE: ").AppendLine(subject.ConfigurationMode.ToString());
    }
}