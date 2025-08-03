namespace Scribbly.Stencil;

/// <summary>
/// When added to an endpoint handler method a configuration function will be
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ConfigureAttribute : Attribute
{
    
}