using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.DataDisplay.Image;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFImageExtendedTest : XForgeTestContext
{
    public XFImageExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFImage> RenderImage(
        Action<ComponentParameterCollectionBuilder<XFImage>>? configure = null)
    {
        return Render<XFImage>(p =>
        {
            p.Add(x => x.Src, "test.jpg");
            p.Add(x => x.Alt, "Test image");
            configure?.Invoke(p);
        });
    }

    // AspectRatio Square (line 75)
    [Fact]
    public void Extended_AspectRatioSquare()
    {
        var cut = RenderImage(p => p.Add(x => x.AspectRatio, XFImageAspectRatio.Square));
        Assert.Contains("aspect-square", cut.Markup);
    }

    // AspectRatio Video (line 76)
    [Fact]
    public void Extended_AspectRatioVideo()
    {
        var cut = RenderImage(p => p.Add(x => x.AspectRatio, XFImageAspectRatio.Video));
        Assert.Contains("aspect-video", cut.Markup);
    }

    // AspectRatio Wide (line 77)
    [Fact]
    public void Extended_AspectRatioWide()
    {
        var cut = RenderImage(p => p.Add(x => x.AspectRatio, XFImageAspectRatio.Wide));
        Assert.Contains("aspect-[21/9]", cut.Markup);
    }

    // AspectRatio Portrait (line 78)
    [Fact]
    public void Extended_AspectRatioPortrait()
    {
        var cut = RenderImage(p => p.Add(x => x.AspectRatio, XFImageAspectRatio.Portrait));
        Assert.Contains("aspect-[3/4]", cut.Markup);
    }

    // AspectRatio Auto default (line 79)
    [Fact]
    public void Extended_AspectRatioAuto()
    {
        var cut = RenderImage();
        Assert.DoesNotContain("aspect-", cut.Markup);
    }

    // Fit Contain (line 84)
    [Fact]
    public void Extended_FitContain()
    {
        var cut = RenderImage(p => p.Add(x => x.Fit, XFImageFit.Contain));
        Assert.Contains("object-contain", cut.Markup);
    }

    // Fit Fill (line 85)
    [Fact]
    public void Extended_FitFill()
    {
        var cut = RenderImage(p => p.Add(x => x.Fit, XFImageFit.Fill));
        Assert.Contains("object-fill", cut.Markup);
    }

    // Fit None (line 86)
    [Fact]
    public void Extended_FitNone()
    {
        var cut = RenderImage(p => p.Add(x => x.Fit, XFImageFit.None));
        Assert.Contains("object-none", cut.Markup);
    }

    // Fit Cover default (line 87)
    [Fact]
    public void Extended_FitDefault()
    {
        var cut = RenderImage();
        Assert.Contains("object-cover", cut.Markup);
    }

    // Radius None (line 92)
    [Fact]
    public void Extended_RadiusNone()
    {
        var cut = RenderImage(p => p.Add(x => x.Radius, XFImageRadius.None));
        Assert.Contains("rounded-none", cut.Markup);
    }

    // Radius Sm (line 93)
    [Fact]
    public void Extended_RadiusSm()
    {
        var cut = RenderImage(p => p.Add(x => x.Radius, XFImageRadius.Sm));
        Assert.Contains("rounded-md", cut.Markup);
    }

    // Radius Md (line 94)
    [Fact]
    public void Extended_RadiusMd()
    {
        var cut = RenderImage(p => p.Add(x => x.Radius, XFImageRadius.Md));
        Assert.Contains("rounded-lg", cut.Markup);
    }

    // Radius Xl (line 95)
    [Fact]
    public void Extended_RadiusXl()
    {
        var cut = RenderImage(p => p.Add(x => x.Radius, XFImageRadius.Xl));
        Assert.Contains("rounded-2xl", cut.Markup);
    }

    // Radius Full (line 96)
    [Fact]
    public void Extended_RadiusFull()
    {
        var cut = RenderImage(p => p.Add(x => x.Radius, XFImageRadius.Full));
        Assert.Contains("rounded-full", cut.Markup);
    }

    // Radius default Lg (line 97)
    [Fact]
    public void Extended_RadiusDefault()
    {
        var cut = RenderImage();
        Assert.Contains("rounded-xl", cut.Markup);
    }

    // Fluid true (line 59)
    [Fact]
    public void Extended_FluidTrue()
    {
        var cut = RenderImage();
        Assert.Contains("w-full", cut.Markup);
    }

    // Fluid false (line 59)
    [Fact]
    public void Extended_FluidFalse()
    {
        var cut = RenderImage(p => p.Add(x => x.Fluid, false));
        Assert.Contains("inline-block", cut.Markup);
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    // Lazy loading (line 49)
    [Fact]
    public void Extended_LazyTrue()
    {
        var cut = RenderImage();
        Assert.Contains("loading=\"lazy\"", cut.Markup);
    }

    // Lazy false (line 49)
    [Fact]
    public void Extended_LazyFalse()
    {
        var cut = RenderImage(p => p.Add(x => x.Lazy, false));
        Assert.Contains("loading=\"eager\"", cut.Markup);
    }

    // Decorative true (line 14)
    [Fact]
    public void Extended_DecorativeTrue()
    {
        var cut = RenderImage(p => p.Add(x => x.Decorative, true));
        Assert.Equal("presentation", cut.Find("img").GetAttribute("role"));
    }

    // Decorative false with Alt (line 41)
    [Fact]
    public void Extended_DecorativeFalse_HasAlt()
    {
        var cut = RenderImage(p => p.Add(x => x.Decorative, false));
        Assert.Contains("Test image", cut.Find("img").GetAttribute("alt"));
    }

    // Interactive class when OnClick (line 69-71)
    [Fact]
    public void Extended_InteractiveClass_WhenOnClick()
    {
        var cut = RenderImage(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));
        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.Contains("focus-visible:ring-2", cut.Markup);
    }

    // Visible false renders nothing (line 17)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderImage(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // BaseClass included (line 58)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderImage();
        Assert.Contains("xf-image", cut.Markup);
    }
}
