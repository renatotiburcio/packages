using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Forms.SelectBar;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFSelectBarItemComponentTest : BunitContext
{
    public XFSelectBarItemComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithText_RendersText()
    {
        var cut = Render<XFSelectBarItem>(parameters =>
            parameters.Add(p => p.Text, "Option 1"));

        Assert.Contains("Option 1", cut.Markup);
        Assert.Contains("xf-select-bar-item", cut.Markup);
    }

    [Fact]
    public void Render_WithDataValue_SetsDataAttribute()
    {
        var cut = Render<XFSelectBarItem>(parameters =>
        {
            parameters.Add(p => p.Text, "Choice");
            parameters.Add(p => p.Value, "choice-1");
        });

        Assert.Contains("data-value=\"choice-1\"", cut.Markup);
    }

    [Fact]
    public void Render_Default_NotDisabled()
    {
        var cut = Render<XFSelectBarItem>(parameters =>
            parameters.Add(p => p.Text, "Active"));

        Assert.Contains("data-disabled=\"false\"", cut.Markup);
        Assert.Contains("aria-disabled=\"false\"", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_SetsDisabledAttributes()
    {
        var cut = Render<XFSelectBarItem>(parameters =>
        {
            parameters.Add(p => p.Text, "Locked");
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("data-disabled=\"true\"", cut.Markup);
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithIcon_RendersIcon()
    {
        var cut = Render<XFSelectBarItem>(parameters =>
        {
            parameters.Add(p => p.Text, "With Icon");
            parameters.Add(p => p.Icon, (RenderFragment)(b => b.AddContent(0, "\u2605")));
        });

        Assert.Contains("\u2605", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_HasExpectedCssClasses()
    {
        var cut = Render<XFSelectBarItem>(parameters =>
            parameters.Add(p => p.Text, "Styled"));

        Assert.Contains("inline-flex", cut.Markup);
        Assert.Contains("items-center", cut.Markup);
        Assert.Contains("font-medium", cut.Markup);
    }
}
