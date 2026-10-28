using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Xunit;
using XForge.DesignSystem.Components.Forms.RangeSlider;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFRangeSliderComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithValues_ShowsRange()
    {
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.Label, "Budget");
            parameters.Add(p => p.Min, 0);
            parameters.Add(p => p.Max, 100);
            parameters.Add(p => p.StartValue, 20);
            parameters.Add(p => p.EndValue, 80);
            parameters.Add(p => p.Required, true);
        });

        Assert.Contains("xf-range-slider", cut.Markup);
        Assert.Contains("Budget", cut.Markup);
        Assert.Contains("20 - 80", cut.Markup);
        Assert.Equal(2, cut.FindAll("input[type='range']").Count);
        Assert.All(cut.FindAll("input[type='range']"), input =>
        {
            Assert.Equal("true", input.GetAttribute("aria-required"));
            Assert.Equal("false", input.GetAttribute("aria-readonly"));
        });
    }

    [Fact]
    public void Input_WhenStartMoves_UpdatesStartValue()
    {
        decimal? start = null;
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.StartValue, 20);
            parameters.Add(p => p.EndValue, 80);
            parameters.Add(p => p.StartValueChanged, EventCallback.Factory.Create<decimal?>(this, value => start = value));
        });

        cut.FindAll("input[type='range']")[0].Input("40");

        Assert.Equal(40, start);
    }

    [Fact]
    public void Input_WhenEndMoves_UpdatesEndValue()
    {
        decimal? end = null;
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.StartValue, 20);
            parameters.Add(p => p.EndValue, 80);
            parameters.Add(p => p.EndValueChanged, EventCallback.Factory.Create<decimal?>(this, value => end = value));
        });

        cut.FindAll("input[type='range']")[1].Input("60");

        Assert.Equal(60, end);
    }

    [Fact]
    public void Input_WhenOverlapIsDisabled_ClampsStartToEnd()
    {
        decimal? start = null;
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.StartValue, 20);
            parameters.Add(p => p.EndValue, 50);
            parameters.Add(p => p.StartValueChanged, EventCallback.Factory.Create<decimal?>(this, value => start = value));
        });

        cut.FindAll("input[type='range']")[0].Input("80");

        Assert.Equal(50, start);
    }

    [Fact]
    public void Render_WhenOverlapIsDisabled_UsesSegmentedNativeInputs()
    {
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.StartValue, 20);
            parameters.Add(p => p.EndValue, 50);
        });

        var inputs = cut.FindAll("input[type='range']");

        Assert.Equal("50", inputs[0].GetAttribute("max"));
        Assert.Equal("20", inputs[1].GetAttribute("min"));
        Assert.Contains("w-[var(--xf-range-end)]", inputs[0].GetAttribute("class"));
        Assert.Contains("left-[var(--xf-range-start)]", inputs[1].GetAttribute("class"));
    }

    [Fact]
    public void Input_WhenOverlapIsAllowed_AllowsCrossedValues()
    {
        decimal? start = null;
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.StartValue, 20);
            parameters.Add(p => p.EndValue, 50);
            parameters.Add(p => p.AllowOverlap, true);
            parameters.Add(p => p.StartValueChanged, EventCallback.Factory.Create<decimal?>(this, value => start = value));
        });

        cut.FindAll("input[type='range']")[0].Input("80");

        Assert.Equal(80, start);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.PrefixText, "$ ");
            parameters.Add(p => p.StartValue, 10);
            parameters.Add(p => p.EndValue, 20);
        });

        Assert.Contains("[--xf-range-color:var(--color-warning)]", cut.Markup);
        Assert.Contains("[&amp;::-webkit-slider-thumb]:h-6", cut.Markup);
        Assert.Contains("$ 10 - $ 20", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomRangeSeparator_UsesLocalizedRangeText()
    {
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.StartValue, 10);
            parameters.Add(p => p.EndValue, 20);
            parameters.Add(p => p.RangeSeparatorText, " to ");
        });

        Assert.Contains("10 to 20", cut.Markup);
        Assert.DoesNotContain("10 - 20", cut.Markup);
    }

    [Fact]
    public void Render_WithCulture_FormatsDisplayedRangeAndAriaValueText()
    {
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.Min, 0m);
            parameters.Add(p => p.Max, 20.5m);
            parameters.Add(p => p.Step, 0.5m);
            parameters.Add(p => p.StartValue, 5.5m);
            parameters.Add(p => p.EndValue, 12.5m);
            parameters.Add(p => p.ValueFormat, "0.0");
            parameters.Add(p => p.RangeSeparatorText, " ate ");
            parameters.Add(p => p.Culture, CultureInfo.GetCultureInfo("pt-BR"));
        });

        var inputs = cut.FindAll("input[type='range']");

        Assert.Contains("5,5 ate 12,5", cut.Markup);
        Assert.Contains(">20,5<", cut.Markup);
        Assert.Equal("5,5", inputs[0].GetAttribute("aria-valuetext"));
        Assert.Equal("12,5", inputs[1].GetAttribute("aria-valuetext"));
        Assert.Equal("5.5", inputs[0].GetAttribute("value"));
        Assert.Equal("12.5", inputs[1].GetAttribute("value"));
    }

    [Fact]
    public void Render_WhenDisabled_DisablesBothInputs()
    {
        var cut = Render<XFRangeSlider>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        Assert.All(cut.FindAll("input[type='range']"), input => Assert.True(input.HasAttribute("disabled")));
        Assert.DoesNotContain("opacity-55", cut.Markup);
        Assert.Contains("[--xf-range-color:color-mix(in_oklab,var(--color-text-muted)_38%,var(--color-bg-surface))]", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new RangeModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Start);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Start is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRangeSlider>(3);
                childBuilder.AddAttribute(4, "StartValue", model.Start);
                childBuilder.AddAttribute(5, "EndValue", model.End);
                childBuilder.AddAttribute(6, "StartValueExpression", (System.Linq.Expressions.Expression<Func<decimal?>>)(() => model.Start));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Start is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    private sealed class RangeModel
    {
        [Required]
        public decimal? Start { get; set; }

        public decimal? End { get; set; } = 80;
    }
}
