using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using XForge.Components.Lgpd;

namespace XForge.DesignSystem.Tests.Components.Lgpd;

public class XFCookieConsentTests : BunitContext
{
    [Fact]
    public void Render_WhenVisible_ShowsConsentForm()
    {
        var cut = Render<XFCookieConsent>();
        Assert.Contains("xf-lgpd-cookies", cut.Markup);
        Assert.Contains("Preferencias de Cookies", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_HidesForm()
    {
        var cut = Render<XFCookieConsent>(p => p.Add(x => x.Visible, false));
        Assert.DoesNotContain("xf-lgpd-cookies", cut.Markup);
    }

    [Fact]
    public void Render_DefaultCategories_ShowsThreeCategories()
    {
        var cut = Render<XFCookieConsent>();
        Assert.Contains("Essenciais", cut.Markup);
        Assert.Contains("Performance", cut.Markup);
        Assert.Contains("Marketing", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomCategories_ShowsCustom()
    {
        var categories = new[]
        {
            new XFCookieConsent.CookieCategory { Name = "Analytics", Description = "Usage data", Enabled = true }
        };

        var cut = Render<XFCookieConsent>(p => p.Add(x => x.Categories, categories));
        Assert.Contains("Analytics", cut.Markup);
    }

    [Fact]
    public async Task ClickSave_InvokesOnSave()
    {
        IReadOnlyList<XFCookieConsent.CookieCategory>? savedCategories = null;
        var cut = Render<XFCookieConsent>(p =>
            p.Add(x => x.OnSave, EventCallback.Factory.Create<IReadOnlyList<XFCookieConsent.CookieCategory>>(this, v => savedCategories = v)));

        await cut.Find("button").ClickAsync();

        Assert.NotNull(savedCategories);
        Assert.Equal(3, savedCategories!.Count);
    }

    [Fact]
    public async Task ClickSave_HidesForm()
    {
        var cut = Render<XFCookieConsent>(p =>
            p.Add(x => x.OnSave, EventCallback.Factory.Create<IReadOnlyList<XFCookieConsent.CookieCategory>>(this, _ => { })));

        await cut.Find("button").ClickAsync();

        Assert.DoesNotContain("xf-lgpd-cookies", cut.Markup);
    }

    [Fact]
    public async Task ToggleMarketing_DisablesCategory()
    {
        var cut = Render<XFCookieConsent>();
        var checkboxes = cut.FindAll("input[type='checkbox']");

        // Marketing is the third checkbox (index 2)
        checkboxes[2].Change(false);

        // Verify the checkbox is unchecked by checking the attribute
        Assert.False(checkboxes[2].HasAttribute("checked"));
    }
}
