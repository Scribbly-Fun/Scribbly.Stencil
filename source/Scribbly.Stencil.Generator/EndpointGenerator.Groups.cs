using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Groups;
using Scribbly.Stencil.Types.Attributes;

namespace Scribbly.Stencil;

public partial class EndpointGenerator
{
    private static bool GroupSyntacticPredicate(SyntaxNode node, CancellationToken cancellation)
    {
        if (node is not ClassDeclarationSyntax classDeclaration)
            return false;

        return classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr => attr.Name.ToString().Contains(EndpointGroupAttribute.UsageName));
    }

    private static (INamedTypeSymbol symbol, TargetGroupCaptureContext handler)? GroupSemanticTransform(
        GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        if (!ValidateGroupCandidateModifiers(classDeclaration))
            return null;

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
            return null;

        var groupAtt = classSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == EndpointGroupAttribute.TypeFullName);

        if (groupAtt is null)
        {
            return null;
        }

        var prefix = groupAtt.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        var tag = groupAtt.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();

        if (prefix is null)
        {
            return null;
        }

        var groupHasConfiguration = false;
        
        var memberAttributeSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName(GroupMemberAttribute.TypeFullName);

        string? genericTypeName = null;

        foreach (var attribute in classSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == ConfigureAttribute.TypeFullName)
            {
                groupHasConfiguration = true;
            }
            if (!SymbolEqualityComparer.Default.Equals(attribute?.AttributeClass?.OriginalDefinition,
                    memberAttributeSymbol))
            {
                continue;
            }

            if (attribute?.AttributeClass is { TypeArguments.Length: 1 } attrSymbol &&
                attrSymbol.TypeArguments[0] is INamedTypeSymbol typeArg)
            {
                genericTypeName = typeArg.ToDisplayString();
            }
        }

        return (classSymbol, new TargetGroupCaptureContext(
            classSymbol.ContainingNamespace.ToDisplayString(),
            classSymbol.Name,
            prefix,
            tag,
            genericTypeName,
            groupHasConfiguration));
    }

    private static bool ValidateGroupCandidateModifiers(ClassDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }

    /// <summary>
    /// Creates the partial class capture from the provided type, method, and args
    /// </summary>
    private static TargetGroupCaptureContext TransformGroupType((
        INamedTypeSymbol symbol,
        TargetGroupCaptureContext metadata) type)
    {
        var @namespace = type.symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.symbol.ContainingNamespace.ToDisplayString();

        var typeName = type.symbol.Name;

        return new TargetGroupCaptureContext(
            @namespace,
            typeName,
            type.metadata.RoutePrefix,
            type.metadata.Tag,
            type.metadata.MemberOf,
            type.metadata.IsConfigurable);
    }
}