namespace Scribbly.Stencil;

/// <summary>
/// Specifies that the decorated class or method belongs to a specific endpoint group.
/// Used to enforce compile-time safety by ensuring the type implements <see cref="IGroup"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class GroupMemberAttribute<TGroup> : Attribute
    where TGroup : IGroup
{
}