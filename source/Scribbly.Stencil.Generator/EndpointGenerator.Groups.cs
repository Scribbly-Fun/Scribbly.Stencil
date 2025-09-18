using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Extensions;
using Scribbly.Stencil.Groups;
using Scribbly.Stencil.Types.Attributes;

namespace Scribbly.Stencil;

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
        
        foreach (var attribute in classSymbol.GetAttributes())
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (attribute.AttributeClass.IsEndpointGroupAttribute())
            {
                prefix = attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
                tag = attribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();
            }

            if (attribute.AttributeClass.IsConfigurationAttribute())
            {
                groupHasConfiguration = true;
            }

            if (!attribute.AttributeClass.IsGroupMemberAttribute(out var typeArg))
            {
                continue;
            }
            
            genericTypeName = typeArg!.ToDisplayString();
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

    private static TargetGroupCaptureContext TransformGroupType(CapturedGroup? captured, CancellationToken cancellation)
    {
        if (captured is null)
        {
            throw new ArgumentNullException(nameof(captured));
        }
        cancellation.ThrowIfCancellationRequested();
        
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