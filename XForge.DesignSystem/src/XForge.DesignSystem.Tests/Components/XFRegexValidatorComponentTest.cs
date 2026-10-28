#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFRegexValidatorComponentTest : XForgeTestContext
{
    public XFRegexValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_PatternMatch_NoError()
    {
        var model = new RegexModel { Code = "12345" };
        var ec = RenderWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Numeric only.");
            b.AddAttribute(6, "Pattern", @"^\d+$");
        });
        ec.Validate();
        Assert.DoesNotContain("Numeric only.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_PatternNoMatch_ShowsError()
    {
        var model = new RegexModel { Code = "abc" };
        var ec = RenderWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Numeric only.");
            b.AddAttribute(6, "Pattern", @"^\d+$");
        });
        ec.Validate();
        Assert.Contains("Numeric only.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_NullValue_NoError()
    {
        var model = new RegexModel();
        var ec = RenderWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Numeric only.");
            b.AddAttribute(6, "Pattern", @"^\d+$");
        });
        ec.Validate();
        Assert.DoesNotContain("Numeric only.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_DefaultPattern_AlwaysMatches()
    {
        var model = new RegexModel { Code = "anything" };
        var ec = RenderWithValidator<XFRegexValidator>(model, () => model.Code, b =>
        {
            b.AddAttribute(5, "Text", "Should not appear.");
        });
        ec.Validate();
        Assert.DoesNotContain("Should not appear.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        RegexModel model,
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

    public class RegexModel
    {
        public string? Code { get; set; }
    }
}

