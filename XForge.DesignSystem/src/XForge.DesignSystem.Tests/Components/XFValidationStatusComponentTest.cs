#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFValidationStatusComponentTest : XForgeTestContext
{
    public XFValidationStatusComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithEditContext_NeutralState()
    {
        var model = new StatusModel();
        var ec = new EditContext(model);
        var markup = RenderWithEditContext(ec, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });
        Assert.Contains("xf-validation-status", markup);
        Assert.Contains("Aguardando validacao", markup);
        Assert.Contains("role=\"status\"", markup);
    }

    [Fact]
    public void Render_DefaultAriaLabel()
    {
        var model = new StatusModel();
        var ec = new EditContext(model);
        var markup = RenderWithEditContext(ec, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });
        Assert.Contains("aria-label=\"Status da validacao\"", markup);
    }

    [Fact]
    public void Render_CustomAriaLabel()
    {
        var model = new StatusModel();
        var ec = new EditContext(model);
        var markup = RenderWithEditContext(ec, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.AddAttribute(1, "AriaLabel", "Custom status label");
            builder.CloseComponent();
        });
        Assert.Contains("aria-label=\"Custom status label\"", markup);
    }

    [Fact]
    public void Render_ValidState_ShowsValidText()
    {
        var model = new StatusModel { Name = "Ana" };
        var ec = new EditContext(model);
        ec.NotifyFieldChanged(ec.Field(nameof(StatusModel.Name)));
        var markup = RenderWithEditContext(ec, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });
        Assert.Contains("Formulario valido", markup);
        Assert.Contains("text-success", markup);
    }

    [Fact]
    public void Render_InvalidState_ShowsInvalidText()
    {
        var model = new StatusModel();
        var ec = new EditContext(model);
        var store = new ValidationMessageStore(ec);
        ec.NotifyFieldChanged(ec.Field(nameof(StatusModel.Name)));
        store.Add(ec.Field(nameof(StatusModel.Name)), "Informe o nome.");
        ec.NotifyValidationStateChanged();

        var markup = RenderWithEditContext(ec, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.CloseComponent();
        });
        Assert.Contains("Formulario contem erros", markup);
        Assert.Contains("text-danger", markup);
    }

    [Fact]
    public void Render_CustomTexts()
    {
        var model = new StatusModel();
        var ec = new EditContext(model);
        var markup = RenderWithEditContext(ec, builder =>
        {
            builder.OpenComponent<XFValidationStatus>(0);
            builder.AddAttribute(1, "NeutralText", "Waiting");
            builder.AddAttribute(2, "ValidText", "All good");
            builder.AddAttribute(3, "InvalidText", "Has errors");
            builder.CloseComponent();
        });
        Assert.Contains("Waiting", markup);
    }

    private string RenderWithEditContext(EditContext editContext, RenderFragment childContent) =>
        Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", childContent);
            builder.CloseComponent();
        }).Markup;

    public class StatusModel
    {
        public string? Name { get; set; }
    }
}
