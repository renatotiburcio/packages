using Bunit;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Forms.SpeechToText;
using Xunit;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFSpeechToTextComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_Default_RendersButton()
    {
        var cut = Render<XFSpeechToText>();

        Assert.Contains("xf-speech-to-text", cut.Markup);
        Assert.Contains("<button", cut.Markup);
        Assert.Contains("Iniciar reconhecimento", cut.Markup);
    }

    [Fact]
    public void Render_WithStartLabel_RendersCustomLabel()
    {
        var cut = Render<XFSpeechToText>(p => p
            .Add(x => x.StartLabel, "Falar")
        );

        Assert.Contains("Falar", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_AppliesDisabled()
    {
        var cut = Render<XFSpeechToText>(p => p
            .Add(x => x.Disabled, true)
        );

        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_RendersAriaPressed()
    {
        var cut = Render<XFSpeechToText>();

        Assert.Contains("aria-pressed=\"false\"", cut.Markup);
    }

    [Fact]
    public void Render_WithVariant_Solid_AppliesClass()
    {
        var cut = Render<XFSpeechToText>(p => p
            .Add(x => x.Variant, XFFormVariant.Filled)
        );

        Assert.Contains("border-transparent", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    [Fact]
    public void Render_WithSize_Lg_AppliesClass()
    {
        var cut = Render<XFSpeechToText>(p => p
            .Add(x => x.Size, XFSize.Lg)
        );

        Assert.Contains("h-12", cut.Markup);
    }

    [Fact]
    public void Render_WithFullWidth_AppliesClass()
    {
        var cut = Render<XFSpeechToText>(p => p
            .Add(x => x.FullWidth, true)
        );

        Assert.Contains("w-full", cut.Markup);
    }
}
