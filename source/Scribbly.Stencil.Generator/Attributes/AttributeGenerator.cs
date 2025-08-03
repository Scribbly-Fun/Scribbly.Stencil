using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Attributes.Endpoints;
using Scribbly.Stencil.Attributes.Groups;
using Scribbly.Stencil.Endpoints;

namespace Scribbly.Stencil.Attributes
{
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
            context.AddSource(EndpointGroupAttribute.FileName, EndpointGroupAttribute.Value);
        }
    }
}
