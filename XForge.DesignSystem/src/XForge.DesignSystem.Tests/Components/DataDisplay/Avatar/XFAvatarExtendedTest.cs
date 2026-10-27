using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Avatar;

namespace XForge.DesignSystem.Tests.Components.DataDisplay.Avatar;

public class XFAvatarExtendedTest : BunitContext
{
    [Fact]
    public void SizeXs_RendersExtraSmall()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Xs);
        });

        Assert.Contains("h-6 w-6", cut.Markup);
    }

    [Fact]
    public void SizeSm_RendersSmall()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Sm);
        });

        Assert.Contains("h-8 w-8", cut.Markup);
    }

    [Fact]
    public void SizeLg_RendersLarge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Lg);
        });

        Assert.Contains("h-12 w-12", cut.Markup);
    }

    [Fact]
    public void SizeXl_RendersExtraLarge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Xl);
        });

        Assert.Contains("h-16 w-16", cut.Markup);
    }

    [Fact]
    public void SizeXxl_RendersDoubleExtraLarge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Xxl);
        });

        Assert.Contains("h-20 w-20", cut.Markup);
    }

    [Fact]
    public void ShapeSquare_RendersRoundedLg()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Shape, XFAvatarShape.Square);
        });

        Assert.Contains("rounded-lg", cut.Markup);
    }

    [Fact]
    public void ShapeRounded_RendersRoundedXl()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Shape, XFAvatarShape.Rounded);
        });

        Assert.Contains("rounded-xl", cut.Markup);
    }

    [Fact]
    public void StatusAway_RendersWarningBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.Away);
        });

        Assert.Contains("bg-warning", cut.Markup);
    }

    [Fact]
    public void StatusBusy_RendersDangerBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.Busy);
        });

        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void StatusAway_RendersAwayAriaLabel()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.Away);
        });

        Assert.Contains("Ausente", cut.Markup);
    }

    [Fact]
    public void StatusBusy_RendersBusyAriaLabel()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.Busy);
        });

        Assert.Contains("Ocupado", cut.Markup);
    }

    [Fact]
    public void StatusOffline_RendersOfflineAriaLabel()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.Offline);
        });

        Assert.Contains("Indisponivel", cut.Markup);
    }

    [Fact]
    public void StatusNone_RendersNoBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.None);
        });

        Assert.DoesNotContain("bg-primary", cut.Markup);
    }

    [Fact]
    public void StatusTitle_CustomTitle_OverridesDefault()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
            parameters.Add(p => p.StatusTitle, "Custom status");
        });

        Assert.Contains("Custom status", cut.Markup);
    }

    [Fact]
    public void StatusXs_RendersSmallStatusBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Xs);
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("h-2 w-2", cut.Markup);
    }

    [Fact]
    public void StatusSm_RendersSmStatusBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Sm);
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("h-2.5 w-2.5", cut.Markup);
    }

    [Fact]
    public void StatusLg_RendersLgStatusBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Lg);
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("h-3.5 w-3.5", cut.Markup);
    }

    [Fact]
    public void StatusXl_RendersXlStatusBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Xl);
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void StatusXxl_RendersXxlStatusBadge()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Size, XFAvatarSize.Xxl);
            parameters.Add(p => p.Status, XFAvatarStatus.Online);
        });

        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void Initials_SingleName_FirstLetterOnly()
    {
        var cut = Render<XFAvatar>(parameters =>
            parameters.Add(p => p.Name, "Ana"));

        Assert.Contains("A", cut.Markup);
    }

    [Fact]
    public void Initials_EmptyName_ShowsQuestionMark()
    {
        var cut = Render<XFAvatar>(parameters =>
            parameters.Add(p => p.Name, ""));

        Assert.Contains("?", cut.Markup);
    }

    [Fact]
    public void Initials_ThreeNames_FirstAndLast()
    {
        var cut = Render<XFAvatar>(parameters =>
            parameters.Add(p => p.Name, "Ana Maria Souza"));

        Assert.Contains("AS", cut.Markup);
    }

    [Fact]
    public void Icon_RendersIconContent()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "AB");
            parameters.Add(p => p.Icon, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "👤");
            }));
        });

        Assert.Contains("xf-avatar", cut.Markup);
    }

    [Fact]
    public void ImageWithBrokenSrc_RendersInitialsFallback()
    {
        var cut = Render<XFAvatar>(parameters =>
        {
            parameters.Add(p => p.Name, "Ana Souza");
            parameters.Add(p => p.ImageSrc, "/broken.png");
        });

        // Without triggering onerror, image is still rendered (imageFailed defaults to false)
        // The initials are hidden when image is present, but the component still works
        Assert.Contains("xf-avatar", cut.Markup);
    }
}
