using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Endpoints.Factories;
using Scribbly.Stencil.Factories;
using Scribbly.Stencil.Groups.Factories;

namespace Scribbly.Stencil.Groups;

public static class GroupExtensionsExecution
{
    /// <summary>
    /// Writes the captured information about the handle method as a Minimal API endpoint.
    /// </summary>
    /// <param name="context">Generator Context</param>
    /// <param name="capture">Captured Group combined with the endpoints Context</param>
    public static void Generate
        (SourceProductionContext context, 
        ((TargetGroupCaptureContext Group, BuilderCaptureContext? Builder) GroupContext, ImmutableArray<TargetMethodCaptureContext> Endpoints) capture)
    {
        var (subject, builder) = capture.GroupContext;

        var endpoints = capture.Endpoints
            .Where(e => e.MemberOf == $"{subject.Namespace}.{subject.TypeName}")
            .ToList();

        var includeNamespace = subject.Namespace is not null;

        var sb = FileHeaderFactory
                .CreateFileHeader()
                .Append("""
                        
                        using Microsoft.AspNetCore.Builder;
                        using Microsoft.AspNetCore.Http;
                        using Microsoft.AspNetCore.Mvc;
                        using Microsoft.AspNetCore.Routing;
                        
                        """)
                .AppendUsingStatements(subject, endpoints)
                .AppendLine()
                .AppendCondition("namespace ", includeNamespace).AppendCondition(subject.Namespace, includeNamespace).AppendLine(";")
                .AppendLine()
                .Append("public static class ").Append(subject.TypeName).AppendLine("Extensions")
                .Append("""
                        {
                            /// <summary>
                            /// Maps the endpoint group 
                        """)
                .Append(subject.TypeName).Append(" to a endpoint builder with the routing prefix ").Append(subject.TypeName).AppendLine(".")
                .AppendLine("    /// </summary>")
                .Append("    ").CreateGroupRegistrationMethodSignature(subject, builder)
                .AppendLine("    {")
                .Append("        ").CreateNewGroup(subject, builder)
                .AppendLine()
                .Append("        var routeGroup = scribblyGroup.Map").Append(subject.TypeName).AppendLine("(builder);")
                .CreateEndpointMapping(endpoints, builder)
                .AppendLine()
                .Append("""
                                return routeGroup;
                            }
                        }
                        """);

        var groupName = subject.Namespace is null ? $"{subject.TypeName}" : $"{subject.Namespace}.{subject.TypeName}";
        context.AddSource($"Group.{groupName}.Extensions.g.cs", sb.ToString());
    }

    private static StringBuilder AppendUsingStatements(this StringBuilder builder, TargetGroupCaptureContext groups, List<TargetMethodCaptureContext> endpoints)
    {
        var namespaces = new List<string?>(endpoints.Count + 1) { groups.Namespace };

        namespaces.AddRange(endpoints.Select(e => e.Namespace));
        
        foreach (var name in namespaces.Distinct())
        {
            builder.Append("using ").Append(name).AppendLine(";");
        }
        return builder;
    }
    
    private static StringBuilder CreateEndpointMapping(this StringBuilder builder, List<TargetMethodCaptureContext> endpoints, BuilderCaptureContext? builderCtx)
    {
        builder.AppendLine();
        foreach (var endpoint in endpoints)
        {
            builder.Append("        routeGroup.").CreateEndpointMappingMethodInvocation(subject: endpoint, builderCtx);
            builder.AppendLine();
        }
        return  builder;
    }
}