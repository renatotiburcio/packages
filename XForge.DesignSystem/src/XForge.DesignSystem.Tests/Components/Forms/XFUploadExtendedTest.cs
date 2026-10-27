using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.Upload;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFUploadExtendedTest : BunitContext
{
    public XFUploadExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFUpload> RenderUpload(
        Action<ComponentParameterCollectionBuilder<XFUpload>>? configure = null)
    {
        return Render<XFUpload>(p =>
        {
            p.Add(x => x.Label, "Upload files");
            configure?.Invoke(p);
        });
    }

    // SizeClass Sm (line 81)
    [Fact]
    public void Extended_SizeSm_RendersSmallDropZone()
    {
        var cut = RenderUpload(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("min-h-16", cut.Markup);
    }

    // SizeClass Lg (line 82)
    [Fact]
    public void Extended_SizeLg_RendersLargeDropZone()
    {
        var cut = RenderUpload(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("min-h-24", cut.Markup);
    }

    // VariantClass Filled (line 88)
    [Fact]
    public void Extended_VariantFilled_RendersFilledDropZone()
    {
        var cut = RenderUpload(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // VariantClass Ghost (line 90)
    [Fact]
    public void Extended_VariantGhost_RendersGhostDropZone()
    {
        var cut = RenderUpload(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // ToneClass Secondary (line 96)
    [Fact]
    public void Extended_ToneSecondary_RendersSecondaryFocus()
    {
        var cut = RenderUpload(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    // ToneClass Success (line 97)
    [Fact]
    public void Extended_ToneSuccess_RendersSuccessFocus()
    {
        var cut = RenderUpload(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("var(--color-success)", cut.Markup);
    }

    // ToneClass Warning (line 98)
    [Fact]
    public void Extended_ToneWarning_RendersWarningFocus()
    {
        var cut = RenderUpload(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    // ToneClass Danger (line 99)
    [Fact]
    public void Extended_ToneDanger_RendersDangerFocus()
    {
        var cut = RenderUpload(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    // StateClass Disabled (line 103)
    [Fact]
    public void Extended_Disabled_RendersDisabledDropZone()
    {
        var cut = RenderUpload(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-60", cut.Markup);
    }

    // Multiple
    [Fact]
    public void Extended_Multiple_RendersMultipleTitle()
    {
        var cut = RenderUpload(p => p.Add(x => x.Multiple, true));
        Assert.Contains("Solte arquivos", cut.Markup);
    }

    // Single
    [Fact]
    public void Extended_Single_RendersSingleTitle()
    {
        var cut = RenderUpload(p => p.Add(x => x.Multiple, false));
        Assert.Contains("Solte um arquivo", cut.Markup);
    }

    // FullWidth false
    [Fact]
    public void Extended_FullWidthFalse_RendersInlineFlex()
    {
        var cut = RenderUpload(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // ShowUploadAll false
    [Fact]
    public void Extended_ShowUploadAllFalse_HidesUploadAll()
    {
        var cut = RenderUpload(p => p.Add(x => x.ShowUploadAll, false));
        Assert.DoesNotContain("Enviar", cut.Markup);
    }

    // HelperText
    [Fact]
    public void Extended_HelperText_RendersHelper()
    {
        var cut = RenderUpload(p => p.Add(x => x.HelperText, "Max 10MB"));
        Assert.Contains("Max 10MB", cut.Markup);
    }

    // Accept
    [Fact]
    public void Extended_Accept_RendersAcceptAttribute()
    {
        var cut = RenderUpload(p => p.Add(x => x.Accept, ".png,.jpg"));
        Assert.Contains(".png,.jpg", cut.Markup);
    }
}
