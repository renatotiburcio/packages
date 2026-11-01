using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Tabs;

namespace XForge.DesignSystem.Tests.Components;

// XFTabs expoe dois slots explicitos: <Tabs> (os gatilhos) e <Panels> (os paineis).
// Os testes abaixo usam essa API — nao existe ChildContent no componente.
public class XFTabPanelComponentTest : XForgeTestContext
{
    public XFTabPanelComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static void AddPanel(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, int seq, string tabId, string content)
    {
        builder.OpenComponent<XFTabPanel>(seq);
        builder.AddAttribute(seq + 1, nameof(XFTabPanel.TabId), tabId);
        builder.AddAttribute(seq + 2, nameof(XFTabPanel.ChildContent), (RenderFragment)(panel => panel.AddContent(0, content)));
        builder.CloseComponent();
    }

    private static IRenderedComponent<XFTabs> RenderTabs(string activeId, string panelContent)
    {
        return Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, activeId);
            parameters.Add(p => p.Panels, builder => AddPanel(builder, 0, activeId, panelContent));
        });
    }

    [Fact]
    public void Render_AsChildOfTabs_RendersPanel()
    {
        var cut = RenderTabs("tab1", "Panel content");
        Assert.Contains("xf-tab-panel", cut.Markup);
    }

    [Fact]
    public void Render_ActivePanel_ShowsContent()
    {
        var cut = RenderTabs("tab1", "Active panel");
        Assert.Contains("Active panel", cut.Markup);
    }

    [Fact]
    public void Render_HasTabpanelRole()
    {
        var cut = RenderTabs("tab1", "Content");
        Assert.Contains("role=\"tabpanel\"", cut.Markup);
    }

    [Fact]
    public void Render_InactivePanel_HasAriaHidden()
    {
        var cut = Render<XFTabs>(parameters =>
        {
            parameters.Add(p => p.ActiveId, "other");
            parameters.Add(p => p.Panels, builder => AddPanel(builder, 0, "tab1", "Content"));
        });

        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_ActivePanel_HasDataStateActive()
    {
        var cut = RenderTabs("tab1", "Content");
        Assert.Contains("data-state=\"active\"", cut.Markup);
    }
}
