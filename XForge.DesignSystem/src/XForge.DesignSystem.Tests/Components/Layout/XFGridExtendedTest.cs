using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Layout;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Layout;

public class XFGridExtendedTest : BunitContext
{
    public XFGridExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFGrid> RenderGrid(
        Action<ComponentParameterCollectionBuilder<XFGrid>>? configure = null)
    {
        return Render<XFGrid>(p =>
        {
            p.AddChildContent("<div>cell</div>");
            configure?.Invoke(p);
        });
    }

    // GetColsClass 1 (line 59)
    [Fact]
    public void Extended_Cols1_RendersGridCols1()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 1));
        Assert.Contains("grid-cols-1", cut.Markup);
    }

    // GetColsClass 2 (line 60)
    [Fact]
    public void Extended_Cols2_RendersGridCols2()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 2));
        Assert.Contains("grid-cols-2", cut.Markup);
    }

    // GetColsClass 3 (line 61)
    [Fact]
    public void Extended_Cols3_RendersGridCols3()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 3));
        Assert.Contains("grid-cols-3", cut.Markup);
    }

    // GetColsClass 4 (line 62)
    [Fact]
    public void Extended_Cols4_RendersGridCols4()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 4));
        Assert.Contains("grid-cols-4", cut.Markup);
    }

    // GetColsClass 6 (line 64)
    [Fact]
    public void Extended_Cols6_RendersGridCols6()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 6));
        Assert.Contains("grid-cols-6", cut.Markup);
    }

    // GetColsClass 12 (line 65)
    [Fact]
    public void Extended_Cols12_RendersGridCols12()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 12));
        Assert.Contains("grid-cols-12", cut.Markup);
    }

    // GetColsClass invalid defaults to 1 (line 66)
    [Fact]
    public void Extended_ColsInvalid_RendersGridCols1()
    {
        var cut = RenderGrid(p => p.Add(x => x.Cols, 7));
        Assert.Contains("grid-cols-1", cut.Markup);
    }

    // GetResponsiveColsClass Sm (line 73)
    [Fact]
    public void Extended_ResponsiveSm_RendersSmGridCols()
    {
        var cut = RenderGrid(p => p.Add(x => x.Sm, 2));
        Assert.Contains("sm:grid-cols-2", cut.Markup);
    }

    // GetResponsiveColsClass Md (line 74)
    [Fact]
    public void Extended_ResponsiveMd_RendersMdGridCols()
    {
        var cut = RenderGrid(p => p.Add(x => x.Md, 3));
        Assert.Contains("md:grid-cols-3", cut.Markup);
    }

    // GetResponsiveColsClass Lg (line 75)
    [Fact]
    public void Extended_ResponsiveLg_RendersLgGridCols()
    {
        var cut = RenderGrid(p => p.Add(x => x.Lg, 4));
        Assert.Contains("lg:grid-cols-4", cut.Markup);
    }

    // GetResponsiveColsClass Xl (line 76)
    [Fact]
    public void Extended_ResponsiveXl_RendersXlGridCols()
    {
        var cut = RenderGrid(p => p.Add(x => x.Xl, 6));
        Assert.Contains("xl:grid-cols-6", cut.Markup);
    }

    // GetResponsiveColsClass null values skipped (line 83)
    [Fact]
    public void Extended_ResponsiveNull_NoResponsiveClass()
    {
        var cut = RenderGrid();
        Assert.DoesNotContain("sm:", cut.Markup);
        Assert.DoesNotContain("md:", cut.Markup);
        Assert.DoesNotContain("lg:", cut.Markup);
        Assert.DoesNotContain("xl:", cut.Markup);
    }

    // GetGapClass token "4" (line 113)
    [Fact]
    public void Extended_GapToken4_RendersGap4()
    {
        var cut = RenderGrid(p => p.Add(x => x.Gap, (XFSizeToken)"4"));
        Assert.Contains("gap-4", cut.Markup);
    }

    // GetRowGapClass token "2" (line 114)
    [Fact]
    public void Extended_RowGapToken2_RendersGapY2()
    {
        var cut = RenderGrid(p => p.Add(x => x.RowGap, (XFSizeToken)"2"));
        Assert.Contains("gap-y-2", cut.Markup);
    }

    // GetColumnGapClass token "3" (line 115)
    [Fact]
    public void Extended_ColumnGapToken3_RendersGapX3()
    {
        var cut = RenderGrid(p => p.Add(x => x.ColumnGap, (XFSizeToken)"3"));
        Assert.Contains("gap-x-3", cut.Markup);
    }

    // GetFlowClass Column (line 129)
    [Fact]
    public void Extended_FlowColumn_RendersGridFlowCol()
    {
        var cut = RenderGrid(p => p.Add(x => x.Flow, XFGridFlow.Column));
        Assert.Contains("grid-flow-col", cut.Markup);
    }

    // GetFlowClass Dense (line 130)
    [Fact]
    public void Extended_FlowDense_RendersGridFlowDense()
    {
        var cut = RenderGrid(p => p.Add(x => x.Flow, XFGridFlow.Dense));
        Assert.Contains("grid-flow-dense", cut.Markup);
    }

    // GetFlowClass RowDense (line 131)
    [Fact]
    public void Extended_FlowRowDense_RendersGridFlowRowDense()
    {
        var cut = RenderGrid(p => p.Add(x => x.Flow, XFGridFlow.RowDense));
        Assert.Contains("grid-flow-row-dense", cut.Markup);
    }

    // GetFlowClass ColumnDense (line 132)
    [Fact]
    public void Extended_FlowColumnDense_RendersGridFlowColDense()
    {
        var cut = RenderGrid(p => p.Add(x => x.Flow, XFGridFlow.ColumnDense));
        Assert.Contains("grid-flow-col-dense", cut.Markup);
    }

    // GetFlowClass Row (default) (line 133)
    [Fact]
    public void Extended_FlowRow_RendersGridFlowRow()
    {
        var cut = RenderGrid(p => p.Add(x => x.Flow, XFGridFlow.Row));
        Assert.Contains("grid-flow-row", cut.Markup);
    }

    // GetAlignItemsClass Start (line 138)
    [Fact]
    public void Extended_AlignItemsStart_RendersItemsStart()
    {
        var cut = RenderGrid(p => p.Add(x => x.AlignItems, XFGridItems.Start));
        Assert.Contains("items-start", cut.Markup);
    }

    // GetAlignItemsClass Center (line 139)
    [Fact]
    public void Extended_AlignItemsCenter_RendersItemsCenter()
    {
        var cut = RenderGrid(p => p.Add(x => x.AlignItems, XFGridItems.Center));
        Assert.Contains("items-center", cut.Markup);
    }

    // GetAlignItemsClass End (line 140)
    [Fact]
    public void Extended_AlignItemsEnd_RendersItemsEnd()
    {
        var cut = RenderGrid(p => p.Add(x => x.AlignItems, XFGridItems.End));
        Assert.Contains("items-end", cut.Markup);
    }

    // GetAlignItemsClass Stretch (line 141)
    [Fact]
    public void Extended_AlignItemsStretch_RendersItemsStretch()
    {
        var cut = RenderGrid(p => p.Add(x => x.AlignItems, XFGridItems.Stretch));
        Assert.Contains("items-stretch", cut.Markup);
    }

    // GetAlignItemsClass Normal (line 142)
    [Fact]
    public void Extended_AlignItemsNormal_NoAlignClass()
    {
        var cut = RenderGrid(p => p.Add(x => x.AlignItems, XFGridItems.Normal));
        Assert.DoesNotContain("items-", cut.Markup);
    }

    // GetJustifyItemsClass Start (line 147)
    [Fact]
    public void Extended_JustifyItemsStart_RendersJustifyItemsStart()
    {
        var cut = RenderGrid(p => p.Add(x => x.JustifyItems, XFGridItems.Start));
        Assert.Contains("justify-items-start", cut.Markup);
    }

    // GetJustifyItemsClass Center (line 148)
    [Fact]
    public void Extended_JustifyItemsCenter_RendersJustifyItemsCenter()
    {
        var cut = RenderGrid(p => p.Add(x => x.JustifyItems, XFGridItems.Center));
        Assert.Contains("justify-items-center", cut.Markup);
    }

    // GetJustifyItemsClass End (line 149)
    [Fact]
    public void Extended_JustifyItemsEnd_RendersJustifyItemsEnd()
    {
        var cut = RenderGrid(p => p.Add(x => x.JustifyItems, XFGridItems.End));
        Assert.Contains("justify-items-end", cut.Markup);
    }

    // GetJustifyItemsClass Stretch (line 150)
    [Fact]
    public void Extended_JustifyItemsStretch_RendersJustifyItemsStretch()
    {
        var cut = RenderGrid(p => p.Add(x => x.JustifyItems, XFGridItems.Stretch));
        Assert.Contains("justify-items-stretch", cut.Markup);
    }

    // GetJustifyItemsClass Normal (line 151)
    [Fact]
    public void Extended_JustifyItemsNormal_NoJustifyClass()
    {
        var cut = RenderGrid(p => p.Add(x => x.JustifyItems, XFGridItems.Normal));
        Assert.DoesNotContain("justify-items-", cut.Markup);
    }

    // GetWidthClass FullWidth (line 154)
    [Fact]
    public void Extended_FullWidth_RendersWFull()
    {
        var cut = RenderGrid(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    // GetWidthClass default (line 154)
    [Fact]
    public void Extended_FullWidthFalse_NoWFull()
    {
        var cut = RenderGrid(p => p.Add(x => x.FullWidth, false));
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    // BaseClass included (line 27)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderGrid();
        Assert.Contains("xf-grid", cut.Markup);
    }

    // Grid class included (line 27)
    [Fact]
    public void Extended_GridClass_Included()
    {
        var cut = RenderGrid();
        Assert.Contains("grid", cut.Markup);
    }
}
