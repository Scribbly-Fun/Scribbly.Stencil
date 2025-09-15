using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Scribbly.Stencil.Analyzer.Endpoints;

/// <inheritdoc />
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PartialClassModifierAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostics ID used for the stencil error.
    /// </summary>
    public const string DiagnosticId = "SCRB002";

    private static readonly DiagnosticDescriptor Rule = new (
        id: DiagnosticId,
        title: "Class containing endpoints must be partial",
        messageFormat: "Class '{0}' with endpoints must be partial",
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
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (methodDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (methodDeclaration.AttributeLists.Count == 0)
        {
            return;
        }


        foreach (var list in methodDeclaration.AttributeLists)
        {
            foreach (var attr in list.Attributes)
            {
                var symbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, attr).Symbol as IMethodSymbol;

                var attrName = symbol?.ContainingType.Name;

                if (attrName is not ("GetEndpointAttribute" or "PostEndpointAttribute" or "PutEndpointAttribute" or "DeleteEndpointAttribute"))
                {
                    continue;
                }

                var classDeclaration = methodDeclaration.GetParent<ClassDeclarationSyntax>();

                if (classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return;
                }

                var classSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);

                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDeclaration.Identifier.GetLocation(),
                    classSymbol?.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}