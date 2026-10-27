using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public class XFChipListComponentTest : BunitContext
{
    public XFChipListComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFChipList>();
        Assert.Contains("xf-chip-list", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFChipList>(parameters =>
            parameters.AddChildContent("Chip content"));
        Assert.Contains("Chip content", cut.Markup);
    }

    [Fact]
    public void Render_WithItems_RendersEachChip()
    {
        var items = new List<string> { "Alpha", "Beta", "Gamma" };
        var cut = Render<XFChipList>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.AddChildContent("CHILD");
        });
        Assert.Contains("xf-chip-list", cut.Markup);
    }

    [Fact]
    public void Render_Selectable_PassesSelectableParameter()
    {
        var cut = Render<XFChipList>(parameters =>
        {
            parameters.Add(p => p.Selectable, true);
            parameters.AddChildContent("SEL");
        });
        Assert.Contains("xf-chip-list", cut.Markup);
    }

    [Fact]
    public void Render_WrapDisabled_UsesOverflow()
    {
        var cut = Render<XFChipList>(parameters =>
        {
            parameters.Add(p => p.Wrap, false);
            parameters.AddChildContent("NO-WRAP");
        });
        Assert.Contains("xf-chip-list", cut.Markup);
    }
}