namespace Scribbly.Stencil.Endpoints;

public class TargetMethodCaptureContext : IComparable<TargetMethodCaptureContext>, IEquatable<TargetMethodCaptureContext>
{
    public string? Namespace { get; }
    public string? TypeName { get; }
    public string? MethodName { get; }
    
    public string? HttpMethod { get; }
    
    public string? HttpRoute { get; }

    public bool HasConfigurationHandler { get; set; }

    public TargetMethodCaptureContext(
        string? @namespace,
        string? name,
        string? methodName,
        string? httpMethod,
        string? httpRoute,
        bool hasConfigurationHandler)
    {
        Namespace = @namespace;
        TypeName = name;
        MethodName = methodName;
        HttpMethod = httpMethod;
        HttpRoute = httpRoute;
        HasConfigurationHandler = hasConfigurationHandler;
    }

    public int CompareTo(TargetMethodCaptureContext? other)
    {
        if(other == null) return -1;

        if (other.Namespace != Namespace) return -1;
        if (other.TypeName != TypeName) return -1;
        if (other.MethodName != MethodName) return -1;
        if (other.HttpMethod != HttpMethod) return -1;
        if (other.HttpRoute != HttpRoute) return -1;
        if (other.HasConfigurationHandler != HasConfigurationHandler) return -1;
        
        return 0;
    }

    public bool Equals(TargetMethodCaptureContext? other)
    {
        if (other == null) return false;

        if (other.Namespace != Namespace) return false;
        if (other.TypeName != TypeName) return false;
        if (other.MethodName != MethodName) return false;
        if (other.HttpMethod != HttpMethod) return false;
        if (other.HttpRoute != HttpRoute) return false;
        if (other.HasConfigurationHandler != HasConfigurationHandler) return false;
       
        return true;
    }
}

internal class TargetMethodCaptureContextComparer : IEqualityComparer<TargetMethodCaptureContext>
{
    private static readonly Lazy<TargetMethodCaptureContextComparer> Lazy = new(() => new TargetMethodCaptureContextComparer());
    public static TargetMethodCaptureContextComparer Instance => Lazy.Value;

    public bool Equals(TargetMethodCaptureContext? x, TargetMethodCaptureContext? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Equals(y);
    }

    public int GetHashCode(TargetMethodCaptureContext? obj)
    {
        unchecked
        {
            var hashCode = obj?.Namespace != null ? obj.Namespace.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ (obj?.TypeName != null ? obj.TypeName.GetHashCode() : 0);
            
            return hashCode;
        }
    }
}