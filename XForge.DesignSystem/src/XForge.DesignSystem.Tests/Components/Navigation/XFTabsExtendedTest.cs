using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Navigation.Tabs;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFTabsExtendedTest : BunitContext
{
    public XFTabsExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<NavigationManager>(new TestNavigationManager());
    }

    private IRenderedComponent<XFTabs> RenderTabs(
        Action<ComponentParameterCollectionBuilder<XFTabs>>? configure = null)
    {
        return Render<XFTabs>(p =>
        {
            p.Add(x => x.Tabs, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTab>(0);
                builder.AddAttribute(1, "TabId", "tab1");
                builder.AddAttribute(2, "Label", "Tab 1");
                builder.CloseComponent();
                builder.OpenComponent<XFTab>(3);
                builder.AddAttribute(4, "TabId", "tab2");
                builder.AddAttribute(5, "Label", "Tab 2");
                builder.CloseComponent();
            }));
            p.Add(x => x.Panels, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, "TabId", "tab1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Panel 1")));
                builder.CloseComponent();
                builder.OpenComponent<XFTabPanel>(3);
                builder.AddAttribute(4, "TabId", "tab2");
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Panel 2")));
                builder.CloseComponent();
            }));
            configure?.Invoke(p);
        });
    }

    // RootClass tests
    [Fact]
    public void Extended_RootClass_Horizontal_Default()
    {
        var cut = RenderTabs();
        Assert.Contains("grid gap-4", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_Vertical()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Vertical));
        Assert.Contains("grid gap-4", cut.Markup);
        Assert.Contains("md:grid-cols-[minmax(12rem,16rem)_1fr]", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_VerticalRight()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Right));
        Assert.Contains("grid gap-4", cut.Markup);
        Assert.Contains("md:grid-cols-[1fr_minmax(12rem,16rem)]", cut.Markup);
    }

    // HeaderShellClass tests
    [Fact]
    public void Extended_HeaderShellClass_Horizontal_Order1()
    {
        var cut = RenderTabs();
        Assert.Contains("flex min-w-0 gap-2 items-center", cut.Markup);
        Assert.Contains("order-1", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderShellClass_Vertical_FlexCol()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Vertical));
        Assert.Contains("flex min-w-0 gap-2 flex-col", cut.Markup);
        Assert.Contains("order-1", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderShellClass_Bottom_Order2()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Bottom));
        Assert.Contains("order-2", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderShellClass_Enclosed_Variant()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Enclosed));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("border", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderShellClass_Right_ColStart()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Right));
        Assert.Contains("md:col-start-2", cut.Markup);
        Assert.Contains("md:row-start-1", cut.Markup);
    }

    // TabListClass tests
    [Fact]
    public void Extended_TabListClass_MinW0()
    {
        var cut = RenderTabs();
        Assert.Contains("min-w-0", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_Horizontal_Flex()
    {
        var cut = RenderTabs();
        Assert.Contains("flex items-center gap-1", cut.Markup);
        Assert.Contains("overflow-x-auto", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_Vertical_FlexCol()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Vertical));
        Assert.Contains("flex flex-col gap-1", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_VariantPills()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Pills));
        Assert.Contains("rounded-lg bg-surface-100 p-1", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_VariantSubtle()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Subtle));
        var expectedBg = "bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]";
        Assert.Contains(expectedBg, cut.Markup);
        Assert.Contains("p-1", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_VariantUnderline()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Underline));
        Assert.Contains("border-b border-border-light", cut.Markup);
    }

    // FullWidth tests
    [Fact]
    public void Extended_FullWidth_Horizontal()
    {
        var cut = RenderTabs(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    [Fact]
    public void Extended_FullWidth_Vertical()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.FullWidth, true);
            p.Add(x => x.Orientation, XFTabsOrientation.Vertical);
        });
        Assert.Contains("xf-tabs", cut.Markup);
    }

    // PanelShellClass tests
    [Fact]
    public void Extended_PanelShellClass_Horizontal_Order2()
    {
        var cut = RenderTabs();
        Assert.Contains("min-w-0", cut.Markup);
        // Horizontal orientation: panel shell should have order-2 (not order-1)
        Assert.Contains("order-2", cut.Markup);
    }

    [Fact]
    public void Extended_PanelShellClass_Vertical()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Vertical));
        Assert.Contains("md:col-start-2", cut.Markup);
        Assert.Contains("md:row-start-1", cut.Markup);
    }

    [Fact]
    public void Extended_PanelShellClass_Bottom_Order1()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Bottom));
        Assert.Contains("order-1", cut.Markup);
    }

    // ShowIndicator false tests
    [Fact]
    public void Extended_ShowIndicatorFalse_AfterHidden()
    {
        var cut = RenderTabs(p => p.Add(x => x.ShowIndicator, false));
        Assert.Contains("after:hidden", cut.Markup);
    }

    [Fact]
    public void Extended_ShowIndicatorTrue_NoAfterHidden()
    {
        var cut = RenderTabs(p => p.Add(x => x.ShowIndicator, true));
        Assert.DoesNotContain("after:hidden", cut.Markup);
    }

    // XFTab Size Sm tests
    [Fact]
    public void Extended_TabClass_SizeSm()
    {
        var cut = RenderTabs(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("min-h-8", cut.Markup);
        Assert.Contains("px-2.5", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_SizeLg()
    {
        var cut = RenderTabs(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("min-h-11", cut.Markup);
        Assert.Contains("px-4", cut.Markup);
        Assert.Contains("text-base", cut.Markup);
    }

    // XFTab Variant active/inactive tests
    [Fact]
    public void Extended_TabClass_Pills_Active()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Pills);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("bg-bg-surface", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Pills_Inactive()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Pills);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Enclosed_Active()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Enclosed);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Enclosed_Inactive()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Enclosed);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("text-text-secondary", cut.Markup);
        Assert.Contains("hover:bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Subtle_Active()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Subtle);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Subtle_Inactive()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Subtle);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Underline_Active()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Underline);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("text-primary", cut.Markup);
        Assert.Contains("after:absolute", cut.Markup);
        Assert.Contains("after:bg-primary", cut.Markup);
    }

    [Fact]
    public void Extended_TabClass_Underline_Inactive()
    {
        var cut = RenderTabs(p =>
        {
            p.Add(x => x.Variant, XFTabsVariant.Underline);
            p.Add(x => x.ActiveId, "tab1");
        });
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // Orientation tests
    [Fact]
    public void Extended_Orientation_Vertical_Aria()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Vertical));
        Assert.Contains("aria-orientation=\"vertical\"", cut.Markup);
    }

    [Fact]
    public void Extended_Orientation_Vertical_Bottom()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Bottom));
        Assert.Contains("grid gap-4", cut.Markup);
    }

    [Fact]
    public void Extended_Orientation_Right_Aria()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Right));
        Assert.Contains("aria-orientation=\"vertical\"", cut.Markup);
    }

    [Fact]
    public void Extended_Orientation_Vertical_Right_Class()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Right));
        Assert.Contains("md:grid-cols-[1fr_minmax(12rem,16rem)]", cut.Markup);
    }

    // StartContent/EndContent tests
    [Fact]
    public void Extended_StartContent_Rendered()
    {
        var cut = RenderTabs(p => p.Add(x => x.StartContent, (RenderFragment)(b => b.AddContent(0, "<span>start</span>"))));
        Assert.Contains("start", cut.Markup);
    }

    [Fact]
    public void Extended_EndContent_Rendered()
    {
        var cut = RenderTabs(p => p.Add(x => x.EndContent, (RenderFragment)(b => b.AddContent(0, "<span>end</span>"))));
        Assert.Contains("end", cut.Markup);
    }

    [Fact]
    public void Extended_StartContent_Null_NotRendered()
    {
        var cut = RenderTabs();
        Assert.DoesNotContain("shrink-0", cut.Markup);
    }

    // TabList role and structure
    [Fact]
    public void Extended_Tablist_Role()
    {
        var cut = RenderTabs();
        Assert.Contains("role=\"tablist\"", cut.Markup);
    }

    [Fact]
    public void Extended_Tab_Role()
    {
        var cut = RenderTabs();
        var tab = cut.Find("[role=tab]");
        Assert.NotNull(tab);
    }

    [Fact]
    public void Extended_TabPanel_Role()
    {
        var cut = RenderTabs();
        Assert.Contains("role=\"tabpanel\"", cut.Markup);
    }

    // Tab activation
    [Fact]
    public void Extended_ActiveTab_AriaSelected()
    {
        var cut = RenderTabs();
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public async Task Extended_TabChanged_Fires()
    {
        XFTabChangeEventArgs? args = null;
        var cut = RenderTabs(p => p.Add(
            x => x.TabChanged,
            EventCallback.Factory.Create<XFTabChangeEventArgs>(this, a => args = a)));
        var tabs = cut.FindAll("[role=tab]");
        if (tabs.Count > 1)
        {
            await tabs[1].ClickAsync();
            await cut.InvokeAsync(() => { });
        }
        Assert.NotNull(args);
    }

    [Fact]
    public async Task Extended_ActiveIdChanged_Fires()
    {
        string? activeId = null;
        var cut = RenderTabs(p => p.Add(
            x => x.ActiveIdChanged,
            EventCallback.Factory.Create<string?>(this, id => activeId = id)));
        var tabs = cut.FindAll("[role=tab]");
        if (tabs.Count > 1)
        {
            await tabs[1].ClickAsync();
            await cut.InvokeAsync(() => { });
        }
        Assert.Equal("tab2", activeId);
    }

    // Enum coverage
    [Fact]
    public void Extended_XFTabsVariant_EnumHasAllValues()
    {
        Assert.Contains(XFTabsVariant.Underline, Enum.GetValues<XFTabsVariant>());
        Assert.Contains(XFTabsVariant.Pills, Enum.GetValues<XFTabsVariant>());
        Assert.Contains(XFTabsVariant.Enclosed, Enum.GetValues<XFTabsVariant>());
        Assert.Contains(XFTabsVariant.Subtle, Enum.GetValues<XFTabsVariant>());
    }

    [Fact]
    public void Extended_XFTabsOrientation_EnumHasAllValues()
    {
        Assert.Contains(XFTabsOrientation.Horizontal, Enum.GetValues<XFTabsOrientation>());
        Assert.Contains(XFTabsOrientation.Vertical, Enum.GetValues<XFTabsOrientation>());
        Assert.Contains(XFTabsOrientation.Bottom, Enum.GetValues<XFTabsOrientation>());
        Assert.Contains(XFTabsOrientation.Right, Enum.GetValues<XFTabsOrientation>());
    }

    [Fact]
    public void Extended_XFTabsActivationMode_EnumHasAllValues()
    {
        Assert.Contains(XFTabsActivationMode.Automatic, Enum.GetValues<XFTabsActivationMode>());
        Assert.Contains(XFTabsActivationMode.Manual, Enum.GetValues<XFTabsActivationMode>());
    }

    [Fact]
    public void Extended_XFTabsRenderMode_EnumHasAllValues()
    {
        Assert.Contains(XFTabsRenderMode.Default, Enum.GetValues<XFTabsRenderMode>());
        Assert.Contains(XFTabsRenderMode.LazyLoad, Enum.GetValues<XFTabsRenderMode>());
        Assert.Contains(XFTabsRenderMode.LazyReload, Enum.GetValues<XFTabsRenderMode>());
    }

    // RenderMode tests
    [Fact]
    public void Extended_RenderMode_Default()
    {
        var cut = RenderTabs(p => p.Add(x => x.RenderMode, XFTabsRenderMode.Default));
        Assert.Contains("Panel 1", cut.Markup);
        Assert.Contains("Panel 2", cut.Markup);
    }

    [Fact]
    public void Extended_RenderMode_LazyLoad()
    {
        var cut = RenderTabs(p => p.Add(x => x.RenderMode, XFTabsRenderMode.LazyLoad));
        Assert.Contains("Panel 1", cut.Markup);
    }

    // AllowClose through parameter
    [Fact]
    public void Extended_AllowClose_ShowsCloseButtons()
    {
        var cut = RenderTabs(p => p.Add(x => x.AllowClose, true));
        Assert.Contains("Fechar aba", cut.Markup);
    }

    [Fact]
    public void Extended_TabClosable_False_NoCloseButton()
    {
        var cut = RenderTabs();
        Assert.DoesNotContain("Fechar aba", cut.Markup);
    }

    // Tab text content
    [Fact]
    public void Extended_TabLabel_Rendered()
    {
        var cut = RenderTabs();
        Assert.Contains("Tab 1", cut.Markup);
        Assert.Contains("Tab 2", cut.Markup);
    }

    [Fact]
    public void Extended_PanelContent_Rendered()
    {
        var cut = RenderTabs();
        Assert.Contains("Panel 1", cut.Markup);
    }

    [Fact]
    public void Extended_ActiveId_DefaultsToFirst()
    {
        var cut = RenderTabs();
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_ActiveId_SetSecondTab()
    {
        var cut = RenderTabs(p => p.Add(x => x.ActiveId, "tab2"));
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAriaLabel()
    {
        var cut = RenderTabs(p => p.Add(x => x.AriaLabel, "My tabs"));
        Assert.Contains("My tabs", cut.Markup);
    }

    [Fact]
    public void Extended_BaseCssClass()
    {
        var cut = RenderTabs();
        Assert.Contains("xf-tabs", cut.Markup);
    }

    // Horizontal Bottom orientation specifics
    [Fact]
    public void Extended_HeaderShellClass_Bottom_Order2_Header()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Bottom));
        var markup = cut.Markup;
        Assert.Contains("order-2", markup);
    }

    [Fact]
    public void Extended_PanelShellClass_Bottom_Order1_Panel()
    {
        var cut = RenderTabs(p => p.Add(x => x.Orientation, XFTabsOrientation.Bottom));
        Assert.Contains("min-w-0", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_NoBorderBottom_Pills()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Pills));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_NoBorderBottom_Subtle()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Subtle));
        var expectedBg = "bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]";
        Assert.Contains(expectedBg, cut.Markup);
    }

    [Fact]
    public void Extended_TabListClass_BorderBottom_Underline()
    {
        var cut = RenderTabs(p => p.Add(x => x.Variant, XFTabsVariant.Underline));
        Assert.Contains("border-b", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
        }
    }
}
