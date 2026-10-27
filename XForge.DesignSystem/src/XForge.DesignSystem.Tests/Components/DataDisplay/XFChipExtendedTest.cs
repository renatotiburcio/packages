using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFChipExtendedTest : BunitContext
{
    public XFChipExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFChip> RenderChip(
        Action<ComponentParameterCollectionBuilder<XFChip>>? configure = null)
    {
        return Render<XFChip>(p =>
        {
            configure?.Invoke(p);
        });
    }

    // ========================================================================
    // BaseClass (line 11)
    // ========================================================================

    [Fact]
    public void Extended_BaseClass_RendersXfChip()
    {
        var cut = RenderChip(p => p.AddChildContent("label"));
        Assert.Contains("xf-chip", cut.Markup);
    }

    [Fact]
    public void Extended_BaseClass_RendersInlineFlex()
    {
        var cut = RenderChip(p => p.AddChildContent("label"));
        Assert.Contains("inline-flex", cut.Markup);
    }

    [Fact]
    public void Extended_BaseClass_RendersItemsCenter()
    {
        var cut = RenderChip(p => p.AddChildContent("label"));
        Assert.Contains("items-center", cut.Markup);
    }

    [Fact]
    public void Extended_BaseClass_RendersRoundedFull()
    {
        var cut = RenderChip(p => p.AddChildContent("label"));
        Assert.Contains("rounded-full", cut.Markup);
    }

    [Fact]
    public void Extended_BaseClass_RendersFontMedium()
    {
        var cut = RenderChip(p => p.AddChildContent("label"));
        Assert.Contains("font-medium", cut.Markup);
    }

    [Fact]
    public void Extended_BaseClass_RendersGap1()
    {
        var cut = RenderChip(p => p.AddChildContent("label"));
        Assert.Contains("gap-1", cut.Markup);
    }

    // ========================================================================
    // SizeClass — Sm (line 70)
    // ========================================================================

    [Fact]
    public void Extended_SizeClassSm_RendersH6Px2TextXs()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.AddChildContent("Sm");
        });
        Assert.Contains("h-6", cut.Markup);
        Assert.Contains("px-2", cut.Markup);
        Assert.Contains("py-0.5", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    // ========================================================================
    // SizeClass — Md default (line 72)
    // ========================================================================

    [Fact]
    public void Extended_SizeClassMd_RendersH7Px3TextXs()
    {
        var cut = RenderChip(p => p.AddChildContent("Md"));
        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("px-3", cut.Markup);
        Assert.Contains("py-1", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    // ========================================================================
    // SizeClass — Lg (line 71)
    // ========================================================================

    [Fact]
    public void Extended_SizeClassLg_RendersH9Px4TextSm()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.AddChildContent("Lg");
        });
        Assert.Contains("h-9", cut.Markup);
        Assert.Contains("px-4", cut.Markup);
        Assert.Contains("py-1.5", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    // ========================================================================
    // VariantClass — Solid + Primary (line 82)
    // ========================================================================

    [Fact]
    public void Extended_VariantClassSolid_Primary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
            p.AddChildContent("SolidPrimary");
        });
        Assert.Contains("bg-primary", cut.Markup);
        Assert.Contains("text-primary-contrast", cut.Markup);
    }

    // ========================================================================
    // VariantClass — Outline + Primary (line 79)
    // ========================================================================

    [Fact]
    public void Extended_VariantClassOutline_Primary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
            p.AddChildContent("OutlinePrimary");
        });
        Assert.Contains("border", cut.Markup);
        Assert.Contains("border-primary", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    // ========================================================================
    // VariantClass — Soft + Primary (line 80)
    // ========================================================================

    [Fact]
    public void Extended_VariantClassSoft_Primary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
            p.AddChildContent("SoftPrimary");
        });
        Assert.Contains("text-primary", cut.Markup);
    }

    // ========================================================================
    // VariantClass — Ghost + Primary (line 81)
    // ========================================================================

    [Fact]
    public void Extended_VariantClassGhost_Primary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
            p.AddChildContent("GhostPrimary");
        });
        Assert.Contains("text-primary", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Solid + Secondary (line 89)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidSecondary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.AddChildContent("Secondary");
        });
        Assert.Contains("bg-secondary", cut.Markup);
        Assert.Contains("text-secondary-contrast", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Outline + Secondary (line 86)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_OutlineSecondary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.AddChildContent("Secondary");
        });
        Assert.Contains("border-secondary", cut.Markup);
        Assert.Contains("text-secondary", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Solid + Success (line 96)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidSuccess()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.AddChildContent("Success");
        });
        Assert.Contains("bg-success", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Outline + Success (line 93)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_OutlineSuccess()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.AddChildContent("Success");
        });
        Assert.Contains("border-success", cut.Markup);
        Assert.Contains("text-success", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Soft + Success (line 94)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SoftSuccess()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.AddChildContent("Success");
        });
        Assert.Contains("text-success", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Ghost + Success (line 95)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_GhostSuccess()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.AddChildContent("Success");
        });
        Assert.Contains("text-success", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Solid + Warning (line 103)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidWarning()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.AddChildContent("Warning");
        });
        Assert.Contains("bg-warning", cut.Markup);
        Assert.Contains("text-warning-contrast", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Outline + Warning (line 100)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_OutlineWarning()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.AddChildContent("Warning");
        });
        Assert.Contains("border-warning", cut.Markup);
        Assert.Contains("text-warning", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Solid + Danger (line 110)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidDanger()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.AddChildContent("Danger");
        });
        Assert.Contains("bg-danger", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Outline + Danger (line 107)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_OutlineDanger()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.AddChildContent("Danger");
        });
        Assert.Contains("border-danger", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Ghost + Danger (line 109)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_GhostDanger()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.AddChildContent("Danger");
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Solid + Info (line 117)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidInfo()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Info);
            p.AddChildContent("Info");
        });
        Assert.Contains("bg-info", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Outline + Info (line 114)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_OutlineInfo()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Info);
            p.AddChildContent("Info");
        });
        Assert.Contains("border-info", cut.Markup);
        Assert.Contains("text-info", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Neutral (falls to _/Default branch, line 119-125)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidNeutral()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Neutral);
            p.AddChildContent("Neutral");
        });
        Assert.Contains("bg-surface-200", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Outline + Neutral (line 121)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_OutlineNeutral()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Neutral);
            p.AddChildContent("Neutral");
        });
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Soft + Neutral (line 122)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SoftNeutral()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Neutral);
            p.AddChildContent("Neutral");
        });
        Assert.Contains("bg-surface-100", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Ghost + Neutral (line 123)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_GhostNeutral()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Neutral);
            p.AddChildContent("Neutral");
        });
        Assert.Contains("text-text-primary", cut.Markup);
        Assert.Contains("hover:bg-surface-100", cut.Markup);
    }

    // ========================================================================
    // ToneClass — Default (XFSemanticColor.Default, same _ branch)
    // ========================================================================

    [Fact]
    public void Extended_ToneClass_SolidDefaultTone()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Solid);
            p.Add(x => x.Tone, XFSemanticColor.Default);
            p.AddChildContent("DefaultTone");
        });
        Assert.Contains("bg-surface-200", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_ToneClass_OutlineDefaultTone()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Default);
            p.AddChildContent("DefaultTone");
        });
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_ToneClass_SoftDefaultTone()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Default);
            p.AddChildContent("DefaultTone");
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_ToneClass_GhostDefaultTone()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Default);
            p.AddChildContent("DefaultTone");
        });
        Assert.Contains("text-text-primary", cut.Markup);
        Assert.Contains("hover:bg-surface-100", cut.Markup);
    }

    // ========================================================================
    // StateClass — Disabled (line 39)
    // ========================================================================

    [Fact]
    public void Extended_StateClass_Disabled_RendersCursorNotAllowed()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent("Dis");
        });
        Assert.Contains("cursor-not-allowed", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Disabled_RendersOpacity60()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent("Dis");
        });
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Disabled_RendersAriaDisabled()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent("Dis");
        });
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    // ========================================================================
    // StateClass — Selected (line 40, 128)
    // ========================================================================

    [Fact]
    public void Extended_StateClass_Selected_RendersRing()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Selected, true);
            p.AddChildContent("Sel");
        });
        Assert.Contains("ring-2", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Selected_RendersRingPrimary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Selected, true);
            p.AddChildContent("Sel");
        });
        Assert.Contains("ring-primary", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Selected_RendersRingOffset1()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Selected, true);
            p.AddChildContent("Sel");
        });
        Assert.Contains("ring-offset-1", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_Selected_RendersRingOffsetBgSurface()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Selected, true);
            p.AddChildContent("Sel");
        });
        Assert.Contains("ring-offset-bg-surface", cut.Markup);
    }

    [Fact]
    public void Extended_StateClass_NotSelected_NoRing()
    {
        var cut = RenderChip(p => p.AddChildContent("NoSel"));
        Assert.DoesNotContain("ring-2", cut.Markup);
    }

    // ========================================================================
    // RootClass — Clickable override (line 39)
    // ========================================================================

    [Fact]
    public void Extended_RootClass_Clickable_RendersCursorPointer()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.AddChildContent("Click");
        });
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_Clickable_RendersRoleButton()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.AddChildContent("Click");
        });
        Assert.Contains("button", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_Clickable_RendersTabindex()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.AddChildContent("Click");
        });
        Assert.Contains("tabindex", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_NotClickable_NoRoleButton()
    {
        var cut = RenderChip(p => p.AddChildContent("NoClick"));
        Assert.DoesNotContain("role=\"button\"", cut.Markup);
    }

    // ========================================================================
    // RootClass — Default cursor (line 39)
    // ========================================================================

    [Fact]
    public void Extended_RootClass_DefaultCursor()
    {
        var cut = RenderChip(p => p.AddChildContent("Default"));
        Assert.Contains("cursor-default", cut.Markup);
    }

    // ========================================================================
    // CloseButtonClass / Dismissible (line 57-66, 33-43)
    // ========================================================================

    [Fact]
    public void Extended_CloseButtonClass_Dismissible_RendersButton()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        var button = cut.Find("button");
        Assert.NotNull(button);
    }

    [Fact]
    public void Extended_CloseButtonClass_Dismissible_RendersAriaLabel()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        Assert.Contains("aria-label=\"Remover\"", cut.Markup);
    }

    [Fact]
    public void Extended_CloseButtonClass_Dismissible_RendersDismissLabel()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.Add(x => x.DismissLabel, "Remove tag");
            p.AddChildContent("Close");
        });
        Assert.Contains("aria-label=\"Remove tag\"", cut.Markup);
    }

    [Fact]
    public void Extended_CloseButtonClass_NoDismissible_NoButton()
    {
        var cut = RenderChip(p => p.AddChildContent("NoClose"));
        Assert.Throws<ElementNotFoundException>(() => cut.Find("button"));
    }

    [Fact]
    public void Extended_CloseButtonClass_DismissButton_RendersShrink0()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        Assert.Contains("shrink-0", cut.Markup);
    }

    [Fact]
    public void Extended_CloseButtonClass_DismissButton_RendersInlineFlex()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        Assert.Contains("inline-flex", cut.Markup);
    }

    // ========================================================================
    // CloseButtonClass — Size Sm (line 63)
    // ========================================================================

    [Fact]
    public void Extended_CloseButtonClass_SizeSm()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        Assert.Contains("h-3.5 w-3.5", cut.Markup);
    }

    // ========================================================================
    // CloseButtonClass — Size Md (line 65)
    // ========================================================================

    [Fact]
    public void Extended_CloseButtonClass_SizeMd()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    // ========================================================================
    // CloseButtonClass — Size Lg (line 64)
    // ========================================================================

    [Fact]
    public void Extended_CloseButtonClass_SizeLg()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Close");
        });
        Assert.Contains("h-5 w-5", cut.Markup);
    }

    // ========================================================================
    // IconClass (line 43-48)
    // ========================================================================

    [Fact]
    public void Extended_IconClass_Sm()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "ic"));
            p.AddChildContent("IconSm");
        });
        Assert.Contains("h-3 w-3", cut.Markup);
        Assert.Contains("shrink-0", cut.Markup);
    }

    [Fact]
    public void Extended_IconClass_Md()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "ic"));
            p.AddChildContent("IconMd");
        });
        Assert.Contains("h-4 w-4", cut.Markup);
        Assert.Contains("shrink-0", cut.Markup);
    }

    [Fact]
    public void Extended_IconClass_Lg()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "ic"));
            p.AddChildContent("IconLg");
        });
        Assert.Contains("h-5 w-5", cut.Markup);
        Assert.Contains("shrink-0", cut.Markup);
    }

    // ========================================================================
    // IconClass — aria-hidden (line 18, 30)
    // ========================================================================

    [Fact]
    public void Extended_IconClass_StartIcon_AriaHidden()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "ic"));
            p.AddChildContent("IconA11y");
        });
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_IconClass_EndIcon_AriaHidden()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.EndIcon, builder => builder.AddContent(0, "ic"));
            p.AddChildContent("IconA11y");
        });
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    // ========================================================================
    // AvatarClass (line 50-55)
    // ========================================================================

    [Fact]
    public void Extended_AvatarClass_Sm()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.Avatar, builder => builder.AddContent(0, "av"));
            p.AddChildContent("AvSm");
        });
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void Extended_AvatarClass_Md()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Avatar, builder => builder.AddContent(0, "av"));
            p.AddChildContent("AvMd");
        });
        Assert.Contains("h-5 w-5", cut.Markup);
    }

    [Fact]
    public void Extended_AvatarClass_Lg()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.Avatar, builder => builder.AddContent(0, "av"));
            p.AddChildContent("AvLg");
        });
        Assert.Contains("h-6 w-6", cut.Markup);
    }

    [Fact]
    public void Extended_AvatarClass_RenderedInMarkup()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Avatar, builder => builder.AddContent(0, "avatar-data"));
            p.AddChildContent("AvExists");
        });
        Assert.Contains("avatar-data", cut.Markup);
    }

    // ========================================================================
    // Label / ChildContent (line 26)
    // ========================================================================

    [Fact]
    public void Extended_Label_RendersTextContent()
    {
        var cut = RenderChip(p => p.AddChildContent("Test Label"));
        Assert.Contains("Test Label", cut.Markup);
    }

    [Fact]
    public void Extended_Label_RendersInTruncateSpan()
    {
        var cut = RenderChip(p => p.AddChildContent("Truncated"));
        Assert.Contains("truncate", cut.Markup);
    }

    [Fact]
    public void Extended_Label_RendersMinW0()
    {
        var cut = RenderChip(p => p.AddChildContent("MinW"));
        Assert.Contains("min-w-0", cut.Markup);
    }

    // ========================================================================
    // Icon — StartIcon (line 16-19)
    // ========================================================================

    [Fact]
    public void Extended_Icon_StartIcon_Rendered()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "★"));
            p.AddChildContent("Start");
        });
        Assert.Contains("★", cut.Markup);
    }

    // ========================================================================
    // Icon — EndIcon (line 28-31)
    // ========================================================================

    [Fact]
    public void Extended_Icon_EndIcon_Rendered()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.EndIcon, builder => builder.AddContent(0, "→"));
            p.AddChildContent("End");
        });
        Assert.Contains("→", cut.Markup);
    }

    // ========================================================================
    // Closable / Dismissible event (line 164-175)
    // ========================================================================

    [Fact]
    public async Task Extended_Close_Dismissible_FiresOnDismiss()
    {
        var dismissed = false;
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.Add(x => x.OnDismiss, EventCallback.Factory.Create(this, () => dismissed = true));
            p.AddChildContent("Close");
        });

        var button = cut.Find("button");
        await button.ClickAsync();
        Assert.True(dismissed);
    }

    [Fact]
    public async Task Extended_Close_Disabled_DoesNotFireOnDismiss()
    {
        var dismissed = false;
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.Add(x => x.Disabled, true);
            p.Add(x => x.OnDismiss, EventCallback.Factory.Create(this, () => dismissed = true));
            p.AddChildContent("Close");
        });

        var button = cut.Find("button");
        await button.ClickAsync();
        Assert.False(dismissed);
    }

    // ========================================================================
    // Clickable / OnClick event (line 135-146)
    // ========================================================================

    [Fact]
    public async Task Extended_Clickable_Click_FiresOnClick()
    {
        var clicked = false;
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true));
            p.AddChildContent("Click");
        });

        await cut.Find(".xf-chip").ClickAsync();
        Assert.True(clicked);
    }

    [Fact]
    public async Task Extended_Clickable_Disabled_DoesNotFireOnClick()
    {
        var clicked = false;
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.Add(x => x.Disabled, true);
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true));
            p.AddChildContent("Click");
        });

        await cut.Find(".xf-chip").ClickAsync();
        Assert.False(clicked);
    }

    // ========================================================================
    // Keyboard — Enter key (line 155-161)
    // ========================================================================

    [Fact]
    public async Task Extended_Clickable_EnterKey_FiresOnClick()
    {
        var clicked = false;
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true));
            p.AddChildContent("Key");
        });

        await cut.Find(".xf-chip").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.True(clicked);
    }

    [Fact]
    public async Task Extended_Clickable_SpaceKey_FiresOnClick()
    {
        var clicked = false;
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true));
            p.AddChildContent("Key");
        });

        await cut.Find(".xf-chip").KeyDownAsync(new KeyboardEventArgs { Key = " " });
        Assert.True(clicked);
    }

    // ========================================================================
    // Visible (line 3)
    // ========================================================================

    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Visible, false);
            p.AddChildContent("Hidden");
        });
        Assert.Equal(string.Empty, cut.Markup);
    }

    [Fact]
    public void Extended_VisibleTrue_RendersContent()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Visible, true);
            p.AddChildContent("Visible");
        });
        Assert.Contains("Visible", cut.Markup);
    }

    // ========================================================================
    // AriaLabel (line 11)
    // ========================================================================

    [Fact]
    public void Extended_AriaLabel_Rendered()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.AriaLabel, "Filter tag");
            p.AddChildContent("A11y");
        });
        Assert.Contains("aria-label=\"Filter tag\"", cut.Markup);
    }

    // ========================================================================
    // Disabled dismiss button (line 38)
    // ========================================================================

    [Fact]
    public void Extended_DisabledDismissButton_RendersDisabledAttribute()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.Add(x => x.Disabled, true);
            p.AddChildContent("DisClose");
        });
        Assert.Contains("disabled", cut.Markup);
    }

    // ========================================================================
    // Transition classes (line 36)
    // ========================================================================

    [Fact]
    public void Extended_Transition_RendersTransitionAll()
    {
        var cut = RenderChip(p => p.AddChildContent("Trans"));
        Assert.Contains("transition-all", cut.Markup);
    }

    [Fact]
    public void Extended_Transition_RendersDuration150()
    {
        var cut = RenderChip(p => p.AddChildContent("Trans"));
        Assert.Contains("duration-150", cut.Markup);
    }

    // ========================================================================
    // Outline variant — all tones covered above; extra verification
    // ========================================================================

    [Fact]
    public void Extended_OutlineVariant_AllTones_HaveBorder()
    {
        var tones = new[]
        {
            XFSemanticColor.Primary,
            XFSemanticColor.Secondary,
            XFSemanticColor.Success,
            XFSemanticColor.Warning,
            XFSemanticColor.Danger,
            XFSemanticColor.Info,
            XFSemanticColor.Neutral,
            XFSemanticColor.Default
        };

        foreach (var tone in tones)
        {
            var cut = RenderChip(p =>
            {
                p.Add(x => x.Variant, XFChipVariant.Outline);
                p.Add(x => x.Tone, tone);
                p.AddChildContent("Outline");
            });
            Assert.Contains("border", cut.Markup);
        }
    }

    // ========================================================================
    // Soft variant — all tones
    // ========================================================================

    [Fact]
    public void Extended_SoftVariant_AllTones_HaveBgMix()
    {
        var tones = new[]
        {
            XFSemanticColor.Primary,
            XFSemanticColor.Secondary,
            XFSemanticColor.Success,
            XFSemanticColor.Warning,
            XFSemanticColor.Danger,
            XFSemanticColor.Info
        };

        foreach (var tone in tones)
        {
            var cut = RenderChip(p =>
            {
                p.Add(x => x.Variant, XFChipVariant.Soft);
                p.Add(x => x.Tone, tone);
                p.AddChildContent("Soft");
            });
            Assert.Contains("color-mix", cut.Markup);
        }
    }

    // ========================================================================
    // Ghost variant — all tones
    // ========================================================================

    [Fact]
    public void Extended_GhostVariant_AllTones_HaveHover()
    {
        var tones = new[]
        {
            XFSemanticColor.Primary,
            XFSemanticColor.Secondary,
            XFSemanticColor.Success,
            XFSemanticColor.Warning,
            XFSemanticColor.Danger,
            XFSemanticColor.Info,
            XFSemanticColor.Neutral,
            XFSemanticColor.Default
        };

        foreach (var tone in tones)
        {
            var cut = RenderChip(p =>
            {
                p.Add(x => x.Variant, XFChipVariant.Ghost);
                p.Add(x => x.Tone, tone);
                p.AddChildContent("Ghost");
            });
            Assert.Contains("hover:", cut.Markup);
        }
    }

    // ========================================================================
    // Solid variant — all tones
    // ========================================================================

    [Fact]
    public void Extended_SolidVariant_AllTones_HaveBg()
    {
        var tones = new[]
        {
            XFSemanticColor.Primary,
            XFSemanticColor.Secondary,
            XFSemanticColor.Success,
            XFSemanticColor.Warning,
            XFSemanticColor.Danger,
            XFSemanticColor.Info,
            XFSemanticColor.Neutral,
            XFSemanticColor.Default
        };

        foreach (var tone in tones)
        {
            var cut = RenderChip(p =>
            {
                p.Add(x => x.Variant, XFChipVariant.Solid);
                p.Add(x => x.Tone, tone);
                p.AddChildContent("Solid");
            });
            Assert.Contains("bg-", cut.Markup);
        }
    }

    // ========================================================================
    // DismissButtonClass — focus-visible ring (line 59)
    // ========================================================================

    [Fact]
    public void Extended_DismissButtonClass_FocusVisibleRing()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Focus");
        });
        Assert.Contains("focus-visible:ring-2", cut.Markup);
    }

    [Fact]
    public void Extended_DismissButtonClass_DisabledPointerEventsNone()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Focus");
        });
        Assert.Contains("disabled:pointer-events-none", cut.Markup);
    }

    [Fact]
    public void Extended_DismissButtonClass_DisabledOpacity60()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.AddChildContent("Focus");
        });
        Assert.Contains("disabled:opacity-60", cut.Markup);
    }

    // ========================================================================
    // Id attribute (line 5)
    // ========================================================================

    [Fact]
    public void Extended_Id_RenderedOnElement()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Id, "my-chip");
            p.AddChildContent("Id");
        });
        Assert.Contains("id=\"my-chip\"", cut.Markup);
    }

    // ========================================================================
    // Style attribute (line 7)
    // ========================================================================

    [Fact]
    public void Extended_Style_RenderedOnElement()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Style, "color: red");
            p.AddChildContent("Style");
        });
        Assert.Contains("color: red", cut.Markup);
    }

    // ========================================================================
    // Additional variant+tone combinations for completeness
    // ========================================================================

    [Fact]
    public void Extended_VariantSoft_Secondary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.AddChildContent("SoftSec");
        });
        Assert.Contains("text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_Secondary()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.AddChildContent("GhostSec");
        });
        Assert.Contains("text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_Warning()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.AddChildContent("SoftWarn");
        });
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_Warning()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.AddChildContent("GhostWarn");
        });
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_Danger()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.AddChildContent("SoftDang");
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_Info()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Soft);
            p.Add(x => x.Tone, XFSemanticColor.Info);
            p.AddChildContent("SoftInfo");
        });
        Assert.Contains("text-info", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_Info()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Ghost);
            p.Add(x => x.Tone, XFSemanticColor.Info);
            p.AddChildContent("GhostInfo");
        });
        Assert.Contains("text-info", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_Success()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.AddChildContent("OutSucc");
        });
        Assert.Contains("border-success", cut.Markup);
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_Warning()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.AddChildContent("OutWarn");
        });
        Assert.Contains("border-warning", cut.Markup);
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_Info()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Info);
            p.AddChildContent("OutInfo");
        });
        Assert.Contains("border-info", cut.Markup);
        Assert.Contains("text-info", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_Danger()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.AddChildContent("OutDang");
        });
        Assert.Contains("border-danger", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_Neutral()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Neutral);
            p.AddChildContent("OutNeut");
        });
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // ========================================================================
    // Combined states
    // ========================================================================

    [Fact]
    public void Extended_SelectedAndDisabled_RendersBoth()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Selected, true);
            p.Add(x => x.Disabled, true);
            p.AddChildContent("SelDis");
        });
        Assert.Contains("ring-2", cut.Markup);
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_ClickableAndDisabled_RendersNotAllowed()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Clickable, true);
            p.Add(x => x.Disabled, true);
            p.AddChildContent("ClickDis");
        });
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.DoesNotContain("cursor-pointer", cut.Markup);
    }

    [Fact]
    public void Extended_SelectedAndClickable_RendersBoth()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Selected, true);
            p.Add(x => x.Clickable, true);
            p.AddChildContent("SelClick");
        });
        Assert.Contains("ring-2", cut.Markup);
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    [Fact]
    public void Extended_DismissibleAndClickable_RendersBoth()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Dismissible, true);
            p.Add(x => x.Clickable, true);
            p.AddChildContent("DisClick");
        });
        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void Extended_AllFeaturesCombined()
    {
        var cut = RenderChip(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.Variant, XFChipVariant.Outline);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
            p.Add(x => x.Selected, true);
            p.Add(x => x.Dismissible, true);
            p.Add(x => x.Clickable, true);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon"));
            p.Add(x => x.Avatar, builder => builder.AddContent(0, "av"));
            p.AddChildContent("All");
        });
        Assert.Contains("xf-chip", cut.Markup);
        Assert.Contains("h-9", cut.Markup);
        Assert.Contains("border-primary", cut.Markup);
        Assert.Contains("ring-2", cut.Markup);
        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.Contains("icon", cut.Markup);
        Assert.Contains("av", cut.Markup);
        Assert.Contains("All", cut.Markup);
        Assert.NotNull(cut.Find("button"));
    }
}
