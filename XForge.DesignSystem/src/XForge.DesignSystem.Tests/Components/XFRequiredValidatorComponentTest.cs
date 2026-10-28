#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFRequiredValidatorComponentTest : XForgeTestContext
{
    public XFRequiredValidatorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_NullValue_ShowsError()
    {
        var model = new RequiredModel();
        var ec = RenderWithValidator<XFRequiredValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Required.");
        });
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_EmptyString_ShowsError()
    {
        var model = new RequiredModel { Name = "" };
        var ec = RenderWithValidator<XFRequiredValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Required.");
        });
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_ValidValue_NoError()
    {
        var model = new RequiredModel { Name = "John" };
        var ec = RenderWithValidator<XFRequiredValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Required.");
        });
        ec.Validate();
        Assert.DoesNotContain("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_WhitespaceOnly_ShowsError()
    {
        var model = new RequiredModel { Name = "   " };
        var ec = RenderWithValidator<XFRequiredValidator>(model, () => model.Name, b =>
        {
            b.AddAttribute(5, "Text", "Required.");
        });
        ec.Validate();
        Assert.Contains("Required.", ec.GetValidationMessages());
    }

    [Fact]
    public void Render_DefaultValue_ShowsError()
    {
        var model = new RequiredModel { Age = 0 };
        var ec = RenderWithValidator<XFRequiredValidator>(model, () => model.Age, b =>
        {
            b.AddAttribute(5, "Text", "Age required.");
            b.AddAttribute(6, "DefaultValue", (object)0);
        });
        ec.Validate();
        Assert.Contains("Age required.", ec.GetValidationMessages());
    }

    private EditContext RenderWithValidator<TValidator>(
        RequiredModel model,
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

    public class RequiredModel
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}

