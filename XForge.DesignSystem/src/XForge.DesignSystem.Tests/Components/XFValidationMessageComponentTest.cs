#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFValidationMessageComponentTest : XForgeTestContext
{
    public XFValidationMessageComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithItems()
    {
        var cut = Render<XFValidationMessage>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "Field is required" });
        });
        Assert.Contains("xf-validation-message", cut.Markup);
        Assert.Contains("Field is required", cut.Markup);
    }

    [Fact]
    public void Render_WithMultipleItems()
    {
        var cut = Render<XFValidationMessage>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "Field is required", "Minimum 3 characters" });
        });
        Assert.Contains("Field is required", cut.Markup);
        Assert.Contains("Minimum 3 characters", cut.Markup);
    }

    [Fact]
    public void Render_WithDangerTone()
    {
        var cut = Render<XFValidationMessage>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Danger);
            parameters.Add(p => p.Items, new[] { "Error occurred" });
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Render_WithSuccessTone()
    {
        var cut = Render<XFValidationMessage>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
            parameters.Add(p => p.Items, new[] { "Valid input" });
        });
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Render_WithWarningTone()
    {
        var cut = Render<XFValidationMessage>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.Items, new[] { "Check this field" });
        });
        Assert.Contains("text-warning", cut.Markup);
    }
}