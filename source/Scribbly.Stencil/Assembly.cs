using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Scribbly.Stencil.UnitTests")]
[assembly: InternalsVisibleTo("Scribbly.Stencil.IntegrationTests")]
[assembly: InternalsVisibleTo("Scribbly.Stencil.Cookbook.Tests")]

namespace Scribbly.Stencil;

/// <summary>
/// Marker to locate this assembly using reflection.
/// </summary>
public interface IAssemblyMarker
{
}