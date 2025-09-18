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