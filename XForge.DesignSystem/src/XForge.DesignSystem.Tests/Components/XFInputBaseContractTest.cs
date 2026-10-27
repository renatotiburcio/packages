#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Components.Forms.Textarea;
using XForge.DesignSystem.Components.Forms.NumberInput;
using XForge.DesignSystem.Components.Forms.DateInput;
using XForge.DesignSystem.Components.Forms.TimeInput;
using XForge.DesignSystem.Components.Forms.InputMask;
using XForge.DesignSystem.Components.Forms.Search;
using XForge.DesignSystem.Components.Forms.PasswordStrength;
using XForge.DesignSystem.Components.Forms.Slider;
using XForge.DesignSystem.Components.Forms.Rating;
using XForge.DesignSystem.Components.Forms.ColorInput;
#endregion

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Tests for S-G014: standardized input base contracts.
/// Verifies that all form input components inherit <see cref="XFInputComponentBase"/>
/// and expose FocusAsync, SelectAsync, BlurAsync, aria-disabled and InputRef.
/// </summary>
public class XFInputBaseContractTest : Bunit.BunitContext
{
    public XFInputBaseContractTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region FocusAsync

    [Fact]
    public async Task TextInput_FocusAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task Textarea_FocusAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextarea>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task NumberInput_FocusAsync_DoesNotThrow()
    {
        int value = 42;
        var cut = Render<XFNumberInput<int>>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task DateInput_FocusAsync_DoesNotThrow()
    {
        DateOnly? value = DateOnly.FromDateTime(DateTime.Today);
        var cut = Render<XFDateInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task TimeInput_FocusAsync_DoesNotThrow()
    {
        TimeOnly? value = TimeOnly.FromDateTime(DateTime.Now);
        var cut = Render<XFTimeInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task Search_FocusAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFSearch>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task Slider_FocusAsync_DoesNotThrow()
    {
        decimal? value = 50m;
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task ColorInput_FocusAsync_DoesNotThrow()
    {
        string? value = "#ff0000";
        var cut = Render<XFColorInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    [Fact]
    public async Task Rating_FocusAsync_DoesNotThrow()
    {
        double value = 3;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.FocusAsync();
    }

    #endregion

    #region SelectAsync

    [Fact]
    public async Task TextInput_SelectAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.SelectAsync();
    }

    [Fact]
    public async Task Textarea_SelectAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextarea>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.SelectAsync();
    }

    #endregion

    #region BlurAsync

    [Fact]
    public async Task TextInput_BlurAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.BlurAsync();
    }

    [Fact]
    public async Task Textarea_BlurAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextarea>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.BlurAsync();
    }

    #endregion

    #region aria-disabled

    [Fact]
    public void TextInput_Disabled_RendersAriaDisabled()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void TextInput_NotDisabled_RendersAriaDisabledFalse()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Disabled, false);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"false\"", cut.Markup);
    }

    [Fact]
    public void Textarea_Disabled_RendersAriaDisabled()
    {
        string? value = "test";
        var cut = Render<XFTextarea>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void NumberInput_Disabled_RendersAriaDisabled()
    {
        int value = 42;
        var cut = Render<XFNumberInput<int>>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void DateInput_Disabled_RendersAriaDisabled()
    {
        DateOnly? value = DateOnly.FromDateTime(DateTime.Today);
        var cut = Render<XFDateInput>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void TimeInput_Disabled_RendersAriaDisabled()
    {
        TimeOnly? value = TimeOnly.FromDateTime(DateTime.Now);
        var cut = Render<XFTimeInput>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Search_Disabled_RendersAriaDisabled()
    {
        string? value = "test";
        var cut = Render<XFSearch>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Slider_Disabled_RendersAriaDisabled()
    {
        decimal? value = 50m;
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Rating_Disabled_RendersAriaDisabled()
    {
        double value = 3;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void ColorInput_Disabled_RendersAriaDisabled()
    {
        string? value = "#ff0000";
        var cut = Render<XFColorInput>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    #endregion

    #region InputRef presence

    [Fact]
    public void TextInput_RendersInputElement()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("<input", cut.Markup);
    }

    [Fact]
    public void Textarea_RendersTextareaElement()
    {
        string? value = "test";
        var cut = Render<XFTextarea>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        Assert.Contains("<textarea", cut.Markup);
    }

    #endregion

    #region DisposeAsync

    [Fact]
    public async Task TextInput_DisposeAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFTextInput>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Instance.DisposeAsync();
    }

    [Fact]
    public async Task Search_DisposeAsync_DoesNotThrow()
    {
        string? value = "test";
        var cut = Render<XFSearch>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v));
        });

        await cut.Instance.DisposeAsync();
    }

    #endregion
}
