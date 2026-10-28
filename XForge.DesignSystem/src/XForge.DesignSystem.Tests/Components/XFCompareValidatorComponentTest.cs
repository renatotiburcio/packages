#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFCompareValidatorComponentTest : XForgeTestContext
{
    public XFCompareValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_EqualOperator_ValidValue_NoError()
    {
        var model = new CompareModel { Password = "secret", ConfirmPassword = "secret" };
        var ec = RenderWithValidator<XFCompareValidator>(model, () => model.ConfirmPassword, b =>
        {
            b.AddAttribute(5, "Text", "Passwords must match.");
            b.AddAttribute(6, "Value", "secret");
            b.AddAttribute(7, "Operator", XFCompareOperator.Equal);
        });
        ec.Validate();
        Assert.DoesNotContain("Passwords must match.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_EqualOperator_InvalidValue_ShowsError()
    {
        var model = new CompareModel { ConfirmPassword = "different" };
        var ec = RenderWithValidator<XFCompareValidator>(model, () => model.ConfirmPassword, b =>
        {
            b.AddAttribute(5, "Text", "Passwords must match.");
            b.AddAttribute(6, "Value", "secret");
            b.AddAttribute(7, "Operator", XFCompareOperator.Equal);
        });
        ec.Validate();
        Assert.Contains("Passwords must match.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_NotEqualOperator_ValidValue_NoError()
    {
        var model = new CompareModel { Username = "user1" };
        var ec = RenderWithValidator<XFCompareValidator>(model, () => model.Username, b =>
        {
            b.AddAttribute(5, "Text", "Must be different.");
            b.AddAttribute(6, "Value", "admin");
            b.AddAttribute(7, "Operator", XFCompareOperator.NotEqual);
        });
        ec.Validate();
        Assert.DoesNotContain("Must be different.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_GreaterThanOperator_ValidValue_NoError()
    {
        var model = new CompareModel { Age = 25 };
        var ec = RenderWithValidator<XFCompareValidator>(model, () => model.Age, b =>
        {
            b.AddAttribute(5, "Text", "Must be greater than 18.");
            b.AddAttribute(6, "Value", 18.0);
            b.AddAttribute(7, "Operator", XFCompareOperator.GreaterThan);
        });
        ec.Validate();
        Assert.DoesNotContain("Must be greater than 18.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_BothNull_EqualOperator_NoError()
    {
        var model = new CompareModel();
        var ec = RenderWithValidator<XFCompareValidator>(model, () => model.Username, b =>
        {
            b.AddAttribute(5, "Text", "Must be equal.");
            b.AddAttribute(6, "Value", (object?)null);
            b.AddAttribute(7, "Operator", XFCompareOperator.Equal);
        });
        ec.Validate();
        Assert.DoesNotContain("Must be equal.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        CompareModel model,
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

    public class CompareModel
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Username { get; set; }
        public double Age { get; set; }
    }
}

