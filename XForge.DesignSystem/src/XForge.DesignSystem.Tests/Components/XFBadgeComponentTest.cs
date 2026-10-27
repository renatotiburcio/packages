#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Badge;
using XForge.Localization;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFBadgeComponentTest : Bunit.BunitContext
{
    public XFBadgeComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithText()
    {
        var cut = Render<XFBadge>(parameters =>
            parameters.Add(p => p.Text, "Active"));

        Assert.Contains("xf-badge", cut.Markup);
        Assert.Contains("Active", cut.Markup);
    }

    [Fact]
    public void Render_WithSolidSecondary_UsesSecondaryTokens()
    {
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.Variant, XFBadgeVariant.Solid);
            parameters.Add(p => p.Color, XFBadgeColor.Secondary);
        });

        Assert.Contains("bg-secondary", cut.Markup);
        Assert.Contains("text-secondary-contrast", cut.Markup);
    }

    [Fact]
    public void Render_WithDotAndIcon_RendersDecorativeSlots()
    {
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.Dot, true);
            parameters.Add(p => p.StartIcon, builder => builder.AddContent(0, "i"));
            parameters.Add(p => p.Text, "Queued");
        });

        Assert.Contains("rounded-full bg-info", cut.Markup);
        Assert.Contains(">i</span>", cut.Markup);
        Assert.Contains("Queued", cut.Markup);
    }

    [Fact]
    public void Render_WithCircleShape_UsesStableCircularSizing()
    {
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.Shape, XFBadgeShape.Circle);
            parameters.Add(p => p.Text, "3");
        });

        Assert.Contains("rounded-full justify-center aspect-square", cut.Markup);
        Assert.Contains("h-7 min-w-7", cut.Markup);
    }

    [Fact]
    public async Task Click_WhenInteractive_CallsOnClick()
    {
        var clicked = false;
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.Text, "Clickable");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("span").ClickAsync();

        Assert.True(clicked);
        Assert.Contains("role=\"button\"", cut.Markup);
    }

    [Fact]
    public async Task Dismiss_Click_CallsOnDismiss()
    {
        var dismissed = false;
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.Text, "Dismissible");
            parameters.Add(p => p.OnDismiss, EventCallback.Factory.Create<MouseEventArgs>(this, _ => dismissed = true));
        });

        await cut.Find("button").ClickAsync();

        Assert.True(dismissed);
        Assert.Contains("badge.dismiss_format", cut.Markup);
        Assert.Contains("badge.dismiss", cut.Markup);
    }

    [Fact]
    public void Dismiss_WithLocalizedFormats_UsesBadgeText()
    {
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.Text, "Filtro ativo");
            parameters.Add(p => p.DismissTitleFormat, "Excluir {0}");
            parameters.Add(p => p.DismissAriaLabelFormat, "Excluir filtro {0}");
            parameters.Add(p => p.OnDismiss, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { }));
        });

        Assert.Contains("badge.dismiss_format", cut.Markup);
        Assert.Contains("badge.dismiss", cut.Markup);
    }

    [Fact]
    public void Dismiss_WithoutTextOrLabel_UsesPortugueseFallbacks()
    {
        var cut = Render<XFBadge>(parameters =>
        {
            parameters.Add(p => p.OnDismiss, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { }));
        });

        Assert.Contains("badge.dismiss", cut.Markup);
        Assert.DoesNotContain("Remover marcador badge", cut.Markup);
    }
}
