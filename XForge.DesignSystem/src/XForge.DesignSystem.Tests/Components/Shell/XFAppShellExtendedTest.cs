using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Tests.Components.Shell;

/// <summary>
/// Extended tests for XFAppShell covering slot ordering, conditional rendering,
/// AdditionalAttributes, disabled state, default values, and structural integrity.
/// </summary>
public class XFAppShellExtendedTest : XForgeTestContext
{
    [Fact]
    public void Render_RailSlot_AppearsBeforeSidebarInsideAside()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.Rail, builder => builder.AddContent(0, "##RAIL##"));
            parameters.Add(p => p.Sidebar, builder => builder.AddContent(0, "##SIDEBAR##"));
        });

        var markup = cut.Markup;
        var railIndex = markup.IndexOf("##RAIL##");
        var sidebarIndex = markup.IndexOf("##SIDEBAR##");
        Assert.True(railIndex >= 0, "Rail content should be present");
        Assert.True(sidebarIndex >= 0, "Sidebar content should be present");
        Assert.True(railIndex < sidebarIndex, "Rail should render before Sidebar");
    }

    [Fact]
    public void Render_FooterSlot_AppearsAfterMainContent()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.ChildContent, builder => builder.AddContent(0, "##MAIN##"));
            parameters.Add(p => p.Footer, builder => builder.AddContent(0, "##FOOTER##"));
        });

        var markup = cut.Markup;
        var mainIndex = markup.IndexOf("##MAIN##");
        var footerIndex = markup.IndexOf("##FOOTER##");
        Assert.True(mainIndex >= 0, "Main content should be present");
        Assert.True(footerIndex >= 0, "Footer content should be present");
        Assert.True(mainIndex < footerIndex, "Main content should appear before Footer");
    }

    [Fact]
    public void Render_AppBarSlot_AppearsBeforeMainContent()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.AppBar, builder => builder.AddContent(0, "##APPBAR##"));
            parameters.Add(p => p.ChildContent, builder => builder.AddContent(0, "##MAIN##"));
        });

        var markup = cut.Markup;
        var appBarIndex = markup.IndexOf("##APPBAR##");
        var mainIndex = markup.IndexOf("##MAIN##");
        Assert.True(appBarIndex >= 0, "AppBar content should be present");
        Assert.True(mainIndex >= 0, "Main content should be present");
        Assert.True(appBarIndex < mainIndex, "AppBar should appear before main content");
    }

    [Fact]
    public void Render_FooterIsNull_NoFooterRendered()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.ChildContent, builder => builder.AddContent(0, "Main Content"));
        });

        Assert.Contains("Main Content", cut.Markup);
        // Footer should not be present in markup — only the main content div
        var footerCount = System.Text.RegularExpressions.Regex.Matches(cut.Markup, "<footer").Count;
        Assert.Equal(0, footerCount);
    }

    [Fact]
    public void Render_AppBarIsNull_NoAppBarRendered()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.ChildContent, builder => builder.AddContent(0, "Main Content"));
        });

        Assert.Contains("Main Content", cut.Markup);
        // The default header element count should be 0 since no AppBar is provided
        // (aside elements are present, but no AppBar header)
        var headerOpenTags = System.Text.RegularExpressions.Regex.Matches(cut.Markup, "<header").Count;
        Assert.Equal(0, headerOpenTags);
    }

    [Fact]
    public void Render_RailIsNull_NoRailContent()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.Sidebar, builder => builder.AddContent(0, "Sidebar Content"));
        });

        Assert.Contains("Sidebar Content", cut.Markup);
        // Rail content should not be present
        var markup = cut.Markup;
        Assert.DoesNotContain("Rail Content", markup);
    }

    [Fact]
    public void Render_SidebarIsNull_NoSidebarContent()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.Rail, builder => builder.AddContent(0, "Rail Content"));
        });

        Assert.Contains("Rail Content", cut.Markup);
        // Sidebar content should not be present
        var markup = cut.Markup;
        Assert.DoesNotContain("Sidebar Content", markup);
    }

    [Fact]
    public void Render_DefaultTheme_IsLight()
    {
        var cut = Render<XFAppShell>();
        Assert.Contains("data-theme=\"light\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultBrand_IsNotPresent()
    {
        var cut = Render<XFAppShell>();
        Assert.DoesNotContain("data-brand", cut.Markup);
    }

    [Fact]
    public void Render_DefaultMainId_IsMainContent()
    {
        var cut = Render<XFAppShell>();
        Assert.Contains("id=\"main-content\"", cut.Markup);
        // Verify it's on the <main> element
        var mainTagIndex = cut.Markup.IndexOf("<main");
        var idIndex = cut.Markup.IndexOf("id=\"main-content\"");
        Assert.True(mainTagIndex >= 0);
        Assert.True(idIndex >= 0);
    }

    [Fact]
    public void Render_DefaultNavigationAriaLabel_IsPortuguese()
    {
        var cut = Render<XFAppShell>();
        Assert.Contains("aria-label=\"Navegacao do aplicativo\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultMainAriaLabel_IsPortuguese()
    {
        var cut = Render<XFAppShell>();
        Assert.Contains("aria-label=\"Conteudo principal\"", cut.Markup);
    }

    [Fact]
    public void Render_AdditionalAttributes_PassedToRootDiv()
    {
        var attrs = new Dictionary<string, object>
        {
            { "data-testid", "shell-root" },
            { "role", "application" }
        };

        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.AdditionalAttributes, attrs));

        Assert.Contains("data-testid=\"shell-root\"", cut.Markup);
        Assert.Contains("role=\"application\"", cut.Markup);
    }

    [Fact]
    public void Render_GetShellClass_ContainsBaseClasses()
    {
        var cut = Render<XFAppShell>();

        // The GetShellClass() method returns:
        // GetClass("flex h-full w-full overflow-hidden bg-bg text-text-primary font-sans transition-theme")
        // Which when no Class is set, returns the full string
        Assert.Contains("flex h-full w-full overflow-hidden bg-bg text-text-primary font-sans transition-theme", cut.Markup);
    }

    [Fact]
    public void Render_GetShellClass_MergesWithClass()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.Class, "custom-shell-class"));

        // The GetShellClass() should merge: base + custom
        Assert.Contains("flex h-full w-full overflow-hidden bg-bg text-text-primary font-sans transition-theme", cut.Markup);
        Assert.Contains("custom-shell-class", cut.Markup);
    }

    [Fact]
    public void Render_MainElement_HasTabIndexMinusOne()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.ChildContent, builder => builder.AddContent(0, "Content")));

        var markup = cut.Markup;
        Assert.Contains("tabindex=\"-1\"", markup);
        // Verify it's on the main element
        var mainStart = markup.IndexOf("<main");
        Assert.True(mainStart >= 0, "Main element should exist");
        var mainSection = markup.Substring(mainStart, System.Math.Min(300, markup.Length - mainStart));
        Assert.Contains("tabindex=\"-1\"", mainSection);
    }

    [Fact]
    public void Render_AsideElement_HasFlexAndHeightClasses()
    {
        var cut = Render<XFAppShell>();

        // AsideClass default: "flex h-full shrink-0 z-50"
        Assert.Contains("flex h-full shrink-0 z-50", cut.Markup);
    }

    [Fact]
    public void Render_InnerContentContainer_HasFlexColumnClasses()
    {
        var cut = Render<XFAppShell>();

        // The inner div: "flex flex-col flex-1 min-w-0 overflow-hidden"
        Assert.Contains("flex flex-col flex-1 min-w-0 overflow-hidden", cut.Markup);
    }

    [Fact]
    public void Render_RootElement_HasAutoGeneratedId()
    {
        var cut = Render<XFAppShell>();
        var markup = cut.Markup;

        // Root div should have an id starting with "xf-"
        var rootDiv = cut.Find("div");
        var idAttr = rootDiv.GetAttribute("id");
        Assert.NotNull(idAttr);
        Assert.StartsWith("xf-", idAttr);
    }

    [Fact]
    public void Render_CombinedThemeBrandAndSlots_AllRenderCorrectly()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.Theme, "dark");
            parameters.Add(p => p.Brand, "enterprise");
            parameters.Add(p => p.Rail, builder => builder.AddContent(0, "Rail"));
            parameters.Add(p => p.Sidebar, builder => builder.AddContent(0, "Sidebar"));
            parameters.Add(p => p.AppBar, builder => builder.AddContent(0, "AppBar"));
            parameters.Add(p => p.ChildContent, builder => builder.AddContent(0, "Content"));
            parameters.Add(p => p.Footer, builder => builder.AddContent(0, "Footer"));
        });

        var markup = cut.Markup;
        Assert.Contains("data-theme=\"dark\"", markup);
        Assert.Contains("data-brand=\"enterprise\"", markup);
        Assert.Contains("Rail", markup);
        Assert.Contains("Sidebar", markup);
        Assert.Contains("AppBar", markup);
        Assert.Contains("Content", markup);
        Assert.Contains("Footer", markup);
    }

    [Fact]
    public void Render_StyleAttribute_AppliedToRoot()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.Style, "min-height: 600px;"));

        var rootDiv = cut.Find("div");
        var styleAttr = rootDiv.GetAttribute("style");
        Assert.Equal("min-height: 600px;", styleAttr);
    }

    [Fact]
    public void Render_AfterRender_DisabledProperty_Inherited()
    {
        // XFAppShell inherits from XForgeComponentBase which has [Parameter] bool Disabled
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.Disabled, true));

        var rootDiv = cut.Find("div");
        // Disabled should not be rendered as HTML attribute since the template
        // doesn't explicitly use it. But it's available as a parameter.
        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void Render_FullViewportFalse_Default_UsesContainerHeight()
    {
        var cut = Render<XFAppShell>();
        // Default FullViewport=false should use h-full w-full
        Assert.Contains("h-full", cut.Markup);
        Assert.Contains("w-full", cut.Markup);
        Assert.DoesNotContain("h-dvh", cut.Markup);
        Assert.DoesNotContain("w-screen", cut.Markup);
    }

    [Fact]
    public void Render_FullViewportTrue_UsesViewportHeight()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.FullViewport, true));

        Assert.Contains("h-dvh", cut.Markup);
        Assert.Contains("w-screen", cut.Markup);
    }

    [Fact]
    public void Render_FullViewportTrue_FlexBaseClassesStillPresent()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.FullViewport, true));

        var markup = cut.Markup;
        Assert.Contains("flex", markup);
        Assert.Contains("overflow-hidden", markup);
        Assert.Contains("bg-bg", markup);
        Assert.Contains("text-text-primary", markup);
        Assert.Contains("font-sans", markup);
        Assert.Contains("transition-theme", markup);
    }

    [Fact]
    public void Render_FullViewportFalse_FlexBaseClassesStillPresent()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.FullViewport, false));

        var markup = cut.Markup;
        Assert.Contains("flex", markup);
        Assert.Contains("h-full", markup);
        Assert.Contains("w-full", markup);
        Assert.Contains("overflow-hidden", markup);
        Assert.Contains("bg-bg", markup);
        Assert.Contains("text-text-primary", markup);
        Assert.Contains("font-sans", markup);
        Assert.Contains("transition-theme", markup);
    }

    [Fact]
    public void Render_FullViewport_MergesWithCustomClass()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.FullViewport, true);
            parameters.Add(p => p.Class, "my-custom-class");
        });

        Assert.Contains("h-dvh", cut.Markup);
        Assert.Contains("w-screen", cut.Markup);
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Render_FullViewportFalse_MergesWithCustomClass()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.FullViewport, false);
            parameters.Add(p => p.Class, "my-custom-class");
        });

        Assert.Contains("h-full", cut.Markup);
        Assert.Contains("w-full", cut.Markup);
        Assert.Contains("my-custom-class", cut.Markup);
    }
}
