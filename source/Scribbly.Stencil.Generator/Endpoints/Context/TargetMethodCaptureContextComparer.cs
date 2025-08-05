namespace Scribbly.Stencil.Endpoints.Context;

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
            hashCode = hashCode * 397 ^ (obj?.MethodName != null ? obj.MethodName.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.HttpMethod != null ? obj.HttpMethod.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.HttpRoute != null ? obj.HttpRoute.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.Name != null ? obj.Name.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.Description != null ? obj.Description.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.MemberOf != null ? obj.MemberOf.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.IsConfigurable != null ? obj.IsConfigurable.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.IsInsideGroup != null ? obj.IsInsideGroup.GetHashCode() : 0);
            
            return hashCode;
        }
    }
}