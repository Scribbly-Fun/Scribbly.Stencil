using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scribbly.Stencil.Endpoints;
using Scribbly.Stencil.Extensions;

namespace Scribbly.Stencil;

public partial class EndpointGenerator
{
    private static bool ValidateHandlerCandidateModifiers(ClassDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;

        if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        if (candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static bool ValidateHandlerCandidateModifiers(MethodDeclarationSyntax? candidate)
    {
        if (candidate == null)
            return false;
        
        if (!candidate.Modifiers.Any(SyntaxKind.StaticKeyword))
            return false;

        return true;
    }
    
    private static CapturedHandler? CaptureEndpointHandler(GeneratorAttributeSyntaxContext context, string httpVerb, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        
        if (context.TargetSymbol is not IMethodSymbol methodSymbol)
        {
            return null;
        }

        var classSymbol = methodSymbol.ContainingType;
        if (classSymbol is null)
        {
            return null;
        }
        
        if (!ValidateHandlerCandidateModifiers(classSymbol.DeclaringSyntaxReferences.First().GetSyntax() as ClassDeclarationSyntax))
        {
            return null;
        } 
        
        if (!ValidateHandlerCandidateModifiers(methodSymbol.DeclaringSyntaxReferences.First().GetSyntax() as MethodDeclarationSyntax))
        {
            return null;
        }

        var endpointAttr = context.Attributes.FirstOrDefault();
        var (httpRoute, name, description) = GetAttributeProperties(endpointAttr);

        string? methodMembership = null;
        string? classMembership = null;
        bool methodConfig = false;
        bool classConfig = false;
        bool isEndpointGroup = false;
        
        // --------------------------------------------------------------> Method Attributes
        foreach (var attr in methodSymbol.GetAttributes())
        {
            cancellation.ThrowIfCancellationRequested();
            
            if (attr.AttributeClass.IsConfigurationAttribute())
            {
                methodConfig = true;
            }
            
            if (attr.AttributeClass.IsGroupMemberAttribute(out var genericGroupArg))
            {
                methodMembership = genericGroupArg!.ToDisplayString();
            }
        }

        // --------------------------------------------------------------> Class Attributes
        foreach (var attr in classSymbol.GetAttributes())
        {
            cancellation.ThrowIfCancellationRequested();

            if (attr.AttributeClass.IsEndpointGroupAttribute())
            {
                isEndpointGroup = true;
            }
            
            if (attr.AttributeClass.IsConfigurationAttribute())
            {
                classConfig = true;
            }

            if (attr.AttributeClass.IsGroupMemberAttribute(out var genericGroupArg))
            {
                classMembership = genericGroupArg!.ToDisplayString();
            }
        }

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();

        var memberOf = methodMembership ?? classMembership;

        var configurationMode = (methodConfig, classConfig) switch
        {
            (false, false) => TargetMethodCaptureContext.DeclarationMode.Na,
            (true, false) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
            (false, true) => TargetMethodCaptureContext.DeclarationMode.ClassDeclaration,
            (true, true) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
        };

        var groupMode = (methodMembership, classMembership) switch
        {
            (null, null) => TargetMethodCaptureContext.DeclarationMode.Na,
            (not null, null) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
            (null, not null) => TargetMethodCaptureContext.DeclarationMode.ClassDeclaration,
            (not null, not null) => TargetMethodCaptureContext.DeclarationMode.MethodDeclaration,
        };

        return new CapturedHandler(
            Namespace: ns,
            ClassName: classSymbol.Name,
            MethodName: methodSymbol.Name,
            HttpVerb: httpVerb,
            Route: httpRoute,
            Name: name,
            Description: description,
            MemberOf: memberOf,
            IsEndpointGroup: isEndpointGroup,
            ConfigurationMode: configurationMode,
            GroupMode: groupMode
        );
    }
    
    private static (string? route, string? name, string? description) GetAttributeProperties(AttributeData? endpointAttr)
    {
        if (endpointAttr is null)
        {
            return (null, null, null);
        }
        return (
            endpointAttr.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString(),
            endpointAttr.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString(),
            endpointAttr.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString());
    }
    
    /// <summary>
    /// Evaluates the captured handlers data and creates a new Target Method Context containing all data required to
    /// used to serialize and generate the handlers and all mapping extensions. 
    /// </summary>
    private static TargetMethodCaptureContext TransformHandlerType(CapturedHandler captured)
    {
        var memberOf = captured.IsEndpointGroup
            ? $"{captured.Namespace}.{captured.ClassName}"
            : captured.MemberOf;

        var configurationMode =
            captured.ConfigurationMode == TargetMethodCaptureContext.DeclarationMode.ClassDeclaration &&
            captured.IsEndpointGroup
                ? TargetMethodCaptureContext.DeclarationMode.Na
                : captured.ConfigurationMode;

        return new TargetMethodCaptureContext(
            captured.Namespace,
            captured.ClassName,
            captured.MethodName,
            captured.HttpVerb,
            captured.Route,
            captured.Name,
            captured.Description,
            memberOf,
            configurationMode,
            captured.IsEndpointGroup
                ? TargetMethodCaptureContext.DeclarationMode.ClassDeclaration
                : captured.GroupMode,
            captured.IsEndpointGroup);
    }
}