#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFLengthValidatorComponentTest : XForgeTestContext
{
    public XFLengthValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithinRange_NoError()
    {
        var model = new LengthModel { Name = "John" };
        var ec = RenderWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Length error.");
            b.AddAttribute(6, "Min", 2);
            b.AddAttribute(7, "Max", 20);
        });
        ec.Validate();
        Assert.DoesNotContain("Length error.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_TooShort_ShowsError()
    {
        var model = new LengthModel { Name = "a" };
        var ec = RenderWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Too short.");
            b.AddAttribute(6, "Min", 3);
        });
        ec.Validate();
        Assert.Contains("Too short.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_TooLong_ShowsError()
    {
        var model = new LengthModel { Name = "This is a very long name" };
        var ec = RenderWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Too long.");
            b.AddAttribute(6, "Max", 10);
        });
        ec.Validate();
        Assert.Contains("Too long.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_NullValue_NoError()
    {
        var model = new LengthModel();
        var ec = RenderWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Length error.");
            b.AddAttribute(6, "Min", 3);
            b.AddAttribute(7, "Max", 20);
        });
        ec.Validate();
        Assert.DoesNotContain("Length error.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_MinOnly_AtBoundary_NoError()
    {
        var model = new LengthModel { Name = "abc" };
        var ec = RenderWithValidator<XFLengthValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Min 3.");
            b.AddAttribute(6, "Min", 3);
        });
        ec.Validate();
        Assert.DoesNotContain("Min 3.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        LengthModel model,
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

    public class LengthModel
    {
        public string? Name { get; set; }
    }
}

