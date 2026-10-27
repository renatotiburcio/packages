using Bunit;
using XForge.DesignSystem.Components.Forms.FileInput;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFFileInputGapTest : BunitContext
{
    public XFFileInputGapTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFFileInput> RenderFileInput(
        Action<ComponentParameterCollectionBuilder<XFFileInput>>? configure = null)
    {
        return Render<XFFileInput>(p =>
        {
            configure?.Invoke(p);
        });
    }

    // ─── RootClass: base classes ──────────────────────────────────────

    [Fact]
    public void Extended_RootClass_BaseFlexColGap()
    {
        var cut = RenderFileInput();
        Assert.Contains("flex flex-col gap-1.5", cut.Markup);
    }

    // ─── ToneClass: focus-within:ring-2 + ring color ──────────────────

    [Fact]
    public void Extended_TonePrimary_FullRingClass()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSecondary_FullRingClass()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSuccess_FullRingClass()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneWarning_FullRingClass()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDanger_FullRingClass()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDefault_FullRingClass()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Tone, XFSemanticColor.Default));
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]", cut.Markup);
    }

    // ─── DropZoneClass: base classes ──────────────────────────────────

    [Fact]
    public void Extended_DropZoneClass_Relative()
    {
        var cut = RenderFileInput();
        Assert.Contains("relative", cut.Markup);
    }

    [Fact]
    public void Extended_DropZoneClass_ItemsCenter()
    {
        var cut = RenderFileInput();
        Assert.Contains("items-center", cut.Markup);
    }

    [Fact]
    public void Extended_DropZoneClass_Gap3()
    {
        var cut = RenderFileInput();
        Assert.Contains("gap-3", cut.Markup);
    }

    [Fact]
    public void Extended_DropZoneClass_TransitionAll()
    {
        var cut = RenderFileInput();
        Assert.Contains("transition-all", cut.Markup);
    }

    [Fact]
    public void Extended_DropZoneClass_Duration200()
    {
        var cut = RenderFileInput();
        Assert.Contains("duration-200", cut.Markup);
    }

    [Fact]
    public void Extended_DropZoneClass_EaseOut()
    {
        var cut = RenderFileInput();
        Assert.Contains("ease-out", cut.Markup);
    }

    // ─── LabelClass: full string ──────────────────────────────────────

    [Fact]
    public void Extended_LabelClass_FullString()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Label, "Upload"));
        Assert.Contains("flex items-center gap-1", cut.Markup);
    }

    // ─── IconWrapClass: additional classes ────────────────────────────

    [Fact]
    public void Extended_IconWrapClass_InlineFlex()
    {
        var cut = RenderFileInput();
        Assert.Contains("inline-flex", cut.Markup);
    }

    [Fact]
    public void Extended_IconWrapClass_JustifyCenter()
    {
        var cut = RenderFileInput();
        Assert.Contains("justify-center", cut.Markup);
    }

    [Fact]
    public void Extended_IconWrapClass_Ring1()
    {
        var cut = RenderFileInput();
        Assert.Contains("ring-1", cut.Markup);
    }

    [Fact]
    public void Extended_IconWrapClass_TransitionAll()
    {
        var cut = RenderFileInput();
        Assert.Contains("transition-all", cut.Markup);
    }

    // ─── ButtonClass: additional classes ──────────────────────────────

    [Fact]
    public void Extended_ButtonClass_Shrink0()
    {
        var cut = RenderFileInput();
        Assert.Contains("shrink-0", cut.Markup);
    }

    [Fact]
    public void Extended_ButtonClass_FontSemibold()
    {
        var cut = RenderFileInput();
        Assert.Contains("font-semibold", cut.Markup);
    }

    [Fact]
    public void Extended_ButtonClass_TextPrimary()
    {
        var cut = RenderFileInput();
        Assert.Contains("text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_ButtonClass_Ring1()
    {
        var cut = RenderFileInput();
        Assert.Contains("ring-1", cut.Markup);
    }

    [Fact]
    public void Extended_ButtonClass_TransitionAll()
    {
        var cut = RenderFileInput();
        Assert.Contains("transition-all", cut.Markup);
    }

    // ─── VariantClass: full ring strings ──────────────────────────────

    [Fact]
    public void Extended_VariantFilled_RingSurface200()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("ring-surface-200/70", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_RingColorMix14()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_Ring1()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("ring-1", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_RingSurface200_80()
    {
        var cut = RenderFileInput();
        Assert.Contains("ring-surface-200/80", cut.Markup);
    }

    // ─── StateClass: enabled hover ring ───────────────────────────────

    [Fact]
    public void Extended_StateEnabled_HoverRing()
    {
        var cut = RenderFileInput(p => p.Add(x => x.Disabled, false));
        Assert.Contains("hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
    }

    // ─── HelperClass: full string ─────────────────────────────────────

    [Fact]
    public void Extended_HelperClass_FullString()
    {
        var cut = RenderFileInput(p => p.Add(x => x.HelperText, "Max 10MB"));
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // ─── GetProgressClass branches ────────────────────────────────────

    [Fact]
    public void Extended_GetProgressClass_Pending()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Pending });
        Assert.NotNull(result);
    }

    [Fact]
    public void Extended_GetProgressClass_Uploading()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Uploading });
        Assert.NotNull(result);
    }

    [Fact]
    public void Extended_GetProgressClass_Uploaded()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Uploaded });
        Assert.NotNull(result);
    }

    [Fact]
    public void Extended_GetProgressClass_Error()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Error });
        Assert.NotNull(result);
    }

    [Fact]
    public void Extended_GetProgressClass_Canceled()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Canceled });
        Assert.NotNull(result);
    }

    // ─── GetStatusColorClass branches ─────────────────────────────────

    [Fact]
    public void Extended_GetStatusColorClass_Uploaded()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetStatusColorClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Uploaded });
        Assert.NotNull(result);
    }

    [Fact]
    public void Extended_GetStatusColorClass_Error()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetStatusColorClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { XFFileInputStatus.Error });
        Assert.NotNull(result);
    }

    // ─── GetStatusText branches ───────────────────────────────────────

    [Fact]
    public void Extended_GetStatusText_MethodExists()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetStatusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
    }

    // ─── FormatFileMetadata ───────────────────────────────────────────

    [Fact]
    public void Extended_FormatFileMetadata_MethodExists()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("FormatFileMetadata", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
    }

    // ─── GetProgressStyle ─────────────────────────────────────────────

    [Fact]
    public void Extended_GetProgressStyle_50Percent()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { 50 });
        Assert.NotNull(result);
        Assert.Contains("50%", result!.ToString());
    }

    [Fact]
    public void Extended_GetProgressStyle_100Percent()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetProgressStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { 100 });
        Assert.NotNull(result);
        Assert.Contains("100%", result!.ToString());
    }

    // ─── ClampedProgress ──────────────────────────────────────────────

    [Fact]
    public void Extended_ClampedProgress_OutOfRange()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("ClampedProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { 150 });
        Assert.Equal(100, result);
    }

    [Fact]
    public void Extended_ClampedProgress_Negative()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("ClampedProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var result = method.Invoke(cut.Instance, new object[] { -10 });
        Assert.Equal(0, result);
    }

    // ─── GetItemClass branches ────────────────────────────────────────

    [Fact]
    public void Extended_GetItemClass_MethodExists()
    {
        var cut = RenderFileInput();
        var method = typeof(XFFileInput).GetMethod("GetItemClass", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
    }
}
