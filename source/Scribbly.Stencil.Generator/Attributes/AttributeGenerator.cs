using Microsoft.CodeAnalysis;
using Scribbly.Stencil.Attributes.Endpoints;
using Scribbly.Stencil.Attributes.Groups;

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
            context.AddSource(GetEndpointAttribute.FileName, GetEndpointAttribute.Value);
            context.AddSource(PostEndpointAttribute.FileName, PostEndpointAttribute.Value);
            context.AddSource(PutEndpointAttribute.FileName, PutEndpointAttribute.Value);
            context.AddSource(DeleteEndpointAttribute.FileName, DeleteEndpointAttribute.Value);
        }
    }
}
