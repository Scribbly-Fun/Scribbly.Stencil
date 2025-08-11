using System.Text;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Groups;

namespace Scribbly.Stencil.Builder.Factories;

public static class ServiceRegistrationPartialMethodFactory
{
    public static StringBuilder CreateServiceRegistration(this StringBuilder builder, IReadOnlyCollection<TargetMethodCaptureContext> endpoints, IReadOnlyCollection<TargetGroupCaptureContext> groups)
    {
        return builder
            .CreateServiceRegistrarHeader()
            .CreateServiceRegistrarEndpoints(endpoints)
            .CreateServiceRegistrarGroups(groups)
            .AppendLine("}");
    }
    
    private static StringBuilder CreateServiceRegistrarHeader(this StringBuilder builder)
    {
        return builder.AppendLine("""
                                  
                                  using Microsoft.Extensions.DependencyInjection;
                                  
                                  namespace Scribbly.Stencil;

                                  /// <summary>
                                  /// Extensions used to register the Stencil application with your DI container.
                                  /// </summary>
                                  """)
            .Append("public static partial class ")
            .AppendLine(BuilderExtensionsClass.TypeName)
            .AppendLine("{");
    }
    
    private static StringBuilder CreateServiceRegistrarEndpoints(this StringBuilder builder, IReadOnlyCollection<TargetMethodCaptureContext> endpoints)
    {
        builder.AppendLine("""
                               /// <summary>
                               /// Registers all stencil endpoints with the DI container.
                               /// </summary>
                               static partial void AddStencilHandlers(this IServiceCollection services, StencilOptions options)
                               {
                           """);
        foreach (var endpoint in endpoints)
        {
            builder.Append("        services.Add(new ServiceDescriptor(typeof(global::").Append(endpoint.Namespace).Append('.').Append(endpoint.TypeName).Append("), ").Append("typeof(global::").Append(endpoint.Namespace).Append('.').Append(endpoint.TypeName).Append("), ").Append("options.ServicesScope").Append("));");
            builder.AppendLine();
        }
        builder.AppendLine("    }");
        return builder;
    }
    
    private static StringBuilder CreateServiceRegistrarGroups(this StringBuilder builder, IReadOnlyCollection<TargetGroupCaptureContext> groups)
    {
        builder.AppendLine("""
                               /// <summary>
                               /// Registers all stencil groups with the provided DI container.
                               /// </summary>
                               static partial void AddStencilGroups(this IServiceCollection services, StencilOptions options)
                               {
                           """);
        foreach (var group in groups)
        {
            builder.Append("        services.Add(new ServiceDescriptor(typeof(global::").Append(group.Namespace).Append('.').Append(group.TypeName).Append("), ").Append("typeof(global::").Append(group.Namespace).Append('.').Append(group.TypeName).Append("), ").Append("options.ServicesScope").Append("));");builder.AppendLine();
        }
        builder.AppendLine("    }");
        return builder;
    }
}