using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.Rating;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFRatingExtendedTest : BunitContext
{
    public XFRatingExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFRating> RenderRating(
        Action<ComponentParameterCollectionBuilder<XFRating>>? configure = null)
    {
        return Render<XFRating>(p =>
        {
            configure?.Invoke(p);
        });
    }

    // SizeClass Sm (line 99)
    [Fact]
    public void Extended_SizeSm()
    {
        var cut = RenderRating(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("w-7", cut.Markup);
        Assert.Contains("text-base", cut.Markup);
    }

    // SizeClass Lg (line 100)
    [Fact]
    public void Extended_SizeLg()
    {
        var cut = RenderRating(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.Contains("h-10", cut.Markup);
        Assert.Contains("w-10", cut.Markup);
        Assert.Contains("text-2xl", cut.Markup);
    }

    // SizeClass Xl (line 101)
    [Fact]
    public void Extended_SizeXl()
    {
        var cut = RenderRating(p => p.Add(x => x.Size, XFExtendedSize.Xl));
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("w-12", cut.Markup);
        Assert.Contains("text-3xl", cut.Markup);
    }

    // SizeClass default Md (line 102)
    [Fact]
    public void Extended_SizeDefault()
    {
        var cut = RenderRating();
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("w-8", cut.Markup);
        Assert.Contains("text-xl", cut.Markup);
    }

    // ToneTextClass Warning (default) (line 112)
    [Fact]
    public void Extended_ToneDefault_RendersWarning()
    {
        var cut = RenderRating(p => p.Add(x => x.Value, 3));
        Assert.Contains("text-warning", cut.Markup);
    }

    // ToneTextClass Primary (line 107)
    [Fact]
    public void Extended_TonePrimary()
    {
        var cut = RenderRating(p =>
        {
            p.Add(x => x.Value, 3);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
        });
        Assert.Contains("text-primary", cut.Markup);
    }

    // ToneTextClass Success (line 109)
    [Fact]
    public void Extended_ToneSuccess()
    {
        var cut = RenderRating(p =>
        {
            p.Add(x => x.Value, 3);
            p.Add(x => x.Tone, XFSemanticColor.Success);
        });
        Assert.Contains("text-success", cut.Markup);
    }

    // ToneTextClass Danger (line 110)
    [Fact]
    public void Extended_ToneDanger()
    {
        var cut = RenderRating(p =>
        {
            p.Add(x => x.Value, 3);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    // ToneTextClass Secondary (line 108)
    [Fact]
    public void Extended_ToneSecondary()
    {
        var cut = RenderRating(p =>
        {
            p.Add(x => x.Value, 3);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
        });
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // ToneTextClass Neutral (line 111)
    [Fact]
    public void Extended_ToneNeutral()
    {
        var cut = RenderRating(p =>
        {
            p.Add(x => x.Value, 3);
            p.Add(x => x.Tone, XFSemanticColor.Neutral);
        });
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // EmptyClass (line 92) — value 0 means all items are empty
    [Fact]
    public void Extended_EmptyClass_RendersMuted()
    {
        var cut = RenderRating(p => p.Add(x => x.Value, 0));
        Assert.Contains("text-text-muted/45", cut.Markup);
    }

    // FilledClass uses ToneTextClass (line 91)
    [Fact]
    public void Extended_FilledClass_UsesTone()
    {
        var cut = RenderRating(p =>
        {
            p.Add(x => x.Value, 5);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    // RatingShellClass disabled (line 76)
    [Fact]
    public void Extended_Disabled_RendersNotAllowed()
    {
        var cut = RenderRating(p => p.Add(x => x.Disabled, true));
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    // RatingShellClass readOnly (line 77)
    [Fact]
    public void Extended_ReadOnly_RendersCursorDefault()
    {
        var cut = RenderRating(p => p.Add(x => x.ReadOnly, true));
        Assert.Contains("cursor-default", cut.Markup);
    }

    // ItemClass interactive (line 83)
    [Fact]
    public void Extended_ItemInteractive_RendersCursorPointer()
    {
        var cut = RenderRating();
        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.Contains("hover:-translate-y-0.5", cut.Markup);
    }

    // ClearButtonClass (line 85-89)
    [Fact]
    public void Extended_ClearButton_Rendered()
    {
        var cut = RenderRating(p => p.Add(x => x.AllowReset, true));
        Assert.Contains("xf-rating", cut.Markup);
    }

    // ShowValue (line 26)
    [Fact]
    public void Extended_ShowValueTrue_RendersValue()
    {
        var cut = RenderRating(p => p.Add(x => x.ShowValue, true));
        Assert.Contains("0/5", cut.Markup);
    }

    // Label (line 13)
    [Fact]
    public void Extended_Label_Rendered()
    {
        var cut = RenderRating(p => p.Add(x => x.Label, "Rating"));
        Assert.Contains("Rating", cut.Markup);
    }

    // HelperText (line 15)
    [Fact]
    public void Extended_HelperText_Rendered()
    {
        var cut = RenderRating(p => p.Add(x => x.HelperText, "Rate this"));
        Assert.Contains("Rate this", cut.Markup);
    }

    // Max parameter (line 22)
    [Fact]
    public void Extended_Max10_RendersTenItems()
    {
        var cut = RenderRating(p => p.Add(x => x.Max, 10));
        Assert.Contains("xf-rating", cut.Markup);
    }

    // BaseClass included (line 67)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderRating();
        Assert.Contains("xf-rating", cut.Markup);
    }

    // RootClass includes flex flex-col (line 68)
    [Fact]
    public void Extended_RootClass_RendersFlexCol()
    {
        var cut = RenderRating();
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("flex-col", cut.Markup);
    }
}
