using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFSwitchKeyboardTest : BunitContext
{
    public XFSwitchKeyboardTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Tab_Key_Moves_Focus_To_Switch()
    {
        bool value = false;
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, next => value = next));
        });

        var button = cut.Find("button[role='switch']");
        Assert.NotNull(button);
    }

    [Fact]
    public void Space_Key_Toggles_Value()
    {
        bool value = false;
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, next => value = next));
        });

        cut.Find("button[role='switch']").Click();

        Assert.True(value);
    }

    [Fact]
    public void Enter_Key_Toggles_Switch()
    {
        bool value = false;
        bool changed = false;
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, next =>
            {
                value = next;
                changed = true;
            }));
        });

        cut.Find("button[role='switch']").KeyDown("Enter");

        Assert.True(changed);
        Assert.True(value);
    }

    [Fact]
    public void Disabled_Space_Does_Not_Toggle()
    {
        bool value = false;
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, next => value = next));
        });

        cut.Find("button[role='switch']").KeyDown("Enter");
        Assert.False(value);
    }

    [Fact]
    public void Aria_Checked_InitialState()
    {
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, false);
        });

        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void Aria_Checked_WhenValueTrue()
    {
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Value, true);
        });

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    [Fact]
    public void Aria_Invalid_When_Validation_Fails()
    {
        var model = new SwitchModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Enabled);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "This field is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFSwitch>(3);
                childBuilder.AddAttribute(4, "Value", model.Enabled);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool>(this, v => model.Enabled = v));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool>>)(() => model.Enabled));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void Aria_Required_Attribute()
    {
        bool value = false;
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Required, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, next => value = next));
        });

        Assert.Contains("aria-required", cut.Markup);
    }

    private sealed class SwitchModel
    {
        [Required]
        public bool Enabled { get; set; }
    }
}
