using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Tabs;

namespace XForge.DesignSystem.Tests.Components;

public class XFTabPanelComponentTest : XForgeTestContext
{
    public XFTabPanelComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_AsChildOfTabs_RendersPanel()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, "tab1");
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(panel =>
                    panel.AddContent(0, "Panel content")));
                builder.CloseComponent();
            });
        });
        Assert.Contains("xf-tab-panel", cut.Markup);
    }

    [Fact]
    public void Render_ActivePanel_ShowsContent()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, "tab1");
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(panel =>
                    panel.AddContent(0, "Active panel")));
                builder.CloseComponent();
            });
        });
        Assert.Contains("Active panel", cut.Markup);
    }

    [Fact]
    public void Render_HasTabpanelRole()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, "tab1");
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(panel =>
                    panel.AddContent(0, "Content")));
                builder.CloseComponent();
            });
        });
        Assert.Contains("role=\"tabpanel\"", cut.Markup);
    }

    [Fact]
    public void Render_InactivePanel_HasAriaHidden()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, "tab1");
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab2");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(panel =>
                    panel.AddContent(0, "Inactive")));
                builder.CloseComponent();
            });
        });
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_ActivePanel_HasDataStateActive()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, "tab1");
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, nameof(XFTabPanel.TabId), "tab1");
                builder.AddAttribute(2, nameof(XFTabPanel.ChildContent), (RenderFragment)(panel =>
                    panel.AddContent(0, "Active")));
                builder.CloseComponent();
            });
        });
        Assert.Contains("data-state=\"active\"", cut.Markup);
    }
}
