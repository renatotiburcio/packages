#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Select;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFSelectGroupComponentTest : Bunit.BunitContext
{
    public XFSelectGroupComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithLabel()
    {
        var cut = Render<XFSelectGroup<string>>(parameters =>
            parameters.Add(p => p.Label, "Group A"));
        Assert.Contains("optgroup", cut.Markup);
        Assert.Contains("Group A", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFSelectGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Label, "Options");
            parameters.AddChildContent("Test item");
        });
        Assert.Contains("Test item", cut.Markup);
    }

    [Fact]
    public void Render_Disabled()
    {
        var cut = Render<XFSelectGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Label, "Disabled Group");
            parameters.Add(p => p.Disabled, true);
        });
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Render_WithIntegerType()
    {
        var cut = Render<XFSelectGroup<int>>(parameters =>
            parameters.Add(p => p.Label, "Numbers"));
        Assert.Contains("optgroup", cut.Markup);
    }
}
