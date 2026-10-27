using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using XForge.Components.Lgpd;

namespace XForge.DesignSystem.Tests.Components.Lgpd;

public class XFPrivacyPolicyModalTests : BunitContext
{
    [Fact]
    public void Render_WhenVisible_ShowsModal()
    {
        var cut = Render<XFPrivacyPolicyModal>();
        Assert.Contains("xf-lgpd-policy-modal", cut.Markup);
        Assert.Contains("Politica de Privacidade", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_HidesModal()
    {
        var cut = Render<XFPrivacyPolicyModal>(p => p.Add(x => x.Visible, false));
        Assert.DoesNotContain("xf-lgpd-policy-modal", cut.Markup);
    }

    [Fact]
    public void Render_WithVersion_ShowsVersion()
    {
        var cut = Render<XFPrivacyPolicyModal>(p => p.Add(x => x.Version, "3.0"));
        Assert.Contains("3.0", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomContent_ShowsContent()
    {
        var cut = Render<XFPrivacyPolicyModal>(p => p.Add(x => x.Content, "<p>Custom privacy content</p>"));
        Assert.Contains("Custom privacy content", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAcceptAllButton()
    {
        var cut = Render<XFPrivacyPolicyModal>();
        Assert.Contains("Aceitar todos", cut.Markup);
    }

    [Fact]
    public void Render_ShowsRejectButton()
    {
        var cut = Render<XFPrivacyPolicyModal>();
        Assert.Contains("Recusar", cut.Markup);
    }

    [Fact]
    public async Task ClickAcceptAll_InvokesOnAccept()
    {
        bool? accepted = null;
        var cut = Render<XFPrivacyPolicyModal>(p =>
            p.Add(x => x.OnAccept, EventCallback.Factory.Create<bool>(this, v => accepted = v)));

        await cut.Find("button:contains('Aceitar todos')").ClickAsync();

        Assert.True(accepted);
    }

    [Fact]
    public async Task ClickAcceptEssential_InvokesOnAccept()
    {
        bool? accepted = null;
        var cut = Render<XFPrivacyPolicyModal>(p =>
            p.Add(x => x.OnAccept, EventCallback.Factory.Create<bool>(this, v => accepted = v)));

        await cut.Find("button:contains('Aceitar apenas essenciais')").ClickAsync();

        Assert.False(accepted);
    }

    [Fact]
    public async Task ClickReject_InvokesOnReject()
    {
        bool rejectCalled = false;
        var cut = Render<XFPrivacyPolicyModal>(p =>
            p.Add(x => x.OnReject, EventCallback.Factory.Create(this, () => rejectCalled = true)));

        await cut.Find("button:contains('Recusar')").ClickAsync();

        Assert.True(rejectCalled);
    }
}
