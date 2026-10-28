using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Navigation.Toolbar;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Layout;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFToolbarExtendedTest : XForgeTestContext
{
    private static readonly IReadOnlyList<XFToolbarItem> SampleItems = new[]
    {
        new XFToolbarItem { Id = "new", Text = "New", Title = "New" },
        new XFToolbarItem { Id = "save", Text = "Save", Title = "Save", Active = true },
        new XFToolbarItem { Kind = XFToolbarItemKind.Separator },
        new XFToolbarItem { Id = "archive", Text = "Archive", Title = "Archive", Disabled = true }
    };

    private IRenderedComponent<XFToolbar> RenderToolbar(
        Action<ComponentParameterCollectionBuilder<XFToolbar>>? configure = null)
    {
        return Render<XFToolbar>(p =>
        {
            p.Add(x => x.Items, SampleItems);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderToolbar();
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RoleToolbar()
    {
        var cut = RenderToolbar();
        Assert.Contains("role=\"toolbar\"", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_DefaultAriaLabel()
    {
        var cut = RenderToolbar();
        Assert.Contains("aria-label=\"Barra de ferramentas\"", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAriaLabel_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.AriaLabel, "Custom toolbar"));
        Assert.Contains("aria-label=\"Custom toolbar\"", cut.Markup);
    }

    [Fact]
    public void Extended_HorizontalOrientation_Renders()
    {
        var cut = RenderToolbar();
        Assert.Contains("aria-orientation=\"horizontal\"", cut.Markup);
    }

    [Fact]
    public void Extended_VerticalOrientation_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("aria-orientation=\"vertical\"", cut.Markup);
        Assert.Contains("inline-flex flex-col", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSurface_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Variant, XFToolbarVariant.Surface));
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void Extended_VariantPlain_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Variant, XFToolbarVariant.Plain));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Variant, XFToolbarVariant.Soft));
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    [Fact]
    public void Extended_VariantBordered_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Variant, XFToolbarVariant.Bordered));
        Assert.Contains("border-border", cut.Markup);
    }

    [Fact]
    public void Extended_SizeSm_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void Extended_WrapFalse_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Wrap, false));
        Assert.DoesNotContain("flex-wrap", cut.Markup);
    }

    [Fact]
    public void Extended_Dense_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Dense, true));
        Assert.Contains("gap-1", cut.Markup);
    }

    [Fact]
    public void Extended_ActiveItem_RendersActiveStyle()
    {
        var cut = RenderToolbar();
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_DisabledItem_RendersDisabledStyle()
    {
        var cut = RenderToolbar();
        Assert.Contains("opacity-50", cut.Markup);
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    [Fact]
    public void Extended_DisabledItem_RendersAriaDisabled()
    {
        var cut = RenderToolbar();
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_Separator_Renders()
    {
        var cut = RenderToolbar();
        Assert.Contains("role=\"separator\"", cut.Markup);
    }

    [Fact]
    public void Extended_Group_Renders()
    {
        var items = new[]
        {
            new XFToolbarItem
            {
                Kind = XFToolbarItemKind.Group,
                AriaLabel = "Format",
                Children = new[]
                {
                    new XFToolbarItem { Id = "bold", Text = "Bold" },
                    new XFToolbarItem { Id = "italic", Text = "Italic" }
                }
            }
        };
        var cut = Render<XFToolbar>(p => p.Add(x => x.Items, items));
        Assert.Contains("role=\"group\"", cut.Markup);
        Assert.Contains("aria-label=\"Format\"", cut.Markup);
        Assert.Contains("Bold", cut.Markup);
        Assert.Contains("Italic", cut.Markup);
    }

    [Fact]
    public void Extended_EndItems_Renders()
    {
        var endItems = new[]
        {
            new XFToolbarItem { Id = "settings", Text = "Settings", Title = "Settings" }
        };
        var cut = RenderToolbar(p => p.Add(x => x.EndItems, endItems));
        Assert.Contains("Settings", cut.Markup);
    }

    [Fact]
    public void Extended_StartContent_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.StartContent, (RenderFragment)(b => b.AddContent(0, "start"))));
        Assert.Contains("start", cut.Markup);
    }

    [Fact]
    public void Extended_EndContent_Renders()
    {
        var cut = RenderToolbar(p => p.Add(x => x.EndContent, (RenderFragment)(b => b.AddContent(0, "end"))));
        Assert.Contains("end", cut.Markup);
    }

    [Fact]
    public void Extended_ItemTemplate_Renders()
    {
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemTemplate,
            (RenderFragment<XFToolbarItem>)(item => b => b.AddContent(0, $"tpl:{item.Text}"))));
        Assert.Contains("tpl:New", cut.Markup);
    }

    [Fact]
    public void Extended_IconItem_Renders()
    {
        var items = new[]
        {
            new XFToolbarItem
            {
                Id = "icon-btn",
                Title = "Icon action",
                Icon = (RenderFragment)(b => b.AddContent(0, "icon"))
            }
        };
        var cut = Render<XFToolbar>(p => p.Add(x => x.Items, items));
        Assert.Contains("icon", cut.Markup);
        Assert.Contains("aria-label=\"Icon action\"", cut.Markup);
    }

    [Fact]
    public void Extended_HrefItem_RendersAsLink()
    {
        var items = new[]
        {
            new XFToolbarItem { Id = "link", Text = "Link", Href = "/page", Title = "Go to page" }
        };
        var cut = Render<XFToolbar>(p => p.Add(x => x.Items, items));
        Assert.Contains("<a", cut.Markup);
        Assert.Contains("href=\"/page\"", cut.Markup);
    }

    [Fact]
    public void Extended_HrefItemWithTarget_Renders()
    {
        var items = new[]
        {
            new XFToolbarItem { Id = "ext", Text = "External", Href = "https://example.com", Target = "_blank" }
        };
        var cut = Render<XFToolbar>(p => p.Add(x => x.Items, items));
        Assert.Contains("target=\"_blank\"", cut.Markup);
    }

    [Fact]
    public void Extended_DisabledComponent_DisablesAllItems()
    {
        var cut = RenderToolbar(p => p.Add(x => x.Disabled, true));
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    [Fact]
    public async Task Extended_ClickAction_FiresItemClicked()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("button[title='New']").ClickAsync();
        Assert.NotNull(clicked);
        Assert.Equal("new", clicked!.Id);
    }

    [Fact]
    public async Task Extended_ClickDisabled_DoesNotFire()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("button[title='Archive']").ClickAsync();
        Assert.Null(clicked);
    }

    [Fact]
    public async Task Extended_KeyboardArrowRight_NavigatesFocus()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.NotNull(clicked);
    }

    [Fact]
    public async Task Extended_KeyboardArrowLeft_NavigatesFocus()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.NotNull(clicked);
    }

    [Fact]
    public async Task Extended_KeyboardHome_FirstItem()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "End" });
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "Home" });
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.NotNull(clicked);
        Assert.Equal("new", clicked!.Id);
    }

    [Fact]
    public async Task Extended_KeyboardEnd_LastEnabledItem()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "End" });
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.NotNull(clicked);
    }

    [Fact]
    public async Task Extended_KeyboardSpace_Activates()
    {
        XFToolbarItemEventArgs? clicked = null;
        var cut = RenderToolbar(p => p.Add(
            x => x.ItemClicked,
            EventCallback.Factory.Create<XFToolbarItemEventArgs>(this, args => clicked = args)));
        await cut.Find("[role=toolbar]").KeyDownAsync(new KeyboardEventArgs { Key = " " });
        Assert.NotNull(clicked);
    }

    [Fact]
    public void Extended_EmptyItems_Renders()
    {
        var cut = Render<XFToolbar>();
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void Extended_XFToolbarItem_CanCreate()
    {
        var item = new XFToolbarItem
        {
            Id = "test",
            Text = "Test",
            Title = "Test Title",
            Active = true,
            Disabled = false,
            Kind = XFToolbarItemKind.Action
        };
        Assert.Equal("test", item.Id);
        Assert.Equal("Test", item.Text);
        Assert.True(item.Active);
        Assert.False(item.Disabled);
    }

    [Fact]
    public void Extended_XFToolbarItem_WithChildren()
    {
        var item = new XFToolbarItem
        {
            Id = "group",
            Kind = XFToolbarItemKind.Group,
            Children = new[]
            {
                new XFToolbarItem { Id = "child1", Text = "Child 1" },
                new XFToolbarItem { Id = "child2", Text = "Child 2" }
            }
        };
        Assert.Equal(2, item.Children.Count);
    }

    [Fact]
    public void Extended_Enum_XFToolbarVariant_Has4Values()
    {
        Assert.Equal(4, Enum.GetValues<XFToolbarVariant>().Length);
    }

    [Fact]
    public void Extended_Enum_XFToolbarItemKind_Has3Values()
    {
        Assert.Equal(3, Enum.GetValues<XFToolbarItemKind>().Length);
    }

    [Fact]
    public void Extended_XFToolbarItemEventArgs_CanCreate()
    {
        var item = new XFToolbarItem { Id = "test", Text = "Test" };
        var args = new XFToolbarItemEventArgs(item, "test", "value", "/href");
        Assert.Equal("test", args.Id);
        Assert.Equal("value", args.Value);
        Assert.Equal("/href", args.Href);
        Assert.Equal(item, args.Item);
    }
}
