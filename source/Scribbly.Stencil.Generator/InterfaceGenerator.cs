using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Groups;

namespace Scribbly.Stencil;

[Generator(LanguageNames.CSharp)]
public class InterfaceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);
    }

    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(GroupMarkerInterface.FileName, GroupMarkerInterface.Value);
        context.AddSource(ConfigureMarkerInterface.FileName, ConfigureMarkerInterface.Value);
    }
}