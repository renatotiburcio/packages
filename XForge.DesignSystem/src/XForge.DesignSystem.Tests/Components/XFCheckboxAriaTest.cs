using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.Checkbox;

namespace XForge.DesignSystem.Tests.Components;

public class XFCheckboxAriaTest : BunitContext
{
    [Fact]
    public void AriaChecked_Unchecked_IsFalse()
    {
        var cut = Render<XFCheckbox>(parameters => parameters.Add(p => p.Value, false));

        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_Checked_IsTrue()
    {
        var cut = Render<XFCheckbox>(parameters => parameters.Add(p => p.Value, true));

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_ThreeStateNull_IsMixed()
    {
        bool? value = null;
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.ThreeState, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<bool?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("aria-checked=\"mixed\"", cut.Markup);
    }

    [Fact]
    public void AriaInvalid_WhenValidationFails_IsTrue()
    {
        var model = new CheckboxModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Accepted);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Acceptance is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFCheckbox>(3);
                childBuilder.AddAttribute(4, "Value", model.Accepted);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool?>(this, value => model.Accepted = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool?>>)(() => model.Accepted));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaInvalid_WhenValid_IsFalse()
    {
        var model = new CheckboxModel { Accepted = true };
        var editContext = new EditContext(model);

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFCheckbox>(3);
                childBuilder.AddAttribute(4, "Value", model.Accepted);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool?>(this, value => model.Accepted = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool?>>)(() => model.Accepted));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("aria-invalid=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaRequired_WhenRequired_IsTrue()
    {
        var cut = Render<XFCheckbox>(parameters => parameters.Add(p => p.Required, true));

        Assert.Contains("aria-required", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_WhenDescriptionPresent_LinksToDescription()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.Label, "Accept terms");
            parameters.Add(p => p.Description, "Required before publishing.");
        });

        var input = cut.Find("input");
        var describedBy = input.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains(describedBy, cut.Markup);
        Assert.Contains("Required before publishing.", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_WhenValidationFails_LinksToError()
    {
        var model = new CheckboxModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Accepted);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Acceptance is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFCheckbox>(3);
                childBuilder.AddAttribute(4, "Value", model.Accepted);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<bool?>(this, value => model.Accepted = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<bool?>>)(() => model.Accepted));
                childBuilder.AddAttribute(7, "Description", "Some description");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var input = cut.Find("input");
        var describedBy = input.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains("Acceptance is required.", cut.Markup);
        Assert.DoesNotContain("Some description", cut.Markup);
    }

    [Fact]
    public void AriaLabel_WhenSet_IsPresent()
    {
        var cut = Render<XFCheckbox>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Toggle agreement");
        });

        Assert.Contains("aria-label=\"Toggle agreement\"", cut.Markup);
    }

    private sealed class CheckboxModel
    {
        [Required]
        public bool? Accepted { get; set; }
    }
}
