#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFCustomValidatorComponentTest : XForgeTestContext
{
    public XFCustomValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_ValidatorReturnsTrue_NoError()
    {
        var model = new CustomModel { Value = "valid" };
        var ec = RenderWithValidator<XFCustomValidator>(model, () => model.Value, b =>
        {
            b.AddAttribute(5, "Text", "Invalid value.");
            b.AddAttribute(6, "Validator", (Func<object?, bool>)(val => val?.ToString() != "admin"));
        });
        ec.Validate();
        Assert.DoesNotContain("Invalid value.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_ValidatorReturnsFalse_ShowsError()
    {
        var model = new CustomModel { Value = "admin" };
        var ec = RenderWithValidator<XFCustomValidator>(model, () => model.Value, b =>
        {
            b.AddAttribute(5, "Text", "Invalid value.");
            b.AddAttribute(6, "Validator", (Func<object?, bool>)(val => val?.ToString() != "admin"));
        });
        ec.Validate();
        Assert.Contains("Invalid value.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_DefaultValidator_AlwaysPasses()
    {
        var model = new CustomModel { Value = "anything" };
        var ec = RenderWithValidator<XFCustomValidator>(model, () => model.Value, b =>
        {
            b.AddAttribute(5, "Text", "Should not appear.");
        });
        ec.Validate();
        Assert.DoesNotContain("Should not appear.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_ComplexValidator_ChecksLength()
    {
        var model = new CustomModel { Value = "ab" };
        var ec = RenderWithValidator<XFCustomValidator>(model, () => model.Value, b =>
        {
            b.AddAttribute(5, "Text", "Too short.");
            b.AddAttribute(6, "Validator", (Func<object?, bool>)(val => val?.ToString()?.Length >= 3));
        });
        ec.Validate();
        Assert.Contains("Too short.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        CustomModel model,
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

    public class CustomModel
    {
        public string? Value { get; set; }
    }
}

