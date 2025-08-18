namespace Scribbly.Stencil.Endpoints;

public class TargetMethodCaptureContext : IComparable<TargetMethodCaptureContext>, IEquatable<TargetMethodCaptureContext>
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

    public TargetMethodCaptureContext()
    {
        
    }

    public int CompareTo(TargetMethodCaptureContext? other)
    {
        if(other == null) return -1;

        if (other.Namespace != Namespace) return -1;
        if (other.TypeName != TypeName) return -1;
        if (other.MethodName != MethodName) return -1;
        if (other.HttpMethod != HttpMethod) return -1;
        if (other.HttpRoute != HttpRoute) return -1;
        if (other.Name != Name) return -1;
        if (other.Description != Description) return -1;
        if (other.MemberOf != MemberOf) return -1;
        if (other.ConfigurationMode != ConfigurationMode) return -1;
        if (other.GroupMode != GroupMode) return -1;
        if (other.IsEndpointGroup != IsEndpointGroup) return -1;
        
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
        if (other.Name != Name) return false;
        if (other.Description != Description) return false;
        if (other.MemberOf != MemberOf) return false;
        if (other.ConfigurationMode != ConfigurationMode) return false;
        if (other.GroupMode != GroupMode) return false;
        if (other.IsEndpointGroup != IsEndpointGroup) return false;
       
        return true;
    }
}