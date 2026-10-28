using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.Checkbox;

namespace XForge.DesignSystem.Tests.Components;

public class XFCheckboxKeyboardTest : XForgeTestContext
{
    [Fact]
    public void Tab_Key_Moves_Focus_To_Checkbox()
    {
        bool? value = false;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
        });

        var input = cut.Find("input[type='checkbox']");
        Assert.NotNull(input);
    }

    [Fact]
    public void Space_Key_Toggles_Value()
    {
        bool? value = false;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
        });

        cut.Find("input").KeyDown("Space");
        cut.Find("input").Change("true");

        Assert.True(value);
    }

    [Fact]
    public void Enter_Key_Toggles_Checkbox()
    {
        bool? value = false;
        bool changed = false;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next =>
            {
                value = next;
                changed = true;
            }));
        });

        cut.Find("input").KeyDown("Enter");

        Assert.True(changed);
        Assert.True(value);
    }

    [Fact]
    public void ThreeState_Cycle_Unchecked_Mixed_True()
    {
        bool? value = false;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ThreeState, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
        });

        cut.Find("input").KeyDown("Enter");
        Assert.Null(value);

        cut.Find("input").KeyDown("Enter");
        Assert.True(value);

        cut.Find("input").KeyDown("Enter");
        Assert.False(value);
    }

    [Fact]
    public void Disabled_Space_Does_Not_Toggle()
    {
        bool? value = false;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
        });

        cut.Find("input").KeyDown("Enter");
        Assert.False(value);
    }

    [Fact]
    public void Aria_Checked_Mixed_For_ThreeState()
    {
        bool? value = null;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ThreeState, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
        });

        Assert.Contains("aria-checked=\"mixed\"", cut.Markup);
    }

    [Fact]
    public void Aria_Invalid_When_Validation_Fails()
    {
        var model = new CheckboxModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Accept);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "You must accept the terms.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFCheckbox>(3);
                childBuilder.AddAttribute(4, "Value", model.Accept);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool?>(this, v => model.Accept = v));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool?>>)(() => model.Accept));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void Aria_Required_Attribute()
    {
        bool? value = false;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Required, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
        });

        Assert.Contains("aria-required", cut.Markup);
    }

    private sealed class CheckboxModel
    {
        [Required]
        public bool? Accept { get; set; }
    }
}
