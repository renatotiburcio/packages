using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.NumberInput;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFNumberInputComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithLabelHelperAndConstraints()
    {
        var value = 12;
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Label, "Quantity");
            parameters.Add(p => p.HelperText, "Between 1 and 100.");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.Min, 1);
            parameters.Add(p => p.Max, 100);
            parameters.Add(p => p.Step, 5m);
        });

        Assert.Contains("xf-number-input", cut.Markup);
        Assert.Contains("Quantity", cut.Markup);
        Assert.Contains("Between 1 and 100.", cut.Markup);
        Assert.Contains("min=\"1\"", cut.Markup);
        Assert.Contains("max=\"100\"", cut.Markup);
        Assert.Contains("step=\"5\"", cut.Markup);
        Assert.Contains("aria-label=\"Increase value\"", cut.Markup);
        Assert.Contains("aria-label=\"Decrease value\"", cut.Markup);
        Assert.Contains("text-right", cut.Markup);

        var input = cut.Find("input");
        Assert.Equal("false", input.GetAttribute("aria-required"));
        Assert.Equal("false", input.GetAttribute("aria-readonly"));
        Assert.Contains(input.GetAttribute("aria-describedby")!, cut.Markup);
    }

    [Fact]
    public void Change_WhenIntSelected_UpdatesValue()
    {
        var value = 0;
        int? changed = null;
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int>(this, next => changed = value = next));
        });

        cut.Find("input").Change("25");

        Assert.Equal(25, changed);
    }

    [Fact]
    public void Change_WhenAboveMax_ClampsValue()
    {
        var value = 0;
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int>(this, next => value = next));
            parameters.Add(p => p.Max, 10);
        });

        cut.Find("input").Change("25");

        Assert.Equal(10, value);
    }

    [Fact]
    public void Click_WhenStepperIncrements_UsesStep()
    {
        var value = 2;
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int>(this, next => value = next));
            parameters.Add(p => p.Step, 3m);
        });

        var button = cut.Find("button");
        button.TriggerEvent("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());
        button.TriggerEvent("onpointerup", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());

        Assert.Equal(5, value);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.PrefixText, "$");
            parameters.Add(p => p.SuffixText, "USD");
        });

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("$", cut.Markup);
        Assert.Contains("USD", cut.Markup);
    }

    [Fact]
    public void Render_WithCurrencyStyle_UsesSymbolAdornmentAndNumericValue()
    {
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Value, 1234.5m);
            parameters.Add(p => p.CultureName, "pt-BR");
            parameters.Add(p => p.FormatStyle, XFNumberInputFormatStyle.Currency);
            parameters.Add(p => p.Scale, 2);
            parameters.Add(p => p.CurrencySymbol, "R$");
        });

        Assert.Contains("R$", cut.Markup);
        Assert.Contains("value=\"1234.50\"", cut.Markup);
        Assert.DoesNotContain("R$ 1.234,50", cut.Markup);
        Assert.Contains("step=\"0.01\"", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaValueTextFormat_UsesLocalizedAccessibleValue()
    {
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Value, 1234.5m);
            parameters.Add(p => p.CultureName, "pt-BR");
            parameters.Add(p => p.Scale, 2);
            parameters.Add(p => p.AriaValueTextFormat, "Valor {0}");
        });

        var input = cut.Find("input");

        Assert.Equal("1234.50", input.GetAttribute("value"));
        Assert.Equal("Valor 1234,50", input.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Render_WithPercentStyle_UsesSuffixAdornment()
    {
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Value, 15.755m);
            parameters.Add(p => p.FormatStyle, XFNumberInputFormatStyle.Percent);
            parameters.Add(p => p.Scale, 2);
        });

        Assert.Contains("%", cut.Markup);
        Assert.Contains("value=\"15.76\"", cut.Markup);
    }

    [Fact]
    public void Change_WithScale_RoundsValue()
    {
        decimal value = 0;
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<decimal>(this, next => value = next));
            parameters.Add(p => p.Scale, 2);
        });

        cut.Find("input").Change("12.345");

        Assert.Equal(12.35m, value);
    }

    [Fact]
    public void Change_WithPrecisionAndScale_LimitsMagnitude()
    {
        decimal value = 0;
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<decimal>(this, next => value = next));
            parameters.Add(p => p.Precision, 5);
            parameters.Add(p => p.Scale, 2);
        });

        cut.Find("input").Change("12345.67");

        Assert.Equal(999.99m, value);
    }

    [Fact]
    public void Change_WithAutoDecimalSeparator_ParsesMinorUnits()
    {
        decimal value = 0;
        var cut = Render<XFNumberInput<decimal>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<decimal>(this, next => value = next));
            parameters.Add(p => p.Scale, 2);
            parameters.Add(p => p.AutoDecimalSeparator, true);
        });

        cut.Find("input").Change("98765");

        Assert.Equal(987.65m, value);
    }

    [Fact]
    public void Render_WithTextAlignOverride_UsesRequestedAlignment()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 123);
            parameters.Add(p => p.TextAlign, XFNumberInputTextAlign.Left);
        });

        Assert.Contains("text-left", cut.Markup);
        Assert.DoesNotContain("text-right", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new NumberModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Quantity);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Quantity is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFNumberInput<int?>>(3);
                childBuilder.AddAttribute(4, "Value", model.Quantity);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<int?>(this, value => model.Quantity = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<int?>>)(() => model.Quantity));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Quantity is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Render_StackedLayout_ShowsStepperWrap()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 10);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Stacked);
        });

        // Stacked: stepper-wrap div with gap-1 contains two buttons
        Assert.Contains("gap-1", cut.Markup);
        Assert.Contains("aria-label=\"Increase value\"", cut.Markup);
        Assert.Contains("aria-label=\"Decrease value\"", cut.Markup);
        // Should NOT have split buttons (border-r or border-l on buttons)
        Assert.DoesNotContain("border-r border-surface-200", cut.Markup);
        Assert.DoesNotContain("border-l border-surface-200", cut.Markup);
    }

    [Fact]
    public void Render_SplitLayout_ShowsDecrementLeftAndIncrementRight()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 10);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Split);
        });

        // Split: same gap as stacked (gap-2)
        Assert.Contains("gap-2", cut.Markup);
        // Split buttons use same premium StepperButtonClass as stacked
        Assert.Contains("h-6 w-6", cut.Markup);
        Assert.Contains("rounded-md", cut.Markup);
        // Two buttons with aria-labels for increase/decrease
        Assert.Contains("aria-label=\"Increase value\"", cut.Markup);
        Assert.Contains("aria-label=\"Decrease value\"", cut.Markup);
        // No split-specific border separators
        Assert.DoesNotContain("border-r border-surface-200", cut.Markup);
        Assert.DoesNotContain("border-l border-surface-200", cut.Markup);
    }

    [Fact]
    public void Render_AutoLayout_RendersBothLayoutsWithResponsiveClasses()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 10);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Auto);
        });

        // Auto: same gap as stacked
        Assert.Contains("gap-2", cut.Markup);
        // Auto: split buttons hidden on desktop (md:hidden)
        Assert.Contains("md:hidden", cut.Markup);
        // Auto: stacked wrap hidden on mobile (hidden md:flex)
        Assert.Contains("hidden md:flex", cut.Markup);
        // Both layouts use same premium StepperButtonClass
        Assert.Contains("h-6 w-6", cut.Markup);
        Assert.Contains("rounded-md", cut.Markup);
        // No split-specific border separators
        Assert.DoesNotContain("border-r border-surface-200", cut.Markup);
        Assert.DoesNotContain("border-l border-surface-200", cut.Markup);
    }

    [Fact]
    public void Render_SplitLayout_SmallSize_UsesSmallButtonWidth()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 10);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Split);
            parameters.Add(p => p.Size, XFSize.Sm);
        });

        // Split buttons use same StepperButtonClass (h-6 w-6) regardless of Size
        Assert.Contains("h-6 w-6", cut.Markup);
        Assert.Contains("rounded-md", cut.Markup);
        // Shell uses small height
        Assert.Contains("h-9", cut.Markup);
    }

    [Fact]
    public void Render_SplitLayout_LargeSize_UsesLargeButtonWidth()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 10);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Split);
            parameters.Add(p => p.Size, XFSize.Lg);
        });

        // Split buttons use same StepperButtonClass (h-6 w-6) regardless of Size
        Assert.Contains("h-6 w-6", cut.Markup);
        Assert.Contains("rounded-md", cut.Markup);
        // Shell uses large height
        Assert.Contains("h-12", cut.Markup);
    }

    [Fact]
    public void Click_SplitIncrementButton_StepsValue()
    {
        var value = 0;
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int>(this, next => value = next));
            parameters.Add(p => p.Step, 5m);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Split);
        });

        // Find the split increment button (the last button with aria-label "Increase value")
        var buttons = cut.FindAll("button");
        var incrementButton = buttons.LastOrDefault(b => b.GetAttribute("aria-label") == "Increase value");
        Assert.NotNull(incrementButton);
        incrementButton!.TriggerEvent("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());
        incrementButton.TriggerEvent("onpointerup", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());

        Assert.Equal(5, value);
    }

    [Fact]
    public void Click_SplitDecrementButton_StepsValue()
    {
        var value = 10;
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int>(this, next => value = next));
            parameters.Add(p => p.Step, 3m);
            parameters.Add(p => p.StepperLayout, XFStepperLayout.Split);
        });

        var buttons = cut.FindAll("button");
        var decrementButton = buttons.FirstOrDefault(b => b.GetAttribute("aria-label") == "Decrease value");
        Assert.NotNull(decrementButton);
        decrementButton!.TriggerEvent("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());
        decrementButton.TriggerEvent("onpointerup", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());

        Assert.Equal(7, value);
    }

    [Fact]
    public void Render_DefaultStepperLayout_IsStacked()
    {
        var cut = Render<XFNumberInput<int>>(parameters =>
        {
            parameters.Add(p => p.Value, 10);
        });

        // Default should behave like Stacked (no split buttons)
        Assert.DoesNotContain("border-r border-surface-200", cut.Markup);
        Assert.DoesNotContain("border-l border-surface-200", cut.Markup);
        Assert.Contains("gap-2", cut.Markup);
    }

    private sealed class NumberModel
    {
        [Required]
        public int? Quantity { get; set; }
    }
}
