using Microsoft.CodeAnalysis.Testing;
using Scribbly.Stencil.Endpoints;

namespace Scribbly.Stencil;

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
            .CompilerError(EndpointMethodModifierAnalyzer.DiagnosticId)
            .WithMessage("Method 'InstanceMethod' with endpoint attribute must be static")
            .WithLocation(0);

        await VerifyAnalyzerAsync(testCode, expected);
    }

    [Fact]
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

        await VerifyCodeFixAsync(testCode, fixedCode);
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

        await VerifyAnalyzerAsync(testCode); // No diagnostics expected
    }
}