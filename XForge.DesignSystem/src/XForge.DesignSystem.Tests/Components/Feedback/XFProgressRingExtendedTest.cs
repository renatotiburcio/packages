using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public class XFProgressRingExtendedTest : XForgeTestContext
{
    public XFProgressRingExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFProgressRing> RenderProgressRing(
        Action<ComponentParameterCollectionBuilder<XFProgressRing>>? configure = null)
    {
        return Render<XFProgressRing>(p =>
        {
            p.Add(x => x.Value, 50);
            configure?.Invoke(p);
        });
    }

    // GetComponentClass Size Xs (line 81)
    [Fact]
    public void Extended_SizeXs()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Size, XFExtendedSize.Xs));
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("w-8", cut.Markup);
    }

    // GetComponentClass Size Sm (line 82)
    [Fact]
    public void Extended_SizeSm()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("w-12", cut.Markup);
    }

    // GetComponentClass Size Lg (line 83)
    [Fact]
    public void Extended_SizeLg()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.Contains("h-24", cut.Markup);
        Assert.Contains("w-24", cut.Markup);
    }

    // GetComponentClass Size Xl (line 84)
    [Fact]
    public void Extended_SizeXl()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Size, XFExtendedSize.Xl));
        Assert.Contains("h-32", cut.Markup);
        Assert.Contains("w-32", cut.Markup);
    }

    // GetComponentClass default Md (line 85)
    [Fact]
    public void Extended_SizeDefault()
    {
        var cut = RenderProgressRing();
        Assert.Contains("h-16", cut.Markup);
        Assert.Contains("w-16", cut.Markup);
    }

    // ToneStrokeClass Primary (line 127)
    [Fact]
    public void Extended_TonePrimary()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Primary));
        Assert.Contains("stroke-primary", cut.Markup);
    }

    // ToneStrokeClass Success (line 120)
    [Fact]
    public void Extended_ToneSuccess()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Success));
        Assert.Contains("stroke-success", cut.Markup);
    }

    // ToneStrokeClass Warning (line 121)
    [Fact]
    public void Extended_ToneWarning()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Warning));
        Assert.Contains("stroke-warning", cut.Markup);
    }

    // ToneStrokeClass Danger (line 122)
    [Fact]
    public void Extended_ToneDanger()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Danger));
        Assert.Contains("stroke-danger", cut.Markup);
    }

    // ToneStrokeClass Neutral (line 123)
    [Fact]
    public void Extended_ToneNeutral()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Neutral));
        Assert.Contains("stroke-surface-700", cut.Markup);
    }

    // ToneStrokeClass Info (line 124)
    [Fact]
    public void Extended_ToneInfo()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Info));
        Assert.Contains("stroke-info", cut.Markup);
    }

    // ToneStrokeClass Light (line 125)
    [Fact]
    public void Extended_ToneLight()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Light));
        Assert.Contains("stroke-surface-300", cut.Markup);
    }

    // ToneStrokeClass Dark (line 126)
    [Fact]
    public void Extended_ToneDark()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Color, XFProgressColor.Dark));
        Assert.Contains("stroke-surface-900", cut.Markup);
    }

    // TrackClass (line 97)
    [Fact]
    public void Extended_TrackClass_Rendered()
    {
        var cut = RenderProgressRing();
        Assert.Contains("stroke-surface-200", cut.Markup);
    }

    // IndicatorClass transition when determinate (line 103)
    [Fact]
    public void Extended_IndicatorDeterminate_HasTransition()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Indeterminate, false));
        Assert.Contains("transition-[stroke-dashoffset]", cut.Markup);
    }

    // IndicatorClass no transition when indeterminate (line 103)
    [Fact]
    public void Extended_IndicatorIndeterminate_NoTransition()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Indeterminate, true));
        Assert.DoesNotContain("transition-[stroke-dashoffset]", cut.Markup);
    }

    // SvgClass indeterminate animated (line 91-94)
    [Fact]
    public void Extended_SvgIndeterminateAnimated()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Indeterminate, true));
        Assert.Contains("xf-progress-ring-animate", cut.Markup);
    }

    // SvgClass indeterminate paused (line 92-93)
    [Fact]
    public void Extended_SvgIndeterminatePaused()
    {
        var cut = RenderProgressRing(p =>
        {
            p.Add(x => x.Indeterminate, true);
            p.Add(x => x.Paused, true);
        });
        Assert.Contains("xf-progress-ring-animate-paused", cut.Markup);
    }

    // CenterClass Size Xs — only rendered when ShowValue (line 108-115)
    [Fact]
    public void Extended_CenterSizeXs()
    {
        var cut = RenderProgressRing(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xs);
            p.Add(x => x.ShowValue, true);
        });
        Assert.Contains("text-[9px]", cut.Markup);
    }

    // CenterClass Size Lg — only rendered when ShowValue (line 108-115)
    [Fact]
    public void Extended_CenterSizeLg()
    {
        var cut = RenderProgressRing(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Lg);
            p.Add(x => x.ShowValue, true);
        });
        Assert.Contains("text-sm", cut.Markup);
    }

    // CenterClass Size Xl — only rendered when ShowValue (line 108-115)
    [Fact]
    public void Extended_CenterSizeXl()
    {
        var cut = RenderProgressRing(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xl);
            p.Add(x => x.ShowValue, true);
        });
        Assert.Contains("text-base", cut.Markup);
    }

    // CenterClass default Md — only rendered when ShowValue (line 108-115)
    [Fact]
    public void Extended_CenterSizeDefault()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.ShowValue, true));
        Assert.Contains("text-xs", cut.Markup);
    }

    // Rounded true (line 57)
    [Fact]
    public void Extended_RoundedTrue()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Rounded, true));
        Assert.Contains("round", cut.Markup);
    }

    // Rounded false (line 57)
    [Fact]
    public void Extended_RoundedFalse()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Rounded, false));
        Assert.Contains("butt", cut.Markup);
    }

    // ShowValue (line 17)
    [Fact]
    public void Extended_ShowValueTrue()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.ShowValue, true));
        Assert.Contains("50%", cut.Markup);
    }

    // Visible false renders nothing (line 16)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderProgressRing(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // BaseClass included (line 78)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderProgressRing();
        Assert.Contains("xf-progress-ring", cut.Markup);
    }

    // GetComponentClass includes inline-grid (line 88)
    [Fact]
    public void Extended_InlineGrid_Included()
    {
        var cut = RenderProgressRing();
        Assert.Contains("inline-grid", cut.Markup);
    }
}
