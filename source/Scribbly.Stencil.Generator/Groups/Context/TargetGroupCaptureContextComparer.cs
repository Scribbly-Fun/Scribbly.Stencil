namespace Scribbly.Stencil.Groups;

internal class TargetGroupCaptureContextComparer : IEqualityComparer<TargetGroupCaptureContext>
{
    private static readonly Lazy<TargetGroupCaptureContextComparer> Lazy = new(() => new TargetGroupCaptureContextComparer());
    public static TargetGroupCaptureContextComparer Instance => Lazy.Value;

    public bool Equals(TargetGroupCaptureContext? x, TargetGroupCaptureContext? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Equals(y);
    }

    public int GetHashCode(TargetGroupCaptureContext? obj)
    {
        unchecked
        {
            var hashCode = obj?.Namespace != null ? obj.Namespace.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ (obj?.TypeName != null ? obj.TypeName.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.RoutePrefix != null ? obj.RoutePrefix.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.Tag != null ? obj.Tag.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (obj?.Parent != null ? obj.Parent.GetHashCode() : 0);
            
            return hashCode;
        }
    }
}