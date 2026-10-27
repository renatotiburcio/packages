using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.TimeSpan;

namespace XForge.DesignSystem.Tests.Components;

public class XFTimeSpanComponentTest : BunitContext
{
    [Fact]
    public void Render_Default_RendersHoursMinutesSeconds()
    {
        var cut = Render<XFTimeSpan>();

        var inputs = cut.FindAll("input");
        Assert.Equal(3, inputs.Count);
        Assert.Contains("Horas", inputs[0].GetAttribute("aria-label"));
        Assert.Contains("Minutos", inputs[1].GetAttribute("aria-label"));
        Assert.Contains("Segundos", inputs[2].GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_WithShowDays_RendersDaysSegment()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.ShowDays, true);
        });

        var inputs = cut.FindAll("input");
        Assert.Equal(4, inputs.Count);
        Assert.Contains("Dias", inputs[0].GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_WithMinutePrecision_HidesSeconds()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Precision, XFTimeSpanPrecision.Minute);
        });

        var inputs = cut.FindAll("input");
        Assert.Equal(2, inputs.Count);
        Assert.Contains("Horas", inputs[0].GetAttribute("aria-label"));
        Assert.Contains("Minutos", inputs[1].GetAttribute("aria-label"));
        Assert.DoesNotContain("Segundos", cut.Markup);
    }

    [Fact]
    public void Render_WithLabel_RendersLabel()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Label, "Duração");
        });

        Assert.Contains("Duração", cut.Markup);
        Assert.Contains("<label", cut.Markup);
    }

    [Fact]
    public void Render_WithSteppers_RendersButtons()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.ShowSteppers, true);
        });

        var buttons = cut.FindAll("button");
        Assert.Equal(2, buttons.Count);
        Assert.Contains("Aumentar", cut.Markup);
        Assert.Contains("Diminuir", cut.Markup);
    }

    [Fact]
    public void Render_WithoutSteppers_HidesButtons()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.ShowSteppers, false);
        });

        var buttons = cut.FindAll("button");
        Assert.Empty(buttons);
    }

    [Fact]
    public void Render_Disabled_AppliesDisabled()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        var inputs = cut.FindAll("input");
        foreach (var input in inputs)
        {
            Assert.True(input.HasAttribute("disabled"));
        }
    }

    [Fact]
    public void Value_SetToTimeSpan_RendersCorrectSegments()
    {
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Value, new System.TimeSpan(1, 30, 45));
        });

        var inputs = cut.FindAll("input");
        Assert.Equal("1", inputs[0].GetAttribute("value"));
        Assert.Equal("30", inputs[1].GetAttribute("value"));
        Assert.Equal("45", inputs[2].GetAttribute("value"));
    }

    [Fact]
    public async Task ArrowUp_IncrementsSegment()
    {
        System.TimeSpan? value = new System.TimeSpan(1, 0, 0);
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<System.TimeSpan?>(this, next => value = next));
        });

        var hoursInput = cut.FindAll("input")[0];
        hoursInput.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });

        Assert.Equal(new System.TimeSpan(2, 0, 0), value);
    }

    [Fact]
    public async Task ArrowDown_DecrementsSegment()
    {
        System.TimeSpan? value = new System.TimeSpan(0, 30, 0);
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<System.TimeSpan?>(this, next => value = next));
        });

        var minutesInput = cut.FindAll("input")[1];
        minutesInput.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowDown" });

        Assert.Equal(new System.TimeSpan(0, 29, 0), value);
    }

    [Fact]
    public async Task Stepper_IncrementButton_IncreasesValue()
    {
        System.TimeSpan? value = new System.TimeSpan(0, 0, 0);
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<System.TimeSpan?>(this, next => value = next));
            parameters.Add(p => p.ShowSteppers, true);
            parameters.Add(p => p.Step, 15);
        });

        var incrementButton = cut.FindAll("button")[0];
        incrementButton.Click();

        Assert.Equal(System.TimeSpan.FromSeconds(15), value);
    }

    [Fact]
    public async Task Stepper_DecrementButton_DecreasesValue()
    {
        System.TimeSpan? value = new System.TimeSpan(0, 1, 0);
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<System.TimeSpan?>(this, next => value = next));
            parameters.Add(p => p.ShowSteppers, true);
            parameters.Add(p => p.Step, 15);
        });

        var decrementButton = cut.FindAll("button")[1];
        decrementButton.Click();

        Assert.Equal(new System.TimeSpan(0, 0, 45), value);
    }

    [Fact]
    public void Clamp_Min_ClampsValue()
    {
        System.TimeSpan? value = null;
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Min, new System.TimeSpan(0, 10, 0));
            parameters.Add(p => p.Clamp, true);
            parameters.Add(p => p.ShowSteppers, true);
            parameters.Add(p => p.Step, 60);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<System.TimeSpan?>(this, next => value = next));
        });

        var decrementButton = cut.FindAll("button")[1];
        decrementButton.Click();

        Assert.Equal(new System.TimeSpan(0, 10, 0), value);
    }

    [Fact]
    public void Clamp_Max_ClampsValue()
    {
        System.TimeSpan? value = new System.TimeSpan(0, 59, 0);
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.Max, new System.TimeSpan(1, 0, 0));
            parameters.Add(p => p.Clamp, true);
            parameters.Add(p => p.ShowSteppers, true);
            parameters.Add(p => p.Step, 120);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<System.TimeSpan?>(this, next => value = next));
        });

        var incrementButton = cut.FindAll("button")[0];
        incrementButton.Click();

        Assert.Equal(new System.TimeSpan(1, 0, 0), value);
    }

    [Fact]
    public void Change_FiresOnSegmentChange()
    {
        System.TimeSpan? changedValue = null;
        var cut = Render<XFTimeSpan>(parameters =>
        {
            parameters.Add(p => p.Change, EventCallback.Factory.Create<System.TimeSpan?>(this, next => changedValue = next));
        });

        var hoursInput = cut.FindAll("input")[0];
        hoursInput.Change("5");

        Assert.Equal(new System.TimeSpan(5, 0, 0), changedValue);
    }
}
