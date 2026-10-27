using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFEditFormFunctionalTest : BunitContext
{
    public XFEditFormFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private record SampleModel(int Id, string Name, string Email) ;

    private static readonly SampleModel DefaultModel = new(1, "Test", "test@example.com");

    private IRenderedComponent<XFEditForm> RenderForm(
        Action<ComponentParameterCollectionBuilder<XFEditForm>> configure)
    {
        return Render<XFEditForm>(p =>
        {
            p.Add(x => x.Model, DefaultModel);
            configure(p);
        });
    }

    // Rendering
    [Fact] public void Renders_Default() => RenderForm(p => { });
    [Fact] public void Renders_WithModel() => RenderForm(p => p.Add(x => x.Model, DefaultModel));
    [Fact] public void Renders_WithEditContext() { var ctx = new EditContext(DefaultModel); RenderForm(p => p.Add(x => x.EditContext, ctx)); }
    [Fact] public void Renders_WithFormName() => RenderForm(p => p.Add(x => x.FormName, "myForm"));
    [Fact] public void Renders_WithAriaLabel() => RenderForm(p => p.Add(x => x.AriaLabel, "Edit form"));

    // Layout
    [Fact] public void Layout_Vertical() => RenderForm(p => p.Add(x => x.Layout, XFEditFormLayout.Vertical));
    [Fact] public void Layout_Horizontal() => RenderForm(p => p.Add(x => x.Layout, XFEditFormLayout.Inline));
    [Fact] public void Layout_Grid() => RenderForm(p => p.Add(x => x.Layout, XFEditFormLayout.Grid));

    // Density
    [Fact] public void Density_Comfortable() => RenderForm(p => p.Add(x => x.Density, XFDensity.Comfortable));
    [Fact] public void Density_Compact() => RenderForm(p => p.Add(x => x.Density, XFDensity.Compact));

    // Validation
    [Fact] public void ValidationMode_DataAnnotations() => RenderForm(p => p.Add(x => x.ValidationMode, XFEditFormValidationMode.DataAnnotations));
    [Fact] public void ValidationMode_Manual() => RenderForm(p => p.Add(x => x.ValidationMode, XFEditFormValidationMode.Custom));
    [Fact] public void ValidationReview_None() => RenderForm(p => p.Add(x => x.ValidationReview, XFEditFormValidationReview.None));
    [Fact] public void ValidationReview_Inline() => RenderForm(p => p.Add(x => x.ValidationReview, XFEditFormValidationReview.Button));
    [Fact] public void ValidationReview_Dialog() => RenderForm(p => p.Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton));
    [Fact] public void ShowValidationSummary_True() => RenderForm(p => p.Add(x => x.ShowValidationSummary, true));
    [Fact] public void BlockActionsWhenInvalid_True() => RenderForm(p => p.Add(x => x.BlockActionsWhenInvalid, true));

    // Submit events
    [Fact] public void OnSubmit_Callback() => RenderForm(p => p.Add(x => x.OnSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { })));
    [Fact] public void OnValidSubmit_Callback() => RenderForm(p => p.Add(x => x.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { })));
    [Fact] public void OnInvalidSubmit_Callback() => RenderForm(p => p.Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { })));
    [Fact] public void OnDiscard_Callback() => RenderForm(p => p.Add(x => x.OnDiscard, EventCallback.Factory.Create<EditContext>(this, _ => { })));

    // State
    [Fact] public void ReadOnly_True() => RenderForm(p => p.Add(x => x.ReadOnly, true));
    [Fact] public void Submitting_True() => RenderForm(p => p.Add(x => x.Submitting, true));
    [Fact] public void Disabled_True() => RenderForm(p => p.Add(x => x.Disabled, true));

    // Track changes
    [Fact] public void TrackChanges_True() => RenderForm(p => p.Add(x => x.TrackChanges, true));
    [Fact] public void TrackChanges_False() => RenderForm(p => p.Add(x => x.TrackChanges, false));
    [Fact] public void ConfirmUnsavedChanges_True() => RenderForm(p => p.Add(x => x.ConfirmUnsavedChanges, true));
    [Fact] public void GuardNavigation_True() => RenderForm(p => p.Add(x => x.GuardNavigationWhenModified, true));

    // Enhance
    [Fact] public void Enhance_True() => RenderForm(p => p.Add(x => x.Enhance, true));

    // Method
    [Fact] public void Method_Post() => RenderForm(p => p.Add(x => x.Method, XFEditFormMethod.Post));
    [Fact] public void Method_Get() => RenderForm(p => p.Add(x => x.Method, XFEditFormMethod.Get));

    // Custom labels (PT-BR defaults)
    [Fact] public void CustomValidationReviewTitle() => RenderForm(p => p.Add(x => x.ValidationReviewTitle, "Erros"));
    [Fact] public void CustomUnsavedChangesTitle() => RenderForm(p => p.Add(x => x.UnsavedChangesTitle, "Dados pendentes"));

    [Fact] public void Dispose_DoesNotThrow() { var cut = RenderForm(p => { }); cut.InvokeAsync(async () => cut.Instance.Dispose()); }
}
