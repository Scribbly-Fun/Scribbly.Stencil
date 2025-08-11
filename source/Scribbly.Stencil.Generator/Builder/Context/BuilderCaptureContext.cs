namespace Scribbly.Stencil.Builder.Context;

public class BuilderCaptureContext : IComparable<BuilderCaptureContext>, IEquatable<BuilderCaptureContext>
{
    public bool AddStencilWasInvoked { get; set; }

    /// <inheritdoc />
    public int CompareTo(BuilderCaptureContext? other)
    {
        if(other == null) return -1;

        if (other.AddStencilWasInvoked != AddStencilWasInvoked) return -1;
        
        return 0;
    }

    /// <inheritdoc />
    public bool Equals(BuilderCaptureContext? other)
    {
        if (other == null) return false;

        if (other.AddStencilWasInvoked != AddStencilWasInvoked) return false;
       
        return true;
    }
}

internal class BuilderCaptureContextComparer : IEqualityComparer<BuilderCaptureContext>
{
    private static readonly Lazy<BuilderCaptureContextComparer> Lazy = new(() => new BuilderCaptureContextComparer());
    public static BuilderCaptureContextComparer Instance => Lazy.Value;

    public bool Equals(BuilderCaptureContext? x, BuilderCaptureContext? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Equals(y);
    }

    public int GetHashCode(BuilderCaptureContext? obj)
    {
        unchecked
        {
            var hashCode = obj?.AddStencilWasInvoked != null ? obj.AddStencilWasInvoked.GetHashCode() : 0;
            return hashCode;
        }
    }
}