using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Types.Attributes;

namespace Scribbly.Stencil;

[Generator(LanguageNames.CSharp)]
public class AttributeGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);
    }

    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(GroupMemberAttribute.FileName, GroupMemberAttribute.Value);
    }
}