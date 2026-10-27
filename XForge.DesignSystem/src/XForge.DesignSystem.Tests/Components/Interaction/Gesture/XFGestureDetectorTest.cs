using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Interaction.Gesture;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Interaction.Gesture;

public class XFGestureDetectorTest : BunitContext
{
    public XFGestureDetectorTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_ShowsChildContent()
    {
        var cut = Render<XFGestureDetector>(p => p.AddChildContent("<div>Content</div>"));
        Assert.Contains("xf-gesture-detector", cut.Markup);
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void Render_WithEnableTap_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.EnableTap, true);
            p.AddChildContent("<div>Tap me</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithEnableLongPress_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.EnableLongPress, true);
            p.AddChildContent("<div>Long press</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithEnableSwipe_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.EnableSwipe, true);
            p.AddChildContent("<div>Swipe me</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithEnablePinch_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.EnablePinch, true);
            p.AddChildContent("<div>Pinch me</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithEnablePan_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.EnablePan, true);
            p.AddChildContent("<div>Pan me</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomSwipeThreshold_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.SwipeThreshold, 100.0);
            p.AddChildContent("<div>Content</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLongPressDelay_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.LongPressDelay, 1000);
            p.AddChildContent("<div>Content</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>();
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void Render_WithAllGesturesEnabled_ShowsComponent()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.EnableTap, true);
            p.Add(x => x.EnableDoubleTap, true);
            p.Add(x => x.EnableLongPress, true);
            p.Add(x => x.EnableSwipe, true);
            p.Add(x => x.EnablePinch, true);
            p.Add(x => x.EnablePan, true);
            p.AddChildContent("<div>All gestures</div>");
        });
        Assert.Contains("All gestures", cut.Markup);
    }

    [Fact]
    public void Render_RespectsDisabled()
    {
        var cut = Render<XFGestureDetector>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent("<div>Content</div>");
        });
        Assert.Contains("xf-gesture-detector", cut.Markup);
    }

    [Fact]
    public void SwipeDirection_Enum_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFGestureSwipeDirection>().Length);
    }

    [Fact]
    public void SwipeEventArgs_HasDefaults()
    {
        var args = new XFGestureSwipeEventArgs();
        Assert.Equal(0, args.DistanceX);
        Assert.Equal(0, args.DistanceY);
        Assert.Equal(0, args.Velocity);
    }

    [Fact]
    public void TapEventArgs_HasDefaults()
    {
        var args = new XFGestureTapEventArgs();
        Assert.Equal(0, args.X);
        Assert.Equal(0, args.Y);
        Assert.Equal(0, args.Duration);
    }

    [Fact]
    public void PinchEventArgs_HasDefaults()
    {
        var args = new XFGesturePinchEventArgs();
        Assert.Equal(0, args.Scale);
        Assert.Equal(0, args.CenterX);
        Assert.Equal(0, args.CenterY);
    }

    [Fact]
    public void PanEventArgs_HasDefaults()
    {
        var args = new XFGesturePanEventArgs();
        Assert.Equal(0, args.DeltaX);
        Assert.Equal(0, args.DeltaY);
        Assert.Equal(0, args.TotalX);
        Assert.Equal(0, args.TotalY);
    }

    [Fact]
    public void LongPressEventArgs_HasDefaults()
    {
        var args = new XFGestureLongPressEventArgs();
        Assert.Equal(0, args.X);
        Assert.Equal(0, args.Y);
        Assert.Equal(0, args.Duration);
    }
}
