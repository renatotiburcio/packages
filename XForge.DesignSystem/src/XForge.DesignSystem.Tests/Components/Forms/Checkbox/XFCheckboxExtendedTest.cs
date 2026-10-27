using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Checkbox;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.Checkbox;

public class XFCheckboxExtendedTest : BunitContext
{
    [Fact]
    public void LabelPosition_Start_RendersLabelBeforeInput()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Label, "Accept");
            parameters.Add(p => p.LabelPosition, XFCheckboxLabelPosition.Start);
        });

        Assert.Contains("Accept", cut.Markup);
        Assert.Contains("xf-checkbox", cut.Markup);
    }

    [Fact]
    public void ChildContent_RendersCustomContent()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(0, "Custom label");
                builder.CloseElement();
            }));
        });

        Assert.Contains("Custom label", cut.Markup);
    }

    [Fact]
    public void Required_ShowsAsterisk()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Label, "Terms");
            parameters.Add(p => p.Required, true);
        });

        Assert.Contains("&ast;", cut.Markup);
    }

    [Fact]
    public void SizeSm_RendersSmallBox()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Size, XFSize.Sm);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void SizeLg_RendersLargeBox()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("h-6 w-6", cut.Markup);
    }

    [Fact]
    public void SizeMd_RendersMediumBox()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Size, XFSize.Md);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("h-5 w-5", cut.Markup);
    }

    [Fact]
    public void ToneSecondary_Unchecked_RendersSecondaryHover()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Secondary);
            parameters.Add(p => p.Value, false);
        });

        Assert.Contains("hover:border-secondary/50", cut.Markup);
    }

    [Fact]
    public void ToneSuccess_Unchecked_RendersSuccessHover()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
            parameters.Add(p => p.Value, false);
        });

        Assert.Contains("hover:border-success/50", cut.Markup);
    }

    [Fact]
    public void ToneWarning_Unchecked_RendersWarningHover()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.Value, false);
        });

        Assert.Contains("hover:border-warning/50", cut.Markup);
    }

    [Fact]
    public void ToneDanger_Unchecked_RendersDangerHover()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Danger);
            parameters.Add(p => p.Value, false);
        });

        Assert.Contains("hover:border-danger/50", cut.Markup);
    }

    [Fact]
    public void ToneSecondary_Checked_RendersSecondaryBg()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Secondary);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("bg-secondary", cut.Markup);
        Assert.Contains("border-secondary", cut.Markup);
    }

    [Fact]
    public void ToneSuccess_Checked_RendersSuccessBg()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("bg-success", cut.Markup);
        Assert.Contains("border-success", cut.Markup);
    }

    [Fact]
    public void ToneWarning_Checked_RendersWarningBg()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("bg-warning", cut.Markup);
        Assert.Contains("border-warning", cut.Markup);
    }

    [Fact]
    public void ToneDanger_Checked_RendersDangerBg()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Tone, XFSemanticColor.Danger);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("bg-danger", cut.Markup);
        Assert.Contains("border-danger", cut.Markup);
    }

    [Fact]
    public void FullWidth_False_RendersInlineFlex()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.FullWidth, false);
        });

        Assert.Contains("inline-flex", cut.Markup);
    }

    [Fact]
    public void FullWidth_True_RendersWFull()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.FullWidth, true);
        });

        Assert.Contains("w-full", cut.Markup);
    }

    [Fact]
    public void Disabled_RendersDisabledClass()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void ReadOnly_RendersCursorDefault()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ReadOnly, true);
        });

        Assert.Contains("cursor-default", cut.Markup);
    }

    [Fact]
    public void ThreeState_Mixed_RendersMixedAriaChecked()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ThreeState, true);
            parameters.Add(p => p.Value, null);
        });

        Assert.Contains("aria-checked=\"mixed\"", cut.Markup);
    }

    [Fact]
    public void ThreeState_Mixed_RendersDashMark()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ThreeState, true);
            parameters.Add(p => p.Value, null);
        });

        Assert.Contains("dash-enter", cut.Markup);
    }

    [Fact]
    public void CheckedStateText_RendersStateDescription()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Value, true);
            parameters.Add(p => p.CheckedStateText, "Is checked");
        });

        Assert.Contains("Is checked", cut.Markup);
    }

    [Fact]
    public void UncheckedStateText_RendersStateDescription()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Value, false);
            parameters.Add(p => p.UncheckedStateText, "Is unchecked");
        });

        Assert.Contains("Is unchecked", cut.Markup);
    }

    [Fact]
    public void AriaLabel_SetsInputAriaLabel()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Accept terms");
        });

        Assert.Equal("Accept terms", cut.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void Name_SetsInputName()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Name, "accept-terms");
        });

        Assert.Equal("accept-terms", cut.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void ChangeEvent_InvokedOnChange()
    {
        bool? changeValue = null;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Value, false);
            parameters.Add(p => p.Change, EventCallback.Factory.Create<bool?>(this, v => changeValue = v));
        });

        cut.Find("input").Change(true);
        Assert.True(changeValue);
    }

    [Fact]
    public void Description_RendersDescriptionSpan()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Label, "Terms");
            parameters.Add(p => p.Description, "Read carefully");
        });

        Assert.Contains("Read carefully", cut.Markup);
        Assert.Contains("aria-describedby", cut.Markup);
    }

    [Fact]
    public void SizeSm_CheckIcon_RendersSmallIcon()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Size, XFSize.Sm);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("h-2.5 w-2.5", cut.Markup);
    }

    [Fact]
    public void SizeLg_CheckIcon_RendersLargeIcon()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("h-4 w-4", cut.Markup);
    }
}
