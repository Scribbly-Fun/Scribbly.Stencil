
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;
using Scribbly.Stencil.Attributes.Endpoints;

namespace Scribbly.Stencil.Endpoints;

[Generator(LanguageNames.CSharp)]
public class EndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TargetMethodCaptureContext> provider = context.SyntaxProvider
            .CreateSyntaxProvider(SyntacticPredicate, SemanticTransform)
            .Where(static (type) => type.HasValue)
            .Select(static (type, _) => TransformType(type!.Value))
            .WithComparer(TargetMethodCaptureContextComparer.Instance);

        context.RegisterSourceOutput(provider, EndpointHandleExecution.Execute);
        
        // context.ReportDiagnostic(Diagnostic.Create(
        //     new DiagnosticDescriptor("SCRBLY001", "Debug", "Found method: {0}", "SourceGen", DiagnosticSeverity.Info, true),
        //     methodDeclaration.GetLocation(),
        //     methodSymbol.Name));
    }

    /// <summary>
    /// Check if the declaration is a Driver Metadata Attribute.
    /// If So, Proceed to the Next Evaluation.
    /// </summary>
    private static bool SyntacticPredicate(SyntaxNode node, CancellationToken cancellation)
    {
        if (node is not MethodDeclarationSyntax method)
            return false;
        
        return method.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr => 
                attr.Name.ToString().Contains(GetEndpointAttribute.UsageName) ||
                attr.Name.ToString().Contains(PostEndpointAttribute.UsageName) ||
                attr.Name.ToString().Contains(PutEndpointAttribute.UsageName) ||
                attr.Name.ToString().Contains(DeleteEndpointAttribute.UsageName));
    }

    private static (INamedTypeSymbol symbol, TargetMethodCaptureContext handler)? SemanticTransform(GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return null;

        var classDeclaration = Extensions.SyntaxNodeExtensions.GetParent<ClassDeclarationSyntax>(methodDeclaration);
        if (!ValidateCandidateModifiers(classDeclaration))
            return null;

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
            return null;

        var getEndpointAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == GetEndpointAttribute.TypeFullName);

        if (getEndpointAttr is null)
            return null;

        // Extract attribute arguments safely
        string httpRoute = getEndpointAttr.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;
        string methodName = methodSymbol.Name;

        return (
            classSymbol,
            new TargetMethodCaptureContext(
                classSymbol.ContainingNamespace.ToDisplayString(),
                classSymbol.Name,
                methodName,
                "Get", // Or infer from attribute type
                httpRoute,
                false));
    }

    private static bool ValidateCandidateModifiers(ClassDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }

    /// <summary>
    /// Creates the partial class capture from the provided type, method, and args
    /// </summary>
    private static TargetMethodCaptureContext TransformType((
        INamedTypeSymbol symbol,
        TargetMethodCaptureContext metadata) type)
    {
        var @namespace = type.symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.symbol.ContainingNamespace.ToDisplayString();

        var name = type.symbol.Name;

        return new TargetMethodCaptureContext(
            @namespace,
            name,
            type.metadata.MethodName,
            type.metadata.HttpMethod,
            type.metadata.HttpRoute,
            type.metadata.HasConfigurationHandler);
    }
}