using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;

namespace XForge.DesignSystem.Tests.Components;

public class XFValidationComponentTest : BunitContext
{
    [Fact]
    public void Message_RendersManualItems()
    {
        var cut = Render<XFValidationMessage>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "Campo obrigatorio." });
        });

        Assert.Contains("role=\"alert\"", cut.Markup);
        Assert.Contains("Campo obrigatorio.", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Summary_RendersEditContextMessages()
    {
        var model = new ValidationModel();
        var editContext = new EditContext(model);
        var store = new ValidationMessageStore(editContext);
        store.Add(editContext.Field(nameof(ValidationModel.Name)), "Informe o nome.");
        store.Add(editContext.Field(nameof(ValidationModel.Email)), "Informe o email.");
        editContext.NotifyValidationStateChanged();

        var markup = RenderWithEditContext(editContext, builder =>
        {
            builder.OpenComponent<XFValidationSummary>(0);
            builder.CloseComponent();
        });

        Assert.Contains("Revise os campos destacados", markup);
        Assert.Contains("Informe o nome.", markup);
        Assert.Contains("Informe o email.", markup);
    }

    [Fact]
    public void Message_RendersFieldMessageFromEditContext()
    {
        var model = new ValidationModel();
        var editContext = new EditContext(model);
        var store = new ValidationMessageStore(editContext);
        store.Add(editContext.Field(nameof(ValidationModel.Name)), "Informe o nome.");
        editContext.NotifyValidationStateChanged();
        System.Linq.Expressions.Expression<Func<object?>> expression = () => model.Name;

        var markup = RenderWithEditContext(editContext, builder =>
        {
            builder.OpenComponent<XFValidationMessage>(0);
            builder.AddAttribute(1, "For", expression);
            builder.CloseComponent();
        });

        Assert.Contains("Informe o nome.", markup);
    }

    [Fact]
    public void Status_RendersNeutralWithoutModifiedFields()
    {
        var markup = RenderWithEditContext(new EditContext(new ValidationModel()), builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });

        Assert.Contains("Aguardando validacao", markup);
        Assert.Contains("role=\"status\"", markup);
    }

    [Fact]
    public void Status_AriaLabel_UsesDefaultAndAllowsOverride()
    {
        var defaultMarkup = RenderWithEditContext(new EditContext(new ValidationModel()), builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });
        var customMarkup = RenderWithEditContext(new EditContext(new ValidationModel()), builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.AddAttribute(1, "AriaLabel", "Status da etapa fiscal");
            builder.CloseComponent();
        });

        Assert.Contains("aria-label=\"Status da validacao\"", defaultMarkup);
        Assert.Contains("aria-label=\"Status da etapa fiscal\"", customMarkup);
    }

    [Fact]
    public void Status_RendersInvalidWhenMessagesExistAfterModification()
    {
        var model = new ValidationModel();
        var editContext = new EditContext(model);
        var store = new ValidationMessageStore(editContext);
        editContext.NotifyFieldChanged(editContext.Field(nameof(ValidationModel.Name)));
        store.Add(editContext.Field(nameof(ValidationModel.Name)), "Informe o nome.");
        editContext.NotifyValidationStateChanged();

        var markup = RenderWithEditContext(editContext, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });

        Assert.Contains("Formulario contem erros", markup);
        Assert.Contains("text-danger", markup);
    }

    [Fact]
    public void Status_RendersValidWhenModifiedAndNoMessages()
    {
        var model = new ValidationModel { Name = "Ana" };
        var editContext = new EditContext(model);
        editContext.NotifyFieldChanged(editContext.Field(nameof(ValidationModel.Name)));

        var markup = RenderWithEditContext(editContext, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });

        Assert.Contains("Formulario valido", markup);
        Assert.Contains("text-success", markup);
    }

    private string RenderWithEditContext(EditContext editContext, RenderFragment childContent) =>
        Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", childContent);
            builder.CloseComponent();
        }).Markup;

    private sealed class ValidationModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
