using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Tabs;

namespace XForge.DesignSystem.Tests.Components;

public class XFTabsKeyboardTest : XForgeTestContext
{
    [Fact]
    public void ArrowRight_Moves_To_Next_Tab()
    {
        var activeId = "tab1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFTabs>(0);
            builder.AddAttribute(1, "ActiveId", activeId);
            builder.AddAttribute(2, "ActiveIdChanged", EventCallback.Factory.Create<string?>(this, next => activeId = next));
            builder.AddAttribute(3, "Tabs", (RenderFragment)(tabsBuilder =>
            {
                tabsBuilder.OpenComponent<XFTab>(0);
                tabsBuilder.AddAttribute(1, "TabId", "tab1");
                tabsBuilder.AddAttribute(2, "Label", "Tab 1");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(3);
                tabsBuilder.AddAttribute(4, "TabId", "tab2");
                tabsBuilder.AddAttribute(5, "Label", "Tab 2");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(6);
                tabsBuilder.AddAttribute(7, "TabId", "tab3");
                tabsBuilder.AddAttribute(8, "Label", "Tab 3");
                tabsBuilder.CloseComponent();
            }));
            builder.AddAttribute(4, "Panels", (RenderFragment)(panelsBuilder =>
            {
                panelsBuilder.OpenComponent<XFTabPanel>(0);
                panelsBuilder.AddAttribute(1, "TabId", "tab1");
                panelsBuilder.AddContent(2, "Panel 1");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(3);
                panelsBuilder.AddAttribute(4, "TabId", "tab2");
                panelsBuilder.AddContent(5, "Panel 2");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(6);
                panelsBuilder.AddAttribute(7, "TabId", "tab3");
                panelsBuilder.AddContent(8, "Panel 3");
                panelsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("[role='tablist']").KeyDown("ArrowRight");

        Assert.Equal("tab2", activeId);
    }

    [Fact]
    public void ArrowLeft_Moves_To_Previous_Tab()
    {
        var activeId = "tab2";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFTabs>(0);
            builder.AddAttribute(1, "ActiveId", activeId);
            builder.AddAttribute(2, "ActiveIdChanged", EventCallback.Factory.Create<string?>(this, next => activeId = next));
            builder.AddAttribute(3, "Tabs", (RenderFragment)(tabsBuilder =>
            {
                tabsBuilder.OpenComponent<XFTab>(0);
                tabsBuilder.AddAttribute(1, "TabId", "tab1");
                tabsBuilder.AddAttribute(2, "Label", "Tab 1");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(3);
                tabsBuilder.AddAttribute(4, "TabId", "tab2");
                tabsBuilder.AddAttribute(5, "Label", "Tab 2");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(6);
                tabsBuilder.AddAttribute(7, "TabId", "tab3");
                tabsBuilder.AddAttribute(8, "Label", "Tab 3");
                tabsBuilder.CloseComponent();
            }));
            builder.AddAttribute(4, "Panels", (RenderFragment)(panelsBuilder =>
            {
                panelsBuilder.OpenComponent<XFTabPanel>(0);
                panelsBuilder.AddAttribute(1, "TabId", "tab1");
                panelsBuilder.AddContent(2, "Panel 1");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(3);
                panelsBuilder.AddAttribute(4, "TabId", "tab2");
                panelsBuilder.AddContent(5, "Panel 2");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(6);
                panelsBuilder.AddAttribute(7, "TabId", "tab3");
                panelsBuilder.AddContent(8, "Panel 3");
                panelsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("[role='tablist']").KeyDown("ArrowLeft");

        Assert.Equal("tab1", activeId);
    }

    [Fact]
    public void Home_Moves_To_First_Tab()
    {
        var activeId = "tab3";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFTabs>(0);
            builder.AddAttribute(1, "ActiveId", activeId);
            builder.AddAttribute(2, "ActiveIdChanged", EventCallback.Factory.Create<string?>(this, next => activeId = next));
            builder.AddAttribute(3, "Tabs", (RenderFragment)(tabsBuilder =>
            {
                tabsBuilder.OpenComponent<XFTab>(0);
                tabsBuilder.AddAttribute(1, "TabId", "tab1");
                tabsBuilder.AddAttribute(2, "Label", "Tab 1");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(3);
                tabsBuilder.AddAttribute(4, "TabId", "tab2");
                tabsBuilder.AddAttribute(5, "Label", "Tab 2");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(6);
                tabsBuilder.AddAttribute(7, "TabId", "tab3");
                tabsBuilder.AddAttribute(8, "Label", "Tab 3");
                tabsBuilder.CloseComponent();
            }));
            builder.AddAttribute(4, "Panels", (RenderFragment)(panelsBuilder =>
            {
                panelsBuilder.OpenComponent<XFTabPanel>(0);
                panelsBuilder.AddAttribute(1, "TabId", "tab1");
                panelsBuilder.AddContent(2, "Panel 1");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(3);
                panelsBuilder.AddAttribute(4, "TabId", "tab2");
                panelsBuilder.AddContent(5, "Panel 2");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(6);
                panelsBuilder.AddAttribute(7, "TabId", "tab3");
                panelsBuilder.AddContent(8, "Panel 3");
                panelsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("[role='tablist']").KeyDown("Home");

        Assert.Equal("tab1", activeId);
    }

    [Fact]
    public void End_Moves_To_Last_Tab()
    {
        var activeId = "tab1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFTabs>(0);
            builder.AddAttribute(1, "ActiveId", activeId);
            builder.AddAttribute(2, "ActiveIdChanged", EventCallback.Factory.Create<string?>(this, next => activeId = next));
            builder.AddAttribute(3, "Tabs", (RenderFragment)(tabsBuilder =>
            {
                tabsBuilder.OpenComponent<XFTab>(0);
                tabsBuilder.AddAttribute(1, "TabId", "tab1");
                tabsBuilder.AddAttribute(2, "Label", "Tab 1");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(3);
                tabsBuilder.AddAttribute(4, "TabId", "tab2");
                tabsBuilder.AddAttribute(5, "Label", "Tab 2");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(6);
                tabsBuilder.AddAttribute(7, "TabId", "tab3");
                tabsBuilder.AddAttribute(8, "Label", "Tab 3");
                tabsBuilder.CloseComponent();
            }));
            builder.AddAttribute(4, "Panels", (RenderFragment)(panelsBuilder =>
            {
                panelsBuilder.OpenComponent<XFTabPanel>(0);
                panelsBuilder.AddAttribute(1, "TabId", "tab1");
                panelsBuilder.AddContent(2, "Panel 1");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(3);
                panelsBuilder.AddAttribute(4, "TabId", "tab2");
                panelsBuilder.AddContent(5, "Panel 2");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(6);
                panelsBuilder.AddAttribute(7, "TabId", "tab3");
                panelsBuilder.AddContent(8, "Panel 3");
                panelsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        cut.Find("[role='tablist']").KeyDown("End");

        Assert.Equal("tab3", activeId);
    }

    [Fact]
    public void Tab_Key_Moves_Focus_From_TabList_To_Panel()
    {
        var activeId = "tab1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFTabs>(0);
            builder.AddAttribute(1, "ActiveId", activeId);
            builder.AddAttribute(2, "ActiveIdChanged", EventCallback.Factory.Create<string?>(this, next => activeId = next));
            builder.AddAttribute(3, "Tabs", (RenderFragment)(tabsBuilder =>
            {
                tabsBuilder.OpenComponent<XFTab>(0);
                tabsBuilder.AddAttribute(1, "TabId", "tab1");
                tabsBuilder.AddAttribute(2, "Label", "Tab 1");
                tabsBuilder.CloseComponent();
            }));
            builder.AddAttribute(4, "Panels", (RenderFragment)(panelsBuilder =>
            {
                panelsBuilder.OpenComponent<XFTabPanel>(0);
                panelsBuilder.AddAttribute(1, "TabId", "tab1");
                panelsBuilder.AddContent(2, "Panel 1 content");
                panelsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("role=\"tablist\"", cut.Markup);
        Assert.Contains("role=\"tab\"", cut.Markup);
    }

    [Fact]
    public void Aria_Selected_On_Tabs()
    {
        var activeId = "tab1";
        var cut = Render(builder =>
        {
            builder.OpenComponent<XFTabs>(0);
            builder.AddAttribute(1, "ActiveId", activeId);
            builder.AddAttribute(2, "ActiveIdChanged", EventCallback.Factory.Create<string?>(this, next => activeId = next));
            builder.AddAttribute(3, "Tabs", (RenderFragment)(tabsBuilder =>
            {
                tabsBuilder.OpenComponent<XFTab>(0);
                tabsBuilder.AddAttribute(1, "TabId", "tab1");
                tabsBuilder.AddAttribute(2, "Label", "Tab 1");
                tabsBuilder.CloseComponent();
                tabsBuilder.OpenComponent<XFTab>(3);
                tabsBuilder.AddAttribute(4, "TabId", "tab2");
                tabsBuilder.AddAttribute(5, "Label", "Tab 2");
                tabsBuilder.CloseComponent();
            }));
            builder.AddAttribute(4, "Panels", (RenderFragment)(panelsBuilder =>
            {
                panelsBuilder.OpenComponent<XFTabPanel>(0);
                panelsBuilder.AddAttribute(1, "TabId", "tab1");
                panelsBuilder.AddContent(2, "Panel 1");
                panelsBuilder.CloseComponent();
                panelsBuilder.OpenComponent<XFTabPanel>(3);
                panelsBuilder.AddAttribute(4, "TabId", "tab2");
                panelsBuilder.AddContent(5, "Panel 2");
                panelsBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-selected=\"true\"", cut.Markup);
        Assert.Contains("aria-selected=\"false\"", cut.Markup);
    }
}
