using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.TextInput;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFTextInputExtendedTest : BunitContext
{
    public XFTextInputExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFTextInput> RenderTextInput(
        Action<ComponentParameterCollectionBuilder<XFTextInput>>? configure = null)
    {
        return Render<XFTextInput>(p =>
        {
            p.Add(x => x.Label, "Name");
            configure?.Invoke(p);
        });
    }

    // InputType Email (line 75)
    [Fact]
    public void Extended_TypeEmail_RendersEmailInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Type, XFTextInputType.Email));
        Assert.Contains("type=\"email\"", cut.Markup);
    }

    // InputType Password (line 76)
    [Fact]
    public void Extended_TypePassword_RendersPasswordInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Type, XFTextInputType.Password));
        Assert.Contains("type=\"password\"", cut.Markup);
    }

    // InputType Search (line 78)
    [Fact]
    public void Extended_TypeSearch_RendersSearchInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Type, XFTextInputType.Search));
        Assert.Contains("type=\"search\"", cut.Markup);
    }

    // InputType Tel (line 79)
    [Fact]
    public void Extended_TypeTel_RendersTelInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Type, XFTextInputType.Tel));
        Assert.Contains("type=\"tel\"", cut.Markup);
    }

    // InputType Url (line 79)
    [Fact]
    public void Extended_TypeUrl_RendersUrlInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Type, XFTextInputType.Url));
        Assert.Contains("type=\"url\"", cut.Markup);
    }

    // FullWidth false (line 91)
    [Fact]
    public void Extended_FullWidthFalse_RendersInlineFlex()
    {
        var cut = RenderTextInput(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // HelperText
    [Fact]
    public void Extended_HelperText_RendersHelper()
    {
        var cut = RenderTextInput(p => p.Add(x => x.HelperText, "Enter your name"));
        Assert.Contains("Enter your name", cut.Markup);
    }

    // Disabled
    [Fact]
    public void Extended_Disabled_RendersDisabledInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-60", cut.Markup);
    }

    // Required
    [Fact]
    public void Extended_Required_RendersRequiredInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.Required, true));
        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-required"));
    }

    // ReadOnly
    [Fact]
    public void Extended_ReadOnly_RendersReadOnlyInput()
    {
        var cut = RenderTextInput(p => p.Add(x => x.ReadOnly, true));
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("readonly"));
    }

    // PrefixText
    [Fact]
    public void Extended_PrefixText_RendersPrefix()
    {
        var cut = RenderTextInput(p => p.Add(x => x.PrefixText, "$"));
        Assert.Contains("$", cut.Markup);
    }

    // SuffixText
    [Fact]
    public void Extended_SuffixText_RendersSuffix()
    {
        var cut = RenderTextInput(p => p.Add(x => x.SuffixText, "kg"));
        Assert.Contains("kg", cut.Markup);
    }

    // FloatingLabel
    [Fact]
    public void Extended_FloatingLabel_RendersFloatingLabel()
    {
        var cut = RenderTextInput(p => p.Add(x => x.FloatingLabel, true));
        Assert.Contains("xf-text-input", cut.Markup);
    }

    // StartIcon
    [Fact]
    public void Extended_StartIcon_RendersStartIcon()
    {
        var cut = RenderTextInput(p =>
        {
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("xf-text-input", cut.Markup);
    }

    // EndIcon
    [Fact]
    public void Extended_EndIcon_RendersEndIcon()
    {
        var cut = RenderTextInput(p =>
        {
            p.Add(x => x.EndIcon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("xf-text-input", cut.Markup);
    }
}
