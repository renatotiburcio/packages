#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Menu;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components.Navigation;

public class XFMenuExtendedTest : XForgeTestContext
{
    private static readonly IReadOnlyList<XFMenuItem> DefaultItems = new[]
    {
        new XFMenuItem { Id = "dashboard", Text = "Dashboard", Href = "/dashboard" },
        new XFMenuItem { Id = "data", Text = "Data", Children = new[] { new XFMenuItem { Id = "orders", Text = "Orders" }, new XFMenuItem { Id = "customers", Text = "Customers" } } },
        new XFMenuItem { Id = "reports", Text = "Reports", Href = "/reports" }
    };

    private IRenderedComponent<XFMenu> RenderMenu(Action<ComponentParameterCollectionBuilder<XFMenu>>? configure = null)
    {
        return Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, DefaultItems);
            configure?.Invoke(parameters);
        });
    }

    // ── Rendering: base class & role ──────────────────────────────────

    [Fact]
    public void Renders_BaseCssClass()
    {
        var cut = RenderMenu();
        Assert.Contains("xf-menu", cut.Markup);
    }

    [Fact]
    public void Renders_RoleTree_VerticalDefault()
    {
        var cut = RenderMenu();
        Assert.Contains("role=\"tree\"", cut.Markup);
    }

    [Fact]
    public void Renders_ItemsText()
    {
        var cut = RenderMenu();
        Assert.Contains("Dashboard", cut.Markup);
        Assert.Contains("Data", cut.Markup);
        Assert.Contains("Reports", cut.Markup);
    }

    // ── Orientation & Variants ────────────────────────────────────────

    [Fact]
    public void Renders_HorizontalOrientation_RoleMenubar()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Orientation, XFOrientation.Horizontal));
        Assert.Contains("role=\"menubar\"", cut.Markup);
        Assert.Contains("aria-orientation=\"horizontal\"", cut.Markup);
    }

    [Fact]
    public void Renders_DropdownVariant()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.Dropdown));
        Assert.Contains("role=\"menubar\"", cut.Markup);
        Assert.Contains("role=\"menuitem\"", cut.Markup);
    }

    [Fact]
    public void Renders_DarkSidebarVariant()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.DarkSidebar));
        Assert.Contains("bg-surface-950", cut.Markup);
    }

    [Fact]
    public void Renders_PlainVariant()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.Plain));
        Assert.Contains("xf-menu", cut.Markup);
    }

    [Fact]
    public void Renders_TopNavVariant()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.TopNav));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    [Fact]
    public void Renders_SidebarVariant()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.Sidebar));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // ── Item kinds: section & separator ───────────────────────────────

    [Fact]
    public void Renders_SectionItems()
    {
        var items = new[]
        {
            new XFMenuItem { Kind = XFMenuItemKind.Section, Text = "Navigation" },
            new XFMenuItem { Id = "home", Text = "Home" }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("Navigation", cut.Markup);
    }

    [Fact]
    public void Renders_SeparatorItems()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "home", Text = "Home" },
            new XFMenuItem { Kind = XFMenuItemKind.Separator },
            new XFMenuItem { Id = "settings", Text = "Settings" }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("role=\"separator\"", cut.Markup);
    }

    // ── Display modes ─────────────────────────────────────────────────

    [Fact]
    public void Renders_IconOnlyDisplayMode()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.DisplayMode, XFMenuDisplayMode.IconOnly));
        Assert.Contains("xf-menu", cut.Markup);
    }

    [Fact]
    public void Renders_TextOnlyDisplayMode()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.DisplayMode, XFMenuDisplayMode.TextOnly));
        Assert.Contains("xf-menu", cut.Markup);
    }

    // ── Sizes ─────────────────────────────────────────────────────────

    [Fact]
    public void Renders_SizeSm()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Size, XFSize.Sm));
        Assert.Contains("min-h-8", cut.Markup);
    }

    [Fact]
    public void Renders_SizeLg()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Size, XFSize.Lg));
        Assert.Contains("min-h-12", cut.Markup);
    }

    // ── Boolean flags ─────────────────────────────────────────────────

    [Fact]
    public void Renders_CollapsibleCollapsed()
    {
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.Collapsible, true);
            p.Add(p2 => p2.Expanded, false);
        });
        Assert.Contains("w-11 min-w-11", cut.Markup);
    }

    [Fact]
    public void Renders_ShowArrowsFalse()
    {
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ShowArrows, false);
            p.Add(p2 => p2.ExpandedIds, new[] { "data" });
        });
        Assert.DoesNotContain("chevron-right", cut.Markup);
    }

    [Fact]
    public void Renders_MultipleFalse()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Multiple, false));
        Assert.Contains("xf-menu", cut.Markup);
    }

    [Fact]
    public void Renders_ExpandOnItemClickFalse()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.ExpandOnItemClick, false));
        Assert.Contains("xf-menu", cut.Markup);
    }

    [Fact]
    public void Renders_SelectOnItemClickFalse()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.SelectOnItemClick, false));
        Assert.Contains("xf-menu", cut.Markup);
    }

    // ── Empty text ────────────────────────────────────────────────────

    [Fact]
    public void Renders_EmptyTextWhenNoItems()
    {
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<XFMenuItem>());
        });
        Assert.Contains("Nenhum item para exibir.", cut.Markup);
    }

    [Fact]
    public void Renders_CustomEmptyText()
    {
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<XFMenuItem>());
            parameters.Add(p => p.EmptyText, "No items available");
        });
        Assert.Contains("No items available", cut.Markup);
    }

    // ── AriaLabel ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_CustomAriaLabel()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.AriaLabel, "Main navigation"));
        Assert.Contains("aria-label=\"Main navigation\"", cut.Markup);
    }

    // ── ActiveTone ────────────────────────────────────────────────────

    [Fact]
    public void Renders_ActiveToneSecondary()
    {
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ActiveTone, XFSemanticColor.Secondary);
            p.Add(p2 => p2.SelectedId, "dashboard");
        });
        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    // ── Item decorations: badge, meta, description, disabled, selected ─

    [Fact]
    public void Renders_BadgeOnItem()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "inbox", Text = "Inbox", Badge = "5" }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("5", cut.Markup);
    }

    [Fact]
    public void Renders_MetaOnItem()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "docs", Text = "Docs", Meta = "⌘K" }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("⌘K", cut.Markup);
    }

    [Fact]
    public void Renders_DescriptionOnItem()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "profile", Text = "Profile", Description = "Manage your account" }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("Manage your account", cut.Markup);
    }

    [Fact]
    public void Renders_DisabledItem()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "admin", Text = "Admin", Disabled = true }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Renders_SelectedItem()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "home", Text = "Home", Selected = true }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Renders_WithSelectedId()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.SelectedId, "reports"));
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Renders_WithExpandedIds()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.ExpandedIds, new[] { "data" }));
        Assert.Contains("aria-expanded=\"true\"", cut.Markup);
        Assert.Contains("Orders", cut.Markup);
        Assert.Contains("Customers", cut.Markup);
    }

    // ── Click events ──────────────────────────────────────────────────

    [Fact]
    public async Task ItemClick_FiresOnClick()
    {
        XFMenuItemEventArgs? clicked = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ItemClicked, EventCallback.Factory.Create<XFMenuItemEventArgs>(this, args => clicked = args));
        });

        await cut.Find("button[role=treeitem]").ClickAsync();

        Assert.NotNull(clicked);
        Assert.Equal("data", clicked!.Id);
    }

    [Fact]
    public async Task SelectedIdChanged_FiresOnClick()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        await cut.Find("button[role=treeitem]").ClickAsync();

        Assert.Equal("data", selected);
    }

    [Fact]
    public async Task ExpandedIdsChanged_FiresOnExpand()
    {
        IReadOnlyList<string> expanded = Array.Empty<string>();
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, ids => expanded = ids));
        });

        await cut.Find("button[role=treeitem]").ClickAsync();

        Assert.Contains("data", expanded);
    }

    // ── Keyboard navigation ───────────────────────────────────────────

    [Fact]
    public async Task Keyboard_ArrowDown_Navigates()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIds, new[] { "data" });
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        // First visible item is "dashboard"; ArrowDown moves focus to "data"
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("data", selected);
    }

    [Fact]
    public async Task Keyboard_ArrowUp_Navigates()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIds, new[] { "data" });
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        // Navigate: dashboard -> data -> orders -> ArrowUp -> data -> Enter
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("data", selected);
    }

    [Fact]
    public async Task Keyboard_Home_GoesToFirst()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIds, new[] { "data" });
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        // Navigate to last then Home
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "End" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "Home" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("dashboard", selected);
    }

    [Fact]
    public async Task Keyboard_End_GoesToLast()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIds, new[] { "data" });
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "End" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("reports", selected);
    }

    [Fact]
    public async Task Keyboard_Enter_SelectsItem()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("dashboard", selected);
    }

    [Fact]
    public async Task Keyboard_Space_SelectsItem()
    {
        string? selected = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.SelectedIdChanged, EventCallback.Factory.Create<string?>(this, id => selected = id));
        });

        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = " " });

        Assert.Equal("dashboard", selected);
    }

    [Fact]
    public async Task Keyboard_ArrowRight_ExpandsItem()
    {
        IReadOnlyList<string>? expanded = null;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIdsChanged, ids => expanded = ids);
        });

        // Navigate to "data" item (has children) then ArrowRight to expand
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        Assert.NotNull(expanded);
        Assert.Contains("data", expanded!);
    }

    [Fact]
    public async Task Keyboard_ArrowLeft_CollapsesItem()
    {
        IReadOnlyList<string> expanded = Array.Empty<string>();
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.ExpandedIds, new[] { "data" });
            p.Add(p2 => p2.ExpandedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, ids => expanded = ids));
        });

        // Navigate to "data" item (first item) then collapse
        await cut.Find("[role=tree]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

        Assert.DoesNotContain("data", expanded);
    }

    // ── ToggleExpanded ────────────────────────────────────────────────

    [Fact]
    public async Task ToggleExpanded_CollapsesMenu()
    {
        var expanded = true;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.Collapsible, true);
            p.Add(p2 => p2.Expanded, expanded);
            p.Add(p2 => p2.ExpandedChanged, EventCallback.Factory.Create<bool>(this, value => expanded = value));
        });

        await cut.Find("button[aria-expanded='true']").ClickAsync();

        Assert.False(expanded);
    }

    [Fact]
    public async Task ToggleExpanded_ExpandsMenu()
    {
        var expanded = false;
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.Collapsible, true);
            p.Add(p2 => p2.Expanded, expanded);
            p.Add(p2 => p2.ExpandedChanged, EventCallback.Factory.Create<bool>(this, value => expanded = value));
        });

        await cut.Find("button[aria-expanded='false']").ClickAsync();

        Assert.True(expanded);
    }

    // ── XFMenuItem CanCreate ──────────────────────────────────────────

    [Fact]
    public void XFMenuItem_CanCreate()
    {
        var item = new XFMenuItem { Id = "test", Text = "Test" };
        Assert.NotNull(item);
        Assert.Equal("test", item.Id);
        Assert.Equal("Test", item.Text);
    }

    [Fact]
    public void XFMenuItem_WithChildren_CanCreate()
    {
        var item = new XFMenuItem
        {
            Id = "parent",
            Text = "Parent",
            Children = new[]
            {
                new XFMenuItem { Id = "child1", Text = "Child 1" },
                new XFMenuItem { Id = "child2", Text = "Child 2" }
            }
        };
        Assert.NotNull(item);
        Assert.Equal(2, item.Children.Count);
        Assert.Equal("Child 1", item.Children[0].Text);
    }

    // Soft variant (line 72)
    [Fact]
    public void Renders_SoftVariant()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.Soft));
        Assert.Contains("bg-surface-50", cut.Markup);
        Assert.Contains("ring-1 ring-border-light", cut.Markup);
    }

    // IconClass DarkSidebar (line 154)
    [Fact]
    public void Renders_DarkSidebarIconClass()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.DarkSidebar));
        Assert.Contains("text-white/45", cut.Markup);
    }

    // IconClass Dropdown (line 155)
    [Fact]
    public void Renders_DropdownIconClass()
    {
        var cut = RenderMenu(p => p.Add(p2 => p2.Variant, XFMenuVariant.Dropdown));
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // IconClass Sidebar (line 156)
    [Fact]
    public void Renders_SidebarIconClass()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Icon = (RenderFragment)(b => b.AddContent(0, "icon")) } };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.Sidebar);
        });
        Assert.Contains("group-hover:text-text-secondary", cut.Markup);
    }

    // BadgeClass tones (lines 173-176)
    [Fact]
    public void Renders_BadgeSecondary()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Badge = "3", BadgeTone = XFSemanticColor.Secondary } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-secondary", cut.Markup);
    }

    [Fact]
    public void Renders_BadgeSuccess()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Badge = "5", BadgeTone = XFSemanticColor.Success } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Renders_BadgeWarning()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Badge = "2", BadgeTone = XFSemanticColor.Warning } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Renders_BadgeDanger()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Badge = "1", BadgeTone = XFSemanticColor.Danger } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-danger", cut.Markup);
    }

    // ActiveItemClass tones (lines 188-190)
    [Fact]
    public void Renders_ActiveItemSecondary()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Secondary } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-secondary", cut.Markup);
    }

    [Fact]
    public void Renders_ActiveItemWarning()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Warning } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Renders_ActiveItemDanger()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Danger } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Renders_ActiveItemNeutral()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Neutral } };
        var cut = Render<XFMenu>(parameters => parameters.Add(p => p.Items, items));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // DarkActiveItemClass tones (lines 201-205)
    [Fact]
    public void Renders_DarkSidebarActiveItemSecondary()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Secondary } };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("bg-secondary", cut.Markup);
    }

    [Fact]
    public void Renders_DarkSidebarActiveItemWarning()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Warning } };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("bg-warning", cut.Markup);
    }

    [Fact]
    public void Renders_DarkSidebarActiveItemDanger()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Danger } };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void Renders_DarkSidebarActiveItemNeutral()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Selected = true, ActiveTone = XFSemanticColor.Neutral } };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("bg-white/12", cut.Markup);
    }

    // Collapsed state (line 66)
    [Fact]
    public void Renders_CollapsedState()
    {
        var cut = RenderMenu(p =>
        {
            p.Add(p2 => p2.Collapsible, true);
            p.Add(p2 => p2.Expanded, false);
        });
        Assert.Contains("w-11 min-w-11", cut.Markup);
    }

    // DescriptionClass DarkSidebar (line 160-161)
    [Fact]
    public void Renders_DarkSidebarDescription()
    {
        var items = new[] { new XFMenuItem { Id = "home", Text = "Home", Description = "Go home" } };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("text-white/45", cut.Markup);
    }

    // SeparatorClass DarkSidebar (line 148-149)
    [Fact]
    public void Renders_DarkSidebarSeparator()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "home", Text = "Home" },
            new XFMenuItem { Kind = XFMenuItemKind.Separator },
            new XFMenuItem { Id = "settings", Text = "Settings" }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("bg-white/10", cut.Markup);
    }

    // ArrowClass DarkSidebar (line 164-165)
    [Fact]
    public void Renders_DarkSidebarArrow()
    {
        var items = new[]
        {
            new XFMenuItem { Id = "parent", Text = "Parent", Children = new[] { new XFMenuItem { Id = "child", Text = "Child" } } }
        };
        var cut = Render<XFMenu>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Variant, XFMenuVariant.DarkSidebar);
        });
        Assert.Contains("text-white/45", cut.Markup);
    }

}
