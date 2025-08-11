using System.Collections.Immutable;
using System.Text;
using Scribbly.Stencil.Endpoints;

namespace Scribbly.Stencil.Groups;

public static class GroupUsingStatementFactory
{
    public static StringBuilder CreateGroupUsingStatements(this StringBuilder builder, ImmutableArray<TargetGroupCaptureContext> groups, IEnumerable<TargetMethodCaptureContext> endpoints)
    {
        var namespaces = new List<string?>();
        
        namespaces.AddRange(groups.Select(g => g.Namespace));
        namespaces.AddRange(endpoints.Select(g => g.Namespace));
        
        foreach (var name in namespaces.Distinct())
        {
            builder.Append($"using ").Append(name).AppendLine(";");
        }
        return builder.AppendLine();
    }
}