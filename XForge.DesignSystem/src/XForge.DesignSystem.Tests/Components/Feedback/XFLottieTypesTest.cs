using Xunit;
using XForge.DesignSystem.Components.Feedback.Lottie;

namespace XForge.DesignSystem.Tests.Components.Feedback;

/// <summary>
/// Tests for Lottie types at 0% coverage: XFLottieRenderer, XFLottieDirection, XFLottieLoadedEventArgs.
/// </summary>
public sealed class XFLottieTypesTest
{
    #region XFLottieRenderer

    [Fact]
    public void XFLottieRenderer_HasAllValues()
    {
        Assert.Equal(3, Enum.GetValues<XFLottieRenderer>().Length);
        Assert.True(Enum.IsDefined<XFLottieRenderer>(XFLottieRenderer.Svg));
        Assert.True(Enum.IsDefined<XFLottieRenderer>(XFLottieRenderer.Canvas));
        Assert.True(Enum.IsDefined<XFLottieRenderer>(XFLottieRenderer.Html));
    }

    [Fact]
    public void XFLottieRenderer_Svg_IsDefault()
    {
        Assert.Equal(0, (int)XFLottieRenderer.Svg);
    }

    #endregion

    #region XFLottieDirection

    [Fact]
    public void XFLottieDirection_Forward_Is1()
    {
        Assert.Equal(1, (int)XFLottieDirection.Forward);
    }

    [Fact]
    public void XFLottieDirection_Backward_IsMinus1()
    {
        Assert.Equal(-1, (int)XFLottieDirection.Backward);
    }

    [Fact]
    public void XFLottieDirection_HasTwoValues()
    {
        Assert.Equal(2, Enum.GetValues<XFLottieDirection>().Length);
    }

    #endregion

    #region XFLottieLoadedEventArgs

    [Fact]
    public void XFLottieLoadedEventArgs_Defaults_AreCorrect()
    {
        var args = new XFLottieLoadedEventArgs();
        Assert.Equal(0, args.TotalFrames);
        Assert.Equal(0, args.CurrentFrame);
        Assert.Equal(0, args.FrameRate);
    }

    [Fact]
    public void XFLottieLoadedEventArgs_WithValues_AssignsCorrectly()
    {
        var args = new XFLottieLoadedEventArgs
        {
            TotalFrames = 120,
            CurrentFrame = 0,
            FrameRate = 30.0
        };
        Assert.Equal(120, args.TotalFrames);
        Assert.Equal(0, args.CurrentFrame);
        Assert.Equal(30.0, args.FrameRate);
    }

    [Fact]
    public void XFLottieLoadedEventArgs_RecordEquality_Works()
    {
        var a = new XFLottieLoadedEventArgs { TotalFrames = 60, CurrentFrame = 10, FrameRate = 24.0 };
        var b = new XFLottieLoadedEventArgs { TotalFrames = 60, CurrentFrame = 10, FrameRate = 24.0 };
        Assert.Equal(a, b);
    }

    #endregion
}
