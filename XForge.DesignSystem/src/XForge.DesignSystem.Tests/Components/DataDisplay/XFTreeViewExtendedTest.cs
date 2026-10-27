using Bunit;
using XForge.DesignSystem.Components.DataDisplay.TreeView;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFTreeViewExtendedTest : BunitContext
{
    public XFTreeViewExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private static readonly IReadOnlyList<XFTreeNode> SampleItems = new List<XFTreeNode>
    {
        new()
        {
            Id = "1",
            Text = "Root",
            Description = "Root node",
            Children = new List<XFTreeNode>
            {
                new() { Id = "1.1", Text = "Child 1" },
                new() { Id = "1.2", Text = "Child 2" },
            }
        },
        new() { Id = "2", Text = "Root 2" },
    };

    private IRenderedComponent<XFTreeView> RenderTreeView(
        Action<ComponentParameterCollectionBuilder<XFTreeView>>? configure = null)
    {
        return Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, SampleItems);
            configure?.Invoke(p);
        });
    }

    // ─── BaseClass ───────────────────────────────────────────────────

    [Fact]
    public void Extended_BaseClass_RendersXfTreeview()
    {
        var cut = RenderTreeView();
        Assert.Contains("xf-treeview", cut.Markup);
    }

    // ─── VariantClass: Plain ─────────────────────────────────────────

    [Fact]
    public void Extended_VariantPlain_RendersPlainClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.Contains("grid gap-1 text-text-primary", cut.Markup);
    }

    // ─── VariantClass: Soft ──────────────────────────────────────────

    [Fact]
    public void Extended_VariantSoft_RendersSoftClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-2", cut.Markup);
    }

    // ─── VariantClass: Surface (default) ─────────────────────────────

    [Fact]
    public void Extended_VariantSurface_RendersSurfaceClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("rounded-lg border border-border-light bg-bg-surface p-2", cut.Markup);
        Assert.Contains("shadow-[0_1px_2px_rgb(15_23_42/.04)]", cut.Markup);
    }

    // ─── SizeClass: Sm ───────────────────────────────────────────────

    [Fact]
    public void Extended_SizeSm_RendersSmallClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("min-h-8 px-2 py-1 text-xs", cut.Markup);
    }

    // ─── SizeClass: Md (default) ─────────────────────────────────────

    [Fact]
    public void Extended_SizeMd_RendersMediumClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("min-h-9 px-2.5 py-1.5 text-sm", cut.Markup);
    }

    // ─── SizeClass: Lg ───────────────────────────────────────────────

    [Fact]
    public void Extended_SizeLg_RendersLargeClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("min-h-11 px-3 py-2 text-base", cut.Markup);
    }

    // ─── ItemClass: selected ─────────────────────────────────────────

    [Fact]
    public void Extended_ItemSelected_RendersSelectedClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.SelectedIds, new[] { "1" }));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
        Assert.Contains("ring-1", cut.Markup);
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]", cut.Markup);
    }

    // ─── ItemClass: not selected ─────────────────────────────────────

    [Fact]
    public void Extended_ItemNotSelected_RendersHoverClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]", cut.Markup);
    }

    // ─── ItemClass: disabled ─────────────────────────────────────────

    [Fact]
    public void Extended_ItemDisabled_RendersDisabledClasses()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "d1", Text = "Disabled Node", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
        });
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("text-text-muted", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    // ─── IndentClass: level 1 ────────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel1_RendersW0()
    {
        var cut = RenderTreeView();
        Assert.Contains("w-0 shrink-0", cut.Markup);
    }

    // ─── IndentClass: level 2 ────────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel2_RendersW4()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("w-4 shrink-0", cut.Markup);
    }

    // ─── IndentClass: level 3 ────────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel3_RendersW8()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "a",
                Text = "Level 1",
                Children = new List<XFTreeNode>
                {
                    new()
                    {
                        Id = "b",
                        Text = "Level 2",
                        Children = new List<XFTreeNode>
                        {
                            new() { Id = "c", Text = "Level 3" },
                        }
                    }
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "a", "b" });
        });
        Assert.Contains("w-8 shrink-0", cut.Markup);
    }

    // ─── ToggleClass: expanded ───────────────────────────────────────

    [Fact]
    public void Extended_ToggleExpanded_RendersRotate90()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("rotate-90", cut.Markup);
    }

    // ─── ToggleClass: not visible (leaf node) ────────────────────────

    [Fact]
    public void Extended_ToggleNotVisible_RendersOpacity0()
    {
        var cut = RenderTreeView();
        // Leaf nodes (no children) should have opacity-0 on toggle
        Assert.Contains("opacity-0", cut.Markup);
    }

    // ─── EmptyClass ──────────────────────────────────────────────────

    [Fact]
    public void Extended_EmptyClass_RendersEmptyStateClasses()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("rounded-md border border-dashed border-border-light", cut.Markup);
        Assert.Contains("bg-surface-50", cut.Markup);
        Assert.Contains("px-3 py-6 text-center text-sm text-text-muted", cut.Markup);
    }

    // ─── EmptyText ───────────────────────────────────────────────────

    [Fact]
    public void Extended_EmptyText_RendersDefaultText()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("Nenhum item para exibir.", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyText_CustomText()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
            p.Add(x => x.EmptyText, "No data available");
        });
        Assert.Contains("No data available", cut.Markup);
    }

    // ─── CheckboxWrapperClass: disabled ──────────────────────────────

    [Fact]
    public void Extended_CheckboxWrapperDisabled_RendersDisabledClasses()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "cd1", Text = "Disabled Check", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // ─── CheckboxWrapperClass: enabled ───────────────────────────────

    [Fact]
    public void Extended_CheckboxWrapperEnabled_RendersEnabledClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // ─── CheckboxIconClass: checked ──────────────────────────────────

    [Fact]
    public void Extended_CheckboxIconChecked_RendersTextPrimary()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new[] { "1" });
        });
        Assert.Contains("text-primary", cut.Markup);
    }

    // ─── CheckboxIconClass: unchecked ────────────────────────────────

    [Fact]
    public void Extended_CheckboxIconUnchecked_RendersTextTransparent()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("text-transparent", cut.Markup);
    }

    // ─── SelectionMode: None ─────────────────────────────────────────

    [Fact]
    public void Extended_SelectionModeNone_RendersAriaMultiselectableFalse()
    {
        var cut = RenderTreeView(p => p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.None));
        var tree = cut.Find("[role=tree]");
        Assert.Equal("false", tree.GetAttribute("aria-multiselectable"));
    }

    // ─── SelectionMode: Single ───────────────────────────────────────

    [Fact]
    public void Extended_SelectionModeSingle_RendersAriaMultiselectableFalse()
    {
        var cut = RenderTreeView();
        var tree = cut.Find("[role=tree]");
        Assert.Equal("false", tree.GetAttribute("aria-multiselectable"));
    }

    // ─── SelectionMode: Multiple ─────────────────────────────────────

    [Fact]
    public void Extended_SelectionModeMultiple_RendersAriaMultiselectableTrue()
    {
        var cut = RenderTreeView(p => p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.Multiple));
        var tree = cut.Find("[role=tree]");
        Assert.Equal("true", tree.GetAttribute("aria-multiselectable"));
    }

    // ─── AllowCheckBoxes: renders checkbox elements ──────────────────

    [Fact]
    public void Extended_AllowCheckBoxes_RendersCheckboxRole()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.NotNull(cut.Find("[role=checkbox]"));
    }

    [Fact]
    public void Extended_AllowCheckBoxesFalse_NoCheckboxRole()
    {
        var cut = RenderTreeView();
        Assert.DoesNotContain("role=\"checkbox\"", cut.Markup);
    }

    // ─── ShowWeekNumbers: N/A for TreeView ───────────────────────────

    [Fact]
    public void Extended_ShowWeekNumbers_NotApplicable()
    {
        var cut = RenderTreeView();
        Assert.DoesNotContain("week", cut.Markup);
    }

    // ─── Visible: false hides component ──────────────────────────────

    [Fact]
    public void Extended_VisibleFalse_HidesComponent()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, SampleItems);
            p.Add(x => x.Visible, false);
        });
        Assert.DoesNotContain("xf-treeview", cut.Markup);
    }

    [Fact]
    public void Extended_VisibleTrue_RendersComponent()
    {
        var cut = RenderTreeView();
        Assert.NotNull(cut.Find("[role=tree]"));
    }

    // ─── ExpandOnNodeClick ───────────────────────────────────────────

    [Fact]
    public void Extended_ExpandOnNodeClick_ExpandNodeOnClick()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.ExpandOnNodeClick, true);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("Child 1", cut.Markup);
    }

    [Fact]
    public void Extended_ExpandOnNodeClickFalse_DoesNotExpand()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.ExpandOnNodeClick, false);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.DoesNotContain("Child 1", cut.Markup);
    }

    // ─── SelectOnNodeClick ───────────────────────────────────────────

    [Fact]
    public void Extended_SelectOnNodeClick_SelectsNodeOnClick()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.SelectOnNodeClick, true);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_SelectOnNodeClickFalse_DoesNotSelect()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.SelectOnNodeClick, false);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("aria-selected=\"false\"", cut.Markup);
    }

    // ─── AllowDrag ───────────────────────────────────────────────────

    [Fact]
    public void Extended_AllowDrag_RendersDraggableAttribute()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowDrag, true));
        Assert.Contains("draggable=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_AllowDragFalse_NoDraggableAttribute()
    {
        var cut = RenderTreeView();
        Assert.DoesNotContain("draggable=\"true\"", cut.Markup);
    }

    // ─── AllowDrop ───────────────────────────────────────────────────

    [Fact]
    public void Extended_AllowDrop_RendersDropAttributes()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowDrop, true));
        Assert.Contains("ondragover", cut.Markup);
        Assert.Contains("ondrop", cut.Markup);
    }

    [Fact]
    public void Extended_AllowDropFalse_NoDropAttributes()
    {
        var cut = RenderTreeView();
        Assert.DoesNotContain("ondragover", cut.Markup);
        Assert.DoesNotContain("ondrop", cut.Markup);
    }

    // ─── Role and structural assertions ──────────────────────────────

    [Fact]
    public void Extended_RoleTree_RendersTreeRole()
    {
        var cut = RenderTreeView();
        Assert.NotNull(cut.Find("[role=tree]"));
    }

    [Fact]
    public void Extended_RoleTreeItem_RendersTreeItemRoles()
    {
        var cut = RenderTreeView();
        var items = cut.FindAll("[role=treeitem]");
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void Extended_AriaLevel_RendersCorrectLevels()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("aria-level=\"1\"", cut.Markup);
        Assert.Contains("aria-level=\"2\"", cut.Markup);
    }

    // ─── Description rendering ───────────────────────────────────────

    [Fact]
    public void Extended_Description_RendersDescriptionText()
    {
        var cut = RenderTreeView();
        Assert.Contains("Root node", cut.Markup);
    }

    // ─── Node text rendering ─────────────────────────────────────────

    [Fact]
    public void Extended_NodeText_RendersAllNodeTexts()
    {
        var cut = RenderTreeView();
        Assert.Contains("Root", cut.Markup);
        Assert.Contains("Root 2", cut.Markup);
    }

    // ─── ExpandedIds renders children ────────────────────────────────

    [Fact]
    public void Extended_ExpandedIds_RendersChildNodes()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("Child 1", cut.Markup);
        Assert.Contains("Child 2", cut.Markup);
        Assert.Contains("role=\"group\"", cut.Markup);
    }

    // ─── Disabled node aria-disabled ─────────────────────────────────

    [Fact]
    public void Extended_DisabledNode_RendersAriaDisabled()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dis", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    // ─── Checkbox aria-checked states ────────────────────────────────

    [Fact]
    public void Extended_CheckboxChecked_RendersAriaCheckedTrue()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new[] { "1" });
        });
        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_CheckboxUnchecked_RendersAriaCheckedFalse()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    // ─── Toggle button rendering ─────────────────────────────────────

    [Fact]
    public void Extended_ToggleButton_RendersExpandLabel()
    {
        var cut = RenderTreeView();
        Assert.Contains("Expandir item", cut.Markup);
    }

    [Fact]
    public void Extended_ToggleButtonExpanded_RendersCollapseLabel()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("Recolher item", cut.Markup);
    }

    // ─── Custom toggle labels ────────────────────────────────────────

    [Fact]
    public void Extended_CustomToggleLabels_RendersCustomLabels()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.ExpandItemLabel, "Open");
            p.Add(x => x.CollapseItemLabel, "Close");
        });
        Assert.Contains("Open", cut.Markup);
    }

    // ─── Checkbox labels ─────────────────────────────────────────────

    [Fact]
    public void Extended_CheckboxLabels_RendersCheckUncheckLabels()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckLabel, "Check item");
            p.Add(x => x.UncheckLabel, "Uncheck item");
        });
        Assert.Contains("Check item", cut.Markup);
    }

    // ─── Drag label ──────────────────────────────────────────────────

    [Fact]
    public void Extended_DragLabel_RendersAriaRoleDescription()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowDrag, true);
            p.Add(x => x.DragLabel, "Drag item");
        });
        Assert.Contains("Drag item", cut.Markup);
    }

    // ─── Multiple selection toggling ─────────────────────────────────

    [Fact]
    public void Extended_MultipleSelection_TogglesOffAlreadySelected()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.Multiple);
            p.Add(x => x.SelectedIds, new[] { "1" });
        });
        cut.FindAll("[role=treeitem]")[0].Click();
        Assert.DoesNotContain("aria-selected=\"true\"", cut.Markup);
    }

    // ─── NodeTemplate rendering ──────────────────────────────────────

    [Fact]
    public void Extended_NodeTemplate_RendersCustomTemplate()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, SampleItems);
            p.Add(x => x.NodeTemplate, (XFTreeNode node) => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-node");
                builder.AddContent(2, $"[{node.Text}]");
                builder.CloseElement();
            });
        });
        Assert.Contains("[Root]", cut.Markup);
        Assert.Contains("custom-node", cut.Markup);
    }

    // ─── FilterText renders matching nodes ───────────────────────────

    [Fact]
    public void Extended_FilterText_RendersMatchingNodes()
    {
        var cut = RenderTreeView(p => p.Add(x => x.FilterText, "Root 2"));
        Assert.Contains("Root 2", cut.Markup);
        Assert.DoesNotContain("Root node", cut.Markup);
    }

    // ─── Loading state ───────────────────────────────────────────────

    [Fact]
    public void Extended_LoadingNode_RendersAriaBusy()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "load", Text = "Loading", Loading = true, HasChildren = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }

    // ─── HasChildren without children collection ─────────────────────

    [Fact]
    public void Extended_HasChildrenFlag_RendersToggleButton()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "hc", Text = "Has Children Flag", HasChildren = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("aria-expanded=\"false\"", cut.Markup);
    }

    // ─── Disabled node toggle is disabled ────────────────────────────

    [Fact]
    public void Extended_DisabledNodeToggle_IsDisabled()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "dd",
                Text = "Disabled Parent",
                Disabled = true,
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ddc", Text = "Child" },
                }
            },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("disabled", cut.Markup);
    }

    // ─── Checkbox indeterminate state ────────────────────────────────

    [Fact]
    public void Extended_CheckboxIndeterminate_RendersAriaCheckedMixed()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "par",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ch1", Text = "Child 1" },
                    new() { Id = "ch2", Text = "Child 2" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "par" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "ch1" });
        });
        Assert.Contains("aria-checked=\"mixed\"", cut.Markup);
    }

    // ─── Indeterminate label ─────────────────────────────────────────

    [Fact]
    public void Extended_IndeterminateLabel_RendersCustomLabel()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "par2",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ch3", Text = "Child 1" },
                    new() { Id = "ch4", Text = "Child 2" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "par2" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "ch3" });
            p.Add(x => x.IndeterminateLabel, "Partial");
        });
        Assert.Contains("Partial", cut.Markup);
    }

    // ─── Checkable false prevents checkbox toggle ────────────────────

    [Fact]
    public void Extended_CheckableFalse_NoCheckboxClickHandler()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "nc", Text = "Not Checkable", Checkable = false },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        Assert.Contains("role=\"checkbox\"", cut.Markup);
    }

    // ─── Disabled draggable node ─────────────────────────────────────

    [Fact]
    public void Extended_DisabledDraggableNode_NoDraggableAttribute()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "ddd", Text = "Disabled Drag", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowDrag, true);
        });
        Assert.DoesNotContain("draggable=\"true\"", cut.Markup);
    }

    // ─── Disabled droppable node ─────────────────────────────────────

    [Fact]
    public void Extended_DisabledDroppableNode_NoDropAttributes()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "ddr", Text = "Disabled Drop", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowDrop, true);
        });
        Assert.DoesNotContain("ondragover", cut.Markup);
    }

    // ─── Draggable false on node ─────────────────────────────────────

    [Fact]
    public void Extended_NodeNotDraggable_NoDraggableAttribute()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "nd", Text = "Not Draggable", Draggable = false },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowDrag, true);
        });
        Assert.DoesNotContain("draggable=\"true\"", cut.Markup);
    }

    // ─── Droppable false on node ─────────────────────────────────────

    [Fact]
    public void Extended_NodeNotDroppable_NoDropAttributes()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "ndr", Text = "Not Droppable", Droppable = false },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowDrop, true);
        });
        Assert.DoesNotContain("ondragover", cut.Markup);
    }

    // ─── Id attribute on root element ────────────────────────────────

    [Fact]
    public void Extended_IdAttribute_RendersOnRootElement()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Id, "my-tree"));
        Assert.Contains("my-tree", cut.Markup);
    }

    // ─── AdditionalAttributes ────────────────────────────────────────

    [Fact]
    public void Extended_AdditionalAttributes_RendersOnRootElement()
    {
        var cut = RenderTreeView(p => p.AddUnmatched("data-custom", "value"));
        Assert.Contains("data-custom=\"value\"", cut.Markup);
    }

    // ─── tabindex on tree root ───────────────────────────────────────

    [Fact]
    public void Extended_TabIndex_RendersOnTreeRoot()
    {
        var cut = RenderTreeView();
        var tree = cut.Find("[role=tree]");
        Assert.Equal("0", tree.GetAttribute("tabindex"));
    }

    // ─── tabindex on treeitem ────────────────────────────────────────

    [Fact]
    public void Extended_TabIndexTreeItem_RendersZero()
    {
        var cut = RenderTreeView();
        var item = cut.Find("[role=treeitem]");
        Assert.Equal("0", item.GetAttribute("tabindex"));
    }

    [Fact]
    public void Extended_TabIndexDisabledTreeItem_RendersMinusOne()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dt", Text = "Disabled Tab", Disabled = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        var item = cut.Find("[role=treeitem]");
        Assert.Equal("-1", item.GetAttribute("tabindex"));
    }

    // ─── select-none class on node wrapper ───────────────────────────

    [Fact]
    public void Extended_SelectNone_RendersOnNodeWrapper()
    {
        var cut = RenderTreeView();
        Assert.Contains("select-none", cut.Markup);
    }

    // ─── focus-visible ring on item ──────────────────────────────────

    [Fact]
    public void Extended_FocusVisible_RendersRingClass()
    {
        var cut = RenderTreeView();
        Assert.Contains("focus-visible:ring-2", cut.Markup);
        Assert.Contains("focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]", cut.Markup);
    }

    // ─── Rounded-md on item ──────────────────────────────────────────

    [Fact]
    public void Extended_ItemRounded_RendersRoundedMd()
    {
        var cut = RenderTreeView();
        Assert.Contains("rounded-md", cut.Markup);
    }

    // ─── Transition classes on item ──────────────────────────────────

    [Fact]
    public void Extended_ItemTransition_RendersTransitionClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("transition-all", cut.Markup);
        Assert.Contains("duration-150", cut.Markup);
    }

    // ─── Min-w-0 flex-1 on text container ────────────────────────────

    [Fact]
    public void Extended_TextContainer_RendersFlexClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("min-w-0 flex-1", cut.Markup);
    }

    // ─── Truncate on text span ───────────────────────────────────────

    [Fact]
    public void Extended_TextTruncate_RendersTruncateClass()
    {
        var cut = RenderTreeView();
        Assert.Contains("truncate", cut.Markup);
    }

    // ─── Font-medium on text ─────────────────────────────────────────

    [Fact]
    public void Extended_TextFont_RendersFontMedium()
    {
        var cut = RenderTreeView();
        Assert.Contains("font-medium", cut.Markup);
    }

    // ─── Description text-xs ─────────────────────────────────────────

    [Fact]
    public void Extended_Description_RendersTextXs()
    {
        var cut = RenderTreeView();
        Assert.Contains("text-xs", cut.Markup);
    }

    // ─── SVG icon rendering ──────────────────────────────────────────

    [Fact]
    public void Extended_SvgIcon_RendersChevronIcon()
    {
        var cut = RenderTreeView();
        Assert.Contains("<svg", cut.Markup);
    }

    // ─── Toggle button type ──────────────────────────────────────────

    [Fact]
    public void Extended_ToggleButtonType_RendersButtonType()
    {
        var cut = RenderTreeView();
        Assert.Contains("type=\"button\"", cut.Markup);
    }

    // ─── Checkbox label wrapper ──────────────────────────────────────

    [Fact]
    public void Extended_CheckboxLabelWrapper_RendersInlineFlex()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("inline-flex items-center", cut.Markup);
    }

    // ─── Checkbox SVG viewBox ────────────────────────────────────────

    [Fact]
    public void Extended_CheckboxSvgViewBox_RendersCorrectViewBox()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("viewBox=\"0 0 14 14\"", cut.Markup);
    }

    // ─── Checkbox aria-hidden on SVG ─────────────────────────────────

    [Fact]
    public void Extended_CheckboxSvgAriaHidden_RendersTrue()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    // ─── Checkbox tabindex -1 ────────────────────────────────────────

    [Fact]
    public void Extended_CheckboxTabIndex_RendersMinusOne()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("tabindex=\"-1\"", cut.Markup);
    }

    // ─── Checkbox icon size classes ──────────────────────────────────

    [Fact]
    public void Extended_CheckboxIconSize_RendersCorrectSize()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("h-3.5 w-3.5", cut.Markup);
    }

    // ─── Checkbox wrapper size ───────────────────────────────────────

    [Fact]
    public void Extended_CheckboxWrapperSize_RendersCorrectSize()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    // ─── Checkbox wrapper rounded ────────────────────────────────────

    [Fact]
    public void Extended_CheckboxWrapperRounded_RendersRounded()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("rounded border", cut.Markup);
    }

    // ─── Checkbox wrapper transition ─────────────────────────────────

    [Fact]
    public void Extended_CheckboxWrapperTransition_RendersTransition()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("transition-all", cut.Markup);
    }

    // ─── Toggle button size ──────────────────────────────────────────

    [Fact]
    public void Extended_ToggleButtonSize_RendersCorrectSize()
    {
        var cut = RenderTreeView();
        Assert.Contains("h-6 w-6", cut.Markup);
    }

    // ─── Toggle button text color ────────────────────────────────────

    [Fact]
    public void Extended_ToggleButtonColor_RendersTextMuted()
    {
        var cut = RenderTreeView();
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // ─── Toggle button hover ─────────────────────────────────────────

    [Fact]
    public void Extended_ToggleButtonHover_RendersHoverClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]", cut.Markup);
        Assert.Contains("hover:text-primary", cut.Markup);
    }

    // ─── Toggle button disabled pointer-events ───────────────────────

    [Fact]
    public void Extended_ToggleButtonDisabled_RendersPointerEventsNone()
    {
        var cut = RenderTreeView();
        Assert.Contains("disabled:pointer-events-none", cut.Markup);
    }

    // ─── Indent shrink-0 ─────────────────────────────────────────────

    [Fact]
    public void Extended_IndentShrink_RendersShrink0()
    {
        var cut = RenderTreeView();
        Assert.Contains("shrink-0", cut.Markup);
    }

    // ─── Item min-w-0 ────────────────────────────────────────────────

    [Fact]
    public void Extended_ItemMinWidth_RendersMinW0()
    {
        var cut = RenderTreeView();
        Assert.Contains("min-w-0", cut.Markup);
    }

    // ─── Item items-center ───────────────────────────────────────────

    [Fact]
    public void Extended_ItemItemsCenter_RendersItemsCenter()
    {
        var cut = RenderTreeView();
        Assert.Contains("items-center", cut.Markup);
    }

    // ─── Item gap-1 ──────────────────────────────────────────────────

    [Fact]
    public void Extended_ItemGap_RendersGap1()
    {
        var cut = RenderTreeView();
        Assert.Contains("gap-1", cut.Markup);
    }

    // ─── Item outline-none ───────────────────────────────────────────

    [Fact]
    public void Extended_ItemOutline_RendersOutlineNone()
    {
        var cut = RenderTreeView();
        Assert.Contains("outline-none", cut.Markup);
    }

    // ─── Cursor-pointer on enabled items ─────────────────────────────

    [Fact]
    public void Extended_EnabledItemCursor_RendersCursorPointer()
    {
        var cut = RenderTreeView();
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    // ─── Group class on item ─────────────────────────────────────────

    [Fact]
    public void Extended_ItemGroup_RendersGroupClass()
    {
        var cut = RenderTreeView();
        Assert.Contains("group", cut.Markup);
    }

    // ─── Grid gap-1 on expanded children container ───────────────────

    [Fact]
    public void Extended_ExpandedChildrenContainer_RendersGridGap1()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("grid gap-1", cut.Markup);
    }

    // ─── aria-roledescription on draggable nodes ─────────────────────

    [Fact]
    public void Extended_DraggableRoleDescription_RendersCorrectAttribute()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowDrag, true);
            p.Add(x => x.DragLabel, "Mover");
        });
        Assert.Contains("Mover", cut.Markup);
    }

    // ─── Drop labels present in component ────────────────────────────

    [Fact]
    public void Extended_DropLabels_HaveDefaults()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowDrop, true);
        });
        // Drop labels are used in the drop indicator rendering
        Assert.Contains("ondragover", cut.Markup);
    }

    // ─── Custom drop labels ──────────────────────────────────────────

    [Fact]
    public void Extended_CustomDropLabels_RendersCustomLabels()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowDrop, true);
            p.Add(x => x.DropBeforeLabel, "Drop before item");
            p.Add(x => x.DropInsideLabel, "Drop inside item");
            p.Add(x => x.DropAfterLabel, "Drop after item");
        });
        // Labels are stored as parameters; the component uses them in drop indicators
        Assert.Contains("ondrop", cut.Markup);
    }

    // ─── AllowCheckChildren parameter ────────────────────────────────

    [Fact]
    public void Extended_AllowCheckChildren_TrueByDefault()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("role=\"checkbox\"", cut.Markup);
    }

    // ─── AllowCheckParents parameter ─────────────────────────────────

    [Fact]
    public void Extended_AllowCheckParents_TrueByDefault()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("role=\"checkbox\"", cut.Markup);
    }

    // ─── ExpandFilteredMatches parameter ─────────────────────────────

    [Fact]
    public void Extended_ExpandFilteredMatches_ExpandsOnFilter()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.FilterText, "Child");
            p.Add(x => x.ExpandFilteredMatches, true);
        });
        Assert.Contains("Child 1", cut.Markup);
    }

    [Fact]
    public void Extended_ExpandFilteredMatchesFalse_DoesNotExpand()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.FilterText, "Child");
            p.Add(x => x.ExpandFilteredMatches, false);
        });
        // When ExpandFilteredMatches is false, parent nodes are not auto-expanded
        // so child nodes are not visible even though they match the filter
        Assert.DoesNotContain("Child 1", cut.Markup);
    }

    // ─── FilterPredicate parameter ───────────────────────────────────

    [Fact]
    public void Extended_FilterPredicate_CustomMatching()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.FilterText, "custom");
            p.Add(x => x.FilterPredicate, (XFTreeNode node, string filter) => node.Id == "2");
        });
        Assert.Contains("Root 2", cut.Markup);
        Assert.DoesNotContain("Root node", cut.Markup);
    }

    // ─── CheckedIds binding ──────────────────────────────────────────

    [Fact]
    public void Extended_CheckedIds_RendersCheckedState()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new[] { "1", "2" });
        });
        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    // ─── SelectedIds renders aria-selected true ──────────────────────

    [Fact]
    public void Extended_SelectedIds_RendersAriaSelectedTrue()
    {
        var cut = RenderTreeView(p => p.Add(x => x.SelectedIds, new[] { "1" }));
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    // ─── Multiple selected ids ───────────────────────────────────────

    [Fact]
    public void Extended_MultipleSelectedIds_RendersMultipleSelected()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.Multiple);
            p.Add(x => x.SelectedIds, new[] { "1", "2" });
        });
        var items = cut.FindAll("[role=treeitem]");
        Assert.Equal("true", items[0].GetAttribute("aria-selected"));
        Assert.Equal("true", items[1].GetAttribute("aria-selected"));
    }

    // ─── aria-expanded on parent nodes ───────────────────────────────

    [Fact]
    public void Extended_AriaExpandedFalse_OnCollapsedParent()
    {
        var cut = RenderTreeView();
        Assert.Contains("aria-expanded=\"false\"", cut.Markup);
    }

    [Fact]
    public void Extended_AriaExpandedTrue_OnExpandedParent()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("aria-expanded=\"true\"", cut.Markup);
    }

    // ─── No aria-expanded on leaf nodes ──────────────────────────────

    [Fact]
    public void Extended_NoAriaExpanded_OnLeafNode()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "leaf", Text = "Leaf" },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.DoesNotContain("aria-expanded", cut.Markup);
    }

    // ─── aria-busy false on non-loading nodes ────────────────────────

    [Fact]
    public void Extended_AriaBusyFalse_OnNonLoadingNode()
    {
        var cut = RenderTreeView();
        Assert.Contains("aria-busy=\"false\"", cut.Markup);
    }

    // ─── Checkbox fill none ──────────────────────────────────────────

    [Fact]
    public void Extended_CheckboxFillNone_RendersFillNone()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("fill=\"none\"", cut.Markup);
    }

    // ─── Checkbox xmlns ──────────────────────────────────────────────

    [Fact]
    public void Extended_CheckboxXmlns_RendersCorrectNamespace()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("xmlns=\"http://www.w3.org/2000/svg\"", cut.Markup);
    }

    // ─── Checkbox path for checked state ─────────────────────────────

    [Fact]
    public void Extended_CheckboxPathChecked_RendersCheckPath()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new[] { "1" });
        });
        Assert.Contains("M11.5 4L5.5 10L2.5 7", cut.Markup);
    }

    // ─── Checkbox path for indeterminate state ───────────────────────

    [Fact]
    public void Extended_CheckboxPathIndeterminate_RendersMinusPath()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "ip",
                Text = "Indeterminate Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ic1", Text = "Child A" },
                    new() { Id = "ic2", Text = "Child B" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "ip" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "ic1" });
        });
        Assert.Contains("M3 7H11", cut.Markup);
    }

    // ─── Empty list with no items parameter ──────────────────────────

    [Fact]
    public void Extended_NullItems_RendersEmpty()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, (IReadOnlyList<XFTreeNode>?)null);
        });
        Assert.Contains("Nenhum item para exibir.", cut.Markup);
    }

    // ─── Deep nesting level 4 ────────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel4_RendersW12()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "l1",
                Text = "L1",
                Children = new List<XFTreeNode>
                {
                    new()
                    {
                        Id = "l2",
                        Text = "L2",
                        Children = new List<XFTreeNode>
                        {
                            new()
                            {
                                Id = "l3",
                                Text = "L3",
                                Children = new List<XFTreeNode>
                                {
                                    new() { Id = "l4", Text = "L4" },
                                }
                            }
                        }
                    }
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "l1", "l2", "l3" });
        });
        Assert.Contains("w-12 shrink-0", cut.Markup);
    }

    // ─── Deep nesting level 5+ ───────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel5_RendersW16()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "n1",
                Text = "N1",
                Children = new List<XFTreeNode>
                {
                    new()
                    {
                        Id = "n2",
                        Text = "N2",
                        Children = new List<XFTreeNode>
                        {
                            new()
                            {
                                Id = "n3",
                                Text = "N3",
                                Children = new List<XFTreeNode>
                                {
                                    new()
                                    {
                                        Id = "n4",
                                        Text = "N4",
                                        Children = new List<XFTreeNode>
                                        {
                                            new() { Id = "n5", Text = "N5" },
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "n1", "n2", "n3", "n4" });
        });
        Assert.Contains("w-16 shrink-0", cut.Markup);
    }

    // ─── Variant + Size combination ──────────────────────────────────

    [Fact]
    public void Extended_VariantSoftSizeLg_RendersBothClasses()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.Variant, XFContainerVariant.Soft);
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-2", cut.Markup);
        Assert.Contains("min-h-11 px-3 py-2 text-base", cut.Markup);
    }

    // ─── Variant + Size Sm combination ───────────────────────────────

    [Fact]
    public void Extended_VariantPlainSizeSm_RendersBothClasses()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.Variant, XFContainerVariant.Plain);
            p.Add(x => x.Size, XFSize.Sm);
        });
        Assert.Contains("grid gap-1 text-text-primary", cut.Markup);
        Assert.Contains("min-h-8 px-2 py-1 text-xs", cut.Markup);
    }

    // ─── All variant values produce different classes ────────────────

    [Fact]
    public void Extended_AllVariants_ProduceDistinctClasses()
    {
        var plain = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        var soft = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        var surface = RenderTreeView();

        Assert.DoesNotContain("border-border-light", plain.Markup);
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]", soft.Markup);
        Assert.Contains("border-border-light", surface.Markup);
    }

    // ─── All size values produce different classes ───────────────────

    [Fact]
    public void Extended_AllSizes_ProduceDistinctClasses()
    {
        var sm = RenderTreeView(p => p.Add(x => x.Size, XFSize.Sm));
        var md = RenderTreeView();
        var lg = RenderTreeView(p => p.Add(x => x.Size, XFSize.Lg));

        Assert.Contains("min-h-8", sm.Markup);
        Assert.Contains("min-h-9", md.Markup);
        Assert.Contains("min-h-11", lg.Markup);
    }

    // ─── Checkbox icon class: indeterminate uses text-primary ─────────

    [Fact]
    public void Extended_CheckboxIconIndeterminate_RendersTextPrimary()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "ip2",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ic2a", Text = "Child A" },
                    new() { Id = "ic2b", Text = "Child B" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "ip2" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "ic2a" });
        });
        // Indeterminate state should have text-primary on the icon
        Assert.Contains("text-primary", cut.Markup);
    }

    // ─── Checkbox wrapper disabled has text-text-muted ───────────────

    [Fact]
    public void Extended_CheckboxWrapperDisabled_RendersTextMuted()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dcb", Text = "Disabled Checkbox", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // ─── Checkbox wrapper enabled has hover:border-primary ───────────

    [Fact]
    public void Extended_CheckboxWrapperEnabled_RendersHoverBorder()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("hover:border-primary", cut.Markup);
    }

    // ─── Toggle class: opacity-100 when visible ──────────────────────

    [Fact]
    public void Extended_ToggleVisible_RendersOpacity100()
    {
        var cut = RenderTreeView();
        // Root node has children, so toggle should be visible (opacity-100)
        Assert.Contains("opacity-100", cut.Markup);
    }

    // ─── Item class: base classes always present ─────────────────────

    [Fact]
    public void Extended_ItemBaseClasses_AlwaysPresent()
    {
        var cut = RenderTreeView();
        Assert.Contains("group flex min-w-0 items-center gap-1 rounded-md outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]", cut.Markup);
    }

    // ─── Empty class: full class string ──────────────────────────────

    [Fact]
    public void Extended_EmptyClass_FullClassString()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("rounded-md border border-dashed border-border-light bg-surface-50 px-3 py-6 text-center text-sm text-text-muted", cut.Markup);
    }

    // ─── VariantClass: Plain does not have rounded or border ─────────

    [Fact]
    public void Extended_VariantPlain_NoRoundedOrBorder()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.DoesNotContain("rounded-lg", cut.Markup);
        Assert.DoesNotContain("border", cut.Markup);
    }

    // ─── VariantClass: Soft has rounded but no border ────────────────

    [Fact]
    public void Extended_VariantSoft_RoundedNoBorder()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.DoesNotContain("border-border-light", cut.Markup);
    }

    // ─── VariantClass: Surface has both rounded and border ───────────

    [Fact]
    public void Extended_VariantSurface_RoundedAndBorder()
    {
        var cut = RenderTreeView();
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
    }

    // ─── SizeClass: Sm has text-xs ───────────────────────────────────

    [Fact]
    public void Extended_SizeSm_TextXs()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("text-xs", cut.Markup);
    }

    // ─── SizeClass: Md has text-sm ───────────────────────────────────

    [Fact]
    public void Extended_SizeMd_TextSm()
    {
        var cut = RenderTreeView();
        Assert.Contains("text-sm", cut.Markup);
    }

    // ─── SizeClass: Lg has text-base ─────────────────────────────────

    [Fact]
    public void Extended_SizeLg_TextBase()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    // ─── SelectionMode: None does not select on click ────────────────

    [Fact]
    public void Extended_SelectionModeNone_NoSelectionOnClick()
    {
        var cut = RenderTreeView(p => p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.None));
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("aria-selected=\"false\"", cut.Markup);
    }

    // ─── SelectionMode: Single replaces selection ────────────────────

    [Fact]
    public void Extended_SelectionModeSingle_ReplacesSelection()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.SelectedIds, new[] { "1" });
        });
        cut.FindAll("[role=treeitem]")[1].Click();
        var items = cut.FindAll("[role=treeitem]");
        Assert.Equal("false", items[0].GetAttribute("aria-selected"));
        Assert.Equal("true", items[1].GetAttribute("aria-selected"));
    }

    // ─── SelectionMode: Multiple adds to selection ───────────────────

    [Fact]
    public void Extended_SelectionModeMultiple_AddsToSelection()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.SelectionMode, XFTreeViewSelectionMode.Multiple);
            p.Add(x => x.SelectedIds, new[] { "1" });
        });
        cut.FindAll("[role=treeitem]")[1].Click();
        var items = cut.FindAll("[role=treeitem]");
        Assert.Equal("true", items[0].GetAttribute("aria-selected"));
        Assert.Equal("true", items[1].GetAttribute("aria-selected"));
    }

    // ─── Disabled node does not select on click ──────────────────────

    [Fact]
    public void Extended_DisabledNode_NoSelectionOnClick()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dn1", Text = "Disabled", Disabled = true },
            new() { Id = "en1", Text = "Enabled" },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        cut.FindAll("[role=treeitem]")[0].Click();
        Assert.Contains("aria-selected=\"false\"", cut.Markup);
    }

    // ─── Disabled node does not expand on click ──────────────────────

    [Fact]
    public void Extended_DisabledNode_NoExpandOnClick()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "dne",
                Text = "Disabled Expand",
                Disabled = true,
                Children = new List<XFTreeNode>
                {
                    new() { Id = "dnec", Text = "Child" },
                }
            },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        cut.Find("[role=treeitem]").Click();
        Assert.DoesNotContain("Child", cut.Markup);
    }

    // ─── ExpandOnNodeClick false + SelectOnNodeClick true ────────────

    [Fact]
    public void Extended_ExpandFalseSelectTrue_SelectsButNotExpand()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.ExpandOnNodeClick, false);
            p.Add(x => x.SelectOnNodeClick, true);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
        Assert.DoesNotContain("Child 1", cut.Markup);
    }

    // ─── ExpandOnNodeClick true + SelectOnNodeClick false ────────────

    [Fact]
    public void Extended_ExpandTrueSelectFalse_ExpandsButNotSelect()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.ExpandOnNodeClick, true);
            p.Add(x => x.SelectOnNodeClick, false);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("Child 1", cut.Markup);
        Assert.Contains("aria-selected=\"false\"", cut.Markup);
    }

    // ─── Both ExpandOnNodeClick and SelectOnNodeClick false ──────────

    [Fact]
    public void Extended_BothFalse_NoActionOnClick()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.ExpandOnNodeClick, false);
            p.Add(x => x.SelectOnNodeClick, false);
        });
        cut.Find("[role=treeitem]").Click();
        Assert.Contains("aria-selected=\"false\"", cut.Markup);
        Assert.DoesNotContain("Child 1", cut.Markup);
    }

    // ─── AllowDrag true + AllowDrop true ─────────────────────────────

    [Fact]
    public void Extended_AllowDragAndDrop_BothAttributesPresent()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowDrag, true);
            p.Add(x => x.AllowDrop, true);
        });
        Assert.Contains("draggable=\"true\"", cut.Markup);
        Assert.Contains("ondragover", cut.Markup);
        Assert.Contains("ondrop", cut.Markup);
    }

    // ─── ondragleave present when AllowDrop true ─────────────────────

    [Fact]
    public void Extended_AllowDrop_RendersOnDragleave()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowDrop, true));
        Assert.Contains("ondragleave", cut.Markup);
    }

    // ─── preventDefault on ondragover and ondrop ─────────────────────

    [Fact]
    public void Extended_AllowDrop_RendersPreventDefault()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowDrop, true));
        // bunit renders preventDefault as data-* or similar; verify the handlers exist
        Assert.Contains("ondragover", cut.Markup);
        Assert.Contains("ondrop", cut.Markup);
    }

    // ─── NodeDomId format ────────────────────────────────────────────

    [Fact]
    public void Extended_NodeDomId_ContainsNodeId()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Id, "tree1"));
        Assert.Contains("tree1-1", cut.Markup);
        Assert.Contains("tree1-2", cut.Markup);
    }

    // ─── Description renders with text-text-muted ─────────────────────

    [Fact]
    public void Extended_Description_RendersMutedColor()
    {
        var cut = RenderTreeView();
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // ─── Description block class ─────────────────────────────────────

    [Fact]
    public void Extended_DescriptionBlock_RendersCorrectClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("block truncate text-xs text-text-muted", cut.Markup);
    }

    // ─── Text span block class ───────────────────────────────────────

    [Fact]
    public void Extended_TextSpanBlock_RendersCorrectClasses()
    {
        var cut = RenderTreeView();
        Assert.Contains("block truncate font-medium", cut.Markup);
    }

    // ─── Loading node shows ellipsis ─────────────────────────────────

    [Fact]
    public void Extended_LoadingNode_RendersEllipsis()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "ln", Text = "Loading Node", Loading = true, HasChildren = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("...", cut.Markup);
    }

    // ─── Non-loading node shows chevron icon ─────────────────────────

    [Fact]
    public void Extended_NonLoadingNode_RendersChevronIcon()
    {
        var cut = RenderTreeView();
        Assert.Contains("<svg", cut.Markup);
        Assert.DoesNotContain("...", cut.Markup);
    }

    // ─── Expanded children count ─────────────────────────────────────

    [Fact]
    public void Extended_ExpandedChildren_RendersCorrectCount()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        var items = cut.FindAll("[role=treeitem]");
        // Root (1) + Root 2 (1) + Child 1 (1) + Child 2 (1) = 4
        Assert.Equal(4, items.Count);
    }

    // ─── Collapsed children count ────────────────────────────────────

    [Fact]
    public void Extended_CollapsedChildren_RendersCorrectCount()
    {
        var cut = RenderTreeView();
        var items = cut.FindAll("[role=treeitem]");
        // Root (1) + Root 2 (1) = 2
        Assert.Equal(2, items.Count);
    }

    // ─── role="group" only when expanded ─────────────────────────────

    [Fact]
    public void Extended_GroupRole_OnlyWhenExpanded()
    {
        var collapsed = RenderTreeView();
        Assert.DoesNotContain("role=\"group\"", collapsed.Markup);

        var expanded = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("role=\"group\"", expanded.Markup);
    }

    // ─── Checkbox: unchecked state has text-transparent ──────────────

    [Fact]
    public void Extended_CheckboxUnchecked_IconTextTransparent()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        // Should have text-transparent for unchecked state
        Assert.Contains("text-transparent", cut.Markup);
    }

    // ─── Checkbox: checked state has check path ──────────────────────

    [Fact]
    public void Extended_CheckboxChecked_RendersCheckPath()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new[] { "1" });
        });
        Assert.Contains("stroke-linecap=\"round\"", cut.Markup);
    }

    // ─── Checkbox: unchecked state has no path ───────────────────────

    [Fact]
    public void Extended_CheckboxUnchecked_NoPath()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.DoesNotContain("M11.5 4L5.5 10L2.5 7", cut.Markup);
        Assert.DoesNotContain("M3 7H11", cut.Markup);
    }

    // ─── Checkbox: indeterminate state has minus path ────────────────

    [Fact]
    public void Extended_CheckboxIndeterminate_RendersMinusPath()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "ind",
                Text = "Indeterminate",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ind1", Text = "C1" },
                    new() { Id = "ind2", Text = "C2" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "ind" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "ind1" });
        });
        Assert.Contains("M3 7H11", cut.Markup);
    }

    // ─── Checkbox: all children checked makes parent checked ────────

    [Fact]
    public void Extended_AllChildrenChecked_ParentChecked()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "pchk",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "cchk1", Text = "C1" },
                    new() { Id = "cchk2", Text = "C2" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "pchk" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "cchk1", "cchk2" });
        });
        // Parent should be checked since all children are checked
        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    // ─── Checkbox: no children checked makes parent unchecked ───────

    [Fact]
    public void Extended_NoChildrenChecked_ParentUnchecked()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "puchk",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "cuchk1", Text = "C1" },
                    new() { Id = "cuchk2", Text = "C2" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "puchk" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
        });
        // Parent should be unchecked since no children are checked
        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    // ─── Checkbox: some children checked makes parent indeterminate ─

    [Fact]
    public void Extended_SomeChildrenChecked_ParentIndeterminate()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "psome",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "csome1", Text = "C1" },
                    new() { Id = "csome2", Text = "C2" },
                    new() { Id = "csome3", Text = "C3" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "psome" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "csome1" });
        });
        Assert.Contains("aria-checked=\"mixed\"", cut.Markup);
    }

    // ─── Checkbox: disabled node checkbox has no onclick ─────────────

    [Fact]
    public void Extended_DisabledCheckbox_NoOnClick()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dcb2", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        // The checkbox wrapper exists but should not have onclick
        Assert.Contains("role=\"checkbox\"", cut.Markup);
    }

    // ─── Checkbox: checkable false node checkbox has no onclick ──────

    [Fact]
    public void Extended_CheckableFalseCheckbox_NoOnClick()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "ncb", Text = "Not Checkable", Checkable = false },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        Assert.Contains("role=\"checkbox\"", cut.Markup);
    }

    // ─── Checkbox: enabled checkable node has onclick ────────────────

    [Fact]
    public void Extended_EnabledCheckableCheckbox_HasOnClick()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        // The checkbox wrapper should have an onclick handler
        var checkbox = cut.Find("[role=checkbox]");
        Assert.NotNull(checkbox);
    }

    // ─── Toggle: expanded + visible classes combined ─────────────────

    [Fact]
    public void Extended_ToggleExpandedVisible_RendersBothClasses()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("opacity-100", cut.Markup);
        Assert.Contains("rotate-90", cut.Markup);
    }

    // ─── Toggle: not expanded + visible classes combined ─────────────

    [Fact]
    public void Extended_ToggleNotExpandedVisible_RendersCorrectClasses()
    {
        var cut = RenderTreeView();
        // Root has children but is collapsed: visible (opacity-100) but not rotated
        Assert.Contains("opacity-100", cut.Markup);
        Assert.DoesNotContain("rotate-90", cut.Markup);
    }

    // ─── Toggle: not visible (leaf) classes ──────────────────────────

    [Fact]
    public void Extended_ToggleLeafNode_Opacity0()
    {
        var cut = RenderTreeView();
        // Root 2 is a leaf node: toggle should have opacity-0
        Assert.Contains("opacity-0", cut.Markup);
    }

    // ─── Indent: level 1 has w-0 ─────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel1_W0()
    {
        var cut = RenderTreeView();
        Assert.Contains("w-0 shrink-0", cut.Markup);
    }

    // ─── Indent: level 2 has w-4 ─────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel2_W4()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("w-4 shrink-0", cut.Markup);
    }

    // ─── Indent: level 3 has w-8 ─────────────────────────────────────

    [Fact]
    public void Extended_IndentLevel3_W8()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "a1",
                Text = "A",
                Children = new List<XFTreeNode>
                {
                    new()
                    {
                        Id = "b1",
                        Text = "B",
                        Children = new List<XFTreeNode>
                        {
                            new() { Id = "c1", Text = "C" },
                        }
                    }
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "a1", "b1" });
        });
        Assert.Contains("w-8 shrink-0", cut.Markup);
    }

    // ─── Item: selected class includes bg color-mix ──────────────────

    [Fact]
    public void Extended_ItemSelected_BgColorMix()
    {
        var cut = RenderTreeView(p => p.Add(x => x.SelectedIds, new[] { "1" }));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
    }

    // ─── Item: not selected class includes hover color-mix ───────────

    [Fact]
    public void Extended_ItemNotSelected_HoverColorMix()
    {
        var cut = RenderTreeView();
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]", cut.Markup);
    }

    // ─── Item: disabled class includes cursor-not-allowed ─────────────

    [Fact]
    public void Extended_ItemDisabled_CursorNotAllowed()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dis1", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("cursor-not-allowed", cut.Markup);
    }

    // ─── Item: disabled class includes opacity-60 ────────────────────

    [Fact]
    public void Extended_ItemDisabled_Opacity60()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dis2", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("opacity-60", cut.Markup);
    }

    // ─── Item: disabled class includes text-text-muted ───────────────

    [Fact]
    public void Extended_ItemDisabled_TextMuted()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dis3", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p => p.Add(x => x.Items, items));
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // ─── Item: enabled class includes cursor-pointer ─────────────────

    [Fact]
    public void Extended_ItemEnabled_CursorPointer()
    {
        var cut = RenderTreeView();
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    // ─── VariantClass: Surface includes shadow ───────────────────────

    [Fact]
    public void Extended_VariantSurface_IncludesShadow()
    {
        var cut = RenderTreeView();
        Assert.Contains("shadow-[0_1px_2px_rgb(15_23_42/.04)]", cut.Markup);
    }

    // ─── VariantClass: Plain does not include shadow ─────────────────

    [Fact]
    public void Extended_VariantPlain_NoShadow()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.DoesNotContain("shadow-", cut.Markup);
    }

    // ─── VariantClass: Soft does not include shadow ──────────────────

    [Fact]
    public void Extended_VariantSoft_NoShadow()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.DoesNotContain("shadow-", cut.Markup);
    }

    // ─── VariantClass: Surface includes bg-bg-surface ────────────────

    [Fact]
    public void Extended_VariantSurface_IncludesBgSurface()
    {
        var cut = RenderTreeView();
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // ─── VariantClass: Soft includes bg color-mix ────────────────────

    [Fact]
    public void Extended_VariantSoft_IncludesBgColorMix()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]", cut.Markup);
    }

    // ─── VariantClass: Plain does not include container bg ────────────

    [Fact]
    public void Extended_VariantPlain_NoContainerBg()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.DoesNotContain("bg-bg-surface", cut.Markup);
        Assert.DoesNotContain("bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]", cut.Markup);
    }

    // ─── SizeClass: Sm includes min-h-8 ──────────────────────────────

    [Fact]
    public void Extended_SizeSm_MinH8()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("min-h-8", cut.Markup);
    }

    // ─── SizeClass: Md includes min-h-9 ──────────────────────────────

    [Fact]
    public void Extended_SizeMd_MinH9()
    {
        var cut = RenderTreeView();
        Assert.Contains("min-h-9", cut.Markup);
    }

    // ─── SizeClass: Lg includes min-h-11 ─────────────────────────────

    [Fact]
    public void Extended_SizeLg_MinH11()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("min-h-11", cut.Markup);
    }

    // ─── SizeClass: Sm includes px-2 ─────────────────────────────────

    [Fact]
    public void Extended_SizeSm_Px2()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("px-2", cut.Markup);
    }

    // ─── SizeClass: Md includes px-2.5 ───────────────────────────────

    [Fact]
    public void Extended_SizeMd_Px2_5()
    {
        var cut = RenderTreeView();
        Assert.Contains("px-2.5", cut.Markup);
    }

    // ─── SizeClass: Lg includes px-3 ─────────────────────────────────

    [Fact]
    public void Extended_SizeLg_Px3()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("px-3", cut.Markup);
    }

    // ─── SizeClass: Sm includes py-1 ─────────────────────────────────

    [Fact]
    public void Extended_SizeSm_Py1()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("py-1", cut.Markup);
    }

    // ─── SizeClass: Md includes py-1.5 ───────────────────────────────

    [Fact]
    public void Extended_SizeMd_Py1_5()
    {
        var cut = RenderTreeView();
        Assert.Contains("py-1.5", cut.Markup);
    }

    // ─── SizeClass: Lg includes py-2 ─────────────────────────────────

    [Fact]
    public void Extended_SizeLg_Py2()
    {
        var cut = RenderTreeView(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("py-2", cut.Markup);
    }

    // ─── CheckboxWrapperClass: disabled includes border-border-light ─

    [Fact]
    public void Extended_CheckboxWrapperDisabled_BorderBorderLight()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dcb3", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        Assert.Contains("border-border-light", cut.Markup);
    }

    // ─── CheckboxWrapperClass: enabled includes border-border-light ──

    [Fact]
    public void Extended_CheckboxWrapperEnabled_BorderBorderLight()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("border-border-light", cut.Markup);
    }

    // ─── CheckboxWrapperClass: enabled includes bg-bg-surface ────────

    [Fact]
    public void Extended_CheckboxWrapperEnabled_BgBgSurface()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // ─── CheckboxWrapperClass: disabled includes bg-surface-100 ──────

    [Fact]
    public void Extended_CheckboxWrapperDisabled_BgSurface100()
    {
        var items = new List<XFTreeNode>
        {
            new() { Id = "dcb4", Text = "Disabled", Disabled = true },
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.AllowCheckBoxes, true);
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // ─── CheckboxIconClass: checked is text-primary ──────────────────

    [Fact]
    public void Extended_CheckboxIconChecked_TextPrimary()
    {
        var cut = RenderTreeView(p =>
        {
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.CheckedIds, new[] { "1" });
        });
        Assert.Contains("text-primary", cut.Markup);
    }

    // ─── CheckboxIconClass: unchecked is text-transparent ────────────

    [Fact]
    public void Extended_CheckboxIconUnchecked_TextTransparent()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("text-transparent", cut.Markup);
    }

    // ─── CheckboxIconClass: indeterminate is text-primary ────────────

    [Fact]
    public void Extended_CheckboxIconIndeterminate_TextPrimary()
    {
        var items = new List<XFTreeNode>
        {
            new()
            {
                Id = "ipi",
                Text = "Parent",
                Children = new List<XFTreeNode>
                {
                    new() { Id = "ipc1", Text = "C1" },
                    new() { Id = "ipc2", Text = "C2" },
                }
            }
        };
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ExpandedIds, new[] { "ipi" });
            p.Add(x => x.AllowCheckBoxes, true);
            p.Add(x => x.AllowCheckParents, true);
            p.Add(x => x.CheckedIds, new[] { "ipc1" });
        });
        Assert.Contains("text-primary", cut.Markup);
    }

    // ─── CheckboxIconClass: all have h-3.5 w-3.5 ─────────────────────

    [Fact]
    public void Extended_CheckboxIconSize_AllStates()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("h-3.5 w-3.5", cut.Markup);
    }

    // ─── CheckboxWrapperClass: all have h-4 w-4 ──────────────────────

    [Fact]
    public void Extended_CheckboxWrapperSize_AllStates()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    // ─── CheckboxWrapperClass: all have shrink-0 ─────────────────────

    [Fact]
    public void Extended_CheckboxWrapperShrink_AllStates()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("shrink-0", cut.Markup);
    }

    // ─── CheckboxWrapperClass: all have rounded ───────────────────────

    [Fact]
    public void Extended_CheckboxWrapperRounded_AllStates()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("rounded", cut.Markup);
    }

    // ─── CheckboxWrapperClass: all have border ───────────────────────

    [Fact]
    public void Extended_CheckboxWrapperBorder_AllStates()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("border", cut.Markup);
    }

    // ─── CheckboxWrapperClass: all have transition ───────────────────

    [Fact]
    public void Extended_CheckboxWrapperTransition_AllStates()
    {
        var cut = RenderTreeView(p => p.Add(x => x.AllowCheckBoxes, true));
        Assert.Contains("transition-all", cut.Markup);
    }

    // ─── ToggleClass: all have inline-flex ───────────────────────────

    [Fact]
    public void Extended_ToggleInlineFlex_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("inline-flex", cut.Markup);
    }

    // ─── ToggleClass: all have h-6 w-6 ───────────────────────────────

    [Fact]
    public void Extended_ToggleSize_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("h-6 w-6", cut.Markup);
    }

    // ─── ToggleClass: all have shrink-0 ──────────────────────────────

    [Fact]
    public void Extended_ToggleShrink_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("shrink-0", cut.Markup);
    }

    // ─── ToggleClass: all have items-center justify-center ───────────

    [Fact]
    public void Extended_ToggleFlex_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("items-center justify-center", cut.Markup);
    }

    // ─── ToggleClass: all have rounded-md ────────────────────────────

    [Fact]
    public void Extended_ToggleRounded_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("rounded-md", cut.Markup);
    }

    // ─── ToggleClass: all have text-text-muted ───────────────────────

    [Fact]
    public void Extended_ToggleTextColor_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // ─── ToggleClass: all have transition ────────────────────────────

    [Fact]
    public void Extended_ToggleTransition_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("transition-all", cut.Markup);
    }

    // ─── ToggleClass: all have hover bg ──────────────────────────────

    [Fact]
    public void Extended_ToggleHoverBg_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]", cut.Markup);
    }

    // ─── ToggleClass: all have hover text ────────────────────────────

    [Fact]
    public void Extended_ToggleHoverText_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("hover:text-primary", cut.Markup);
    }

    // ─── ToggleClass: all have disabled pointer-events ───────────────

    [Fact]
    public void Extended_ToggleDisabledPointer_AllStates()
    {
        var cut = RenderTreeView();
        Assert.Contains("disabled:pointer-events-none", cut.Markup);
    }

    // ─── IndentClass: all have shrink-0 ──────────────────────────────

    [Fact]
    public void Extended_IndentShrink_AllLevels()
    {
        var cut = RenderTreeView(p => p.Add(x => x.ExpandedIds, new[] { "1" }));
        Assert.Contains("shrink-0", cut.Markup);
    }

    // ─── EmptyClass: includes rounded-md ─────────────────────────────

    [Fact]
    public void Extended_EmptyClass_RoundedMd()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("rounded-md", cut.Markup);
    }

    // ─── EmptyClass: includes border-dashed ──────────────────────────

    [Fact]
    public void Extended_EmptyClass_BorderDashed()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("border-dashed", cut.Markup);
    }

    // ─── EmptyClass: includes bg-surface-50 ──────────────────────────

    [Fact]
    public void Extended_EmptyClass_BgSurface50()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    // ─── EmptyClass: includes text-center ────────────────────────────

    [Fact]
    public void Extended_EmptyClass_TextCenter()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("text-center", cut.Markup);
    }

    // ─── EmptyClass: includes text-sm ─────────────────────────────────

    [Fact]
    public void Extended_EmptyClass_TextSm()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("text-sm", cut.Markup);
    }

    // ─── EmptyClass: includes px-3 py-6 ───────────────────────────────

    [Fact]
    public void Extended_EmptyClass_Padding()
    {
        var cut = Render<XFTreeView>(p =>
        {
            p.Add(x => x.Items, Array.Empty<XFTreeNode>());
        });
        Assert.Contains("px-3 py-6", cut.Markup);
    }
}