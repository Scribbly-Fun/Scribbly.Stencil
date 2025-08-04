namespace Scribbly.Stencil;

/// <summary>
/// Marks a handle or endpoint group for configuration.
/// When applied to the type or method a route configuration handler will be generated and invoked for the endpoint or group.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ConfigureAttribute : Attribute
{
}