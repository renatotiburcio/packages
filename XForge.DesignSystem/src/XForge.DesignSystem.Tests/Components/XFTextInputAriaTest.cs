using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.TextInput;

namespace XForge.DesignSystem.Tests.Components;

public class XFTextInputAriaTest : BunitContext
{
    [Fact]
    public void AriaInvalid_WhenValidationFails_IsTrue()
    {
        var model = new InputModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Name);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Name is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFTextInput>(3);
                childBuilder.AddAttribute(4, "Value", model.Name);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaInvalid_WhenValid_IsFalse()
    {
        var model = new InputModel { Name = "Valid" };
        var editContext = new EditContext(model);

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFTextInput>(3);
                childBuilder.AddAttribute(4, "Value", model.Name);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaRequired_WhenRequired_IsTrue()
    {
        var cut = Render<XFTextInput>(parameters => parameters.Add(p => p.Required, true));

        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-required"));
        Assert.Equal("false", input.GetAttribute("aria-readonly"));
    }

    [Fact]
    public void AriaDescribedBy_WhenHelperTextPresent_LinksToHelper()
    {
        string? value = null;
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Email");
            parameters.Add(p => p.HelperText, "Use your work email.");
            parameters.Add(p => p.ValueExpression, () => value);
        });

        var input = cut.Find("input");
        var describedBy = input.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains(describedBy, cut.Markup);
        Assert.Contains("Use your work email.", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_WhenValidationFails_LinksToErrorOverridesHelper()
    {
        var model = new InputModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Name);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Name is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFTextInput>(3);
                childBuilder.AddAttribute(4, "Value", model.Name);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                childBuilder.AddAttribute(7, "HelperText", "Enter your name");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var input = cut.Find("input");
        var describedBy = input.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains("Name is required.", cut.Markup);
        Assert.DoesNotContain("Enter your name", cut.Markup);
    }

    [Fact]
    public void AriaLabel_WhenSet_IsPresent()
    {
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Search input");
        });

        Assert.Contains("aria-label=\"Search input\"", cut.Markup);
    }

    private sealed class InputModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
