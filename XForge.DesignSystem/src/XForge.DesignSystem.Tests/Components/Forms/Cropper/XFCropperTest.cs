using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Forms.Cropper;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Forms.Cropper;

public class XFCropperTest : XForgeTestContext
{
    public XFCropperTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithoutSource_ShowsEmptyState()
    {
        var cut = Render<XFCropper>();
        Assert.Contains("xf-cropper", cut.Markup);
        Assert.Contains("cropper.no_image", cut.Markup);
    }

    [Fact]
    public void Render_WithSource_ShowsLoadingOrCropArea()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("xf-cropper", cut.Markup);
    }

    [Fact]
    public void Render_WithSource_ShowsCropArea()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test-image.jpg"));
        Assert.Contains("xf-cropper", cut.Markup);
        // Image source is present in the markup
        Assert.Contains("test-image.jpg", cut.Markup);
    }

    [Fact]
    public void Render_WithSource_HasCropContainer()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("xf-cropper", cut.Markup);
    }

    [Fact]
    public void Render_WithSource_HasZoomControls()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("cropper.zoom", cut.Markup);
    }

    [Fact]
    public void Render_ShowsActionButtons()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("cropper.crop", cut.Markup);
        Assert.Contains("cropper.cancel", cut.Markup);
    }

    [Fact]
    public void Render_WithFreeCropDisabled_NoFreeButton()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.AllowFreeCrop, false);
        });
        Assert.DoesNotContain(">Free<", cut.Markup);
    }

    [Fact]
    public void Render_WithShowGridDisabled_NoGridLines()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.ShowGrid, false);
        });
        Assert.DoesNotContain("rule of thirds", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomAspectRatios()
    {
        var ratios = new[]
        {
            new XFCropperAspectRatio(21.0 / 9, "21:9")
        };

        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.CustomAspectRatios, ratios);
        });
        Assert.Contains("21:9", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomMaxDimensions()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.MaxWidth, 800);
            p.Add(x => x.MaxHeight, 600);
        });
        Assert.Contains("xf-cropper", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPreviewWhenEnabled()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.ShowPreview, true);
        });
        Assert.Contains("xf-cropper", cut.Markup);
    }

    [Fact]
    public void Render_HidesPreviewWhenDisabled()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.ShowPreview, false);
        });
        Assert.DoesNotContain("cropper.preview", cut.Markup);
    }

    [Fact]
    public void Render_ShowsRotationControls()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("cropper.rotation", cut.Markup);
    }

    [Fact]
    public void Render_ShowsImageAltText()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.ImageAlt, "My photo");
        });
        Assert.Contains("My photo", cut.Markup);
    }

    [Fact]
    public void Render_RespectsDisabled()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.Disabled, true);
        });
        Assert.Contains("opacity-50", cut.Markup);
    }

    [Fact]
    public void Render_WithSource_HasActionButtons()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("xf-cropper", cut.Markup);
    }

    [Fact]
    public void Render_ShowsCropButton()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("cropper.crop", cut.Markup);
    }

    [Fact]
    public void Render_ShowsCancelButton()
    {
        var cut = Render<XFCropper>(p => p.Add(x => x.Source, "test.jpg"));
        Assert.Contains("cropper.cancel", cut.Markup);
    }

    [Fact]
    public void Render_WithInitialAspectRatio()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.InitialAspectRatio, 16.0 / 9);
        });
        Assert.Contains("xf-cropper", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomOutputFormat()
    {
        var cut = Render<XFCropper>(p =>
        {
            p.Add(x => x.Source, "test.jpg");
            p.Add(x => x.OutputFormat, XFCropperOutputFormat.Jpeg);
            p.Add(x => x.OutputQuality, 80);
        });
        Assert.Contains("xf-cropper", cut.Markup);
    }
}
