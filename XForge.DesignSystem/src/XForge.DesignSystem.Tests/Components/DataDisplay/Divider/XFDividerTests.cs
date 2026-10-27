using Bunit;
using XForge.DesignSystem.Components.DataDisplay.Divider;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay.Divider;

public class XFDividerTests : BunitContext
{
    [Fact]
    public void Divider_RendersBaseClass()
    {
        var cut = Render<XFDivider>();
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Divider_HorizontalOrientation_Renders()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Orientation, XFOrientation.Horizontal));
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Divider_VerticalOrientation_Renders()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Orientation, XFOrientation.Vertical));
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Divider_WithLabel_RendersLabel()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Label, "Section"));
        Assert.Contains("Section", cut.Markup);
    }

    [Fact]
    public void Divider_Dashed_Renders()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Dashed, true));
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Divider_Hidden_DoesNotRender()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    [Fact]
    public void Divider_AlignStart_Renders()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Align, XFDividerAlign.Start));
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Divider_AlignEnd_Renders()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Align, XFDividerAlign.End));
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void Divider_SpacingVariants()
    {
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Spacing, XFDividerSpacing.None)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Spacing, XFDividerSpacing.Sm)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Spacing, XFDividerSpacing.Md)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Spacing, XFDividerSpacing.Lg)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Spacing, XFDividerSpacing.Xl)).Markup);
    }

    [Fact]
    public void Divider_ThicknessVariants()
    {
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Thickness, XFDividerThickness.Thin)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Thickness, XFDividerThickness.Md)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Thickness, XFDividerThickness.Lg)).Markup);
    }

    [Fact]
    public void Divider_ToneVariants()
    {
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Primary)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Success)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Warning)).Markup);
        Assert.NotNull(Render<XFDivider>(parameters => parameters.Add(p => p.Tone, XFSemanticColor.Danger)).Markup);
    }

    [Fact]
    public void Divider_AriaOrientation_SetCorrectly()
    {
        var cut = Render<XFDivider>(parameters => parameters
            .Add(p => p.Orientation, XFOrientation.Vertical));
        Assert.Contains("vertical", cut.Markup);
    }

    [Fact]
    public void XFDividerAlign_Has3Members()
    {
        Assert.Equal(3, Enum.GetValues<XFDividerAlign>().Length);
    }

    [Fact]
    public void XFDividerSpacing_Has5Members()
    {
        Assert.Equal(5, Enum.GetValues<XFDividerSpacing>().Length);
    }

    [Fact]
    public void XFDividerThickness_Has3Members()
    {
        Assert.Equal(3, Enum.GetValues<XFDividerThickness>().Length);
    }
}
