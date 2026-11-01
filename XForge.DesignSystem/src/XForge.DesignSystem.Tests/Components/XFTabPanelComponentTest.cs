using Bunit;
using Microsoft.AspNetCore.Components;
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

    private IRenderedComponent<XFTabs> RenderTabs(string activeId, string panelContent)
    {
        return Render<XFTabs>(p =>
        {
            p.Add(x => x.ActiveId, activeId);
            p.Add(x => x.Panels, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, "TabId", activeId);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, panelContent)));
                builder.CloseComponent();
            }));
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
        var cut = Render<XFTabs>(p =>
        {
            p.Add(x => x.ActiveId, "other");
            p.Add(x => x.Panels, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFTabPanel>(0);
                builder.AddAttribute(1, "TabId", "tab1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Content")));
                builder.CloseComponent();
            }));
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
