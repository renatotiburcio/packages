#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFRangeValidatorComponentTest : XForgeTestContext
{
    public XFRangeValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithinRange_NoError()
    {
        var model = new RangeModel { Score = 50 };
        var ec = RenderWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Range error.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.DoesNotContain("Range error.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_BelowMin_ShowsError()
    {
        var model = new RangeModel { Score = 5 };
        var ec = RenderWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Below minimum.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.Contains("Below minimum.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_AboveMax_ShowsError()
    {
        var model = new RangeModel { Score = 150 };
        var ec = RenderWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Above maximum.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.Contains("Above maximum.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_NullValue_AllowNullTrue_NoError()
    {
        var model = new RangeModel();
        var ec = RenderWithValidator<XFRangeValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Range error.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
            b.AddAttribute(8, "AllowNull", true);
        });
        ec.Validate();
        Assert.DoesNotContain("Range error.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_NullValue_AllowNullFalse_ShowsError()
    {
        var model = new RangeModel();
        var ec = RenderWithValidator<XFRangeValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Value required.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
            b.AddAttribute(8, "AllowNull", false);
        });
        ec.Validate();
        Assert.Contains("Value required.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        RangeModel model,
        System.Linq.Expressions.Expression<Func<object?>> forExpr,
        Action<RenderTreeBuilder>? configure = null)
        where TValidator : IComponent
    {
        var ec = new EditContext(model);
        Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", ec);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(cb =>
            {
                cb.OpenComponent<TValidator>(3);
                cb.AddAttribute(4, "For", forExpr);
                configure?.Invoke(cb);
                cb.CloseComponent();
            }));
            builder.CloseComponent();
        });
        return ec;
    }

    public class RangeModel
    {
        public double Score { get; set; }
        public string? Name { get; set; }
    }
}

