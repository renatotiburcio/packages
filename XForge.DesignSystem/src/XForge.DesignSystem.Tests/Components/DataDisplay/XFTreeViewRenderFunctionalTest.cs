using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.TreeView;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFTreeViewRenderFunctionalTest : XForgeTestContext
{
    public XFTreeViewRenderFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    private static XFTreeNode SampleTree => new() { Text = "Root", Children = new[] { new XFTreeNode { Text = "Child1", HasChildren = true, Children = new[] { new XFTreeNode { Text = "Grandchild1" } } }, new XFTreeNode { Text = "Child2" }, new XFTreeNode { Text = "Child3", Disabled = true } } };
    [Fact] public void Renders_WithItems() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }));
    [Fact] public void Renders_EmptyItems() => Render<XFTreeView>(p => p.Add(x => x.Items, Array.Empty<XFTreeNode>()));
    [Fact] public void Renders_SingleItem() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "Single" } }));
    [Fact] public void Renders_SelectionNone() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.SelectionMode, XFTreeViewSelectionMode.None));
    [Fact] public void Renders_SelectionSingle() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.SelectionMode, XFTreeViewSelectionMode.Single));
    [Fact] public void Renders_SelectionMultiple() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.SelectionMode, XFTreeViewSelectionMode.Multiple));
    [Fact] public void Renders_DisabledNode() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "Off", Disabled = true } }));
    [Fact] public void Renders_LoadingNode() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "Busy", Loading = true, HasChildren = true } }));
    [Fact] public void Renders_DeepNesting() { var leaf = new XFTreeNode { Text = "L3" }; var mid = new XFTreeNode { Text = "L2", Children = new[] { leaf } }; var root = new XFTreeNode { Text = "L1", Children = new[] { mid } }; Render<XFTreeView>(p => p.Add(x => x.Items, new[] { root })); }
    [Fact] public void Renders_ManySiblings() { var children = Enumerable.Range(1, 20).Select(i => new XFTreeNode { Text = $"Item{i}" }).ToArray(); Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "Root", Children = children } })); }
    [Fact] public void Renders_NonCheckableNode() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "X", Checkable = false } }));
    [Fact] public void Renders_WithDescription() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "Node", Description = "Desc" } }));
    [Fact] public void Renders_MultipleRoots() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { new XFTreeNode { Text = "A" }, new XFTreeNode { Text = "B" } }));
    [Fact] public void Renders_WithExpandedIds() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.ExpandedIds, new[] { SampleTree.Children[0].Id }));
    [Fact] public void Renders_WithSelectedIds() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.SelectedIds, new[] { SampleTree.Children[0].Id }));
    [Fact] public void Renders_WithFilterText() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.FilterText, "Child"));
    [Fact] public void Renders_VariantSurface() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.Variant, XFContainerVariant.Surface));

    [Fact] public void Renders_SizeSm() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_VisibleFalse() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.Visible, false));
    [Fact] public void Renders_EmptyText() => Render<XFTreeView>(p => p.Add(x => x.Items, Array.Empty<XFTreeNode>()).Add(x => x.EmptyText, "No items"));
    [Fact] public void Renders_ExpandOnNodeClickFalse() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.ExpandOnNodeClick, false));
    [Fact] public void Renders_AllowCheckBoxes() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.AllowCheckBoxes, true));
    [Fact] public void Renders_AllowDrag() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.AllowDrag, true));
    [Fact] public void Renders_AllowDrop() => Render<XFTreeView>(p => p.Add(x => x.Items, new[] { SampleTree }).Add(x => x.AllowDrop, true));
}
