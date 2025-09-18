namespace Scribbly.Stencil.Groups;

public record TargetGroupCaptureContext
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
}