using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Feedback.Progress;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback.Progress;

public sealed class XFProgressRingExtendedTest : BunitContext
{
    public XFProgressRingExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFProgressRing> RenderProgress(
        Action<ComponentParameterCollectionBuilder<XFProgressRing>>? configure = null)
    {
        return Render<XFProgressRing>(p =>
        {
            p.Add(x => x.Value, 50.0);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void DefaultRender_RendersBaseCssClass()
    {
        var cut = RenderProgress();
        Assert.Contains("xf-progress-ring", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersRoleProgressbar()
    {
        var cut = RenderProgress();
        Assert.Contains("role=\"progressbar\"", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersAriaValueMin()
    {
        var cut = RenderProgress();
        Assert.Contains("aria-valuemin=\"0\"", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersAriaValueMax()
    {
        var cut = RenderProgress();
        Assert.Contains("aria-valuemax=\"100\"", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersAriaValueNow()
    {
        var cut = RenderProgress();
        Assert.Contains("aria-valuenow=\"50\"", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersAriaValueText()
    {
        var cut = RenderProgress();
        Assert.Contains("aria-valuetext", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersAriaLabel()
    {
        var cut = RenderProgress(p => p.Add(x => x.AriaLabel, "Loading data"));
        Assert.Contains("aria-label=\"Loading data\"", cut.Markup);
    }

    [Fact]
    public void DefaultRender_RendersSvgWithViewBox()
    {
        var cut = RenderProgress();
        var svg = cut.Find("svg");
        Assert.Equal("0 0 120 120", svg.GetAttribute("viewBox"));
    }

    [Fact]
    public void DefaultRender_RendersTrackCircle()
    {
        var cut = RenderProgress();
        var circles = cut.FindAll("circle");
        Assert.True(circles.Count >= 2, "Expected at least 2 circles (track + indicator)");
        Assert.Contains("stroke-surface-200", circles[0].GetAttribute("class"));
    }

    [Fact]
    public void DefaultRender_RendersIndicatorCircle()
    {
        var cut = RenderProgress();
        var circles = cut.FindAll("circle");
        Assert.True(circles.Count >= 2, "Expected at least 2 circles (track + indicator)");
        Assert.Contains("stroke-primary", circles[1].GetAttribute("class"));
    }

    [Fact]
    public void ShowValueTrue_RendersValueText()
    {
        var cut = RenderProgress(p => p.Add(x => x.ShowValue, true));
        Assert.Contains("50%", cut.Markup);
    }

    [Fact]
    public void ShowValueFalse_HidesValueText()
    {
        var cut = RenderProgress(p => p.Add(x => x.ShowValue, false));
        // ShowValue=false hides the center div, but aria-valuetext still has the value
        // Check that the center div with value text is not rendered
        Assert.DoesNotContain("xf-progress-ring-center", cut.Markup);
    }

    [Fact]
    public void Indeterminate_HidesValueText()
    {
        var cut = RenderProgress(p =>
        {
            p.Add(x => x.Indeterminate, true);
            p.Add(x => x.ShowValue, true);
        });
        // Indeterminate hides the center div
        Assert.DoesNotContain("xf-progress-ring-center", cut.Markup);
    }

    [Fact]
    public void ChildContent_Renders()
    {
        var cut = Render<XFProgressRing>(p => p
            .Add(x => x.Value, 50.0)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "<span>Custom</span>"))));
        Assert.Contains("Custom", cut.Markup);
    }

    [Fact]
    public void SizeSm_RendersSmClass()
    {
        var cut = RenderProgress(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-12 w-12", cut.Markup);
    }

    [Fact]
    public void SizeLg_RendersLgClass()
    {
        var cut = RenderProgress(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.Contains("h-24 w-24", cut.Markup);
    }

    [Fact]
    public void ColorPrimary_RendersPrimaryStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Primary));
        Assert.Contains("stroke-primary", cut.Markup);
    }

    [Fact]
    public void ColorSecondary_RendersSecondaryStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Secondary));
        Assert.Contains("stroke-secondary", cut.Markup);
    }

    [Fact]
    public void ColorSuccess_RendersSuccessStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Success));
        Assert.Contains("stroke-success", cut.Markup);
    }

    [Fact]
    public void ColorWarning_RendersWarningStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Warning));
        Assert.Contains("stroke-warning", cut.Markup);
    }

    [Fact]
    public void ColorDanger_RendersDangerStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Danger));
        Assert.Contains("stroke-danger", cut.Markup);
    }

    [Fact]
    public void ColorNeutral_RendersNeutralStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Neutral));
        Assert.Contains("stroke-surface-700", cut.Markup);
    }

    [Fact]
    public void ColorInfo_RendersInfoStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Info));
        Assert.Contains("stroke-info", cut.Markup);
    }

    [Fact]
    public void ColorLight_RendersLightStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Light));
        Assert.Contains("stroke-surface-300", cut.Markup);
    }

    [Fact]
    public void ColorDark_RendersDarkStroke()
    {
        var cut = RenderProgress(p => p.Add(x => x.Color, XFProgressColor.Dark));
        Assert.Contains("stroke-surface-900", cut.Markup);
    }

    [Fact]
    public void MinMax_CustomRange()
    {
        var cut = Render<XFProgressRing>(p => p
            .Add(x => x.Min, 10.0)
            .Add(x => x.Max, 200.0)
            .Add(x => x.Value, 50.0));
        Assert.Contains("xf-progress-ring", cut.Markup);
    }

    [Fact]
    public void Rounded_RendersRoundLineCap()
    {
        var cut = RenderProgress(p => p.Add(x => x.Rounded, true));
        Assert.Contains("round", cut.Markup);
    }

    [Fact]
    public void Paused_RendersPausedAnimation()
    {
        var cut = RenderProgress(p => p
            .Add(x => x.Indeterminate, true)
            .Add(x => x.Paused, true));
        Assert.Contains("xf-progress-ring-animate-paused", cut.Markup);
    }

    [Fact]
    public void LoadingText_RendersAriaValueText()
    {
        var cut = RenderProgress(p => p
            .Add(x => x.Indeterminate, true)
            .Add(x => x.LoadingText, "Loading..."));
        Assert.Contains("Loading...", cut.Markup);
    }

    [Fact]
    public void ChildContent_RendersInsideRing()
    {
        var cut = RenderProgress(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "50%"))));
        Assert.Contains("50%", cut.Markup);
    }

    [Fact]
    public void Enum_XFProgressColor_HasValues()
    {
        Assert.Contains(XFProgressColor.Primary, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Secondary, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Success, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Warning, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Danger, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Neutral, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Info, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Light, Enum.GetValues<XFProgressColor>());
        Assert.Contains(XFProgressColor.Dark, Enum.GetValues<XFProgressColor>());
    }
}
