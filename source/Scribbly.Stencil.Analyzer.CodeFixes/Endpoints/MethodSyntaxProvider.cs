using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Scribbly.Stencil.Analyzer.Endpoints;

/// <inheritdoc />
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodSyntaxProvider)), Shared]
public class MethodSyntaxProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => [ MethodModifierAnalyzer.DiagnosticId ];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var node = root?.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;
        if (node == null)
        {
            return;
        }

        context.RegisterCodeFix(
            Microsoft.CodeAnalysis.CodeActions.CodeAction.Create(
                title: CodeFixResources.EndpointModifier,
                createChangedDocument: c => MakeMethodStaticAsync(context.Document, node, c),
                equivalenceKey: CodeFixResources.EndpointModifier),
            diagnostic);
    }

    private async Task<Document> MakeMethodStaticAsync(
        Document document,
        MethodDeclarationSyntax methodDecl,
        CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newMethod = methodDecl.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

        editor.ReplaceNode(methodDecl, newMethod);
        return editor.GetChangedDocument();
    }
}