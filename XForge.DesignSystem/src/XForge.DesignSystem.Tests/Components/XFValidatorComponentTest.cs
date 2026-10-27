using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;

namespace XForge.DesignSystem.Tests.Components;

public class XFValidatorComponentTest : BunitContext
{
    [Fact]
    public void RequiredValidator_NullValue_ShowsError()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Name, b => b.AddAttribute(5, "Text", "Required."));
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void RequiredValidator_EmptyString_ShowsError()
    {
        var model = new TestModel { Name = "" };
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Name, b => b.AddAttribute(5, "Text", "Required."));
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void RequiredValidator_ValidValue_NoError()
    {
        var model = new TestModel { Name = "John" };
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Name, b => b.AddAttribute(5, "Text", "Required."));
        ec.Validate();
        Assert.DoesNotContain("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void RequiredValidator_DefaultValue_ShowsError()
    {
        var model = new TestModel { Age = 0 };
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Age, b =>
        {
            b.AddAttribute(5, "Text", "Age required.");
            b.AddAttribute(6, "DefaultValue", (object)0);
        });
        ec.Validate();
        Assert.Contains("Age required.", ec.GetValidationMessages());
    }

    [Fact]
    public void LengthValidator_TooShort_ShowsError()
    {
        var model = new TestModel { Name = "ab" };
        var ec = CreateWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Min 3.");
            b.AddAttribute(6, "Min", 3);
        });
        ec.Validate();
        Assert.Contains("Min 3.", ec.GetValidationMessages());
    }

    [Fact]
    public void LengthValidator_TooLong_ShowsError()
    {
        var model = new TestModel { Name = "This is a very long name" };
        var ec = CreateWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Max 10.");
            b.AddAttribute(6, "Max", 10);
        });
        ec.Validate();
        Assert.Contains("Max 10.", ec.GetValidationMessages());
    }

    [Fact]
    public void LengthValidator_WithinRange_NoError()
    {
        var model = new TestModel { Name = "John" };
        var ec = CreateWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Length error.");
            b.AddAttribute(6, "Min", 2);
            b.AddAttribute(7, "Max", 20);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void EmailValidator_InvalidEmail_ShowsError()
    {
        var model = new TestModel { Email = "not-an-email" };
        var ec = CreateWithValidator<XFEmailValidator>(model, () => model.Email, b => b.AddAttribute(5, "Text", "Bad email."));
        ec.Validate();
        Assert.Contains("Bad email.", ec.GetValidationMessages());
    }

    [Fact]
    public void EmailValidator_ValidEmail_NoError()
    {
        var model = new TestModel { Email = "user@example.com" };
        var ec = CreateWithValidator<XFEmailValidator>(model, () => model.Email, b => b.AddAttribute(5, "Text", "Bad email."));
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_BelowMin_ShowsError()
    {
        var model = new TestModel { Score = 5 };
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Range 10-100.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.Contains("Range 10-100.", ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_AboveMax_ShowsError()
    {
        var model = new TestModel { Score = 150 };
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Range 10-100.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.Contains("Range 10-100.", ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_WithinRange_NoError()
    {
        var model = new TestModel { Score = 50 };
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Range error.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void RegexValidator_NoMatch_ShowsError()
    {
        var model = new TestModel { Code = "abc" };
        var ec = CreateWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Numeric only.");
            b.AddAttribute(6, "Pattern", @"^\d+$");
        });
        ec.Validate();
        Assert.Contains("Numeric only.", ec.GetValidationMessages());
    }

    [Fact]
    public void RegexValidator_Match_NoError()
    {
        var model = new TestModel { Code = "12345" };
        var ec = CreateWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Numeric only.");
            b.AddAttribute(6, "Pattern", @"^\d+$");
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_Equal_NoError()
    {
        var model = new TestModel { ConfirmPassword = "secret" };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.ConfirmPassword, b =>
        {
            b.AddAttribute(5, "Text", "Must match.");
            b.AddAttribute(6, "Value", "secret");
            b.AddAttribute(7, "Operator", XFCompareOperator.Equal);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_NotEqual_ShowsError()
    {
        var model = new TestModel { ConfirmPassword = "different" };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.ConfirmPassword, b =>
        {
            b.AddAttribute(5, "Text", "Must match.");
            b.AddAttribute(6, "Value", "secret");
            b.AddAttribute(7, "Operator", XFCompareOperator.Equal);
        });
        ec.Validate();
        Assert.Contains("Must match.", ec.GetValidationMessages());
    }

    [Fact]
    public void CustomValidator_ReturnsFalse_ShowsError()
    {
        var model = new TestModel { Name = "admin" };
        var ec = CreateWithValidator<XFCustomValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Not 'admin'.");
            b.AddAttribute(6, "Validator", (Func<object?, bool>)(val => val?.ToString() != "admin"));
        });
        ec.Validate();
        Assert.Contains("Not 'admin'.", ec.GetValidationMessages());
    }

    [Fact]
    public void CustomValidator_ReturnsTrue_NoError()
    {
        var model = new TestModel { Name = "user" };
        var ec = CreateWithValidator<XFCustomValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Not 'admin'.");
            b.AddAttribute(6, "Validator", (Func<object?, bool>)(val => val?.ToString() != "admin"));
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    // ==================== CompareValidator — Extended Coverage ====================

    [Fact]
    public void CompareValidator_BothNull_EqualPasses()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Must be equal.");
            b.AddAttribute(6, "Value", (object?)null);
            b.AddAttribute(7, "Operator", XFCompareOperator.Equal);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_BothNull_NotEqualFails()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Must differ.");
            b.AddAttribute(6, "Value", (object?)null);
            b.AddAttribute(7, "Operator", XFCompareOperator.NotEqual);
        });
        ec.Validate();
        Assert.Contains("Must differ.", ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_ValueNull_NotEqualPasses()
    {
        var model = new TestModel { Name = "hello" };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Must differ.");
            b.AddAttribute(6, "Value", (object?)null);
            b.AddAttribute(7, "Operator", XFCompareOperator.NotEqual);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_TargetNull_NotEqualPasses()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.ConfirmPassword, b =>
        {
            b.AddAttribute(5, "Text", "Must differ.");
            b.AddAttribute(6, "Value", "secret");
            b.AddAttribute(7, "Operator", XFCompareOperator.NotEqual);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_GreaterThan_Passes()
    {
        var model = new TestModel { Score = 20 };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Must be greater.");
            b.AddAttribute(6, "Value", 10.0);
            b.AddAttribute(7, "Operator", XFCompareOperator.GreaterThan);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_GreaterThan_Fails()
    {
        var model = new TestModel { Score = 5 };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Must be greater.");
            b.AddAttribute(6, "Value", 10.0);
            b.AddAttribute(7, "Operator", XFCompareOperator.GreaterThan);
        });
        ec.Validate();
        Assert.Contains("Must be greater.", ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_GreaterThanOrEqual_Passes()
    {
        var model = new TestModel { Score = 10 };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Must be >= 10.");
            b.AddAttribute(6, "Value", 10.0);
            b.AddAttribute(7, "Operator", XFCompareOperator.GreaterThanOrEqual);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_LessThan_Passes()
    {
        var model = new TestModel { Score = 5 };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Must be less.");
            b.AddAttribute(6, "Value", 10.0);
            b.AddAttribute(7, "Operator", XFCompareOperator.LessThan);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_LessThanOrEqual_Passes()
    {
        var model = new TestModel { Score = 10 };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Must be <= 10.");
            b.AddAttribute(6, "Value", 10.0);
            b.AddAttribute(7, "Operator", XFCompareOperator.LessThanOrEqual);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void CompareValidator_NotEqual_Passes()
    {
        var model = new TestModel { ConfirmPassword = "other" };
        var ec = CreateWithValidator<XFCompareValidator>(model, () => model.ConfirmPassword, b =>
        {
            b.AddAttribute(5, "Text", "Must differ.");
            b.AddAttribute(6, "Value", "secret");
            b.AddAttribute(7, "Operator", XFCompareOperator.NotEqual);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    // ==================== RangeValidator — Extended Coverage ====================

    [Fact]
    public void RangeValidator_NullValue_AllowNullTrue_NoError()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Range error.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
            b.AddAttribute(8, "AllowNull", true);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_NullValue_AllowNullFalse_ShowsError()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Value required.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
            b.AddAttribute(8, "AllowNull", false);
        });
        ec.Validate();
        Assert.Contains("Value required.", ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_MinOnly_BelowMin_ShowsError()
    {
        var model = new TestModel { Score = 3 };
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Min 10.");
            b.AddAttribute(6, "Min", 10.0);
        });
        ec.Validate();
        Assert.Contains("Min 10.", ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_MaxOnly_AboveMax_ShowsError()
    {
        var model = new TestModel { Score = 200 };
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Max 100.");
            b.AddAttribute(6, "Max", 100.0);
        });
        ec.Validate();
        Assert.Contains("Max 100.", ec.GetValidationMessages());
    }

    [Fact]
    public void RangeValidator_ExactBoundary_Passes()
    {
        var model = new TestModel { Score = 10 };
        var ec = CreateWithValidator<XFRangeValidator>(model, () => model.Score, b =>
        {
            b.AddAttribute(5, "Text", "Range error.");
            b.AddAttribute(6, "Min", 10.0);
            b.AddAttribute(7, "Max", 100.0);
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    // ==================== RequiredValidator — Extended Coverage ====================

    [Fact]
    public void RequiredValidator_WhitespaceOnly_ShowsError()
    {
        var model = new TestModel { Name = "   " };
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Name, b => b.AddAttribute(5, "Text", "Required."));
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void RequiredValidator_DefaultValueNotSet_NonZeroPasses()
    {
        var model = new TestModel { Age = 25 };
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Age, b => b.AddAttribute(5, "Text", "Age required."));
        ec.Validate();
        Assert.DoesNotContain("Age required.", ec.GetValidationMessages());
    }

    // ==================== EmailValidator — Extended Coverage ====================

    [Fact]
    public void EmailValidator_NullValue_NoError()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFEmailValidator>(model, () => model.Email, b => b.AddAttribute(5, "Text", "Bad email."));
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    // ==================== RegexValidator — Extended Coverage ====================

    [Fact]
    public void RegexValidator_NullValue_NoError()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Numeric only.");
            b.AddAttribute(6, "Pattern", @"^\d+$");
        });
        ec.Validate();
        Assert.Empty(ec.GetValidationMessages());
    }

    // ==================== LengthValidator — Extended Coverage ====================

    [Fact]
    public void LengthValidator_MinOnly_TooShort_ShowsError()
    {
        var model = new TestModel { Name = "a" };
        var ec = CreateWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Min 3.");
            b.AddAttribute(6, "Min", 3);
        });
        ec.Validate();
        Assert.Contains("Min 3.", ec.GetValidationMessages());
    }

    [Fact]
    public void LengthValidator_MaxOnly_TooLong_ShowsError()
    {
        var model = new TestModel { Name = "This is way too long" };
        var ec = CreateWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Max 5.");
            b.AddAttribute(6, "Max", 5);
        });
        ec.Validate();
        Assert.Contains("Max 5.", ec.GetValidationMessages());
    }

    // ==================== ValidatorBase — Field Change & Re-validation ====================

    [Fact]
    public void RequiredValidator_AfterFixingValue_ClearsError()
    {
        var model = new TestModel();
        var ec = CreateWithValidator<XFRequiredValidator>(model, () => model.Name, b => b.AddAttribute(5, "Text", "Required."));
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());

        model.Name = "Now valid";
        ec.NotifyFieldChanged(global::Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(() => model.Name));
        ec.Validate();
        Assert.DoesNotContain("Required.", ec.GetValidationMessages());
    }

    private EditContext CreateWithValidator<TValidator>(
        TestModel model,
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

    public class TestModel
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        public double Score { get; set; }
        public string? Code { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
