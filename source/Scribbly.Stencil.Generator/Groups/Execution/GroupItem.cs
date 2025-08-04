namespace Scribbly.Stencil.Groups;

/// <summary>
/// Data structure used to map groups together with other groups.
/// </summary>
internal class GroupItem
{
    public string? GroupName { get; set; }
    public TargetGroupCaptureContext? Context { get; set; }

    public List<GroupItem> Children { get; set; } = [];

    public bool IsParent => Children.Any();
    
    public GroupItem(string groupName, TargetGroupCaptureContext? context)
    {
        GroupName = groupName;
        Context = context;
    }
}