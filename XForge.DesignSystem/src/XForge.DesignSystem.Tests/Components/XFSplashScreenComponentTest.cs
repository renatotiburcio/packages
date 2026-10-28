using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Feedback.SplashScreen;

namespace XForge.DesignSystem.Tests.Components;

public class XFSplashScreenComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_FullScreenStatusWithDefaultLogo()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Title, "Carregando");
            parameters.Add(p => p.Message, "Preparando");
        });

        Assert.Contains("role=\"status\"", cut.Markup);
        Assert.Contains("fixed inset-0", cut.Markup);
        Assert.Contains("Carregando", cut.Markup);
        Assert.Contains("XF", cut.Markup);
    }

    [Fact]
    public void Render_ProgressClampsValue()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, false);
            parameters.Add(p => p.Progress, 150);
            parameters.Add(p => p.ShowProgressValue, true);
        });

        Assert.Contains("width:100", cut.Markup);
        Assert.Contains("100%", cut.Markup);
        Assert.Contains("min-h-80", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomProgressValueFormat_UsesLocalizedProgressText()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Progress, 42);
            parameters.Add(p => p.ShowProgressValue, true);
            parameters.Add(p => p.ProgressValueFormat, "{0} percent loaded");
        });

        Assert.Contains("42 percent loaded", cut.Markup);
        Assert.DoesNotContain(">42%<", cut.Markup);
    }

    [Fact]
    public void Render_InvisibleProducesNoMarkup()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
        });

        Assert.Equal(string.Empty, cut.Markup);
    }

    [Fact]
    public void Render_CustomLogoAndChildContent()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Logo, builder => builder.AddContent(0, "Logo"));
            parameters.AddChildContent("Auxiliar");
        });

        Assert.Contains("Logo", cut.Markup);
        Assert.Contains("Auxiliar", cut.Markup);
    }

    [Fact]
    public void Render_DefaultTitle_IsCarregando()
    {
        var cut = Render<XFSplashScreen>();

        Assert.Contains("Carregando", cut.Markup);
    }

    [Fact]
    public void Render_Message_RendersWhenSet()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Message, "Sincronizando dados");
        });

        Assert.Contains("Sincronizando dados", cut.Markup);
    }

    [Fact]
    public void Render_AriaLabel_DefaultApplied()
    {
        var cut = Render<XFSplashScreen>();

        Assert.Contains("aria-label=\"Carregando aplicacao\"", cut.Markup);
    }

    [Fact]
    public void Render_AriaLabel_CustomOverridden()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Loading screen");
        });

        Assert.Contains("aria-label=\"Loading screen\"", cut.Markup);
    }

    [Fact]
    public void Render_FullScreen_HasAriaLivePolite()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, true);
        });

        Assert.Contains("aria-live=\"polite\"", cut.Markup);
    }

    [Fact]
    public void Render_NonFullScreen_HasAriaLiveOff()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, false);
        });

        Assert.Contains("aria-live=\"off\"", cut.Markup);
    }

    [Fact]
    public void Render_FullScreen_HasRoleStatus()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, true);
        });

        Assert.Contains("role=\"status\"", cut.Markup);
    }

    [Fact]
    public void Render_NonFullScreen_HasRoleRegion()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, false);
        });

        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void Render_AriaBusy_AlwaysTrue()
    {
        var cut = Render<XFSplashScreen>();

        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_CustomInitials_ShownWhenNoLogo()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Initials, "ACME");
        });

        Assert.Contains("ACME", cut.Markup);
        Assert.DoesNotContain("XF", cut.Markup);
    }

    [Fact]
    public void Render_ProgressZero_ShowsZeroPercent()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Progress, 0);
            parameters.Add(p => p.ShowProgressValue, true);
        });

        Assert.Contains("width:0", cut.Markup);
        Assert.Contains("0%", cut.Markup);
    }

    [Fact]
    public void Render_ProgressNegative_ClampsToZero()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Progress, -50);
            parameters.Add(p => p.ShowProgressValue, true);
        });

        Assert.Contains("width:0", cut.Markup);
        Assert.Contains("0%", cut.Markup);
    }

    [Fact]
    public void Render_ProgressFifty_HalfWidth()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Progress, 50);
        });

        Assert.Contains("width:50", cut.Markup);
    }

    [Fact]
    public void Render_NoProgress_ShowsIndeterminateAnimation()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Progress, null);
        });

        Assert.Contains("animate-pulse", cut.Markup);
    }

    [Fact]
    public void Render_Logo_HiddenAttributeApplied()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Logo, builder => builder.AddContent(0, "Logo"));
        });

        Assert.Contains("aria-hidden", cut.Markup);
    }

    [Fact]
    public void Render_NoLogo_NoAriaHiddenOnLogoArea()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Initials, "XF");
        });

        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_FullScreen_FixedPosition()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, true);
        });

        Assert.Contains("fixed inset-0 z-50", cut.Markup);
    }

    [Fact]
    public void Render_NonFullScreen_RoundedBorder()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.FullScreen, false);
        });

        Assert.Contains("rounded-lg border border-border-light", cut.Markup);
    }

    [Fact]
    public void Render_ProgressValueFormat_FractionalProgress()
    {
        var cut = Render<XFSplashScreen>(parameters =>
        {
            parameters.Add(p => p.Progress, 33.7);
            parameters.Add(p => p.ShowProgressValue, true);
        });

        Assert.Contains("34%", cut.Markup);
    }
}
