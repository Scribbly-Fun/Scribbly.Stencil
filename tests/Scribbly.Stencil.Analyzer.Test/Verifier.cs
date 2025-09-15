
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Scribbly.Stencil.Analyzer.Endpoints;

namespace Scribbly.Stencil.Analyzer.Test;

public static class Verifier
{
    public class Test : CSharpCodeFixTest<MethodModifierAnalyzer, MethodSyntaxProvider, DefaultVerifier> { }

    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    public static async Task VerifyCodeFixAsync(string source, string fixedSource)
    {
        var test = new Test
        {
            TestCode = source,
            FixedCode = fixedSource
        };
        await test.RunAsync();
    }
}