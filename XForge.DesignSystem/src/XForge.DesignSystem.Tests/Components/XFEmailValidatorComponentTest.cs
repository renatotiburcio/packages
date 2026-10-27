#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFEmailValidatorComponentTest : Bunit.BunitContext
{
    public XFEmailValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_ValidEmail_NoError()
    {
        var model = new EmailModel { Email = "user@example.com" };
        var ec = RenderWithValidator<XFEmailValidator>(model, () => model.Email, b =>
        {
            b.AddAttribute(5, "Text", "Invalid email.");
        });
        ec.Validate();
        Assert.DoesNotContain("Invalid email.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_InvalidEmail_ShowsError()
    {
        var model = new EmailModel { Email = "not-an-email" };
        var ec = RenderWithValidator<XFEmailValidator>(model, () => model.Email, b =>
        {
            b.AddAttribute(5, "Text", "Invalid email.");
        });
        ec.Validate();
        Assert.Contains("Invalid email.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_NullValue_NoError()
    {
        var model = new EmailModel();
        var ec = RenderWithValidator<XFEmailValidator>(model, () => model.Email, b =>
        {
            b.AddAttribute(5, "Text", "Invalid email.");
        });
        ec.Validate();
        Assert.DoesNotContain("Invalid email.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_EmptyString_ShowsError()
    {
        var model = new EmailModel { Email = "" };
        var ec = RenderWithValidator<XFEmailValidator>(model, () => model.Email, b =>
        {
            b.AddAttribute(5, "Text", "Invalid email.");
        });
        ec.Validate();
        Assert.Contains("Invalid email.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        EmailModel model,
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

    public class EmailModel
    {
        public string? Email { get; set; }
    }
}

