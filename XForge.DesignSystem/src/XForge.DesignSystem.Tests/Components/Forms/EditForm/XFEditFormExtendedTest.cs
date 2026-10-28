using System.ComponentModel.DataAnnotations;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.EditForm;

public class XFEditFormExtendedTest : XForgeTestContext
{
    public XFEditFormExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private sealed class SimpleModel
    {
        [Required(ErrorMessage = "Name required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email required.")]
        public string? Email { get; set; }
    }

    [Fact]
    public void CompactDensity_RendersCompactClasses()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Density, XFDensity.Compact);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("text-sm", cut.Markup);
    }

    [Fact]
    public void InlineLayout_RendersInlineFieldset()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Layout, XFEditFormLayout.Inline);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("flex flex-wrap", cut.Markup);
    }

    [Fact]
    public void ReadOnly_RendersDataReadonlyAttribute()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ReadOnly, true);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("data-readonly=\"true\"", cut.Markup);
    }

    [Fact]
    public void OnSubmit_InvokesOnSubmitDelegate()
    {
        var model = new SimpleModel { Name = "Test", Email = "test@test.com" };
        var submitCalled = false;

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.OnSubmit, EventCallback.Factory.Create<EditContext>(this, _ => submitCalled = true));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        cut.Find("form").Submit();
        Assert.True(submitCalled);
    }

    [Fact]
    public void ContinueEditing_ClosesUnsavedDialog()
    {
        var model = new SimpleModel { Name = "Test" };
        var editContext = new EditContext(model);

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.EditContext, editContext);
            parameters.Add(p => p.ValidationMode, XFEditFormValidationMode.Native);
            parameters.Add(p => p.ConfirmUnsavedChanges, true);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
            parameters.Add(p => p.Actions, context => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "type", "button");
                builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => context.RequestCloseAsync(XFEditFormCloseIntent.Cancel)));
                builder.AddContent(3, "Cancel");
                builder.CloseElement();
            });
        });

        editContext.NotifyFieldChanged(editContext.Field(nameof(SimpleModel.Name)));
        cut.Render();
        cut.Find("button[type='button']").Click();
        Assert.Contains("Alteracoes nao salvas", cut.Markup);

        // Click "Voltar e continuar" (ContinueEditing)
        var continueBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Voltar e continuar"));
        Assert.NotNull(continueBtn);
        continueBtn.Click();

        Assert.DoesNotContain("Alteracoes nao salvas", cut.Markup);
    }

    [Fact]
    public void FormMethod_Get_RendersMethodAttribute()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Method, XFEditFormMethod.Get);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        var form = cut.Find("form");
        Assert.Equal("get", form.GetAttribute("method"));
    }

    [Fact]
    public void FormAction_RendersActionAttribute()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.FormAction, "/api/save");
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        var form = cut.Find("form");
        Assert.Equal("/api/save", form.GetAttribute("action"));
    }

    [Fact]
    public void AdditionalAttributes_RendersOnForm()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-custom", "value" },
                { "novalidate", "true" }
            });
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("data-custom=\"value\"", cut.Markup);
    }

    [Fact]
    public void CustomClass_AppendedToRoot()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Class, "my-custom-form");
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("my-custom-form", cut.Markup);
    }

    [Fact]
    public void Style_RendersOnForm()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Style, "max-width: 600px");
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("max-width: 600px", cut.Markup);
    }

    [Fact]
    public void Header_RendersInForm()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Header, context => builder =>
            {
                builder.AddContent(0, "Form Header");
            });
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("Form Header", cut.Markup);
    }

    [Fact]
    public void Footer_RendersInForm()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Footer, context => builder =>
            {
                builder.AddContent(0, "Form Footer");
            });
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("Form Footer", cut.Markup);
    }

    [Fact]
    public void Validator_RendersCustomValidator()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ValidationMode, XFEditFormValidationMode.Custom);
            parameters.Add(p => p.Validator, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "CustomValidator");
            }));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("CustomValidator", cut.Markup);
    }

    [Fact]
    public void FormName_ParameterAccepted()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.FormName, "my-form");
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        // FormName is passed to EditForm which renders it as formname attribute
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void Enhance_RendersDataEnhance()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Enhance, true);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("data-enhance", cut.Markup);
    }

    [Fact]
    public void GuardNavigationWhenModified_ParameterAccepted()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.GuardNavigationWhenModified, false);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void TrackChanges_Disabled_ParameterAccepted()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.TrackChanges, false);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void ShowValidationSummary_RendersSummary()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ShowValidationSummary, true);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        // XFValidationSummary renders inside the form when ShowValidationSummary=true
        Assert.Contains("xf-edit-form", cut.Markup);
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void CloseApproved_CallbackFires()
    {
        var model = new SimpleModel { Name = "Test" };
        var editContext = new EditContext(model);
        XFEditFormCloseIntent? receivedIntent = null;

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.EditContext, editContext);
            parameters.Add(p => p.ValidationMode, XFEditFormValidationMode.Native);
            parameters.Add(p => p.ConfirmUnsavedChanges, true);
            parameters.Add(p => p.CloseApproved, EventCallback.Factory.Create<XFEditFormCloseIntent>(this, intent => receivedIntent = intent));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
            parameters.Add(p => p.Actions, context => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "type", "button");
                builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => context.RequestCloseAsync(XFEditFormCloseIntent.Close)));
                builder.AddContent(3, "Close");
                builder.CloseElement();
            });
        });

        // No unsaved changes, so close should fire immediately
        cut.Find("button[type='button']").Click();
        Assert.Equal(XFEditFormCloseIntent.Close, receivedIntent);
    }

    [Fact]
    public void RequestCloseAsync_NoUnsavedChanges_ApprovesImmediately()
    {
        var model = new SimpleModel();
        var editContext = new EditContext(model);
        var approved = false;

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.EditContext, editContext);
            parameters.Add(p => p.ValidationMode, XFEditFormValidationMode.Native);
            parameters.Add(p => p.ConfirmUnsavedChanges, true);
            parameters.Add(p => p.CloseApproved, EventCallback.Factory.Create<XFEditFormCloseIntent>(this, _ => approved = true));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
            parameters.Add(p => p.Actions, context => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "type", "button");
                builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => context.RequestCloseAsync(XFEditFormCloseIntent.Close)));
                builder.AddContent(3, "Close");
                builder.CloseElement();
            });
        });

        cut.Find("button[type='button']").Click();
        Assert.True(approved);
    }

    [Fact]
    public void SaveAndClose_Invalid_InvokesOnInvalidSubmit()
    {
        var model = new SimpleModel { Name = "Test" };
        var editContext = new EditContext(model);
        var invalidCalled = false;

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.EditContext, editContext);
            parameters.Add(p => p.ConfirmUnsavedChanges, true);
            parameters.Add(p => p.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => invalidCalled = true));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.OpenComponent<XFTextInput>(0);
                builder.AddAttribute(1, "Value", model.Name);
                builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                builder.CloseComponent();
            });
            parameters.Add(p => p.Actions, context => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "type", "button");
                builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => context.RequestCloseAsync(XFEditFormCloseIntent.Close)));
                builder.AddContent(3, "Close");
                builder.CloseElement();
            });
        });

        editContext.NotifyFieldChanged(editContext.Field(nameof(SimpleModel.Name)));
        cut.Render();
        cut.Find("button[type='button']").Click();
        Assert.Contains("Alteracoes nao salvas", cut.Markup);

        // Click "Salvar e fechar" — should fail validation
        var saveBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Salvar e fechar"));
        Assert.NotNull(saveBtn);
        saveBtn.Click();

        Assert.True(invalidCalled);
    }

    [Fact]
    public void SaveAndClose_Valid_CallsOnValidSubmit()
    {
        var model = new SimpleModel { Name = "Test", Email = "test@test.com" };
        var editContext = new EditContext(model);
        var validCalled = false;

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.EditContext, editContext);
            parameters.Add(p => p.ConfirmUnsavedChanges, true);
            parameters.Add(p => p.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => validCalled = true));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
            parameters.Add(p => p.Actions, context => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "type", "button");
                builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => context.RequestCloseAsync(XFEditFormCloseIntent.Close)));
                builder.AddContent(3, "Close");
                builder.CloseElement();
            });
        });

        editContext.NotifyFieldChanged(editContext.Field(nameof(SimpleModel.Name)));
        cut.Render();
        cut.Find("button[type='button']").Click();
        Assert.Contains("Alteracoes nao salvas", cut.Markup);

        var saveBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Salvar e fechar"));
        Assert.NotNull(saveBtn);
        saveBtn.Click();

        Assert.True(validCalled);
    }

    [Fact]
    public void ValidationReview_Button_ShowsReviewButton()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ValidationReview, XFEditFormValidationReview.Button);
            parameters.Add(p => p.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.OpenComponent<XFTextInput>(0);
                builder.AddAttribute(1, "Value", model.Name);
                builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                builder.CloseComponent();
            });
        });

        cut.Find("form").Submit();

        Assert.Contains("pendencia", cut.Markup);
    }

    [Fact]
    public void UnsavedChanges_CustomTexts_Renders()
    {
        var model = new SimpleModel { Name = "Test" };
        var editContext = new EditContext(model);

        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.EditContext, editContext);
            parameters.Add(p => p.ValidationMode, XFEditFormValidationMode.Native);
            parameters.Add(p => p.ConfirmUnsavedChanges, true);
            parameters.Add(p => p.UnsavedChangesTitle, "Custom Title");
            parameters.Add(p => p.UnsavedChangesDescription, "Custom Description");
            parameters.Add(p => p.UnsavedChangesContinueText, "Stay");
            parameters.Add(p => p.UnsavedChangesDiscardText, "Throw away");
            parameters.Add(p => p.UnsavedChangesSaveText, "Save and exit");
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
            parameters.Add(p => p.Actions, context => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "type", "button");
                builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => context.RequestCloseAsync(XFEditFormCloseIntent.Cancel)));
                builder.AddContent(3, "Cancel");
                builder.CloseElement();
            });
        });

        editContext.NotifyFieldChanged(editContext.Field(nameof(SimpleModel.Name)));
        cut.Render();
        cut.Find("button[type='button']").Click();

        Assert.Contains("Custom Title", cut.Markup);
        Assert.Contains("Custom Description", cut.Markup);
        Assert.Contains("Stay", cut.Markup);
        Assert.Contains("Throw away", cut.Markup);
        Assert.Contains("Save and exit", cut.Markup);
    }

    [Fact]
    public void ValidationReview_CustomTexts_Renders()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ValidationReview, XFEditFormValidationReview.BannerAndButton);
            parameters.Add(p => p.ValidationReviewTitle, "Custom Review Title");
            parameters.Add(p => p.ValidationReviewDescription, "Custom Review Desc");
            parameters.Add(p => p.ValidationReviewLead, "Custom Lead");
            parameters.Add(p => p.ValidationReviewHint, "Custom Hint");
            parameters.Add(p => p.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }));
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.OpenComponent<XFTextInput>(0);
                builder.AddAttribute(1, "Value", model.Name);
                builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                builder.AddAttribute(3, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                builder.CloseComponent();
            });
        });

        cut.Find("form").Submit();
        cut.Find("button[type='button']").Click();

        Assert.Contains("Custom Review Title", cut.Markup);
        Assert.Contains("Custom Review Desc", cut.Markup);
        Assert.Contains("Custom Lead", cut.Markup);
        Assert.Contains("Custom Hint", cut.Markup);
    }

    [Fact]
    public void AriaLabel_RendersOnForm()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.AriaLabel, "My Form");
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("aria-label=\"My Form\"", cut.Markup);
    }

    [Fact]
    public void GridLayout_RendersGridFieldset()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Layout, XFEditFormLayout.Grid);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("md:grid-cols-2", cut.Markup);
    }

    [Fact]
    public void Submitting_RendersDisabledFieldset()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Submitting, true);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("disabled", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Disabled_RendersDisabledFieldset()
    {
        var model = new SimpleModel();
        var cut = Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        });

        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void NoModelNoEditContext_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Render<XFEditForm>(parameters =>
        {
            parameters.Add(p => p.ChildContent, context => builder =>
            {
                builder.AddContent(0, "Content");
            });
        }));

        Assert.Contains("requires either a Model or an EditContext", exception.Message);
    }
}
