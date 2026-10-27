using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Table;

namespace XForge.DesignSystem.Tests.Components;

public class XFTableHeaderComponentTest : BunitContext
{
    public XFTableHeaderComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithChildContent_RendersThead()
    {
        var cut = Render<XFTableHeader>(parameters =>
            parameters.AddChildContent("Header content"));
        Assert.Contains("<thead", cut.Markup);
        Assert.Contains("Header content", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFTableHeader>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden");
        });
        Assert.DoesNotContain("<thead", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_PassesThrough()
    {
        var cut = Render<XFTableHeader>(parameters =>
        {
            parameters.Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-test", "header-value" }
            });
            parameters.AddChildContent("Content");
        });
        Assert.Contains("data-test=\"header-value\"", cut.Markup);
    }

    [Fact]
    public void Render_HasBackgroundClass()
    {
        var cut = Render<XFTableHeader>(parameters =>
            parameters.AddChildContent("H"));
        var thead = cut.Find("thead");
        Assert.Contains("bg-surface-50", thead.GetAttribute("class"));
    }
}
