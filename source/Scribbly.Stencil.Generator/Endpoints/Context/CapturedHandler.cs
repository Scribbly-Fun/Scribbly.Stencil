namespace Scribbly.Stencil.Endpoints;

internal record CapturedHandler(
    string Namespace,
    string ClassName,
    string MethodName,
    string HttpVerb,
    string? Route,
    string? Name,
    string? Description,
    string? MemberOf,
    bool IsEndpointGroup,
    TargetMethodCaptureContext.DeclarationMode ConfigurationMode,
    TargetMethodCaptureContext.DeclarationMode GroupMode
);

internal class CapturedHandlerComparer : IEqualityComparer<CapturedHandler?>
{
    private static readonly Lazy<CapturedHandlerComparer> Lazy = new(() => new CapturedHandlerComparer());
    public static CapturedHandlerComparer Instance => Lazy.Value;

    public bool Equals(CapturedHandler? x, CapturedHandler? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Equals(y);
    }

    public int GetHashCode(CapturedHandler? obj)
    {
        return obj?.GetHashCode() ?? 0;
    }
}