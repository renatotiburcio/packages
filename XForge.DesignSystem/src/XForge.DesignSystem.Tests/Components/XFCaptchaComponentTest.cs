using Bunit;
using XForge.DesignSystem.Components.Forms.Captcha;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFCaptchaComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_Default_RendersContainer()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-site-key")
        );

        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("aria-label=\"Verificação de segurança\"", cut.Markup);
        Assert.Contains("<div", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomAriaLabel_RendersLabel()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
            .Add(x => x.AriaLabel, "Prove que você não é um robô")
        );

        Assert.Contains("aria-label=\"Prove que você não é um robô\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomId_RendersId()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
            .Add(x => x.Id, "my-captcha")
        );

        Assert.Contains("id=\"my-captcha\"", cut.Markup);
    }

    [Fact]
    public void Render_WithContainerClass_RendersClass()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
            .Add(x => x.ContainerClass, "g-recaptcha my-class")
        );

        Assert.Contains("g-recaptcha", cut.Markup);
        Assert.Contains("my-class", cut.Markup);
    }

    [Fact]
    public void Render_WithStyle_RendersStyle()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
            .Add(x => x.Style, "margin: 10px;")
        );

        Assert.Contains("style=\"margin: 10px;\"", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_RendersAttributes()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "captcha-widget"
            })
        );

        Assert.Contains("data-testid=\"captcha-widget\"", cut.Markup);
    }

    [Fact]
    public void Parameters_HaveCorrectDefaults()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
        );

        var instance = cut.Instance;
        Assert.Equal(XFCaptchaTheme.Light, instance.Theme);
        Assert.Equal(XFCaptchaSize.Normal, instance.Size);
        Assert.Equal(XFCaptchaBadgePosition.BottomRight, instance.BadgePosition);
        Assert.Null(instance.LanguageCode);
        Assert.Equal("Verificação de segurança", instance.AriaLabel);
    }

    [Fact]
    public void State_InitiallyInvalid()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
        );

        var instance = cut.Instance;
        Assert.False(instance.State.Valid);
        Assert.Equal(string.Empty, instance.State.Response);
    }

    [Fact]
    public void Theme_Enum_HasTwoValues()
    {
        Assert.Equal(2, Enum.GetValues<XFCaptchaTheme>().Length);
        Assert.Contains(XFCaptchaTheme.Light, Enum.GetValues<XFCaptchaTheme>());
        Assert.Contains(XFCaptchaTheme.Dark, Enum.GetValues<XFCaptchaTheme>());
    }

    [Fact]
    public void Size_Enum_HasTwoValues()
    {
        Assert.Equal(2, Enum.GetValues<XFCaptchaSize>().Length);
        Assert.Contains(XFCaptchaSize.Normal, Enum.GetValues<XFCaptchaSize>());
        Assert.Contains(XFCaptchaSize.Compact, Enum.GetValues<XFCaptchaSize>());
    }

    [Fact]
    public void BadgePosition_Enum_HasThreeValues()
    {
        Assert.Equal(3, Enum.GetValues<XFCaptchaBadgePosition>().Length);
        Assert.Contains(XFCaptchaBadgePosition.BottomRight, Enum.GetValues<XFCaptchaBadgePosition>());
        Assert.Contains(XFCaptchaBadgePosition.BottomLeft, Enum.GetValues<XFCaptchaBadgePosition>());
        Assert.Contains(XFCaptchaBadgePosition.Inline, Enum.GetValues<XFCaptchaBadgePosition>());
    }

    [Fact]
    public async Task OnSuccessHandler_SetsStateValid()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
        );

        var instance = cut.Instance;
        await instance.OnSuccessHandler("test-response-token");

        Assert.True(instance.State.Valid);
        Assert.Equal("test-response-token", instance.State.Response);
    }

    [Fact]
    public async Task OnExpiredHandler_ResetsState()
    {
        var cut = Render<XFCaptcha>(p => p
            .Add(x => x.SiteKey, "6Le-test-key")
        );

        var instance = cut.Instance;
        await instance.OnSuccessHandler("token");
        Assert.True(instance.State.Valid);

        await instance.OnExpiredHandler();
        Assert.False(instance.State.Valid);
        Assert.Equal(string.Empty, instance.State.Response);
    }
}
