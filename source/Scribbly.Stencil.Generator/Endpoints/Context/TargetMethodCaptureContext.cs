namespace Scribbly.Stencil.Endpoints;

public record TargetMethodCaptureContext
{
    /// <summary>
    /// Tells us how we determined something.  If it was declared on the method or the class.
    /// </summary>
    public enum DeclarationMode
    {
        Na,
        ClassDeclaration,
        MethodDeclaration
    }
    
    public string? Namespace { get; }
    public string? TypeName { get; }
    public string? MethodName { get; }
    public string? HttpMethod { get; }
    public string? HttpRoute { get; }
    public string? Name { get; }
    public string? Description { get; }
    public string? MemberOf { get; set; }
    public DeclarationMode ConfigurationMode { get; set; }
    public DeclarationMode GroupMode { get; set; }

    public bool IsEndpointGroup { get; set; }
    public bool IsConfigurable => ConfigurationMode is DeclarationMode.ClassDeclaration or DeclarationMode.MethodDeclaration;
    
    public TargetMethodCaptureContext(
        string? @namespace,
        string? typeName,
        string? methodName,
        string? httpMethod,
        string? httpRoute,
        string? name,
        string? description,
        string? memberOf = null,
        DeclarationMode configurationMode = DeclarationMode.Na,
        DeclarationMode groupMode = DeclarationMode.Na,
        bool isEndpointGroup = false)
    {
        Namespace = @namespace;
        TypeName = typeName;
        MethodName = methodName;
        HttpMethod = httpMethod;
        HttpRoute = httpRoute;
        Name = name;
        Description = description;
        MemberOf = memberOf;
        ConfigurationMode = configurationMode;
        GroupMode = groupMode;
        IsEndpointGroup = isEndpointGroup;
    }
}