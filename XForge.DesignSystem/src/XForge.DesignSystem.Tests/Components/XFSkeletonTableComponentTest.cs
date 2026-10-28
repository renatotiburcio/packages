#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFSkeletonTableComponentTest : XForgeTestContext
{
    public XFSkeletonTableComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFSkeletonTable>();
        Assert.Contains("xf-skeleton-table", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomRowsAndColumns()
    {
        var cut = Render<XFSkeletonTable>(parameters =>
        {
            parameters.Add(p => p.Rows, 3);
            parameters.Add(p => p.Columns, 2);
        });
        Assert.Contains("grid-cols-2", cut.Markup);
    }

    [Fact]
    public void Render_DecorativeHidesFromAria()
    {
        var cut = Render<XFSkeletonTable>(parameters =>
        {
            parameters.Add(p => p.Decorative, true);
        });
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_StatusRoleWhenNotDecorative()
    {
        var cut = Render<XFSkeletonTable>(parameters =>
        {
            parameters.Add(p => p.Decorative, false);
            parameters.Add(p => p.AriaLabel, "Loading table data");
        });
        Assert.Contains("role=\"status\"", cut.Markup);
        Assert.Contains("aria-label=\"Loading table data\"", cut.Markup);
    }

    [Fact]
    public void Render_InvisibleDoesNotRender()
    {
        var cut = Render<XFSkeletonTable>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
        });
        Assert.DoesNotContain("xf-skeleton-table", cut.Markup);
    }
}
