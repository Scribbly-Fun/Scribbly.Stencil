using Microsoft.CodeAnalysis;

namespace Scribbly.Stencil.Extensions;

/// <summary>
/// Extensions to query and search Typed Symbols
/// </summary>
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// True when an ITypedSymbol is a Configure Attribute
    /// </summary>
    /// <param name="typeSymbol">The symbol to query</param>
    /// <returns>True when the Attribute is a ConfigureAttribute</returns>
    public static bool IsConfigurationAttribute(this ITypeSymbol? typeSymbol) =>
        typeSymbol is INamedTypeSymbol
        {
            ContainingNamespace:
            {
                Name: "Stencil",
                ContainingNamespace:
                {
                    Name: "Scribbly",
                    ContainingNamespace.IsGlobalNamespace: true,
                },
            },
            Name: "ConfigureAttribute",
        }
        || (typeSymbol?.BaseType is { } bt && IsConfigurationAttribute(bt)); 
    
    /// <summary>
    /// True when an ITypedSymbol is an Endpoint Group Attribute
    /// </summary>
    /// <param name="typeSymbol">The symbol to query</param>
    /// <returns>True when the Attribute is a EndpointGroupAttribute</returns>
    public static bool IsEndpointGroupAttribute(this ITypeSymbol? typeSymbol) =>
        typeSymbol is INamedTypeSymbol
        {
            ContainingNamespace:
            {
                Name: "Stencil",
                ContainingNamespace:
                {
                    Name: "Scribbly",
                    ContainingNamespace.IsGlobalNamespace: true,
                },
            },
            Name: "EndpointGroupAttribute",
        }
        || (typeSymbol?.BaseType is { } bt && IsConfigurationAttribute(bt));

    /// <summary>
    /// True when an ITypedSymbol is a Group Member Attribute
    /// </summary>
    /// <param name="typedSymbol">The symbol to query</param>
    /// <param name="typeArgument">The type symbol passed into the Group Member generic parameter.</param>
    /// <returns>True when the Attribute is a EndpointGroupAttribute</returns>
    public static bool IsGroupMemberAttribute(this ITypeSymbol? typedSymbol, out INamedTypeSymbol? typeArgument)
    {
        if (typedSymbol is INamedTypeSymbol 
            {
                ContainingNamespace:
                {
                    Name: "Stencil",
                    ContainingNamespace:
                    {
                        Name: "Scribbly",
                        ContainingNamespace.IsGlobalNamespace: true,
                    },
                },
                Name: "GroupMemberAttribute",
                Arity: 1,
                TypeArguments.Length: 1
            } typeSymbol && typeSymbol.TypeArguments[0] is INamedTypeSymbol genericTypeArg)
        {
            typeArgument = genericTypeArg;
            return true;
        }

        typeArgument = null;
        return false;
    }
}