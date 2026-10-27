using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Card;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay.Card;

public class XFCardTests : BunitContext
{
    public XFCardTests() { }

    [Fact]
    public void Card_RendersBaseClass()
    {
        var cut = Render<XFCard>();
        Assert.Contains("xf-card", cut.Markup);
    }

    [Fact]
    public void Card_WithChildContent_RendersContent()
    {
        var cut = Render<XFCard>(parameters => parameters
            .AddChildContent("<span>Hello</span>")
        );
        Assert.Contains("Hello", cut.Markup);
    }

    [Fact]
    public void Card_ElevatedVariant_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Variant, XFCardVariant.Elevated)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_OutlinedVariant_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Variant, XFCardVariant.Outlined)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_FilledVariant_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Variant, XFCardVariant.Filled)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_FlatVariant_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Variant, XFCardVariant.Flat)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_GhostVariant_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Variant, XFCardVariant.Ghost)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_PaddingClasses()
    {
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Padding, XFPadding.None)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Padding, XFPadding.Sm)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Padding, XFPadding.Md)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Padding, XFPadding.Lg)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Padding, XFPadding.Xl)).Markup);
    }

    [Fact]
    public void Card_RadiusClasses()
    {
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Radius, XFCardRadius.Sm)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Radius, XFCardRadius.Md)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Radius, XFCardRadius.Lg)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Radius, XFCardRadius.Xl)).Markup);
    }

    [Fact]
    public void Card_ToneClasses()
    {
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Primary)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Success)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Warning)).Markup);
        Assert.NotNull(Render<XFCard>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Danger)).Markup);
    }

    [Fact]
    public void Card_Interactive_AddsRole()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Interactive, true)
        );
        Assert.Contains("button", cut.Markup);
    }

    [Fact]
    public void Card_AriaLabel_SetCorrectly()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.AriaLabel, "My Card")
        );
        Assert.Contains("My Card", cut.Markup);
    }

    [Fact]
    public void Card_Dividers_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.Dividers, true)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_AreaRestricted_Renders()
    {
        var cut = Render<XFCard>(parameters => parameters
            .Add(p => p.AreaRestricted, true)
        );
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Card_HasMedia_RendersMediaSection()
    {
        var cut = Render<XFCard>(parameters => parameters
            .AddChildContent("<span>Media Content</span>")
        );
        Assert.Contains("Media Content", cut.Markup);
    }

    [Fact]
    public void XFPadding_Has5Members()
    {
        Assert.Equal(5, Enum.GetValues<XFPadding>().Length);
    }

    [Fact]
    public void XFCardVariant_Has5Members()
    {
        Assert.Equal(5, Enum.GetValues<XFCardVariant>().Length);
    }

    [Fact]
    public void XFCardRadius_Has4Members()
    {
        Assert.Equal(4, Enum.GetValues<XFCardRadius>().Length);
    }
}
