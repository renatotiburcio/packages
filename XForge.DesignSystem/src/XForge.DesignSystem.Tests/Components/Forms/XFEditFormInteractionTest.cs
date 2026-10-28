using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFEditFormInteractionTest : XForgeTestContext
{
    public XFEditFormInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    private class TestModel { public string? Name { get; set; } public string? Email { get; set; } }
    [Fact] public void Renders_WithModel() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel { Name = "Test" }));

    [Fact] public void Renders_VerticalLayout() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Vertical));
    [Fact] public void Renders_GridLayout() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Grid));
    [Fact] public void Renders_InlineLayout() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Inline));
    [Fact] public void Renders_DataAnnotationValidation() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.DataAnnotations));
    [Fact] public void Renders_NativeValidation() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.Native));
    [Fact] public void Renders_CustomValidation() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.Custom));
    [Fact] public void Renders_ValidationReviewNone() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.None));
    [Fact] public void Renders_ValidationReviewButton() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.Button));
    [Fact] public void Renders_ValidationReviewBannerAndButton() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton));
    [Fact] public void Renders_ShowValidationSummary() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ShowValidationSummary, true));
    [Fact] public void Renders_BlockActionsWhenInvalid() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.BlockActionsWhenInvalid, true));
    [Fact] public void Renders_ReadOnly() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ReadOnly, true));
    [Fact] public void Renders_Submitting() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Submitting, true));
    [Fact] public void Renders_TrackChangesFalse() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.TrackChanges, false));
    [Fact] public void Renders_ConfirmUnsavedChanges() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ConfirmUnsavedChanges, true));
    [Fact] public void Renders_WithFormName() { var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.FormName, "edit-form")); Assert.Contains("edit-form", cut.Markup); }
    [Fact] public void Renders_WithAriaLabel() { var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.AriaLabel, "form-label")); Assert.Contains("form-label", cut.Markup); }
    [Fact] public void Renders_ValidationReviewTitle() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton).Add(x => x.ValidationReviewTitle, "Revise"));
    [Fact] public void EditFormLayout_Has3Values() => Assert.Equal(3, Enum.GetValues<XFEditFormLayout>().Length);
    [Fact] public void EditFormValidationMode_Has3Values() => Assert.Equal(3, Enum.GetValues<XFEditFormValidationMode>().Length);
    [Fact] public void EditFormValidationReview_Has3Values() => Assert.Equal(3, Enum.GetValues<XFEditFormValidationReview>().Length);
    [Fact] public void EditFormMethod_Has2Values() => Assert.Equal(2, Enum.GetValues<XFEditFormMethod>().Length);
}
