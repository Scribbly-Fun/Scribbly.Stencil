using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Groups;
using Scribbly.Stencil.Types.Attributes;

namespace Scribbly.Stencil;

internal sealed record CapturedGroup(
    INamedTypeSymbol ClassSymbol,
    string? Prefix,
    string? Tag,
    string? MemberOf,
    bool IsConfigurable
);

public partial class EndpointGenerator
{
    private static bool GroupSyntacticPredicate(SyntaxNode node, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        
        if (node is not ClassDeclarationSyntax classDeclaration)
            return false;
        
        if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static CapturedGroup? GroupAttributeTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;

        string? prefix = null;
        string? tag = null;
        bool groupHasConfiguration = false;
        string? genericTypeName = null;

        var memberAttributeSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName(GroupMemberAttribute.TypeFullName);

        foreach (var attribute in classSymbol.GetAttributes())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (attribute.AttributeClass?.ToDisplayString() == EndpointGroupAttribute.TypeFullName)
            {
                prefix = attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
                tag = attribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();
            }

            if (attribute.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName)
            {
                groupHasConfiguration = true;
            }

            if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass?.OriginalDefinition, memberAttributeSymbol))
                continue;

            if (attribute.AttributeClass is { TypeArguments.Length: 1 } attrSymbol &&
                attrSymbol.TypeArguments[0] is INamedTypeSymbol typeArg)
            {
                genericTypeName = typeArg.ToDisplayString();
            }
        }

        if (prefix is null)
            return null;

        return new CapturedGroup(
            classSymbol,
            prefix,
            tag,
            genericTypeName,
            groupHasConfiguration);
    }

    private static TargetGroupCaptureContext TransformGroupType(CapturedGroup captured)
    {
        var @namespace = captured.ClassSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : captured.ClassSymbol.ContainingNamespace.ToDisplayString();

        return new TargetGroupCaptureContext(
            @namespace,
            captured.ClassSymbol.Name,
            captured.Prefix,
            captured.Tag,
            captured.MemberOf,
            captured.IsConfigurable);
    }
}