using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Builder.Context;
using Scribbly.Stencil.Builder.Factories;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Endpoints.Factories;
using Scribbly.Stencil.Factories;

namespace Scribbly.Stencil.Groups;

public static class GroupRegistrarExecution
{
    public static void Generate(
        SourceProductionContext context, 
        ((ImmutableArray<TargetMethodCaptureContext> Endpoints, ImmutableArray<TargetGroupCaptureContext> Groups) Tree, BuilderCaptureContext? Builder) captureContext)
    {
        var (tree, builder) = captureContext;
        
        if (tree.Endpoints.IsDefaultOrEmpty)
        {
            return;
        }
        if (tree.Groups.IsDefaultOrEmpty)
        {
            GenerateWithoutGroups(context, tree, builder);
            return;
        }
        
        GenerateWithGroups(context, tree, builder);
    }

    private static void GenerateWithGroups(SourceProductionContext context, (ImmutableArray<TargetMethodCaptureContext> Endpoints, ImmutableArray<TargetGroupCaptureContext> Groups) tree, BuilderCaptureContext? builderCtx)
    {
        var (endpoints, groups) = tree;
        
        var endpointsWithoutGroup = endpoints.Where(e => e.MemberOf == null).ToList();
        
        var groupMap = groups.ToFrozenDictionary(
            g => $"{g.Namespace}.{g.TypeName}",
            v => new GroupItem($"{v.Namespace}.{v.TypeName}", v));
        
        var groupDictionary = CreateTree(groupMap);

        var sb = CreateFileHeader(endpointsWithoutGroup, groups, builderCtx)
            .AppendLine();
        
        foreach (var targetMethodCaptureContext in endpointsWithoutGroup)
        {
            sb.Append("        app.").CreateEndpointMappingMethodInvocation(subject: targetMethodCaptureContext, builderCtx);
            sb.AppendLine();
        }
        
        sb.AppendLine();
      
        foreach (var root in groupDictionary)
        {
            EmitGroup(sb, root, parentBuilderName: "app", builderCtx is {AddStencilWasInvoked: true});
        }
        
        sb.AppendLine("""
                              return app;
                          }
                      }
                      """);
        context.AddSource($"Registrar.Scribbly.Stencil.GroupRegistry.g.cs", sb.ToString());
    }
    
    private static void GenerateWithoutGroups(SourceProductionContext context, (ImmutableArray<TargetMethodCaptureContext> Endpoints, ImmutableArray<TargetGroupCaptureContext> Groups) tree, BuilderCaptureContext? builderCtx)
    {
        var (endpoints, groups) = tree;
        var endpointsWithoutGroup = endpoints.Where(e => e.MemberOf == null).ToList();

        var sb = CreateFileHeader(endpointsWithoutGroup, groups, builderCtx);
        
        sb.AppendLine();
        foreach (var targetMethodCaptureContext in endpoints.Where(e => e.MemberOf == null))
        {
            sb.Append("        app.Map").Append(targetMethodCaptureContext.TypeName).Append(targetMethodCaptureContext.MethodName).Append("();");
            sb.AppendLine();
        }
        
        sb.AppendLine("""
                              return app;
                          }
                      }
                      """);
        context.AddSource($"Registrar.Scribbly.Stencil.GroupRegistry.g.cs", sb.ToString());
    }

    private static StringBuilder CreateFileHeader(List<TargetMethodCaptureContext> endpoints, ImmutableArray<TargetGroupCaptureContext> groups, BuilderCaptureContext? builderCtx)
    {
        return FileHeaderFactory.CreateFileHeader()
            .AppendLine("""
                        using Microsoft.AspNetCore.Builder;
                        using Microsoft.AspNetCore.Http;
                        using Microsoft.AspNetCore.Mvc;
                        using Microsoft.AspNetCore.Routing;
                        """)
            .CreateGroupUsingStatements(groups, endpoints)
            .AppendLine("namespace Scribbly.Stencil;")
            .AppendLine()
            .Append("""
                    /// <summary>
                    /// Extension method to register all Scribbly.Stencil endpoints and routing Groups.
                    /// </summary>   
                    public static class GroupRegistrationExtensions
                    {
                        /// <summary>
                        /// Maps all Stencil generated groups and endpoints to your root application builder or endpoint group.
                        /// </summary>   
                        public static global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder MapStencilApp(this global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder webApplication, string? prefix = null)
                        { 
                            global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app = webApplication;  
                            
                            if(!string.IsNullOrWhiteSpace(prefix))
                            {
                                app = webApplication.MapGroup(prefix);
                            }
                    """)
            .AppendLine()
            .CreateServiceScopeUsing(builderCtx);
    }
    
    private static IReadOnlyList<GroupItem> CreateTree(FrozenDictionary<string, GroupItem> map)
    {
        var childKeys = new HashSet<string>();

        foreach (var kvp in map)
        {
            var group = kvp.Value;

            if (!string.IsNullOrEmpty(group.Context?.MemberOf))
            {
                if (map.TryGetValue(group.Context?.MemberOf!, out var parent))
                {
                    parent.Children.Add(group);
                    childKeys.Add(group.GroupName!);
                }
            }
        }
        
        return map
            .Where(kvp => !childKeys.Contains(kvp.Key))
            .Select(kvp => kvp.Value)
            .ToList();
    }
    
    private static void EmitGroup(
        StringBuilder sb,
        GroupItem group,
        string parentBuilderName,
        bool withScope)
    {
        var builderName = CreateGroupName(group.Context);
        sb.Append($"        var ").Append(builderName).Append("= ").Append(parentBuilderName).Append(".Map").Append(group.Context?.TypeName).AppendLine(withScope ? "(scope);" :"();");
     
        foreach (var child in group.Children)
        {
            EmitGroup(sb, child, builderName, withScope);
        }
    }
    
    private static string CreateGroupName(TargetGroupCaptureContext? context)
    {
        return $"{context?.Namespace}_{context?.TypeName}".Replace(".", "_").ToLower();
    }
}