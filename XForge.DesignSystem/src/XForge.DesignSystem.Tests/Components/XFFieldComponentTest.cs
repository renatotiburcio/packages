using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.Field;

namespace XForge.DesignSystem.Tests.Components;

public class XFFieldComponentTest : BunitContext
{
    [Fact]
    public void Field_RendersLabelHelperAndRequired()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Label, "Nome");
            parameters.Add(p => p.For, "name");
            parameters.Add(p => p.Required, true);
            parameters.Add(p => p.HelperText, "Informe o nome completo.");
            parameters.AddChildContent("<input id=\"name\" />");
        });

        Assert.Contains("for=\"name\"", cut.Markup);
        Assert.Contains("Nome", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
        Assert.Contains("Informe o nome completo.", cut.Markup);
    }

    [Fact]
    public void Field_ErrorTextOverridesHelper()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Helper");
            parameters.Add(p => p.ErrorText, "Erro");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("role=\"alert\"", cut.Markup);
        Assert.Contains("Erro", cut.Markup);
        Assert.DoesNotContain("Helper", cut.Markup);
    }

    [Fact]
    public void Field_RendersAdornmentsInsideControlShell()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.StartAdornment, "R$");
            parameters.Add(p => p.EndAdornment, "BRL");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("R$", cut.Markup);
        Assert.Contains("BRL", cut.Markup);
        Assert.Contains("focus-within:border", cut.Markup);
    }

    [Fact]
    public void Fieldset_RendersLegendDescriptionAndDisabled()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Legend, "Preferencias");
            parameters.Add(p => p.Description, "Escolha os canais.");
            parameters.Add(p => p.Disabled, true);
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("<fieldset", cut.Markup);
        Assert.Contains("disabled", cut.Markup);
        Assert.Contains("Preferencias", cut.Markup);
        Assert.Contains("Escolha os canais.", cut.Markup);
    }

    [Fact]
    public void Field_ReadsEditContextValidationMessage()
    {
        var model = new FieldModel();
        var editContext = new EditContext(model);
        var messageStore = new ValidationMessageStore(editContext);
        var field = editContext.Field(nameof(FieldModel.Name));
        System.Linq.Expressions.Expression<Func<object?>> valueExpression = () => model.Name;
        messageStore.Add(field, "Nome obrigatorio.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFField>(3);
                childBuilder.AddAttribute(4, "ValueExpression", valueExpression);
                childBuilder.AddContent(5, "<input />");
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Nome obrigatorio.", cut.Markup);
    }

    [Fact]
    public void Field_RendersWithoutLabel()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.AddChildContent("<input />");
        });

        Assert.DoesNotContain("<label", cut.Markup);
    }

    [Fact]
    public void Field_HorizontalOrientation_GridColumns()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Label, "Name");
            parameters.Add(p => p.Orientation, XFFieldOrientation.Horizontal);
            parameters.Add(p => p.For, "name");
            parameters.AddChildContent("<input id=\"name\" />");
        });

        Assert.Contains("sm:grid-cols-[12rem_1fr]", cut.Markup);
    }

    [Fact]
    public void Field_VerticalOrientation_NoGridColumns()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Label, "Name");
            parameters.Add(p => p.Orientation, XFFieldOrientation.Vertical);
            parameters.AddChildContent("<input />");
        });

        Assert.DoesNotContain("grid-cols-[12rem_1fr]", cut.Markup);
    }

    [Fact]
    public void Field_RequiredFalse_HidesAsterisk()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Label, "Name");
            parameters.Add(p => p.Required, false);
            parameters.AddChildContent("<input />");
        });

        Assert.DoesNotContain("aria-hidden=\"true\"*", cut.Markup);
    }

    [Fact]
    public void Field_NoErrorNoHelper_ShowsNothingBelowControl()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.AddChildContent("<input />");
        });

        Assert.DoesNotContain("role=\"alert\"", cut.Markup);
        Assert.DoesNotContain("text-xs", cut.Markup);
    }

    [Fact]
    public void Field_WithCustomClass_IncludedInRoot()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Class, "custom-field");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("custom-field", cut.Markup);
    }

    [Fact]
    public void Field_WithStyle_IncludedInRoot()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Style, "margin-top: 1rem");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("margin-top: 1rem", cut.Markup);
    }

    [Fact]
    public void Field_WithId_AppliesToRoot()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.Id, "field-123");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("id=\"field-123\"", cut.Markup);
    }

    [Fact]
    public void Fieldset_RendersWithChildContent()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Legend, "Options");
            parameters.AddChildContent("<button>Submit</button>");
        });

        Assert.Contains("Submit", cut.Markup);
    }

    [Fact]
    public void Fieldset_NoBorder_BorderedFalse()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Legend, "No border");
            parameters.Add(p => p.Bordered, false);
            parameters.AddChildContent("<input />");
        });

        Assert.DoesNotContain("border-border-light", cut.Markup);
    }

    [Fact]
    public void Fieldset_WithCustomClass()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Class, "fieldset-custom");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("fieldset-custom", cut.Markup);
    }

    [Fact]
    public void Fieldset_RendersLegendContentFragment()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.LegendContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(1, "Custom Legend");
                builder.CloseElement();
            }));
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("Custom Legend", cut.Markup);
    }

    [Fact]
    public void Fieldset_WithStyle()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Style, "padding: 2rem");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("padding: 2rem", cut.Markup);
    }

    [Fact]
    public void Fieldset_DescriptionShown()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Legend, "Group");
            parameters.Add(p => p.Description, "This is a description");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("This is a description", cut.Markup);
    }

    [Fact]
    public void Field_OnlyChildContent_RendersContent()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.AddChildContent("<input placeholder=\"Email\" />");
        });

        Assert.Contains("Email", cut.Markup);
    }

    [Fact]
    public void Field_WithStartAdornmentOnly_RendersShell()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.StartAdornment, "@");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains("@", cut.Markup);
        Assert.Contains("focus-within:border", cut.Markup);
    }

    [Fact]
    public void Field_WithEndAdornmentOnly_RendersShell()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.EndAdornment, ".com");
            parameters.AddChildContent("<input />");
        });

        Assert.Contains(".com", cut.Markup);
        Assert.Contains("focus-within:border", cut.Markup);
    }

    [Fact]
    public void Field_ErrorText_HasAlertRole()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.ErrorText, "Required field");
            parameters.AddChildContent("<input />");
        });

        var alert = cut.Find("[role=\"alert\"]");
        Assert.Equal("Required field", alert.TextContent);
    }

    [Fact]
    public void Field_HelperText_HasNoAlertRole()
    {
        var cut = Render<XFField>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Help text");
            parameters.AddChildContent("<input />");
        });

        Assert.DoesNotContain("role=\"alert\"", cut.Markup);
        Assert.Contains("Help text", cut.Markup);
    }

    private sealed class FieldModel
    {
        public string? Name { get; set; }
    }
}
