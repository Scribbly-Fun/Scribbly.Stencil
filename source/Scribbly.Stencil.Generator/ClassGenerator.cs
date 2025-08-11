using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Types.Classes;

namespace Scribbly.Stencil;

[Generator(LanguageNames.CSharp)]
public class ClassGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);
    }

    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(StencilOptionsClass.FileName, StencilOptionsClass.Value);
    }
}