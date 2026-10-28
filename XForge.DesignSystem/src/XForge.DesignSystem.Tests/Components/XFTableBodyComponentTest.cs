using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Table;

namespace XForge.DesignSystem.Tests.Components;

public class XFTableBodyComponentTest : XForgeTestContext
{
    public XFTableBodyComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithChildContent_RendersTbody()
    {
        var cut = Render<XFTableBody>(parameters =>
            parameters.AddChildContent("Body content"));
        Assert.Contains("<tbody", cut.Markup);
        Assert.Contains("Body content", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFTableBody>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden");
        });
        Assert.DoesNotContain("<tbody", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_PassesThrough()
    {
        var cut = Render<XFTableBody>(parameters =>
        {
            parameters.Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-test", "body-value" }
            });
            parameters.AddChildContent("Content");
        });
        Assert.Contains("data-test=\"body-value\"", cut.Markup);
    }

    [Fact]
    public void Render_WithRows_RendersAllCells()
    {
        var cut = Render<XFTableBody>(parameters =>
            parameters.AddChildContent(builder =>
            {
                builder.OpenElement(0, "tr");
                builder.OpenElement(1, "td");
                builder.AddContent(2, "Cell1");
                builder.CloseElement();
                builder.OpenElement(3, "td");
                builder.AddContent(4, "Cell2");
                builder.CloseElement();
                builder.CloseElement();
            }));
        Assert.Contains("Cell1", cut.Markup);
        Assert.Contains("Cell2", cut.Markup);
    }
}
