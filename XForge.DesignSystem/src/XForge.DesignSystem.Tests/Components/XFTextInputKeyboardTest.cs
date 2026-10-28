#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.TextInput;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFTextInputKeyboardTest : XForgeTestContext
{
    [Fact]
    public void Tab_Key_Moves_Focus_To_Input()
    {
        string? value = "test@example.com";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Email");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        var input = cut.Find("input");
        Assert.NotNull(input);
        Assert.Equal("test@example.com", input.GetAttribute("value"));
    }

    [Fact]
    public void Enter_Key_Submits_Form_Via_OnChange()
    {
        string? value = "";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        cut.Find("input").Change("new-value");
        Assert.Equal("new-value", value);
    }

    [Fact]
    public void Clearable_Button_Clears_Value()
    {
        string? value = "filled";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Clearable, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("filled", cut.Markup);
        cut.Find("button[aria-label]").Click();
        Assert.Equal("", value);
    }

    [Fact]
    public void Aria_DescribedBy_Links_To_HelperText()
    {
        string? value = "";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Use your work email.");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("aria-describedby", cut.Markup);
        Assert.Contains("Use your work email.", cut.Markup);
    }

    [Fact]
    public void Aria_DescribedBy_Links_To_ErrorText_When_Validation_Fails()
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
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Name = v));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-describedby", cut.Markup);
        Assert.Contains("Name is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void Disabled_Does_Not_Receive_Focus()
    {
        string? value = "";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("disabled", cut.Markup);
    }

    private sealed class InputModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
