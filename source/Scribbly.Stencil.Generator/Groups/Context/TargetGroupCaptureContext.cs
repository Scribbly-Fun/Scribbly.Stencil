using Scribbly.Stencil.Endpoints.Context;

namespace Scribbly.Stencil.Groups;

public class TargetGroupCaptureContext : IComparable<TargetGroupCaptureContext>, IEquatable<TargetGroupCaptureContext>
{
    public string? Namespace { get; }
    public string? TypeName { get; }
    public string? RoutePrefix { get; }
    public string? Tag { get; }
    public string? MemberOf { get; }

    public bool IsConfigurable { get; set; }
    
    public TargetGroupCaptureContext(
        string? @namespace,
        string? typeName,
        string? routePrefix,
        string? tag,
        string? memberOf,
        bool isConfigurable)
    {
        Namespace = @namespace;
        TypeName = typeName;
        RoutePrefix = routePrefix;
        Tag = tag;
        MemberOf = memberOf;
        IsConfigurable = isConfigurable;
    }

    public int CompareTo(TargetGroupCaptureContext? other)
    {
        if(other == null) return -1;

        if (other.Namespace != Namespace) return -1;
        if (other.TypeName != TypeName) return -1;
        if (other.RoutePrefix != RoutePrefix) return -1;
        if (other.Tag != Tag) return -1;
        if (other.MemberOf != MemberOf) return -1;
        if (other.IsConfigurable != IsConfigurable) return -1;
        
        return 0;
    }

    public bool Equals(TargetGroupCaptureContext? other)
    {
        if (other == null) return false;

        if (other.Namespace != Namespace) return false;
        if (other.TypeName != TypeName) return false;
        if (other.RoutePrefix != RoutePrefix) return false;
        if (other.Tag != Tag) return false;
        if (other.MemberOf != MemberOf) return false;
        if (other.IsConfigurable != IsConfigurable) return false;
       
        return true;
    }
}