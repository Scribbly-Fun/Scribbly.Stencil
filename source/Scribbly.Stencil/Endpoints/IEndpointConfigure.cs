namespace Scribbly.Stencil;

/// <summary>
/// Marks a type as a configurable endpoint group.
/// This interface is automatically appended to the endpoint group class when
/// the configure attribute is applied and used to enforce type safety.
/// </summary>
public interface IEndpointConfigure;