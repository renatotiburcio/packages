using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using XForge.Components.Lgpd;

namespace XForge.DesignSystem.Tests.Components.Lgpd;

public class XFConsentBannerTests : XForgeTestContext
{
    [Fact]
    public void Render_WhenVisible_ShowsBanner()
    {
        var cut = Render<XFConsentBanner>();
        Assert.Contains("xf-lgpd-consent", cut.Markup);
        Assert.Contains("Aceitar", cut.Markup);
        Assert.Contains("Recusar", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_HidesBanner()
    {
        var cut = Render<XFConsentBanner>(p => p.Add(x => x.Visible, false));
        Assert.DoesNotContain("xf-lgpd-consent", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomMessage_ShowsMessage()
    {
        var cut = Render<XFConsentBanner>(p => p.Add(x => x.Message, "Custom LGPD message"));
        Assert.Contains("Custom LGPD message", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomPolicyUrl_ShowsLink()
    {
        var cut = Render<XFConsentBanner>(p => p.Add(x => x.PolicyUrl, "/custom-policy"));
        Assert.Contains("/custom-policy", cut.Markup);
    }

    [Fact]
    public void Render_WithPolicyVersion_ShowsVersion()
    {
        var cut = Render<XFConsentBanner>(p => p.Add(x => x.PolicyVersion, "2.0"));
        Assert.Contains("2.0", cut.Markup);
    }

    [Fact]
    public async Task ClickAccept_InvokesConsentChanged()
    {
        bool? result = null;
        var cut = Render<XFConsentBanner>(p =>
            p.Add(x => x.OnConsentChanged, EventCallback.Factory.Create<bool>(this, v => result = v)));

        await cut.Find(".btn-accept").ClickAsync();

        Assert.True(result);
    }

    [Fact]
    public async Task ClickReject_InvokesConsentChanged()
    {
        bool? result = null;
        var cut = Render<XFConsentBanner>(p =>
            p.Add(x => x.OnConsentChanged, EventCallback.Factory.Create<bool>(this, v => result = v)));

        await cut.Find(".btn-reject").ClickAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task ClickAccept_HidesBanner()
    {
        var cut = Render<XFConsentBanner>(p =>
            p.Add(x => x.OnConsentChanged, EventCallback.Factory.Create<bool>(this, _ => { })));

        await cut.Find(".btn-accept").ClickAsync();

        Assert.DoesNotContain("xf-lgpd-consent", cut.Markup);
    }
}
