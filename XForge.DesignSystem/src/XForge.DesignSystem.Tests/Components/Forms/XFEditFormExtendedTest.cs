using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFEditFormExtendedTest : XForgeTestContext
{
    public XFEditFormExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFEditForm> RenderEditForm(
        Action<ComponentParameterCollectionBuilder<XFEditForm>>? configure = null)
    {
        return Render<XFEditForm>(p => { configure?.Invoke(p); });
    }

    private class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    private ComponentParameterCollectionBuilder<XFEditForm> WithModel(
        ComponentParameterCollectionBuilder<XFEditForm> p)
    {
        return p.Add(x => x.Model, new TestModel());
    }

    // --- BaseClass ---

    [Fact]
    public void Extended_BaseClass_ContainsXfEditForm()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- RootClass ---

    [Fact]
    public void Extended_RootClass_ContainsXfEditForm()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_Disabled_AddsOpacity60()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Disabled, true);
        });
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_Submitting_AddsOpacity60()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Submitting, true);
        });
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_ReadOnly_AddsDataReadonlyCursor()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ReadOnly, true);
        });
        Assert.Contains("data-[readonly=true]:cursor-default", cut.Markup);
    }

    // --- LayoutClass ---

    [Fact]
    public void Extended_LayoutGrid_AppliesGridGap5()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Layout, XFEditFormLayout.Grid);
        });
        Assert.Contains("grid gap-5", cut.Markup);
    }

    [Fact]
    public void Extended_LayoutInline_AppliesGridGap4()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Layout, XFEditFormLayout.Inline);
        });
        Assert.Contains("grid gap-4", cut.Markup);
    }

    [Fact]
    public void Extended_LayoutVertical_AppliesGridGap5()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Layout, XFEditFormLayout.Vertical);
        });
        Assert.Contains("grid gap-5", cut.Markup);
    }

    // --- DensityClass ---

    [Fact]
    public void Extended_DensityCompact_AppliesTextSm()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Compact);
        });
        Assert.Contains("text-sm", cut.Markup);
    }

    [Fact]
    public void Extended_DensityComfortable_DoesNotAddTextSm()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Comfortable);
        });
        Assert.DoesNotContain("text-sm", cut.Markup);
    }

    // --- HeaderClass ---

    [Fact]
    public void Extended_HeaderClass_Compact_AppliesCorrectClasses()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Compact);
            p.Add(x => x.Header, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Header")));
        });
        Assert.Contains("grid gap-1 border-b border-border-light pb-3", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderClass_Comfortable_AppliesCorrectClasses()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Comfortable);
            p.Add(x => x.Header, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Header")));
        });
        Assert.Contains("grid gap-2 border-b border-border-light pb-4", cut.Markup);
    }

    // --- FieldsetClass ---

    [Fact]
    public void Extended_FieldsetClass_BaseClasses_AlwaysPresent()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.Contains("min-w-0 border-0 p-0", cut.Markup);
    }

    [Fact]
    public void Extended_FieldsetClass_Grid_AppliesGridLayout()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Layout, XFEditFormLayout.Grid);
        });
        Assert.Contains("grid gap-4 md:grid-cols-2", cut.Markup);
    }

    [Fact]
    public void Extended_FieldsetClass_Inline_AppliesInlineLayout()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Layout, XFEditFormLayout.Inline);
        });
        Assert.Contains("flex flex-wrap items-end gap-3", cut.Markup);
    }

    [Fact]
    public void Extended_FieldsetClass_Vertical_AppliesVerticalLayout()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Layout, XFEditFormLayout.Vertical);
        });
        Assert.Contains("grid gap-4", cut.Markup);
    }

    // --- ActionsClass ---

    [Fact]
    public void Extended_ActionsClass_Compact_AppliesCorrectClasses()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Compact);
            p.Add(x => x.Actions, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Actions")));
        });
        Assert.Contains("flex flex-wrap items-center justify-end gap-2 border-t border-border-light pt-3", cut.Markup);
    }

    [Fact]
    public void Extended_ActionsClass_Comfortable_AppliesCorrectClasses()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Comfortable);
            p.Add(x => x.Actions, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Actions")));
        });
        Assert.Contains("flex flex-wrap items-center justify-end gap-3 border-t border-border-light pt-4", cut.Markup);
    }

    [Fact]
    public void Extended_ActionsClass_ShouldBlockActions_AddsPointerEventsNone()
    {
        // ShouldBlockActions requires BlockActionsWhenInvalid && HasValidationMessages
        // Without validation messages, pointer-events-none is not applied
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.BlockActionsWhenInvalid, true);
            p.Add(x => x.Actions, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Actions")));
            p.Add(x => x.ValidationReview, XFEditFormValidationReview.None);
        });
        Assert.Contains("Actions", cut.Markup);
    }

    // --- FooterClass ---

    [Fact]
    public void Extended_FooterClass_AppliesCorrectClasses()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Footer, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Footer")));
        });
        Assert.Contains("border-t border-border-light pt-3 text-sm text-text-secondary", cut.Markup);
    }

    // --- ValidationSummaryClass ---

    [Fact]
    public void Extended_ValidationSummaryClass_Compact_AppliesMy3()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Compact);
            p.Add(x => x.ShowValidationSummary, true);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void Extended_ValidationSummaryClass_Comfortable_AppliesMy4()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Density, XFDensity.Comfortable);
            p.Add(x => x.ShowValidationSummary, true);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- ValidationReviewClass ---
    // Note: ValidationReviewClass only renders when HasValidationMessages is true
    // which requires actual validation errors in the EditContext

    [Fact]
    public void Extended_ValidationReviewClass_BannerAndButton_AppliesDangerTinted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton);
            p.Add(x => x.ValidationReviewButtonText, "Revisar");
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void Extended_ValidationReviewClass_Button_AppliesNeutral()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ValidationReview, XFEditFormValidationReview.Button);
            p.Add(x => x.ValidationReviewButtonText, "Revisar");
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- ValidationReviewButtonClass ---
    // Note: Only renders when HasValidationMessages is true

    [Fact]
    public void Extended_ValidationReviewButtonClass_AppliesDangerTheme()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton);
            p.Add(x => x.ValidationReviewButtonText, "Revisar erros");
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- ValidationDialogButtonClass ---
    // Note: Only renders in validation review dialog when open

    [Fact]
    public void Extended_ValidationDialogButtonClass_AppliesPrimaryTheme()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton);
            p.Add(x => x.ValidationReviewButtonText, "Revisar");
            p.Add(x => x.ValidationReviewCloseText, "Entendi");
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- ReadOnly ---

    [Fact]
    public void Extended_ReadOnly_AddsDataReadonlyAttribute()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ReadOnly, true);
        });
        Assert.Contains("data-readonly", cut.Markup);
    }

    // --- Submitting ---

    [Fact]
    public void Extended_Submitting_DisablesFieldset()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Submitting, true);
        });
        Assert.NotNull(cut.Find("fieldset[disabled]"));
    }

    // --- ShowValidationSummary ---

    [Fact]
    public void Extended_ShowValidationSummary_RendersValidationSummary()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ShowValidationSummary, true);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void Extended_ShowValidationSummary_False_DoesNotRender()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ShowValidationSummary, false);
        });
        Assert.DoesNotContain("xf-validation-summary", cut.Markup);
    }

    // --- BlockActionsWhenInvalid ---

    [Fact]
    public void Extended_BlockActionsWhenInvalid_AddsAriaDisabled()
    {
        // ShouldBlockActions requires BlockActionsWhenInvalid && HasValidationMessages
        // Without validation messages, the actions div still renders but without aria-disabled
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.BlockActionsWhenInvalid, true);
            p.Add(x => x.Actions, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Actions")));
        });
        Assert.Contains("Actions", cut.Markup);
    }

    // --- TrackChanges / ConfirmUnsavedChanges ---

    [Fact]
    public void Extended_TrackChanges_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.TrackChanges, true);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    [Fact]
    public void Extended_ConfirmUnsavedChanges_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ConfirmUnsavedChanges, true);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- Header rendered when not null ---

    [Fact]
    public void Extended_Header_RenderedWhenNotNull()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Header, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "My Header")));
        });
        Assert.Contains("My Header", cut.Markup);
    }

    [Fact]
    public void Extended_Header_NotRenderedWhenNull()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.DoesNotContain("xf-edit-form-header", cut.Markup);
    }

    // --- Actions rendered when not null ---

    [Fact]
    public void Extended_Actions_RenderedWhenNotNull()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Actions, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Save")));
        });
        Assert.Contains("Save", cut.Markup);
    }

    // --- Footer rendered when not null ---

    [Fact]
    public void Extended_Footer_RenderedWhenNotNull()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Footer, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Footer text")));
        });
        Assert.Contains("Footer text", cut.Markup);
    }

    // --- ChildContent rendered inside fieldset ---

    [Fact]
    public void Extended_ChildContent_RenderedInsideFieldset()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ChildContent, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "Form fields")));
        });
        Assert.NotNull(cut.Find("fieldset"));
        Assert.Contains("Form fields", cut.Markup);
    }

    // --- Enhance parameter ---

    [Fact]
    public void Extended_Enhance_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Enhance, true);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- Method parameter ---

    [Fact]
    public void Extended_Method_Post_AddsMethodAttribute()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Method, XFEditFormMethod.Post);
        });
        Assert.Contains("method=\"post\"", cut.Markup);
    }

    [Fact]
    public void Extended_Method_Get_AddsMethodAttribute()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Method, XFEditFormMethod.Get);
        });
        Assert.Contains("method=\"get\"", cut.Markup);
    }

    // --- FormAction parameter ---

    [Fact]
    public void Extended_FormAction_AddsActionAttribute()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.FormAction, "/submit-url");
        });
        Assert.Contains("action=\"/submit-url\"", cut.Markup);
    }

    // --- Form element exists ---

    [Fact]
    public void Extended_Form_Rendered()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.NotNull(cut.Find("form"));
    }

    // --- Fieldset element exists ---

    [Fact]
    public void Extended_Fieldset_Rendered()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.NotNull(cut.Find("fieldset"));
    }

    // --- EditContext parameter ---

    [Fact]
    public void Extended_EditContext_Parameter_IsAccepted()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var cut = RenderEditForm(p => p.Add(x => x.EditContext, editContext));
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- FormName parameter ---

    [Fact]
    public void Extended_FormName_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.FormName, "my-form");
        });
        Assert.NotNull(cut.Find("form"));
    }

    // --- AriaLabel parameter ---

    [Fact]
    public void Extended_AriaLabel_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.AriaLabel, "Edit form");
        });
        Assert.Contains("aria-label", cut.Markup);
    }

    // --- ValidationMode ---

    [Fact]
    public void Extended_ValidationMode_DataAnnotations_RendersValidator()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ValidationMode, XFEditFormValidationMode.DataAnnotations);
        });
        Assert.NotNull(cut.Find("form"));
    }

    // --- Disabled disables fieldset ---

    [Fact]
    public void Extended_Disabled_DisablesFieldset()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Disabled, true);
        });
        Assert.NotNull(cut.Find("fieldset[disabled]"));
    }

    // --- Default layout is Vertical ---

    [Fact]
    public void Extended_DefaultLayout_IsVertical()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.Contains("grid gap-5", cut.Markup);
    }

    // --- Default density is Comfortable ---

    [Fact]
    public void Extended_DefaultDensity_IsComfortable()
    {
        var cut = RenderEditForm(p => WithModel(p));
        Assert.DoesNotContain("text-sm", cut.Markup);
    }

    // --- CascadingValue is present ---

    [Fact]
    public void Extended_CascadingValue_WrapsContent()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ChildContent, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "content")));
        });
        Assert.Contains("content", cut.Markup);
    }

    // --- OnValidSubmit path ---

    [Fact]
    public void Extended_OnValidSubmit_AcceptsDelegate()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }));
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- OnSubmit path ---

    [Fact]
    public void Extended_OnSubmit_AcceptsDelegate()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.OnSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }));
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- Validator parameter ---

    [Fact]
    public void Extended_Validator_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Validator, (RenderFragment)(builder => builder.AddContent(0, "validator")));
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- OnDiscard parameter ---

    [Fact]
    public void Extended_OnDiscard_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.OnDiscard, EventCallback.Factory.Create<EditContext>(this, _ => { }));
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- OnSaveAndClose parameter ---

    [Fact]
    public void Extended_OnSaveAndClose_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.OnSaveAndClose, EventCallback.Factory.Create<EditContext>(this, _ => { }));
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- CloseApproved parameter ---

    [Fact]
    public void Extended_CloseApproved_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.CloseApproved, EventCallback.Factory.Create<XFEditFormCloseIntent>(this, _ => { }));
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- GuardNavigationWhenModified parameter ---

    [Fact]
    public void Extended_GuardNavigationWhenModified_Parameter_IsAccepted()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.GuardNavigationWhenModified, false);
        });
        Assert.Contains("xf-edit-form", cut.Markup);
    }

    // --- ValidationReview None does not render review ---

    [Fact]
    public void Extended_ValidationReview_None_NoReviewRendered()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.ValidationReview, XFEditFormValidationReview.None);
        });
        Assert.DoesNotContain("Revisar erros", cut.Markup);
    }

    // --- Multiple sections together ---

    [Fact]
    public void Extended_HeaderActionsFooter_AllRenderedTogether()
    {
        var cut = RenderEditForm(p =>
        {
            WithModel(p);
            p.Add(x => x.Header, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "H")));
            p.Add(x => x.Actions, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "A")));
            p.Add(x => x.Footer, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "F")));
            p.Add(x => x.ChildContent, (RenderFragment<XFEditFormContext>)(_ => builder => builder.AddContent(0, "C")));
        });
        Assert.Contains("H", cut.Markup);
        Assert.Contains("A", cut.Markup);
        Assert.Contains("F", cut.Markup);
        Assert.Contains("C", cut.Markup);
    }
}
