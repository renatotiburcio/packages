using Bunit;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFToastExtendedTest : XForgeTestContext
{
    public XFToastExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private static XFToastItem CreateItem(XFIntent intent, string title = "Test", string? message = null)
    {
        return new XFToastItem(
            Guid.NewGuid(),
            new XFToastOptions
            {
                Intent = intent,
                Title = title,
                Message = message ?? $"{title} message"
            });
    }

    // ToneClass Rich Primary (line 81)
    [Fact]
    public void Extended_RichPrimary_RendersPrimaryRichTone()
    {
        var item = CreateItem(XFIntent.Primary);
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.RichColors, true);
        });
        Assert.Contains("var(--color-primary)", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    // ToneClass Rich Info (line 82)
    [Fact]
    public void Extended_RichInfo_RendersInfoRichTone()
    {
        var item = CreateItem(XFIntent.Info);
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.RichColors, true);
        });
        Assert.Contains("var(--color-info)", cut.Markup);
        Assert.Contains("text-info", cut.Markup);
    }

    // ToneClass default Warning (line 85)
    [Fact]
    public void Extended_DefaultWarning_RendersWarningDefaultTone()
    {
        var item = CreateItem(XFIntent.Warning);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    // ToneClass default Danger (line 86)
    [Fact]
    public void Extended_DefaultDanger_RendersDangerDefaultTone()
    {
        var item = CreateItem(XFIntent.Danger);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    // ToneClass default Primary (line 87)
    [Fact]
    public void Extended_DefaultPrimary_RendersPrimaryDefaultTone()
    {
        var item = CreateItem(XFIntent.Primary);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("var(--color-primary)", cut.Markup);
    }

    // ToneClass default Neutral (line 88)
    [Fact]
    public void Extended_DefaultNeutral_RendersNeutralDefaultTone()
    {
        var item = CreateItem(XFIntent.Neutral);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("border-border-light", cut.Markup);
    }

    // IconClass Primary (line 107)
    [Fact]
    public void Extended_IconPrimary_RendersPrimaryIcon()
    {
        var item = CreateItem(XFIntent.Primary);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("text-primary", cut.Markup);
    }

    // IconClass Neutral (line 108)
    [Fact]
    public void Extended_IconNeutral_RendersNeutralIcon()
    {
        var item = CreateItem(XFIntent.Neutral);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // DefaultIcon Primary (line 117) â€” icon rendered via XFSvgIcons.Markup, verify icon container
    [Fact]
    public void Extended_IconPrimary_RendersIconContainer()
    {
        var item = CreateItem(XFIntent.Primary);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        // Icon container should have the intent-specific class
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]", cut.Markup);
    }

    // DefaultIcon Neutral (line 118)
    [Fact]
    public void Extended_IconNeutral_RendersIconContainer()
    {
        var item = CreateItem(XFIntent.Neutral);
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // TitleClass solid (line 94-95)
    [Fact]
    public void Extended_SolidTitle_RendersInheritColor()
    {
        var item = CreateItem(XFIntent.Success);
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.SolidColors, true);
        });
        Assert.Contains("text-inherit", cut.Markup);
    }

    // MessageClass solid (line 98-99)
    [Fact]
    public void Extended_SolidMessage_RendersInheritColor()
    {
        var item = CreateItem(XFIntent.Success);
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.SolidColors, true);
        });
        Assert.Contains("opacity-90", cut.Markup);
    }

    // AriaLive Off (line 30)
    [Fact]
    public void Extended_LiveOff_RendersOffAriaLive()
    {
        var item = new XFToastItem(
            Guid.NewGuid(),
            new XFToastOptions { Intent = XFIntent.Info, Title = "Test", Live = XFAriaLive.Off });
        var cut = Render<XFToast>(p => p.Add(x => x.Item, item));
        var toast = cut.Find(".xf-toast");
        Assert.Equal("off", toast.GetAttribute("aria-live"));
    }

    // StackClass collapsed (line 45)
    [Fact]
    public void Extended_NotExpanded_RendersCollapsedStack()
    {
        var item = CreateItem(XFIntent.Success);
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.Expanded, false);
        });
        Assert.Contains("group-hover/toaster", cut.Markup);
    }

    // StackClass expanded (line 45)
    [Fact]
    public void Extended_Expanded_RendersRelativeStack()
    {
        var item = CreateItem(XFIntent.Success);
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.Expanded, true);
        });
        Assert.Contains("relative", cut.Markup);
    }

    // CloseButton false (line 20) â€” close button condition is Item.Dismissible || CloseButton
    [Fact]
    public void Extended_NoCloseButton_HidesDismissButton()
    {
        var item = new XFToastItem(
            Guid.NewGuid(),
            new XFToastOptions { Intent = XFIntent.Success, Title = "Test", Dismissible = false });
        var cut = Render<XFToast>(p =>
        {
            p.Add(x => x.Item, item);
            p.Add(x => x.CloseButton, false);
        });
        Assert.DoesNotContain("Fechar notificacao", cut.Markup);
    }
}
