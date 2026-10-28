using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public class XFSkeletonTableExtendedTest : XForgeTestContext
{
    public XFSkeletonTableExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFSkeletonTable> RenderSkeletonTable(
        Action<ComponentParameterCollectionBuilder<XFSkeletonTable>>? configure = null)
    {
        return Render<XFSkeletonTable>(p => configure?.Invoke(p));
    }

    // GetComponentClass Compact (line 36)
    [Fact]
    public void Extended_CompactTrue_RendersGap3()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Compact, true));
        Assert.Contains("gap-3", cut.Markup);
    }

    // GetComponentClass non-compact (line 36)
    [Fact]
    public void Extended_CompactFalse_RendersGap4()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Compact, false));
        Assert.Contains("gap-4", cut.Markup);
    }

    // ColumnClass 1 (line 44)
    [Fact]
    public void Extended_Cols1_RendersGridCols1()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Columns, 1));
        Assert.Contains("grid-cols-1", cut.Markup);
    }

    // ColumnClass 3 (line 46)
    [Fact]
    public void Extended_Cols3_RendersGridCols3()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Columns, 3));
        Assert.Contains("grid-cols-3", cut.Markup);
    }

    // ColumnClass 5 (line 48)
    [Fact]
    public void Extended_Cols5_RendersGridCols5()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Columns, 5));
        Assert.Contains("grid-cols-5", cut.Markup);
    }

    // ColumnClass 8 (line 51)
    [Fact]
    public void Extended_Cols8_RendersGridCols8()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Columns, 8));
        Assert.Contains("grid-cols-8", cut.Markup);
    }

    // ColumnClass invalid defaults (line 44)
    [Fact]
    public void Extended_ColsInvalid_RendersGridCols1()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Columns, 0));
        Assert.Contains("grid-cols-1", cut.Markup);
    }

    // ColumnClass > 8 clamped (line 51)
    [Fact]
    public void Extended_ColsOverflow_RendersGridCols8()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Columns, 12));
        Assert.Contains("grid-cols-8", cut.Markup);
    }

    // ShowHeader true renders header row (line 12-24)
    [Fact]
    public void Extended_ShowHeaderTrue_RendersHeaderRow()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.ShowHeader, true));
        // Header renders as a div with RowClass containing XFSkeleton components
        Assert.Contains("xf-skeleton-table", cut.Markup);
        // With 4 columns and header, there should be 4 header skeletons + 4*4 body skeletons
        Assert.Contains("xf-skeleton", cut.Markup);
    }

    // ShowHeader false skips header row (line 12)
    [Fact]
    public void Extended_ShowHeaderFalse_NoHeaderRow()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.ShowHeader, false));
        Assert.Contains("xf-skeleton-table", cut.Markup);
    }

    // Animation Pulse (line 18)
    [Fact]
    public void Extended_AnimationPulse()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Animation, XFSkeletonAnimation.Pulse));
        Assert.Contains("animate-pulse", cut.Markup);
    }

    // Animation Shimmer (line 18)
    [Fact]
    public void Extended_AnimationShimmer()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Animation, XFSkeletonAnimation.Shimmer));
        Assert.Contains("before:absolute", cut.Markup);
    }

    // Animation None (line 18)
    [Fact]
    public void Extended_AnimationNone()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Animation, XFSkeletonAnimation.None));
        Assert.DoesNotContain("animate-pulse", cut.Markup);
    }

    // Visible false renders nothing (line 14)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // Decorative true renders aria-hidden (line 30)
    [Fact]
    public void Extended_DecorativeTrue_RendersAriaHidden()
    {
        var cut = RenderSkeletonTable(p => p.Add(x => x.Decorative, true));
        Assert.Equal("true", cut.Find("*").GetAttribute("aria-hidden"));
    }

    // BaseClass included (line 37)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderSkeletonTable();
        Assert.Contains("xf-skeleton-table", cut.Markup);
    }

    // GetComponentClass includes grid (line 37)
    [Fact]
    public void Extended_GridClass_Included()
    {
        var cut = RenderSkeletonTable();
        Assert.Contains("grid", cut.Markup);
    }
}
