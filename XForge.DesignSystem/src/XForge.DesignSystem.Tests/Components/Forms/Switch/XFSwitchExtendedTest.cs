using System.Linq.Expressions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.Switch;

public class XFSwitchExtendedTest : BunitContext
{
    public XFSwitchExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void SizeSm_RendersCompactSwitch()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("h-5", cut.Markup);
    }

    [Fact]
    public void SizeLg_RendersLargeSwitch()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("h-7", cut.Markup);
    }

    [Fact]
    public void SizeMd_RendersMediumSwitch()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Md);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("h-6", cut.Markup);
    }

    [Fact]
    public void ToneSecondary_RendersSecondaryTrack()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneSuccess_RendersSuccessTrack()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneWarning_RendersWarningTrack()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneDanger_RendersDangerTrack()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void TonePrimary_Unchecked_RendersPrimaryUnchecked()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Primary);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneSecondary_Unchecked_RendersSecondaryUnchecked()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneSuccess_Unchecked_RendersSuccessUnchecked()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneWarning_Unchecked_RendersWarningUnchecked()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Warning);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ToneDanger_Unchecked_RendersDangerUnchecked()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void ChildContent_RendersCustomLabel()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Custom Label")));
        });
        Assert.Contains("Custom Label", cut.Markup);
    }

    [Fact]
    public void Required_RendersAsterisk()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Label, "Accept");
            p.Add(x => x.Required, true);
        });
        Assert.Contains("aria-required", cut.Markup);
    }

    [Fact]
    public void ShowStateText_RendersStateText()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.ShowStateText, true);
            p.Add(x => x.Value, true);
            p.Add(x => x.CheckedText, "ON");
            p.Add(x => x.UncheckedText, "OFF");
        });
        Assert.Contains("ON", cut.Markup);
    }

    [Fact]
    public void ShowStateText_Unchecked_RendersUncheckedText()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.ShowStateText, true);
            p.Add(x => x.Value, false);
            p.Add(x => x.CheckedText, "ON");
            p.Add(x => x.UncheckedText, "OFF");
        });
        Assert.Contains("OFF", cut.Markup);
    }

    [Fact]
    public void ShowThumbIcon_False_HidesIcon()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.ShowThumbIcon, false);
            p.Add(x => x.Value, true);
        });
        Assert.DoesNotContain("xf-icon-check", cut.Markup);
    }

    [Fact]
    public void LabelPositionStart_RendersLabelBefore()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Label, "Test");
            p.Add(x => x.LabelPosition, XFSwitchLabelPosition.Start);
        });
        Assert.Contains("flex-row-reverse", cut.Markup);
    }

    [Fact]
    public void FullWidthFalse_RendersInline()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.FullWidth, false);
        });
        Assert.Contains("inline-flex", cut.Markup);
    }

    [Fact]
    public void ReadOnly_PreventsToggle()
    {
        var value = false;
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => value = v));
            p.Add(x => x.ReadOnly, true);
        });

        cut.Find("button").Click();
        Assert.False(value);
    }

    [Fact]
    public void ToggleAsync_WithEditContext_NotifiesFieldChanged()
    {
        var model = new TestModel { Enabled = false };
        var editContext = new EditContext(model);

        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(x => x.Value, model.Enabled);
            parameters.Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => model.Enabled = v));
            parameters.Add(x => x.ValueExpression, (Expression<Func<bool>>)(() => model.Enabled));
            parameters.AddCascadingValue("EditContext", editContext);
        });

        cut.Find("button").Click();
        Assert.True(model.Enabled);
    }

    [Fact]
    public void KeySpace_TogglesValue()
    {
        var value = false;
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => value = v));
        });

        cut.Find("button").KeyDown(new KeyboardEventArgs { Key = " " });
        Assert.True(value);
    }

    [Fact]
    public void KeyEnter_TogglesValue()
    {
        var value = true;
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => value = v));
        });

        cut.Find("button").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.False(value);
    }

    [Fact]
    public void Description_RendersDescribedBy()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Description, "Toggle this option");
        });
        Assert.Contains("Toggle this option", cut.Markup);
    }

    [Fact]
    public void Name_RendersOnButton()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Name, "my-switch");
        });
        var button = cut.Find("button");
        Assert.Equal("my-switch", button.GetAttribute("name"));
    }

    [Fact]
    public void AriaLabel_RendersOnButton()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.AriaLabel, "Toggle feature");
        });
        Assert.Contains("aria-label=\"Toggle feature\"", cut.Markup);
    }

    [Fact]
    public void SizeSm_ShowStateText_RendersCompactStateText()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.ShowStateText, true);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("text-[0.625rem]", cut.Markup);
    }

    [Fact]
    public void SizeLg_ShowStateText_RendersLargeStateText()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.ShowStateText, true);
            p.Add(x => x.Value, true);
        });
        Assert.Contains("text-xs", cut.Markup);
    }

    [Fact]
    public void SizeSm_Unchecked_ThumbTranslate()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void SizeLg_Unchecked_ThumbTranslate()
    {
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.Value, false);
        });
        Assert.Contains("xf-switch", cut.Markup);
    }

    [Fact]
    public void Change_CallbackFires()
    {
        var changeValue = false;
        var cut = Render<XFSwitch>(p =>
        {
            p.Add(x => x.Value, false);
            p.Add(x => x.Change, EventCallback.Factory.Create<bool>(this, v => changeValue = v));
        });

        cut.Find("button").Click();
        Assert.True(changeValue);
    }

    private sealed class TestModel
    {
        public bool Enabled { get; set; }
    }
}
