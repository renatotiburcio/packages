using Xunit;
using XForge.DesignSystem.Components.DataDisplay.TreeView;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Tests for TreeView types at 0% coverage: XFTreeDragEventArgs, XFTreeDropEventArgs, XFTreeDropPosition, XFTreeViewSelectionMode.
/// </summary>
public sealed class XFTreeViewTypesTest
{
    #region XFTreeViewSelectionMode

    [Fact]
    public void XFTreeViewSelectionMode_HasAllValues()
    {
        Assert.Equal(3, Enum.GetValues<XFTreeViewSelectionMode>().Length);
        Assert.True(Enum.IsDefined<XFTreeViewSelectionMode>(XFTreeViewSelectionMode.None));
        Assert.True(Enum.IsDefined<XFTreeViewSelectionMode>(XFTreeViewSelectionMode.Single));
        Assert.True(Enum.IsDefined<XFTreeViewSelectionMode>(XFTreeViewSelectionMode.Multiple));
    }

    #endregion

    #region XFTreeDropPosition

    [Fact]
    public void XFTreeDropPosition_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFTreeDropPosition>().Length);
        Assert.True(Enum.IsDefined<XFTreeDropPosition>(XFTreeDropPosition.None));
        Assert.True(Enum.IsDefined<XFTreeDropPosition>(XFTreeDropPosition.Before));
        Assert.True(Enum.IsDefined<XFTreeDropPosition>(XFTreeDropPosition.Inside));
        Assert.True(Enum.IsDefined<XFTreeDropPosition>(XFTreeDropPosition.After));
    }

    #endregion

    #region XFTreeDragEventArgs

    [Fact]
    public void XFTreeDragEventArgs_Constructs_WithSourceNode()
    {
        var source = new XFTreeNode { Id = "node-1", Text = "Item A" };
        var args = new XFTreeDragEventArgs(source);
        Assert.Equal(source, args.Source);
        Assert.False(args.Cancel);
    }

    [Fact]
    public void XFTreeDragEventArgs_Cancel_CanBeSet()
    {
        var source = new XFTreeNode { Id = "node-2", Text = "Item B" };
        var args = new XFTreeDragEventArgs(source) { Cancel = true };
        Assert.True(args.Cancel);
    }

    #endregion

    #region XFTreeDropEventArgs

    [Fact]
    public void XFTreeDropEventArgs_Constructs_WithSourceTargetPosition()
    {
        var source = new XFTreeNode { Id = "node-1", Text = "Dragged" };
        var target = new XFTreeNode { Id = "node-2", Text = "Target" };
        var args = new XFTreeDropEventArgs(source, target, XFTreeDropPosition.Inside);
        Assert.Equal(source, args.Source);
        Assert.Equal(target, args.Target);
        Assert.Equal(XFTreeDropPosition.Inside, args.Position);
        Assert.False(args.Cancel);
    }

    [Fact]
    public void XFTreeDropEventArgs_Cancel_CanBeSet()
    {
        var source = new XFTreeNode { Id = "a", Text = "A" };
        var target = new XFTreeNode { Id = "b", Text = "B" };
        var args = new XFTreeDropEventArgs(source, target, XFTreeDropPosition.After) { Cancel = true };
        Assert.Equal(XFTreeDropPosition.After, args.Position);
        Assert.True(args.Cancel);
    }

    [Fact]
    public void XFTreeDropEventArgs_AllPositions_Work()
    {
        var source = new XFTreeNode { Id = "x" };
        var target = new XFTreeNode { Id = "y" };
        foreach (XFTreeDropPosition pos in Enum.GetValues<XFTreeDropPosition>())
        {
            var args = new XFTreeDropEventArgs(source, target, pos);
            Assert.Equal(pos, args.Position);
        }
    }

    #endregion
}
