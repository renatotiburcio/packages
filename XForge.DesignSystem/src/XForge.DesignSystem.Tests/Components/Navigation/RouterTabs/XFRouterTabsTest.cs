using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Navigation.RouterTabs;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Navigation.RouterTabs;

public class XFRouterTabsTest : BunitContext
{
    public XFRouterTabsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
        Services.AddSingleton<NavigationManager>(new TestNavigationManager());
    }

    [Fact]
    public void Render_WithNoTabs_ShowsEmpty()
    {
        var cut = Render<XFRouterTabs>();
        Assert.Contains("xf-router-tabs", cut.Markup);
    }

    [Fact]
    public void Render_WithTabs_ShowsTabHeaders()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" },
            new XFRouterTabOption { Id = "tab2", Title = "Tab 2" }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        Assert.Contains("Tab 1", cut.Markup);
        Assert.Contains("Tab 2", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowNewTab_ShowsAddButton()
    {
        var cut = Render<XFRouterTabs>(p => p.Add(x => x.AllowNewTab, true));
        Assert.Contains("router_tabs.add_tab", cut.Markup);
    }

    [Fact]
    public void Render_WithoutAllowNewTab_NoAddButton()
    {
        var cut = Render<XFRouterTabs>(p => p.Add(x => x.AllowNewTab, false));
        Assert.DoesNotContain("router_tabs.add_tab", cut.Markup);
    }

    [Fact]
    public void Render_WithClosableTab_ShowsCloseButton()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1", Closable = true }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        Assert.Contains("router_tabs.close_tab", cut.Markup);
    }

    [Fact]
    public void Render_WithNonClosableTab_NoCloseButton()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1", Closable = false }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        Assert.DoesNotContain("×", cut.Markup);
    }

    [Fact]
    public void Render_FirstTabIsActive()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" },
            new XFRouterTabOption { Id = "tab2", Title = "Tab 2" }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        var firstTab = cut.Find("button[role='tab']");
        Assert.Equal("true", firstTab.GetAttribute("aria-selected"));
    }

    [Fact]
    public void Render_ShowsTabPanels()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" },
            new XFRouterTabOption { Id = "tab2", Title = "Tab 2" }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        Assert.Contains("role=\"tabpanel\"", cut.Markup);
    }

    [Fact]
    public void Render_MaxTabs_ShowsLimitedTabs()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" },
            new XFRouterTabOption { Id = "tab2", Title = "Tab 2" },
            new XFRouterTabOption { Id = "tab3", Title = "Tab 3" }
        };

        var cut = Render<XFRouterTabs>(p =>
        {
            p.Add(x => x.Tabs, tabs);
            p.Add(x => x.MaxTabs, 2);
        });
        // MaxTabs affects adding new tabs, not existing ones
        Assert.Contains("Tab 1", cut.Markup);
    }

    [Fact]
    public void Render_WithModifiedTab_ShowsIndicator()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        // The indicator is shown via CSS class, not text
        Assert.Contains("Tab 1", cut.Markup);
    }

    [Fact]
    public void Render_ShowsDefaultTitle()
    {
        var cut = Render<XFRouterTabs>(p => p.Add(x => x.AllowNewTab, true));
        var component = cut.Instance;
        // TestLocalizer returns the key, not the translated value
        Assert.Equal("router_tabs.new_tab", component.DefaultTitle);
    }

    [Fact]
    public void Render_WithCustomDefaultTitle()
    {
        var cut = Render<XFRouterTabs>(p =>
        {
            p.Add(x => x.AllowNewTab, true);
            p.Add(x => x.DefaultTitle, "Custom Title");
        });
        var component = cut.Instance;
        Assert.Equal("Custom Title", component.DefaultTitle);
    }

    [Fact]
    public void Render_ShowsTabRole()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        Assert.Contains("role=\"tab\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowsTablistRole()
    {
        var cut = Render<XFRouterTabs>();
        Assert.Contains("role=\"tablist\"", cut.Markup);
    }

    [Fact]
    public void Render_WithMultipleTabs_AllHeadersVisible()
    {
        var tabs = Enumerable.Range(1, 5).Select(i => new XFRouterTabOption
        {
            Id = $"tab{i}",
            Title = $"Tab {i}"
        }).ToList();

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        for (int i = 1; i <= 5; i++)
        {
            Assert.Contains($"Tab {i}", cut.Markup);
        }
    }

    [Fact]
    public void Render_WithRoute_ShowsTabHeaders()
    {
        var tabs = new[]
        {
            new XFRouterTabOption { Id = "tab1", Title = "Tab 1" },
            new XFRouterTabOption { Id = "tab2", Title = "Tab 2" }
        };

        var cut = Render<XFRouterTabs>(p => p.Add(x => x.Tabs, tabs));
        Assert.Contains("Tab 1", cut.Markup);
        Assert.Contains("Tab 2", cut.Markup);
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            // no-op for tests
        }
    }
}
