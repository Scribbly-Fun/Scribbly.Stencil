using System.Text;

namespace Scribbly.Stencil;

internal static class StringBuilderExtensions
{
    internal static StringBuilder AppendCondition(this StringBuilder builder, string? value, bool condition)
    {
        return condition && value is not null ? builder.Append(value) : builder;
    }
    
    internal static StringBuilder AppendCondition(this StringBuilder builder, string value, Func<string, bool> condition)
    {
        return condition(value) ? builder.Append(value) : builder;
    }
    
    internal static StringBuilder AppendCondition(this StringBuilder builder, Func<string> value, bool condition)
    {
        return condition ? builder.Append(value()) : builder;
    }
}