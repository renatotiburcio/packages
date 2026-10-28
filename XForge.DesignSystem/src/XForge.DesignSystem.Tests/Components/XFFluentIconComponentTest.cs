using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Icon;

namespace XForge.DesignSystem.Tests.Components;

public class XFFluentIconComponentTest : XForgeTestContext
{
    public XFFluentIconComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithVariantAndSize_RendersSpan()
    {
        var cut = Render<XFFluentIcon<XFIconInfo>>(parameters =>
        {
            parameters.Add(p => p.Variant, XFIconVariant.Filled);
            parameters.Add(p => p.Size, XFIconSize.Size24);
        });
        Assert.Contains("xf-icon", cut.Markup);
    }

    [Fact]
    public void Render_WithName_RendersIconName()
    {
        var cut = Render<XFFluentIcon<XFIconInfo>>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Home icon");
        });
        Assert.Contains("Home icon", cut.Markup);
    }

    [Fact]
    public void Render_Focusable_HasTabIndexAndButtonRole()
    {
        var cut = Render<XFFluentIcon<XFIconInfo>>(parameters =>
        {
            parameters.Add(p => p.Focusable, true);
            parameters.Add(p => p.AriaLabel, "Clickable icon");
        });
        Assert.Contains("role=\"button\"", cut.Markup);
        Assert.Contains("tabindex=\"0\"", cut.Markup);
    }

    [Fact]
    public void Render_NotFocusable_HasImgRole()
    {
        var cut = Render<XFFluentIcon<XFIconInfo>>(parameters =>
        {
            parameters.Add(p => p.Focusable, false);
            parameters.Add(p => p.AriaLabel, "Static icon");
        });
        Assert.Contains("role=\"img\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultVariant_Regular()
    {
        var cut = Render<XFFluentIcon<XFIconInfo>>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Default icon");
        });
        Assert.Contains("data-variant=\"Regular\"", cut.Markup);
    }

    [Fact]
    public void Render_WithColor_SetsDataColor()
    {
        var cut = Render<XFFluentIcon<XFIconInfo>>(parameters =>
        {
            parameters.Add(p => p.Color, "Success");
            parameters.Add(p => p.AriaLabel, "Success icon");
        });
        Assert.Contains("Success", cut.Markup);
    }
}
