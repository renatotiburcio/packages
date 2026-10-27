using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.TreeView;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.DataDisplay.TreeView;

/// <summary>
/// Extended bUnit tests for XFTreeView covering rendering, ARIA, selection, expansion,
/// checkboxes, filtering, keyboard navigation, and visibility.
/// Uses BunitContext (not obsolete TestContext) with JSRuntimeMode.Loose.
/// </summary>
public sealed class XFTreeViewExtendedTest : BunitContext
{
    public XFTreeViewExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static XFTreeNode CreateNode(
        string id,
        string text,
        string? description = null,
        bool disabled = false,
        IEnumerable<XFTreeNode>? children = null)
    {
        return new XFTreeNode
        {
            Id = id,
            Text = text,
            Description = description,
            Disabled = disabled,
            Children = children?.ToList() ?? new List<XFTreeNode>()
        };
    }

    private new IRenderedComponent<XFTreeView> RenderTree(
        Action<ComponentParameterCollectionBuilder<XFTreeView>>? configure = null)
    {
        return Render<XFTreeView>(configure ?? (_ => { }));
    }

    private static XFTreeNode CreateLeaf(string id, string text, string? description = null)
    {
        return CreateNode(id, text, description);
    }

    private static XFTreeNode CreateBranch(string id, string text, params XFTreeNode[] children)
    {
        return CreateNode(id, text, children: children);
    }

    // ════════════════════════════════════════════════════════════════════
    //  1. Default render
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void DefaultRender_RendersBaseClassWithRoleTree()
    {
        var cut = RenderTree();

        var root = cut.Find(".xf-treeview");
        Assert.NotNull(root);
        Assert.Equal("tree", root.GetAttribute("role"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  2. Empty text renders when no items
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void EmptyText_RendersDefault_WhenNoItems()
    {
        var cut = RenderTree();

        Assert.Contains("Nenhum item para exibir.", cut.Markup);
    }

    // ════════════════════════════════════════════════════════════════════
    //  3. Custom empty text renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void EmptyText_RendersCustomText()
    {
        var cut = RenderTree(p => p.Add(x => x.EmptyText, "No data available"));

        Assert.Contains("No data available", cut.Markup);
        Assert.DoesNotContain("Nenhum item para exibir.", cut.Markup);
    }

    // ════════════════════════════════════════════════════════════════════
    //  4. Items render with role="treeitem"
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void Items_RenderWithRoleTreeitem()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item A"),
            CreateLeaf("2", "Item B")
        };

        var cut = RenderTree(p => p.Add(x => x.Items, items));

        var treeitems = cut.FindAll("div[role=treeitem]");
        Assert.Equal(2, treeitems.Count);
    }

    // ════════════════════════════════════════════════════════════════════
    //  5. Node text renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void NodeText_RendersInTreeitem()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Root Node")
        };

        var cut = RenderTree(p => p.Add(x => x.Items, items));

        var treeitem = cut.Find("div[role=treeitem]");
        Assert.Contains("Root Node", treeitem.InnerHtml);
    }

    // ════════════════════════════════════════════════════════════════════
    //  6. Node description renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void NodeDescription_RendersInTreeitem()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Node", description: "A description")
        };

        var cut = RenderTree(p => p.Add(x => x.Items, items));

        var treeitem = cut.Find("div[role=treeitem]");
        Assert.Contains("A description", treeitem.InnerHtml);
    }

    // ════════════════════════════════════════════════════════════════════
    //  7. aria-level renders correctly
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void AriaLevel_RendersCorrectly()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent",
                CreateLeaf("1-1", "Child"))
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new List<string> { "1" });
        });

        var treeitems = cut.FindAll("div[role=treeitem]");
        Assert.Equal(2, treeitems.Count);

        // First node (root level) should have aria-level="1"
        Assert.Equal("1", treeitems[0].GetAttribute("aria-level"));

        // Second node (child level) should have aria-level="2"
        Assert.Equal("2", treeitems[1].GetAttribute("aria-level"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  8. aria-selected renders for selected nodes
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void AriaSelected_RendersForSelectedNodes()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Selected"),
            CreateLeaf("2", "Not Selected")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectedIds, new List<string> { "1" });
        });

        var treeitems = cut.FindAll("div[role=treeitem]");

        Assert.Equal("true", treeitems[0].GetAttribute("aria-selected"));
        Assert.Equal("false", treeitems[1].GetAttribute("aria-selected"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  9. aria-disabled renders for disabled nodes
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void AriaDisabled_RendersForDisabledNodes()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Enabled"),
            CreateNode("2", "Disabled", disabled: true)
        };

        var cut = RenderTree(p => p.Add(x => x.Items, items));

        var treeitems = cut.FindAll("div[role=treeitem]");

        Assert.Equal("false", treeitems[0].GetAttribute("aria-disabled"));
        Assert.Equal("true", treeitems[1].GetAttribute("aria-disabled"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  10. aria-expanded renders for nodes with children
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void AriaExpanded_RendersForNodesWithChildren()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new List<string> { "1" });
        });

        var parent = cut.Find("div[role=treeitem]");
        Assert.Equal("true", parent.GetAttribute("aria-expanded"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  11. ExpandOnNodeClick toggles expansion
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task ExpandOnNodeClick_TogglesExpansion()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        var expandedIds = new List<string>();
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandOnNodeClick, true);
            p.Add(x => x.ExpandedIds, expandedIds);
            p.Add(x => x.ExpandedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => expandedIds = v.ToList()));
        });

        // Initially collapsed — no group
        Assert.Empty(cut.FindAll("div[role=group]"));

        // Click the node to expand
        var treeitem = cut.Find("div[role=treeitem]");
        await treeitem.ClickAsync();
        await cut.InvokeAsync(() => { });

        // Should now have a group with the child
        var groups = cut.FindAll("div[role=group]");
        Assert.Single(groups);
    }

    // ════════════════════════════════════════════════════════════════════
    //  12. SelectOnNodeClick selects node
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task SelectOnNodeClick_SelectsNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item A"),
            CreateLeaf("2", "Item B")
        };

        var selectedIds = new List<string>();
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectOnNodeClick, true);
            p.Add(x => x.SelectedIds, selectedIds);
            p.Add(x => x.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => selectedIds = v.ToList()));
        });

        var treeitems = cut.FindAll("div[role=treeitem]");
        await treeitems[1].ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.Contains("2", selectedIds);
    }

    // ════════════════════════════════════════════════════════════════════
    //  13. NodeSelected callback fires
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task NodeSelected_CallbackFires()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item A")
        };

        XFTreeNodeEventArgs? eventArgs = null;
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectOnNodeClick, true);
            p.Add(x => x.NodeSelected, EventCallback.Factory.Create<XFTreeNodeEventArgs>(this, args => eventArgs = args));
        });

        var treeitem = cut.Find("div[role=treeitem]");
        await treeitem.ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.NotNull(eventArgs);
        Assert.Equal("1", eventArgs.Node.Id);
    }

    // ════════════════════════════════════════════════════════════════════
    //  14. NodeExpanded callback fires
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task NodeExpanded_CallbackFires()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        XFTreeNodeEventArgs? eventArgs = null;
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandOnNodeClick, true);
            p.Add(x => x.NodeExpanded, EventCallback.Factory.Create<XFTreeNodeEventArgs>(this, args => eventArgs = args));
        });

        var treeitem = cut.Find("div[role=treeitem]");
        await treeitem.ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.NotNull(eventArgs);
        Assert.Equal("1", eventArgs.Node.Id);
    }

    // ════════════════════════════════════════════════════════════════════
    //  15. NodeCollapsed callback fires
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task NodeCollapsed_CallbackFires()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        XFTreeNodeEventArgs? eventArgs = null;
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new List<string> { "1" });
            p.Add(x => x.NodeCollapsed, EventCallback.Factory.Create<XFTreeNodeEventArgs>(this, args => eventArgs = args));
        });

        // Node is already expanded, click to collapse
        var treeitem = cut.Find("div[role=treeitem]");
        await treeitem.ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.NotNull(eventArgs);
        Assert.Equal("1", eventArgs.Node.Id);
    }

    // ════════════════════════════════════════════════════════════════════
    //  16. NodeTemplate renders with node context
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void NodeTemplate_RendersWithNodeContext()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Node Text")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.NodeTemplate, (RenderFragment<XFTreeNode>)(node =>
                (builder) => builder.AddContent(0, $"Custom: {node.Text}")));
        });

        Assert.Contains("Custom: Node Text", cut.Markup);
    }

    // ════════════════════════════════════════════════════════════════════
    //  17. FilterText filters nodes
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void FilterText_FiltersNodes()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Apple"),
            CreateLeaf("2", "Banana"),
            CreateLeaf("3", "Apricot")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.FilterText, "Ap");
        });

        var treeitems = cut.FindAll("div[role=treeitem]");
        Assert.Equal(2, treeitems.Count);
        Assert.Contains("Apple", cut.Markup);
        Assert.Contains("Apricot", cut.Markup);
        Assert.DoesNotContain("Banana", cut.Markup);
    }

    // ════════════════════════════════════════════════════════════════════
    //  18. FilterPredicate filters nodes
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void FilterPredicate_FiltersNodes()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Apple"),
            CreateLeaf("2", "Banana"),
            CreateLeaf("3", "Cherry")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.FilterText, "x");
            p.Add(x => x.FilterPredicate, (XFTreeNode node, string filter) =>
                node.Id == "2"); // Only match Banana by custom predicate
        });

        var treeitems = cut.FindAll("div[role=treeitem]");
        Assert.Single(treeitems);
        Assert.Contains("Banana", cut.Markup);
    }

    // ════════════════════════════════════════════════════════════════════
    //  19. SelectionMode Multiple sets aria-multiselectable="true"
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void SelectionMode_Multiple_SetsAriaMultiselectableTrue()
    {
        var cut = RenderTree(p =>
            p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.Multiple));

        var root = cut.Find(".xf-treeview");
        Assert.Equal("true", root.GetAttribute("aria-multiselectable"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  20. SelectionMode Single sets aria-multiselectable="false"
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void SelectionMode_Single_SetsAriaMultiselectableFalse()
    {
        var cut = RenderTree(p =>
            p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.Single));

        var root = cut.Find(".xf-treeview");
        Assert.Equal("false", root.GetAttribute("aria-multiselectable"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  21. Size Sm renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void Size_Sm_RendersSizeClasses()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Small Item")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.Size, XFSize.Sm);
        });

        var treeitem = cut.Find("div[role=treeitem]");
        Assert.Contains("min-h-8", treeitem.GetAttribute("class"));
        Assert.Contains("text-xs", treeitem.GetAttribute("class"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  22. Size Lg renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void Size_Lg_RendersSizeClasses()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Large Item")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.Size, XFSize.Lg);
        });

        var treeitem = cut.Find("div[role=treeitem]");
        Assert.Contains("min-h-11", treeitem.GetAttribute("class"));
        Assert.Contains("text-base", treeitem.GetAttribute("class"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  23. Variant Plain renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void Variant_Plain_RendersPlainClasses()
    {
        var cut = RenderTree(p =>
            p.Add(x => x.Variant, XFContainerVariant.Plain));

        var root = cut.Find(".xf-treeview");
        var cls = root.GetAttribute("class");
        Assert.Contains("grid gap-1 text-text-primary", cls);
        Assert.DoesNotContain("rounded-lg", cls);
        Assert.DoesNotContain("border", cls);
    }

    // ════════════════════════════════════════════════════════════════════
    //  24. Variant Soft renders
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void Variant_Soft_RendersSoftClasses()
    {
        var cut = RenderTree(p =>
            p.Add(x => x.Variant, XFContainerVariant.Soft));

        var root = cut.Find(".xf-treeview");
        var cls = root.GetAttribute("class");
        Assert.Contains("rounded-lg", cls);
        Assert.DoesNotContain("border-border-light", cls);
    }

    // ════════════════════════════════════════════════════════════════════
    //  25. AllowCheckBoxes renders checkboxes with role="checkbox"
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void AllowCheckBoxes_RendersCheckboxes()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Checkable Item")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });

        var checkboxes = cut.FindAll("span[role=checkbox]");
        Assert.Single(checkboxes);
    }

    // ════════════════════════════════════════════════════════════════════
    //  26. CheckLabel renders as aria-label
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void CheckLabel_RendersAsAriaLabel()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckLabel, "Select item");
        });

        var checkbox = cut.Find("span[role=checkbox]");
        Assert.Equal("Select item", checkbox.GetAttribute("aria-label"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  27. UncheckLabel renders as aria-label
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void UncheckLabel_RendersAsAriaLabel()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item")
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new List<string> { "1" });
            p.Add(x => x.UncheckLabel, "Deselect item");
        });

        var checkbox = cut.Find("span[role=checkbox]");
        Assert.Equal("Deselect item", checkbox.GetAttribute("aria-label"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  28. NodeChecked callback fires
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task NodeChecked_CallbackFires()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item")
        };

        XFTreeNodeEventArgs? eventArgs = null;
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.NodeChecked, EventCallback.Factory.Create<XFTreeNodeEventArgs>(this, args => eventArgs = args));
        });

        var checkbox = cut.Find("span[role=checkbox]");
        await checkbox.ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.NotNull(eventArgs);
        Assert.Equal("1", eventArgs.Node.Id);
    }

    // ════════════════════════════════════════════════════════════════════
    //  29. NodeUnchecked callback fires
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task NodeUnchecked_CallbackFires()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Item")
        };

        XFTreeNodeEventArgs? eventArgs = null;
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new List<string> { "1" });
            p.Add(x => x.NodeUnchecked, EventCallback.Factory.Create<XFTreeNodeEventArgs>(this, args => eventArgs = args));
        });

        var checkbox = cut.Find("span[role=checkbox]");
        await checkbox.ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.NotNull(eventArgs);
        Assert.Equal("1", eventArgs.Node.Id);
    }

    // ════════════════════════════════════════════════════════════════════
    //  30. Disabled node doesn't respond to click
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task DisabledNode_DoesNotRespondToClick()
    {
        var items = new List<XFTreeNode>
        {
            CreateNode("1", "Disabled", disabled: true)
        };

        var selectedIds = new List<string>();
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectOnNodeClick, true);
            p.Add(x => x.SelectedIds, selectedIds);
            p.Add(x => x.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => selectedIds = v.ToList()));
        });

        var treeitem = cut.Find("div[role=treeitem]");
        await treeitem.ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.DoesNotContain("1", selectedIds);
    }

    // ════════════════════════════════════════════════════════════════════
    //  31. ExpandItemLabel renders as aria-label
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void ExpandItemLabel_RendersAsAriaLabel()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandItemLabel, "Open this section");
        });

        var toggle = cut.Find("button[type=button]");
        Assert.Equal("Open this section", toggle.GetAttribute("aria-label"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  32. CollapseItemLabel renders as aria-label
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void CollapseItemLabel_RendersAsAriaLabel()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new List<string> { "1" });
            p.Add(x => x.CollapseItemLabel, "Close this section");
        });

        var toggle = cut.Find("button[type=button]");
        Assert.Equal("Close this section", toggle.GetAttribute("aria-label"));
    }

    // ════════════════════════════════════════════════════════════════════
    //  33-38. Keyboard navigation
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Keyboard_ArrowDown_SelectsNextNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "First"),
            CreateLeaf("2", "Second"),
            CreateLeaf("3", "Third")
        };

        var selectedIds = new List<string> { "1" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectedIds, selectedIds);
            p.Add(x => x.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => selectedIds = v.ToList()));
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        await cut.InvokeAsync(() => { });

        Assert.Contains("2", selectedIds);
    }

    [Fact]
    public async Task Keyboard_ArrowUp_SelectsPreviousNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "First"),
            CreateLeaf("2", "Second"),
            CreateLeaf("3", "Third")
        };

        var selectedIds = new List<string> { "2" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectedIds, selectedIds);
            p.Add(x => x.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => selectedIds = v.ToList()));
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp" });
        await cut.InvokeAsync(() => { });

        Assert.Contains("1", selectedIds);
    }

    [Fact]
    public async Task Keyboard_ArrowRight_ExpandsCollapsedNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        var expandedIds = new List<string>();
        var selectedIds = new List<string> { "1" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, expandedIds);
            p.Add(x => x.ExpandedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => expandedIds = v.ToList()));
            p.Add(x => x.SelectedIds, selectedIds);
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await cut.InvokeAsync(() => { });

        Assert.Contains("1", expandedIds);
    }

    [Fact]
    public async Task Keyboard_ArrowLeft_CollapsesExpandedNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateBranch("1", "Parent", CreateLeaf("1-1", "Child"))
        };

        var expandedIds = new List<string> { "1" };
        var selectedIds = new List<string> { "1" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, expandedIds);
            p.Add(x => x.ExpandedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => expandedIds = v.ToList()));
            p.Add(x => x.SelectedIds, selectedIds);
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        await cut.InvokeAsync(() => { });

        Assert.DoesNotContain("1", expandedIds);
    }

    [Fact]
    public async Task Keyboard_Home_SelectsFirstNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "First"),
            CreateLeaf("2", "Second"),
            CreateLeaf("3", "Third")
        };

        var selectedIds = new List<string> { "3" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectedIds, selectedIds);
            p.Add(x => x.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => selectedIds = v.ToList()));
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = "Home" });
        await cut.InvokeAsync(() => { });

        Assert.Contains("1", selectedIds);
    }

    [Fact]
    public async Task Keyboard_End_SelectsLastNode()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "First"),
            CreateLeaf("2", "Second"),
            CreateLeaf("3", "Third")
        };

        var selectedIds = new List<string> { "1" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.SelectedIds, selectedIds);
            p.Add(x => x.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => selectedIds = v.ToList()));
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = "End" });
        await cut.InvokeAsync(() => { });

        Assert.Contains("3", selectedIds);
    }

    // ════════════════════════════════════════════════════════════════════
    //  39. Keyboard Space toggles checkbox when AllowCheckBoxes
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Keyboard_Space_TogglesCheckbox()
    {
        var items = new List<XFTreeNode>
        {
            CreateLeaf("1", "Checkable")
        };

        var checkedIds = new List<string>();
        var selectedIds = new List<string> { "1" };
        var cut = RenderTree(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, checkedIds);
            p.Add(x => x.CheckedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => checkedIds = v.ToList()));
            p.Add(x => x.SelectedIds, selectedIds);
        });

        var root = cut.Find(".xf-treeview");
        await root.KeyDownAsync(new KeyboardEventArgs { Key = " " });
        await cut.InvokeAsync(() => { });

        Assert.Contains("1", checkedIds);
    }

    // ════════════════════════════════════════════════════════════════════
    //  40. Visible false hides component
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void VisibleFalse_HidesComponent()
    {
        var cut = RenderTree(p => p.Add(x => x.Visible, false));

        Assert.DoesNotContain("xf-treeview", cut.Markup);
    }

    // ════════════════════════════════════════════════════════════════════
    //  41. Custom class appended
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void CustomClass_AppendedToRoot()
    {
        var cut = RenderTree(p => p.Add(x => x.Class, "my-custom-class"));

        var root = cut.Find(".xf-treeview");
        var cls = root.GetAttribute("class");
        Assert.Contains("my-custom-class", cls);
    }

    // ════════════════════════════════════════════════════════════════════
    //  42. Id renders on root div
    // ════════════════════════════════════════════════════════════════════

    [Fact]
    public void Id_RendersOnRootDiv()
    {
        var cut = RenderTree(p => p.Add(x => x.Id, "my-treeview"));

        var root = cut.Find("#my-treeview");
        Assert.NotNull(root);
        Assert.Equal("tree", root.GetAttribute("role"));
    }
}
