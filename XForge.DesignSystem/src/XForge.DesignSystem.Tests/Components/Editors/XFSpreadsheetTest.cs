using Bunit;
using Xunit;
using XForge.Components.Editors;

namespace XForge.DesignSystem.Tests.Components.Editors;

public class XFSpreadsheetTest : Bunit.BunitContext
{
    public XFSpreadsheetTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFSpreadsheet>();
        Assert.Contains("xf-spreadsheet", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomRowsAndCols()
    {
        var cut = Render<XFSpreadsheet>(p =>
        {
            p.Add(x => x.Rows, 10);
            p.Add(x => x.Cols, 5);
        });

        Assert.Contains("data-rows=\"10\"", cut.Markup);
        Assert.Contains("data-cols=\"5\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowsTable()
    {
        var cut = Render<XFSpreadsheet>();
        Assert.Contains("<table", cut.Markup);
        Assert.Contains("contenteditable", cut.Markup);
    }
}
