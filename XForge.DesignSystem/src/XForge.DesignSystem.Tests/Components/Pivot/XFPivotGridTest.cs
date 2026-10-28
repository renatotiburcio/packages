using Bunit;
using Xunit;
using XForge.Components.Pivot;

namespace XForge.DesignSystem.Tests.Components.Pivot;

public class XFPivotGridTest : XForgeTestContext
{
    public XFPivotGridTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFPivotGrid>();
        Assert.Contains("xf-pivotgrid", cut.Markup);
    }

    [Fact]
    public void Render_WithRowsAndColumns()
    {
        var cut = Render<XFPivotGrid>(p =>
        {
            p.Add(x => x.Rows, new List<string> { "Row1", "Row2" });
            p.Add(x => x.Columns, new List<string> { "Col1", "Col2" });
        });

        Assert.Contains("Row1", cut.Markup);
        Assert.Contains("Col1", cut.Markup);
    }
}
