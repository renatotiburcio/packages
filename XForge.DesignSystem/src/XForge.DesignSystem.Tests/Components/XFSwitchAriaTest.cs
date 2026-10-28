using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFSwitchAriaTest : XForgeTestContext
{
    public XFSwitchAriaTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Role_IsSwitch()
    {
        var cut = Render<XFSwitch>();

        Assert.Contains("role=\"switch\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_Checked_IsTrue()
    {
        var cut = Render<XFSwitch>(parameters => parameters.Add(p => p.Value, true));

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_Unchecked_IsFalse()
    {
        var cut = Render<XFSwitch>(parameters => parameters.Add(p => p.Value, false));

        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaInvalid_WhenValidationFails_IsTrue()
    {
        var model = new SwitchModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Enabled);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Switch is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFSwitch>(3);
                childBuilder.AddAttribute(4, "Value", model.Enabled);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool>(this, value => model.Enabled = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool>>)(() => model.Enabled));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaRequired_WhenRequired_IsTrue()
    {
        var cut = Render<XFSwitch>(parameters => parameters.Add(p => p.Required, true));

        Assert.Contains("aria-required", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_WhenDescriptionPresent_LinksToDescription()
    {
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.Label, "Notifications");
            parameters.Add(p => p.Description, "Send updates.");
        });

        var button = cut.Find("button[role='switch']");
        var describedBy = button.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains(describedBy, cut.Markup);
        Assert.Contains("Send updates.", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_WhenValidationFails_LinksToError()
    {
        var model = new SwitchModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Enabled);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Switch is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFSwitch>(3);
                childBuilder.AddAttribute(4, "Value", model.Enabled);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool>(this, value => model.Enabled = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool>>)(() => model.Enabled));
                childBuilder.AddAttribute(7, "Description", "Some description");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var button = cut.Find("button[role='switch']");
        var describedBy = button.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains("Switch is required.", cut.Markup);
        Assert.DoesNotContain("Some description", cut.Markup);
    }

    [Fact]
    public void AriaLabel_WhenSet_IsPresent()
    {
        var cut = Render<XFSwitch>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Toggle notifications");
        });

        Assert.Contains("aria-label=\"Toggle notifications\"", cut.Markup);
    }

    private sealed class SwitchModel
    {
        [Range(typeof(bool), "true", "true")]
        public bool Enabled { get; set; }
    }
}
