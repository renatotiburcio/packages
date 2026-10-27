using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Table;

namespace XForge.DesignSystem.Tests.Components;

public class XFTableRowComponentTest : BunitContext
{
    public XFTableRowComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithChildContent_RendersTr()
    {
        var cut = Render<XFTableRow>(parameters =>
            parameters.AddChildContent("Row content"));
        Assert.Contains("<tr", cut.Markup);
        Assert.Contains("Row content", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFTableRow>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden");
        });
        Assert.DoesNotContain("<tr", cut.Markup);
    }

    [Fact]
    public void Render_Selected_AddsSelectedClass()
    {
        var cut = Render<XFTableRow>(parameters =>
        {
            parameters.Add(p => p.Selected, true);
            parameters.AddChildContent("Selected");
        });
        Assert.Contains("bg-[color-mix", cut.Find("tr").GetAttribute("class"));
    }

    [Fact]
    public void Render_WithClick_AddsCursorPointer()
    {
        var cut = Render<XFTableRow>(parameters =>
        {
            parameters.Add(p => p.OnClick, EventCallback<MouseEventArgs>.Empty);
            parameters.AddChildContent("Clickable");
        });
        Assert.Contains("cursor-pointer", cut.Find("tr").GetAttribute("class"));
    }

    [Fact]
    public void Render_WithAdditionalAttributes_PassesThrough()
    {
        var cut = Render<XFTableRow>(parameters =>
        {
            parameters.Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-row-id", "42" }
            });
            parameters.AddChildContent("Row");
        });
        Assert.Contains("data-row-id=\"42\"", cut.Markup);
    }
}
