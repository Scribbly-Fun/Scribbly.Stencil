using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Factories;

namespace Scribbly.Stencil.Groups;

public static class GroupBuilderExecution
{
    /// <summary>
    /// Writes the captured information about the handle method as a Minimal API endpoint.
    /// </summary>
    /// <param name="context">Generator Context</param>
    /// <param name="subject">Captured Method Context</param>
    public static void Generate(SourceProductionContext context, TargetGroupCaptureContext subject)
    {
        var builder = CreateFileHeader(subject);
        
        if (subject.IsConfigurable)
        {
            builder.GenerateConfigurableGroup(subject);
        }
        else
        {
            builder.GenerateSimpleGroup(subject);
        }
        
        var groupName = subject.Namespace is null ? $"{subject.TypeName}" : $"{subject.Namespace}.{subject.TypeName}";
        context.AddSource($"Group.{groupName}.g.cs", builder.ToString());
    }
    
    private static void GenerateConfigurableGroup(this StringBuilder builder, TargetGroupCaptureContext subject)
    {
        var span = new Span<char>(subject.TypeName!.ToCharArray());
        span[0] = char.ToLowerInvariant(span[0]);
        var builderParameter = new string(span.ToArray());
            
        builder
            .Append("public partial class ").Append(subject.TypeName).Append(": global::").Append(subject.Namespace).Append(".").Append(subject.TypeName).Append(".").Append("I").Append(subject.TypeName).Append("Configure, global::").AppendLine(GroupMarkerInterface.TypeFullName)
            .Append("""
                    {
                        public interface I
                    """)
            .Append(subject.TypeName).Append("Configure: global::Scribbly.Stencil.").AppendLine(ConfigureMarkerInterface.TypeName)
            .Append("""
                        {
                            void Configure(global::Microsoft.AspNetCore.Routing.RouteGroupBuilder 
                    """)
            .Append(builderParameter).AppendLine("Builder);")
            .Append("""
                        }
                        
                        /// <summary>
                        /// Maps the endpoint group 
                    """)
            .Append(subject.TypeName).Append(" to a endpoint builder with the routing prefix ").Append(subject.TypeName).AppendLine(".")
            .Append("""
                        /// </summary>
                        public global::Microsoft.AspNetCore.Routing.RouteGroupBuilder Map
                    """)
            .Append(subject.TypeName).AppendLine("(global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)")
            .Append("""
                        {
                            var group = builder.MapGroup("
                    """)
            .Append(subject.RoutePrefix).AppendLine("\");")
            .Append("""
                    
                            Configure(group);
                            
                    """)
            .AppendApiDocumentation(subject)
            .Append("""
                    
                            return group;
                        }
                    }
                    """);
        
    }
    
    private static void GenerateSimpleGroup(this StringBuilder builder, TargetGroupCaptureContext subject)
    {
        builder
            .Append("public partial class ").Append(subject.TypeName).Append(": global::").AppendLine(GroupMarkerInterface.TypeFullName)
            .Append("""
                    {
                        /// <summary>
                        /// Maps the endpoint group 
                    """)
            .Append(subject.TypeName).Append(" to a endpoint builder with the routing prefix ").Append(subject.TypeName).AppendLine(".")
            .Append("""
                        /// </summary>
                        public global::Microsoft.AspNetCore.Routing.RouteGroupBuilder Map
                    """)
            .Append(subject.TypeName).AppendLine("(global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder)")
            .Append("""
                        {
                            var group = builder.MapGroup("
                    """)
            .Append(subject.RoutePrefix).AppendLine("\");")
            .Append("        ").AppendApiDocumentation(subject)
            .Append("""

                            return group;
                        }
                    }
                    """);
    }

    private static StringBuilder CreateFileHeader(TargetGroupCaptureContext subject)
    {
        var includeNamespace = subject.Namespace is not null;
        
        var sb = FileHeaderFactory
                .CreateFileHeader()
                .AppendLine(
                    """

                    using Microsoft.AspNetCore.Builder;
                    using Microsoft.AspNetCore.Http;
                    using Microsoft.AspNetCore.Mvc;
                    using Microsoft.AspNetCore.Routing;

                    """)
                .AppendLine()
                .AppendCondition("namespace ", includeNamespace).AppendCondition(subject.Namespace, includeNamespace).AppendLine(";")
                .AppendLine();

        return sb;
    } 

    private static StringBuilder AppendApiDocumentation(this StringBuilder builder, TargetGroupCaptureContext subject)
    {
        return subject switch
        {
            { Tag: not null} => builder.Append("group.WithTags(\"").Append(subject.Tag).AppendLine("\");"),
            _ => builder
        };
    }
}