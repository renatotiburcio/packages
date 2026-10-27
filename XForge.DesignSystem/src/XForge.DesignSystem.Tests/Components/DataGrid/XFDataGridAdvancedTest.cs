using Bunit;
using Xunit;
using XForge.Components.DataGrid;

namespace XForge.DesignSystem.Tests.Components.DataGrid;

public class XFDataGridAdvancedTest : Bunit.BunitContext
{
    public XFDataGridAdvancedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFDataGridAdvanced>();
        Assert.Contains("xf-datagrid-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithServerSide()
    {
        var cut = Render<XFDataGridAdvanced>(p => p.Add(x => x.ServerSide, true));
        Assert.Contains("data-server-side", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPager()
    {
        var cut = Render<XFDataGridAdvanced>();
        Assert.Contains("xf-dg-pager", cut.Markup);
    }

    [Fact]
    public void Render_ShowsStatus()
    {
        var cut = Render<XFDataGridAdvanced>(p =>
        {
            p.Add(x => x.VisibleCount, 10);
            p.Add(x => x.TotalCount, 100);
        });

        Assert.Contains("Showing 10 of 100", cut.Markup);
    }
}
