using Xunit;
using XForge.DesignSystem.Components.DataDisplay.TreeView;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFTreeViewDomainFunctionalTest
{
    // XFTreeNode
    [Fact] public void TreeNode_DefaultId_IsNotEmpty() => Assert.NotEmpty(new XFTreeNode().Id);
    [Fact] public void TreeNode_DefaultText_IsEmpty() => Assert.Equal(string.Empty, new XFTreeNode().Text);
    [Fact] public void TreeNode_DefaultDescription_IsNull() => Assert.Null(new XFTreeNode().Description);
    [Fact] public void TreeNode_DefaultDisabled_IsFalse() => Assert.False(new XFTreeNode().Disabled);
    [Fact] public void TreeNode_DefaultHasChildren_IsFalse() => Assert.False(new XFTreeNode().HasChildren);
    [Fact] public void TreeNode_DefaultLoading_IsFalse() => Assert.False(new XFTreeNode().Loading);
    [Fact] public void TreeNode_DefaultCheckable_IsTrue() => Assert.True(new XFTreeNode().Checkable);
    [Fact] public void TreeNode_DefaultDraggable_IsTrue() => Assert.True(new XFTreeNode().Draggable);
    [Fact] public void TreeNode_DefaultDroppable_IsTrue() => Assert.True(new XFTreeNode().Droppable);
    [Fact] public void TreeNode_DefaultChildren_IsEmpty() => Assert.Empty(new XFTreeNode().Children);
    [Fact] public void TreeNode_SetText() => Assert.Equal("Root", new XFTreeNode { Text = "Root" }.Text);
    [Fact] public void TreeNode_SetDescription() => Assert.Equal("Desc", new XFTreeNode { Description = "Desc" }.Description);
    [Fact] public void TreeNode_SetDisabled() => Assert.True(new XFTreeNode { Disabled = true }.Disabled);
    [Fact] public void TreeNode_SetHasChildren() => Assert.True(new XFTreeNode { HasChildren = true }.HasChildren);
    [Fact] public void TreeNode_SetLoading() => Assert.True(new XFTreeNode { Loading = true }.Loading);
    [Fact] public void TreeNode_SetCheckable() => Assert.False(new XFTreeNode { Checkable = false }.Checkable);
    [Fact] public void TreeNode_SetDraggable() => Assert.False(new XFTreeNode { Draggable = false }.Draggable);
    [Fact] public void TreeNode_SetDroppable() => Assert.False(new XFTreeNode { Droppable = false }.Droppable);
    [Fact] public void TreeNode_SetChildren() { var c = new XFTreeNode { Text = "Child" }; var n = new XFTreeNode { Children = new[] { c } }; Assert.Single(n.Children); Assert.Equal("Child",n.Children[0].Text); }
    [Fact] public void TreeNode_IdsAreUnique() => Assert.NotEqual(new XFTreeNode().Id, new XFTreeNode().Id);

    // XFTreeNodeEventArgs
    [Fact] public void TreeNodeEventArgs_HasNode() { var n = new XFTreeNode { Text = "X" }; var a = new XFTreeNodeEventArgs(n); Assert.Equal("X",a.Node.Text); }

    // XFTreeLoadChildrenEventArgs
    [Fact] public void LoadChildrenArgs_HasNode() { var n = new XFTreeNode(); var a = new XFTreeLoadChildrenEventArgs(n); Assert.Same(n,a.Node); }
    [Fact] public void LoadChildrenArgs_ChildrenDefaultNull() { var a = new XFTreeLoadChildrenEventArgs(new()); Assert.Null(a.Children); }
    [Fact] public void LoadChildrenArgs_SetChildren() { var c = new[] { new XFTreeNode { Text = "C1" } }; var a = new XFTreeLoadChildrenEventArgs(new()) { Children = c }; Assert.Single(a.Children!); }

    // XFTreeDragEventArgs
    [Fact] public void DragArgs_HasSource() { var n = new XFTreeNode { Text = "S" }; var a = new XFTreeDragEventArgs(n); Assert.Equal("S",a.Source.Text); }
    [Fact] public void DragArgs_CancelDefaultFalse() => Assert.False(new XFTreeDragEventArgs(new()).Cancel);
    [Fact] public void DragArgs_SetCancel() => Assert.True(new XFTreeDragEventArgs(new()) { Cancel = true }.Cancel);

    // XFTreeDropEventArgs
    [Fact] public void DropArgs_HasSourceTargetPosition() { var s = new XFTreeNode { Text = "S" }; var t = new XFTreeNode { Text = "T" }; var a = new XFTreeDropEventArgs(s,t,XFTreeDropPosition.After); Assert.Equal("S",a.Source.Text); Assert.Equal("T",a.Target.Text); Assert.Equal(XFTreeDropPosition.After,a.Position); }
    [Fact] public void DropArgs_CancelDefaultFalse() => Assert.False(new XFTreeDropEventArgs(new(),new(),XFTreeDropPosition.Inside).Cancel);
    [Fact] public void DropArgs_SetCancel() => Assert.True(new XFTreeDropEventArgs(new(),new(),XFTreeDropPosition.Before) { Cancel = true }.Cancel);

    // XFTreeViewSelectionMode enum
    [Fact] public void SelectionMode_Has3Values() => Assert.Equal(3, Enum.GetValues<XFTreeViewSelectionMode>().Length);
    [Fact] public void SelectionMode_None() => Assert.True(Enum.IsDefined(XFTreeViewSelectionMode.None));
    [Fact] public void SelectionMode_Single() => Assert.True(Enum.IsDefined(XFTreeViewSelectionMode.Single));
    [Fact] public void SelectionMode_Multiple() => Assert.True(Enum.IsDefined(XFTreeViewSelectionMode.Multiple));

    // XFTreeDropPosition enum
    [Fact] public void DropPosition_Has4Values() => Assert.Equal(4, Enum.GetValues<XFTreeDropPosition>().Length);
    [Fact] public void DropPosition_None() => Assert.True(Enum.IsDefined(XFTreeDropPosition.None));
    [Fact] public void DropPosition_Before() => Assert.True(Enum.IsDefined(XFTreeDropPosition.Before));
    [Fact] public void DropPosition_Inside() => Assert.True(Enum.IsDefined(XFTreeDropPosition.Inside));
    [Fact] public void DropPosition_After() => Assert.True(Enum.IsDefined(XFTreeDropPosition.After));

    // Nested tree structure
    [Fact] public void TreeNode_DeepNesting() { var leaf = new XFTreeNode { Text = "Leaf" }; var mid = new XFTreeNode { Text = "Mid", Children = new[] { leaf } }; var root = new XFTreeNode { Text = "Root", Children = new[] { mid } }; Assert.Equal("Leaf", root.Children[0].Children[0].Text); }
    [Fact] public void TreeNode_MultipleChildren() { var root = new XFTreeNode { Children = new[] { new XFTreeNode { Text = "A" }, new XFTreeNode { Text = "B" }, new XFTreeNode { Text = "C" } } }; Assert.Equal(3, root.Children.Count); }
}
