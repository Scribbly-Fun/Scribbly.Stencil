using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Scribbly.Stencil.Analyzer.Endpoints;

/// <inheritdoc />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MethodModifierAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostics ID used for the stencil error.
    /// </summary>
    public const string DiagnosticId = "SCRB001";

    private static readonly DiagnosticDescriptor Rule = new (
        id: DiagnosticId,
        title: "Endpoint attribute on non-static method",
        messageFormat: "Method '{0}' with endpoint attribute must be static",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Endpoint attributes such as [GetEndpoint] [PostEndpoint] [PutEndpoint] or [DeleteEndpoint] must only be applied to static methods.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [ Rule ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDecl = (MethodDeclarationSyntax)context.Node;

        if (methodDecl.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (methodDecl.AttributeLists.Count == 0)
        {
            return;
        }

        foreach (var list in methodDecl.AttributeLists)
        {
            foreach (var attr in list.Attributes)
            {
                var symbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, attr).Symbol as IMethodSymbol;

                var attrName = symbol?.ContainingType.Name;

                if (attrName is not ("GetEndpointAttribute" or "PostEndpointAttribute" or "PutEndpointAttribute" or "DeleteEndpointAttribute"))
                {
                    continue;
                }

                var methodSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDecl);
                if (methodSymbol is null)
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDecl.Identifier.GetLocation(),
                    methodSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}