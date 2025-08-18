using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Endpoints.Factories;
using Scribbly.Stencil.Factories;

namespace Scribbly.Stencil.Endpoints;

public static class EndpointRegistrarExecution
{
    public static void Generate(SourceProductionContext context,
        (ImmutableArray<TargetMethodCaptureContext> endpoints, BuilderCaptureContext? builder) captureContext)
    {
        var (endpoints, builder) = captureContext;
        if (endpoints.IsDefaultOrEmpty)
            return;

        var sb = FileHeaderFactory.CreateFileHeader(useStencil: false)
            .Append(
                """
                using System;
                using Microsoft.AspNetCore.Builder;
                using Microsoft.AspNetCore.Http;
                using Microsoft.AspNetCore.Mvc;
                using Microsoft.AspNetCore.Routing;                 
                """)
            .AppendLine();

        foreach (var ns in endpoints
                     .GroupBy(e => e.Namespace)
                     .Select(g => g.First())
                     .Select(n => n.Namespace))
        {
            sb.Append("using ").Append(ns).AppendLine(";");
        }

        sb.AppendLine("""
                      namespace Scribbly.Stencil;

                      public static class ScribblyEndpointRegistry
                      {
                          /// <summary>
                          /// Maps the endpoints collected to the group or application.
                          /// </summary>
                      """)
            .Append("    ").CreateEndpointRegistrationMethodSignature(builder)
            .AppendLine("    {");
        
        foreach (var endpoint in endpoints.Distinct(TargetMethodCaptureContextComparer.Instance))
        {
            sb.Append("        builder.").CreateEndpointMappingMethodInvocation(subject: endpoint, builder).AppendLine(); 
        }

        sb.AppendLine("""
                              return builder;
                          } 
                      }
                      """);
        context.AddSource($"Registrar.Scribbly.Stencil.EndpointRegistry.g.cs", sb.ToString());
    }
}