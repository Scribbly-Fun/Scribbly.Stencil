using Scribbly.Stencil.Endpoints.Context;

namespace Scribbly.Stencil.Groups;

public class TargetGroupCaptureContext : IComparable<TargetGroupCaptureContext>, IEquatable<TargetGroupCaptureContext>
{
    public string? Namespace { get; }
    public string? TypeName { get; }
    public string? RoutePrefix { get; }
    public string? Tag { get; }
    public string? Parent { get; }
    
    public TargetGroupCaptureContext(
        string? @namespace,
        string? typeName,
        string? routePrefix,
        string? tag,
        string? parent)
    {
        Namespace = @namespace;
        TypeName = typeName;
        RoutePrefix = routePrefix;
        Tag = tag;
        Parent = parent;
    }

    public int CompareTo(TargetGroupCaptureContext? other)
    {
        if(other == null) return -1;

        if (other.Namespace != Namespace) return -1;
        if (other.TypeName != TypeName) return -1;
        if (other.RoutePrefix != RoutePrefix) return -1;
        if (other.Tag != Tag) return -1;
        if (other.Parent != Parent) return -1;
        
        return 0;
    }

    public bool Equals(TargetGroupCaptureContext? other)
    {
        if (other == null) return false;

        if (other.Namespace != Namespace) return false;
        if (other.TypeName != TypeName) return false;
        if (other.RoutePrefix != RoutePrefix) return false;
        if (other.Tag != Tag) return false;
        if (other.Parent != Parent) return false;
       
        return true;
    }
}