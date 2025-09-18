using Microsoft.CodeAnalysis;

namespace Scribbly.Stencil;

internal sealed record CapturedGroup(
    INamedTypeSymbol ClassSymbol,
    string? Prefix,
    string? Tag,
    string? MemberOf,
    bool IsConfigurable
);