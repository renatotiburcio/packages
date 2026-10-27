using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Select;

namespace XForge.DesignSystem.Tests.Components;

public class XFSelectOptionComponentTest : Bunit.BunitContext
{
    public XFSelectOptionComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithText()
    {
        var cut = Render<XFSelectOption<string>>(parameters =>
        {
            parameters.Add(p => p.Value, "opt1");
            parameters.Add(p => p.Text, "Option 1");
        });
        Assert.Contains("xf-select-option", cut.Markup);
        Assert.Contains("Option 1", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFSelectOption<string>>(parameters =>
        {
            parameters.Add(p => p.Value, "opt2");
            parameters.AddChildContent("Custom Content");
        });
        Assert.Contains("Custom Content", cut.Markup);
    }

    [Fact]
    public void Render_Disabled()
    {
        var cut = Render<XFSelectOption<string>>(parameters =>
        {
            parameters.Add(p => p.Value, "opt3");
            parameters.Add(p => p.Text, "Disabled Option");
            parameters.Add(p => p.Disabled, true);
        });
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Render_Hidden()
    {
        var cut = Render<XFSelectOption<string>>(parameters =>
        {
            parameters.Add(p => p.Value, "opt4");
            parameters.Add(p => p.Text, "Hidden Option");
            parameters.Add(p => p.Hidden, true);
        });
        Assert.Contains("hidden", cut.Markup);
    }

    [Fact]
    public void Render_WithIntegerValue()
    {
        var cut = Render<XFSelectOption<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 42);
            parameters.Add(p => p.Text, "The Answer");
        });
        Assert.Contains("The Answer", cut.Markup);
    }
}
