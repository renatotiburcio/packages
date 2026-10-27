using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Button;

public class XFButtonExtendedTest : BunitContext
{
    public XFButtonExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void SizeXs_RendersExtraSmall()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xs);
        });
        Assert.Contains("h-7", cut.Markup);
    }

    [Fact]
    public void SizeLg_RendersLarge()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Lg);
        });
        Assert.Contains("h-11", cut.Markup);
    }

    [Fact]
    public void SizeXl_RendersExtraLarge()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xl);
        });
        Assert.Contains("h-12", cut.Markup);
    }

    [Fact]
    public void VariantOutline_RendersOutline()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    [Fact]
    public void VariantGhost_RendersGhost()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Variant, XFButtonVariant.Ghost);
        });
        Assert.Contains("bg-transparent", cut.Markup);
    }

    [Fact]
    public void VariantSoft_RendersSoft()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void VariantSubtle_RendersSubtle()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Variant, XFButtonVariant.Subtle);
        });
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    [Fact]
    public void VariantLink_RendersLink()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Variant, XFButtonVariant.Link);
        });
        Assert.Contains("underline-offset-4", cut.Markup);
    }

    [Fact]
    public void Active_RendersPressedState()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Active, true);
        });
        Assert.Contains("ring-2", cut.Markup);
    }

    [Fact]
    public void Pressed_RendersPressedState()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Pressed, true);
        });
        Assert.Contains("ring-2", cut.Markup);
    }

    [Fact]
    public void FormTargetInvalid_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Render<XFButton>(p =>
        {
            p.Add(x => x.FormTarget, "invalid");
        }));
        Assert.Contains("FormTarget", ex.Message);
    }

    [Fact]
    public void FormTargetSelf_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FormTarget, "_self");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void FormTargetBlank_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FormTarget, "_blank");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void Icon_SetsStartIcon()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Icon, "check");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void StartIcon_OverridesIcon()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Icon, "check");
            p.Add(x => x.StartIcon, "star");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void EndIcon_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.EndIcon, "arrow-right");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void Loading_RendersLoadingState()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Loading, true);
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void TypeSubmit_RendersSubmitType()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Type, XFButtonType.Submit);
        });
        var button = cut.Find("button");
        Assert.Equal("submit", button.GetAttribute("type"));
    }

    [Fact]
    public void TypeReset_RendersResetType()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Type, XFButtonType.Reset);
        });
        var button = cut.Find("button");
        Assert.Equal("reset", button.GetAttribute("type"));
    }

    [Fact]
    public void FullWidth_RendersFullWidth()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FullWidth, true);
        });
        Assert.Contains("w-full", cut.Markup);
    }

    [Fact]
    public void FullWidthFalse_RendersAutoWidth()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FullWidth, false);
        });
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    [Fact]
    public void OnClick_CallbackFires()
    {
        var clicked = false;
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Disabled_PreventsOnClick()
    {
        var clicked = false;
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });
        cut.Find("button").Click();
        Assert.False(clicked);
    }

    [Fact]
    public void ChildContent_RendersCustomContent()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Custom")));
        });
        Assert.Contains("Custom", cut.Markup);
    }

    [Fact]
    public void Text_RendersTextContent()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Text, "Click Me");
        });
        Assert.Contains("Click Me", cut.Markup);
    }

    [Fact]
    public void Title_RendersTitleAttribute()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Title, "Button title");
        });
        Assert.Contains("Button title", cut.Markup);
    }

    [Fact]
    public void LoadingText_RendersLoadingText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Loading, true);
            p.Add(x => x.LoadingText, "Saving...");
        });
        Assert.Contains("Saving...", cut.Markup);
    }

    [Fact]
    public void LoadingContent_RendersCustomLoading()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Loading, true);
            p.Add(x => x.LoadingContent, (RenderFragment)(b => b.AddContent(0, "Custom loading")));
        });
        Assert.Contains("Custom loading", cut.Markup);
    }

    [Fact]
    public void UseFormState_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.UseFormState, true);
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void DisableWhenFormInvalid_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.DisableWhenFormInvalid, true);
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void Autofocus_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Autofocus, true);
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void FormAction_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FormAction, "/submit");
        });
        Assert.Contains("/submit", cut.Markup);
    }

    [Fact]
    public void FormEnctype_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FormEnctype, "multipart/form-data");
        });
        Assert.Contains("multipart/form-data", cut.Markup);
    }

    [Fact]
    public void FormMethod_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FormMethod, "post");
        });
        Assert.Contains("post", cut.Markup);
    }

    [Fact]
    public void FormNoValidate_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.FormNoValidate, true);
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void CommandParameter_Renders()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.CommandParameter, "param-value");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void Enum_XFButtonColor_HasValues()
    {
        Assert.Contains(XFButtonColor.Primary, Enum.GetValues<XFButtonColor>());
        Assert.Contains(XFButtonColor.Secondary, Enum.GetValues<XFButtonColor>());
        Assert.Contains(XFButtonColor.Success, Enum.GetValues<XFButtonColor>());
        Assert.Contains(XFButtonColor.Warning, Enum.GetValues<XFButtonColor>());
        Assert.Contains(XFButtonColor.Danger, Enum.GetValues<XFButtonColor>());
    }

    [Fact]
    public void Enum_XFButtonShape_HasValues()
    {
        Assert.Contains(XFButtonShape.Auto, Enum.GetValues<XFButtonShape>());
        Assert.Contains(XFButtonShape.Rounded, Enum.GetValues<XFButtonShape>());
        Assert.Contains(XFButtonShape.Circle, Enum.GetValues<XFButtonShape>());
        Assert.Contains(XFButtonShape.Square, Enum.GetValues<XFButtonShape>());
    }

    [Fact]
    public void AriaLabel_RendersAriaLabel()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.AriaLabel, "Action button");
        });
        Assert.Contains("aria-label=\"Action button\"", cut.Markup);
    }

    [Fact]
    public void Name_RendersNameAttribute()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Name, "my-button");
        });
        Assert.Contains("my-button", cut.Markup);
    }

    [Fact]
    public void Value_RendersValueAttribute()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Value, "btn-value");
        });
        Assert.Contains("btn-value", cut.Markup);
    }

    [Fact]
    public void Form_RendersFormAttribute()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Form, "my-form");
        });
        Assert.Contains("my-form", cut.Markup);
    }

    [Fact]
    public void Class_RendersCustomClass()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Class, "custom-btn");
        });
        Assert.Contains("custom-btn", cut.Markup);
    }

    [Fact]
    public void Style_ParameterAccepted()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Style, "margin: 10px");
        });
        Assert.Contains("xf-button", cut.Markup);
    }

    [Fact]
    public void AdditionalAttributes_Render()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "value" } });
        });
        Assert.Contains("data-test", cut.Markup);
    }

    [Fact]
    public void Enum_XFButtonVariant_HasValues()
    {
        Assert.Contains(XFButtonVariant.Solid, Enum.GetValues<XFButtonVariant>());
        Assert.Contains(XFButtonVariant.Outline, Enum.GetValues<XFButtonVariant>());
        Assert.Contains(XFButtonVariant.Ghost, Enum.GetValues<XFButtonVariant>());
        Assert.Contains(XFButtonVariant.Soft, Enum.GetValues<XFButtonVariant>());
        Assert.Contains(XFButtonVariant.Subtle, Enum.GetValues<XFButtonVariant>());
        Assert.Contains(XFButtonVariant.Link, Enum.GetValues<XFButtonVariant>());
    }

    [Fact]
    public void Enum_XFButtonType_HasValues()
    {
        Assert.Contains(XFButtonType.Button, Enum.GetValues<XFButtonType>());
        Assert.Contains(XFButtonType.Submit, Enum.GetValues<XFButtonType>());
        Assert.Contains(XFButtonType.Reset, Enum.GetValues<XFButtonType>());
    }

    [Fact]
    public void Enum_XFExtendedSize_HasValues()
    {
        Assert.Contains(XFExtendedSize.Xs, Enum.GetValues<XFExtendedSize>());
        Assert.Contains(XFExtendedSize.Sm, Enum.GetValues<XFExtendedSize>());
        Assert.Contains(XFExtendedSize.Md, Enum.GetValues<XFExtendedSize>());
        Assert.Contains(XFExtendedSize.Lg, Enum.GetValues<XFExtendedSize>());
        Assert.Contains(XFExtendedSize.Xl, Enum.GetValues<XFExtendedSize>());
    }

    // Size Sm (line 101)
    [Fact]
    public void SizeSm_RendersSmall()
    {
        var cut = Render<XFButton>(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-8", cut.Markup);
    }

    // Shape Square (line 159)
    [Fact]
    public void ShapeSquare_RendersSquare()
    {
        var cut = Render<XFButton>(p => p.Add(x => x.Shape, XFButtonShape.Square));
        Assert.Contains("rounded-lg", cut.Markup);
    }

    // Shape Rounded (line 160)
    [Fact]
    public void ShapeRounded_RendersRounded()
    {
        var cut = Render<XFButton>(p => p.Add(x => x.Shape, XFButtonShape.Rounded));
        Assert.Contains("rounded-md", cut.Markup);
    }

    // ToneTextClass branches (lines 167-171) — used in Outline/Ghost/Subtle/Link variants
    [Fact]
    public void ColorSecondary_OutlineVariant_RendersSecondaryText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Secondary);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void ColorSuccess_OutlineVariant_RendersSuccessText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void ColorWarning_OutlineVariant_RendersWarningText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void ColorDanger_OutlineVariant_RendersDangerText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void ColorNeutral_OutlineVariant_RendersNeutralText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Neutral);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void ColorInfo_OutlineVariant_RendersInfoText()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-info", cut.Markup);
    }

    // ToneBorderClass branches (lines 179-182)
    [Fact]
    public void ColorSuccess_BorderSuccess()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-success", cut.Markup);
    }

    [Fact]
    public void ColorWarning_BorderWarning()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-warning", cut.Markup);
    }

    [Fact]
    public void ColorDanger_BorderDanger()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-danger", cut.Markup);
    }

    [Fact]
    public void ColorInfo_BorderInfo()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-info", cut.Markup);
    }

    // ToneSolidClass branches (lines 189-193)
    [Fact]
    public void ColorSuccess_SolidVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-success", cut.Markup);
    }

    [Fact]
    public void ColorWarning_SolidVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-warning", cut.Markup);
    }

    [Fact]
    public void ColorDanger_SolidVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void ColorNeutral_SolidVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Neutral);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-surface-950", cut.Markup);
    }

    [Fact]
    public void ColorInfo_SolidVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-info", cut.Markup);
    }

    // ToneSoftClass branches (lines 199-203)
    [Fact]
    public void ColorSecondary_SoftVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Secondary);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void ColorSuccess_SoftVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-success)", cut.Markup);
    }

    [Fact]
    public void ColorWarning_SoftVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    [Fact]
    public void ColorDanger_SoftVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    [Fact]
    public void ColorNeutral_SoftVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Neutral);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void ColorInfo_SoftVariant()
    {
        var cut = Render<XFButton>(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-info)", cut.Markup);
    }
}
