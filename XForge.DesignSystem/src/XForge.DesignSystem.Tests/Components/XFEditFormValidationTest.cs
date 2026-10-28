using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public sealed class XFEditFormValidationTest : XForgeTestContext
{
    public XFEditFormValidationTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private class TestModel { public string? Name { get; set; } public int Age { get; set; } }

    [Fact] public void EditForm_OnValidSubmit_CallbackSet()
    {
        EditContext? ctx = null;
        var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, v => ctx = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void EditForm_OnInvalidSubmit_CallbackSet()
    {
        EditContext? ctx = null;
        var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, v => ctx = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void EditForm_OnSubmit_CallbackSet()
    {
        EditContext? ctx = null;
        var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnSubmit, EventCallback.Factory.Create<EditContext>(this, v => ctx = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void EditForm_OnDiscard_CallbackSet()
    {
        EditContext? ctx = null;
        var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnDiscard, EventCallback.Factory.Create<EditContext>(this, v => ctx = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void EditForm_OnSaveAndClose_CallbackSet()
    {
        EditContext? ctx = null;
        var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnSaveAndClose, EventCallback.Factory.Create<EditContext>(this, v => ctx = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void EditForm_ValidationModeDataAnnotations() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.DataAnnotations));
    [Fact] public void EditForm_ValidationModeNative() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.Native));
    [Fact] public void EditForm_ValidationModeCustom() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.Custom));
    [Fact] public void EditForm_ShowValidationSummary() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ShowValidationSummary, true));
    [Fact] public void EditForm_ValidationReviewNone() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.None));
    [Fact] public void EditForm_ValidationReviewButton() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.Button));
    [Fact] public void EditForm_ValidationReviewBannerAndButton() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton));
    [Fact] public void EditForm_ValidationReviewTitle() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReviewTitle, "Review"));
    [Fact] public void EditForm_BlockActionsWhenInvalid() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.BlockActionsWhenInvalid, true));
    [Fact] public void EditForm_ConfirmUnsavedChanges() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ConfirmUnsavedChanges, true));
    [Fact] public void EditForm_TrackChanges() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.TrackChanges, true));
    [Fact] public void EditForm_FormName() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.FormName, "my-form"));
    [Fact] public void EditForm_MethodGet() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Method, XFEditFormMethod.Get));
    [Fact] public void EditForm_MethodPost() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Method, XFEditFormMethod.Post));
    [Fact] public void EditForm_LayoutGrid() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Grid));
    [Fact] public void EditForm_LayoutInline() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Inline));
    [Fact] public void EditForm_DensityCompact() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Density, XFDensity.Compact));
    [Fact] public void EditForm_DensityComfortable() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Density, XFDensity.Comfortable));
    [Fact] public void EditForm_AriaLabel() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.AriaLabel, "Form"));
    [Fact] public void EditForm_Submitting() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Submitting, true));
    [Fact] public void EditForm_ReadOnly() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ReadOnly, true));
}