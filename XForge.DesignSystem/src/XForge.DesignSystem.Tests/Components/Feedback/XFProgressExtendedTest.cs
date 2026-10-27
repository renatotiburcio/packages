using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public class XFProgressExtendedTest : BunitContext
{
    public XFProgressExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFProgress> RenderProgress(
        Action<ComponentParameterCollectionBuilder<XFProgress>>? configure = null)
    {
        return Render<XFProgress>(p =>
        {
            p.Add(x => x.Value, 50);
            configure?.Invoke(p);
        });
    }

    // TrackClass Size Xs (line 80)
    [Fact]
    public void Extended_TrackSizeXs()
    {
        var cut = RenderProgress(p => p.Add(x => x.Size, XFExtendedSize.Xs));
        Assert.Contains("h-1", cut.Markup);
    }

    // TrackClass Size Sm (line 81)
    [Fact]
    public void Extended_TrackSizeSm()
    {
        var cut = RenderProgress(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-1.5", cut.Markup);
    }

    // TrackClass Size Lg (line 82)
    [Fact]
    public void Extended_TrackSizeLg()
    {
        var cut = RenderProgress(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.Contains("h-4", cut.Markup);
    }

    // TrackClass Size Xl (line 83)
    [Fact]
    public void Extended_TrackSizeXl()
    {
        var cut = RenderProgress(p => p.Add(x => x.Size, XFExtendedSize.Xl));
        Assert.Contains("h-6", cut.Markup);
    }

    // TrackClass default Md (line 84)
    [Fact]
    public void Extended_TrackSizeDefault()
    {
        var cut = RenderProgress();
        Assert.Contains("h-2.5", cut.Markup);
    }

    // TrackClass Rounded true (line 87)
    [Fact]
    public void Extended_TrackRoundedTrue()
    {
        var cut = RenderProgress(p => p.Add(x => x.Rounded, true));
        Assert.Contains("rounded-full", cut.Markup);
    }

    // TrackClass Rounded false (line 87)
    [Fact]
    public void Extended_TrackRoundedFalse()
    {
        var cut = RenderProgress(p => p.Add(x => x.Rounded, false));
        Assert.Contains("rounded", cut.Markup);
        Assert.DoesNotContain("rounded-full", cut.Markup);
    }

    // TrackClass Variant Soft (line 90)
    [Fact]
    public void Extended_TrackVariantSoft()
    {
        var cut = RenderProgress(p => p.Add(x => x.Variant, XFProgressVariant.Soft));
        Assert.Contains("bg-[color-mix", cut.Markup);
    }

    // TrackClass Variant Subtle (line 91)
    [Fact]
    public void Extended_TrackVariantSubtle()
    {
        var cut = RenderProgress(p => p.Add(x => x.Variant, XFProgressVariant.Subtle));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // TrackClass Variant Solid default (line 92)
    [Fact]
    public void Extended_TrackVariantSolid()
    {
        var cut = RenderProgress(p => p.Add(x => x.Variant, XFProgressVariant.Solid));
        Assert.Contains("bg-surface-200", cut.Markup);
    }

    // ToneClass Primary (line 143)
    [Fact]
    public void Extended_TonePrimary()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Primary));
        Assert.Contains("bg-primary", cut.Markup);
        Assert.Contains("text-primary-contrast", cut.Markup);
    }

    // ToneClass Success (line 136)
    [Fact]
    public void Extended_ToneSuccess()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Success));
        Assert.Contains("bg-success", cut.Markup);
    }

    // ToneClass Warning (line 137)
    [Fact]
    public void Extended_ToneWarning()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Warning));
        Assert.Contains("bg-warning", cut.Markup);
    }

    // ToneClass Danger (line 138)
    [Fact]
    public void Extended_ToneDanger()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Danger));
        Assert.Contains("bg-danger", cut.Markup);
    }

    // ToneClass Neutral (line 139)
    [Fact]
    public void Extended_ToneNeutral()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Neutral));
        Assert.Contains("bg-surface-700", cut.Markup);
    }

    // ToneClass Info (line 140)
    [Fact]
    public void Extended_ToneInfo()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Info));
        Assert.Contains("bg-info", cut.Markup);
    }

    // ToneClass Light (line 141)
    [Fact]
    public void Extended_ToneLight()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Light));
        Assert.Contains("bg-surface-200", cut.Markup);
    }

    // ToneClass Dark (line 142)
    [Fact]
    public void Extended_ToneDark()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Dark));
        Assert.Contains("bg-surface-900", cut.Markup);
    }

    // BarClass Striped (line 104)
    [Fact]
    public void Extended_BarStriped()
    {
        var cut = RenderProgress(p => p.Add(x => x.Striped, true));
        Assert.Contains("bg-[length:1rem_1rem]", cut.Markup);
    }

    // BarClass Animated + Striped (line 105-106)
    [Fact]
    public void Extended_BarAnimatedStriped()
    {
        var cut = RenderProgress(p =>
        {
            p.Add(x => x.Striped, true);
            p.Add(x => x.Animated, true);
        });
        Assert.Contains("xf-progress-stripe-animate", cut.Markup);
    }

    // BarClass Paused + Striped (line 106)
    [Fact]
    public void Extended_BarPausedStriped()
    {
        var cut = RenderProgress(p =>
        {
            p.Add(x => x.Striped, true);
            p.Add(x => x.Animated, true);
            p.Add(x => x.Paused, true);
        });
        Assert.Contains("xf-progress-stripe-animate-paused", cut.Markup);
    }

    // IndeterminateBarClass (line 113-122)
    [Fact]
    public void Extended_Indeterminate_RendersAnimatedBar()
    {
        var cut = RenderProgress(p => p.Add(x => x.Indeterminate, true));
        Assert.Contains("w-1/3", cut.Markup);
        Assert.Contains("xf-progress-indeterminate-animate", cut.Markup);
    }

    // IndeterminateBarClass Paused (line 118-119)
    [Fact]
    public void Extended_IndeterminatePaused()
    {
        var cut = RenderProgress(p =>
        {
            p.Add(x => x.Indeterminate, true);
            p.Add(x => x.Paused, true);
        });
        Assert.Contains("xf-progress-indeterminate-animate-paused", cut.Markup);
    }

    // InlineLabelClass Xl (line 128)
    [Fact]
    public void Extended_InlineLabelXl()
    {
        var cut = RenderProgress(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xl);
            p.Add(x => x.InlineValue, true);
            p.Add(x => x.ShowValue, true);
        });
        Assert.Contains("text-xs", cut.Markup);
    }

    // InlineLabelClass Lg (line 129)
    [Fact]
    public void Extended_InlineLabelLg()
    {
        var cut = RenderProgress(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Lg);
            p.Add(x => x.InlineValue, true);
            p.Add(x => x.ShowValue, true);
        });
        Assert.Contains("text-[11px]", cut.Markup);
    }

    // ShowHeader (line 45)
    [Fact]
    public void Extended_ShowHeaderTrue()
    {
        var cut = RenderProgress(p => p.Add(x => x.Label, "Loading..."));
        Assert.Contains("Loading...", cut.Markup);
    }

    // ShowValue (line 17)
    [Fact]
    public void Extended_ShowValueTrue()
    {
        var cut = RenderProgress(p => p.Add(x => x.ShowValue, true));
        Assert.Contains("50%", cut.Markup);
    }

    // Visible false renders nothing (line 16)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderProgress(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // BaseClass included (line 69)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderProgress();
        Assert.Contains("xf-progress", cut.Markup);
    }

    // GetComponentClass includes grid gap-2 (line 69)
    [Fact]
    public void Extended_GridClass_Included()
    {
        var cut = RenderProgress();
        Assert.Contains("grid", cut.Markup);
        Assert.Contains("gap-2", cut.Markup);
    }
}
