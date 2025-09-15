using Microsoft.CodeAnalysis.Testing;
using Scribbly.Stencil.Analyzer.Endpoints;
using Xunit;

namespace Scribbly.Stencil.Analyzer.Test.Endpoints;

public class EndpointAnalyzerTests
{
    [Fact]
    public async Task ReportsDiagnostic_OnNonStaticGetEndpoint()
    {
        var testCode = @"
using System;

class MyEndpoints
{
    [GetEndpoint]
    public void {|#0:InstanceMethod|}() { }
}

class GetEndpointAttribute : Attribute { }
";

        var expected = DiagnosticResult
            .CompilerError(MethodModifierAnalyzer.DiagnosticId)
            .WithMessage("Method 'InstanceMethod' with endpoint attribute must be static")
            .WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(testCode, expected);
    }

    [Fact(Skip = "Code fixes aren't working in test harness")]
    public async Task CodeFix_MakesMethodStatic()
    {
        var testCode = @"
using System;

class MyEndpoints
{
    [GetEndpoint]
    public void InstanceMethod() { }
}

class GetEndpointAttribute : Attribute { }
";

        var fixedCode = @"
using System;

class MyEndpoints
{
    [GetEndpoint]
    public static void InstanceMethod() { }
}

class GetEndpointAttribute : Attribute { }
";

        await Verifier.VerifyCodeFixAsync(testCode, fixedCode);
    }

    [Fact]
    public async Task NoDiagnostic_OnStaticMethod()
    {
        var testCode = @"
using System;

class MyEndpoints
{
    [PostEndpoint]
    public static void StaticMethod() { }
}

class PostEndpointAttribute : Attribute { }
";

        await Verifier.VerifyAnalyzerAsync(testCode); // No diagnostics expected
    }
}