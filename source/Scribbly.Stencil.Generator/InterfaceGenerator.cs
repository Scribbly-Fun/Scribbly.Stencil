using Microsoft.CodeAnalysis;

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
        context.AddSource(Groups.GroupMarkerInterface.FileName, Groups.GroupMarkerInterface.Value);
        context.AddSource(Groups.ConfigureMarkerInterface.FileName, Groups.ConfigureMarkerInterface.Value);
        context.AddSource(Endpoints.ConfigureMarkerInterface.FileName, Endpoints.ConfigureMarkerInterface.Value);
    }
}