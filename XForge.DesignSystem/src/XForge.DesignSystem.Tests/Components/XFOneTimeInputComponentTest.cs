using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Forms.OneTimeInput;

namespace XForge.DesignSystem.Tests.Components;

public class XFOneTimeInputComponentTest : BunitContext
{
    public XFOneTimeInputComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithDefaultSlotsAndGrouping()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Codigo");
            parameters.Add(p => p.Value, "123456");
        });

        Assert.Contains("xf-one-time-input", cut.Markup);
        Assert.Contains("Codigo", cut.Markup);
        Assert.Equal(6, cut.FindAll("input").Count);
        Assert.Contains("-", cut.Markup);
        Assert.Contains("value=\"1\"", cut.Markup);
        Assert.Contains("autocomplete=\"one-time-code\"", cut.Markup);
    }

    [Fact]
    public async Task Input_WithMultipleCharacters_FillsFollowingSlotsAndCompletes()
    {
        string? value = null;
        string? completed = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.OnComplete, EventCallback.Factory.Create<string>(this, next => completed = next));
        });

        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "123456" });

        Assert.Equal("123456", value);
        Assert.Equal("123456", completed);
    }

    [Fact]
    public async Task Backspace_ClearsCurrentSlotAndRaisesChange()
    {
        string? changed = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, "123456");
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, next => changed = next));
        });

        await cut.FindAll("input")[2].KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });

        Assert.Equal("12456", changed);
    }

    [Fact]
    public void Render_WithCustomDigitsAndGroup()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Digits, 4);
            parameters.Add(p => p.Group, "2,2");
            parameters.Add(p => p.Separator, " ");
            parameters.Add(p => p.Required, true);
            parameters.Add(p => p.HelperText, "Informe o codigo.");
            parameters.Add(p => p.SlotAriaLabelFormat, "Digit {0} of {1}");
        });

        Assert.Equal(4, cut.FindAll("input").Count);
        Assert.Contains("aria-label=\"Digit 4 of 4\"", cut.Markup);

        var group = cut.Find("[role=group]");
        Assert.Equal("true", group.GetAttribute("aria-required"));
        Assert.Equal("false", group.GetAttribute("aria-disabled"));
        Assert.Equal("false", group.GetAttribute("aria-readonly"));

        var firstSlot = cut.Find("input");
        Assert.Equal("true", firstSlot.GetAttribute("aria-required"));
        Assert.Contains(firstSlot.GetAttribute("aria-describedby")!, cut.Markup);
    }

    [Fact]
    public void Render_NoLabel_NoLabelElement()
    {
        var cut = Render<XFOneTimeInput>();

        Assert.DoesNotContain("<label", cut.Markup);
        Assert.Equal(6, cut.FindAll("input").Count);
    }

    [Fact]
    public void Render_WithHelperText_ShowsHelperAndDescribedBy()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Digite o codigo OTP.");
        });

        Assert.Contains("Digite o codigo OTP.", cut.Markup);
        var group = cut.Find("[role=group]");
        Assert.Contains("xf-one-time-input-helper", group.GetAttribute("aria-describedby")!);
    }

    [Fact]
    public void Render_WithMask_ShowsBullets()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, "123456");
            parameters.Add(p => p.Mask, true);
        });

        var inputs = cut.FindAll("input");
        Assert.Contains("value=\"\u2022\"", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_AllInputsDisabled()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        var inputs = cut.FindAll("input");
        Assert.All(inputs, i => Assert.True(i.HasAttribute("disabled")));
        Assert.Equal("true", cut.Find("[role=group]").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Render_ReadOnly_AllInputsReadonly()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.ReadOnly, true);
        });

        var inputs = cut.FindAll("input");
        Assert.All(inputs, i => Assert.True(i.HasAttribute("readonly")));
        Assert.Equal("true", cut.Find("[role=group]").GetAttribute("aria-readonly"));
    }

    [Fact]
    public void Render_FullWidth_HasFullWidthClass()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.FullWidth, true);
        });

        Assert.Contains("w-full", cut.Markup);
    }

    [Fact]
    public void Render_DefaultSeparator_IsDash()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Digits, 6);
            parameters.Add(p => p.Group, "3,3");
        });

        Assert.Contains("-", cut.Markup);
    }

    [Fact]
    public void Render_CustomSeparator()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Digits, 4);
            parameters.Add(p => p.Group, "2,2");
            parameters.Add(p => p.Separator, " ");
        });

        var separators = cut.FindAll("[aria-hidden=\"true\"]");
        Assert.Contains(separators, s => s.TextContent == " ");
    }

    [Fact]
    public async Task Input_SingleDigit_FillsFirstSlot()
    {
        string? value = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "7" });

        Assert.StartsWith("7", value);
    }

    [Fact]
    public async Task Input_WhenDisabled_DoesNotAcceptInput()
    {
        string? value = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1" });

        Assert.Null(value);
    }

    [Fact]
    public async Task Input_WhenReadOnly_DoesNotAcceptInput()
    {
        string? value = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.ReadOnly, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1" });

        Assert.Null(value);
    }

    [Fact]
    public async Task Backspace_FirstSlot_ClearsFirstSlot()
    {
        string? changed = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, "123456");
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, next => changed = next));
        });

        await cut.FindAll("input")[0].KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });

        Assert.Equal("23456", changed);
    }

    [Fact]
    public async Task Backspace_WhenDisabled_DoesNothing()
    {
        string? changed = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, "123456");
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, next => changed = next));
        });

        await cut.FindAll("input")[2].KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });

        Assert.Null(changed);
    }

    [Fact]
    public async Task Backspace_WhenReadOnly_DoesNothing()
    {
        string? changed = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, "123456");
            parameters.Add(p => p.ReadOnly, true);
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, next => changed = next));
        });

        await cut.FindAll("input")[2].KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });

        Assert.Null(changed);
    }

    [Fact]
    public void Render_WithDefaultValues_ValuesDisplayed()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, "987654");
        });

        Assert.Contains("value=\"9\"", cut.Markup);
        Assert.Contains("value=\"8\"", cut.Markup);
        Assert.Contains("value=\"7\"", cut.Markup);
        Assert.Contains("value=\"6\"", cut.Markup);
        Assert.Contains("value=\"5\"", cut.Markup);
        Assert.Contains("value=\"4\"", cut.Markup);
    }

    [Fact]
    public async Task Input_PartialFill_DoesNotFireOnComplete()
    {
        string? completed = null;
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Digits, 6);
            parameters.Add(p => p.OnComplete, EventCallback.Factory.Create<string>(this, next => completed = next));
        });

        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "123" });

        Assert.Null(completed);
    }

    [Fact]
    public void Render_GroupWithSingleGroup()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Digits, 4);
            parameters.Add(p => p.Group, "4");
        });

        Assert.Equal(4, cut.FindAll("input").Count);
        Assert.DoesNotContain("aria-hidden=\"true\">-<", cut.Markup);
    }

    [Fact]
    public void Render_SlotAriaLabelsAreSequential()
    {
        var cut = Render<XFOneTimeInput>(parameters =>
        {
            parameters.Add(p => p.Digits, 3);
            parameters.Add(p => p.Group, "3");
        });

        var inputs = cut.FindAll("input");
        Assert.Equal("Caractere 1 de 3", inputs[0].GetAttribute("aria-label"));
        Assert.Equal("Caractere 2 de 3", inputs[1].GetAttribute("aria-label"));
        Assert.Equal("Caractere 3 de 3", inputs[2].GetAttribute("aria-label"));
    }
}
