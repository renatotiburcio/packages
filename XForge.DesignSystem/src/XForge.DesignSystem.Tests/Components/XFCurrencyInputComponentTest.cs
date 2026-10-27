using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.CurrencyInput;
using XForge.DesignSystem.Components.Forms.NumberInput;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFCurrencyInputComponentTest : BunitContext
{
    [Fact]
    public void Render_WithCurrencyDefaults_UsesMoneyContract()
    {
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Amount");
            parameters.Add(p => p.Value, 1234.5m);
            parameters.Add(p => p.CurrencySymbol, "R$");
            parameters.Add(p => p.CurrencyCode, "BRL");
        });

        Assert.Contains("Amount", cut.Markup);
        Assert.Contains("R$", cut.Markup);
        Assert.Contains("BRL", cut.Markup);
        Assert.Contains("value=\"1234.50\"", cut.Markup);
        Assert.Contains("step=\"0.01\"", cut.Markup);
        Assert.Contains("aria-label=\"Increase value\"", cut.Markup);
        Assert.Contains("aria-label=\"Decrease value\"", cut.Markup);
        Assert.DoesNotContain("Increase valor", cut.Markup);
        Assert.DoesNotContain("Decrease valor", cut.Markup);
        Assert.Contains("text-right", cut.Markup);
    }

    [Fact]
    public void Change_WhenValueChanges_UpdatesDecimalValue()
    {
        decimal? value = 0;
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<decimal?>(this, next => value = next));
            parameters.Add(p => p.CurrencySymbol, "$");
        });

        cut.Find("input").Change("45.678");

        Assert.Equal(45.68m, value);
    }

    [Fact]
    public void Render_WithVisualOptions_ForwardsToNumberInput()
    {
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.CurrencySymbol, "â‚¬");
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Tone, XFSemanticColor.Secondary);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.ShowSteppers, false);
        });

        Assert.Contains("â‚¬", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Empty(cut.FindAll("button"));
    }

    [Fact]
    public void Render_WithAriaValueTextFormat_ForwardsLocalizedAccessibleValue()
    {
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.Value, 1234.5m);
            parameters.Add(p => p.CultureName, "pt-BR");
            parameters.Add(p => p.CurrencySymbol, "R$");
            parameters.Add(p => p.AriaValueTextFormat, "Valor {0}");
        });

        var input = cut.Find("input");

        Assert.Equal("1234.50", input.GetAttribute("value"));
        Assert.Equal("Valor 1234,50", input.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Change_WithAutoDecimalSeparator_TreatsDigitsAsMinorUnits()
    {
        decimal? value = 0;
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<decimal?>(this, next => value = next));
            parameters.Add(p => p.CurrencySymbol, "R$");
            parameters.Add(p => p.AutoDecimalSeparator, true);
        });

        cut.Find("input").Change("123456");

        Assert.Equal(1234.56m, value);
    }

    [Fact]
    public void Change_WithAutoDecimalSeparatorAndScale_UsesConfiguredScale()
    {
        decimal? value = 0;
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<decimal?>(this, next => value = next));
            parameters.Add(p => p.CurrencySymbol, "$");
            parameters.Add(p => p.Scale, 4);
            parameters.Add(p => p.AutoDecimalSeparator, true);
        });

        cut.Find("input").Change("123456");

        Assert.Equal(12.3456m, value);
    }

    [Fact]
    public void Render_WithTextAlignOverride_ForwardsAlignment()
    {
        var cut = Render<XFCurrencyInput>(parameters =>
        {
            parameters.Add(p => p.Value, 123.45m);
            parameters.Add(p => p.CurrencySymbol, "$");
            parameters.Add(p => p.TextAlign, XFNumberInputTextAlign.Center);
        });

        Assert.Contains("text-center", cut.Markup);
    }
}
