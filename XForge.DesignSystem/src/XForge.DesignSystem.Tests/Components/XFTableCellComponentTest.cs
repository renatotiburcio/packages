using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Table;

namespace XForge.DesignSystem.Tests.Components;

public class XFTableCellComponentTest : XForgeTestContext
{
    public XFTableCellComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_AsTd_WhenNotHeader()
    {
        var cut = Render<XFTableCell>(parameters =>
            parameters.AddChildContent("Cell"));
        Assert.Contains("<td", cut.Markup);
        Assert.Contains("Cell", cut.Markup);
    }

    [Fact]
    public void Render_AsTh_WhenHeader()
    {
        var cut = Render<XFTableCell>(parameters =>
        {
            parameters.Add(p => p.Header, true);
            parameters.AddChildContent("Header cell");
        });
        Assert.Contains("<th", cut.Markup);
        Assert.Contains("Header cell", cut.Markup);
    }

    [Fact]
    public void Render_WithColSpan_SetsColspanAttribute()
    {
        var cut = Render<XFTableCell>(parameters =>
        {
            parameters.Add(p => p.ColSpan, 3);
            parameters.AddChildContent("Spanned");
        });
        Assert.Equal("3", cut.Find("td").GetAttribute("colspan"));
    }

    [Fact]
    public void Render_AlignEnd_UsesTextRightClass()
    {
        var cut = Render<XFTableCell>(parameters =>
        {
            parameters.Add(p => p.Align, XFTableAlign.End);
            parameters.AddChildContent("Right");
        });
        Assert.Contains("text-right", cut.Find("td").GetAttribute("class"));
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFTableCell>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden");
        });
        Assert.DoesNotContain("<td", cut.Markup);
    }

    [Fact]
    public void Render_WithSticky_AddsStickyClass()
    {
        var cut = Render<XFTableCell>(parameters =>
        {
            parameters.Add(p => p.Sticky, true);
            parameters.AddChildContent("Sticky");
        });
        Assert.Contains("sticky", cut.Find("td").GetAttribute("class"));
    }
}
