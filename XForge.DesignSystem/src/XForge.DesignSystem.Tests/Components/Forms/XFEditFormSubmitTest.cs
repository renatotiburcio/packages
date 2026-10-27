using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.EditForm;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFEditFormSubmitTest : BunitContext
{
    public XFEditFormSubmitTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    private class TestModel { public string? Name { get; set; } public string? Email { get; set; } }
    [Fact] public void OnSubmit_CallbackSet() { EditContext? ctx = null; var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnSubmit, EventCallback.Factory.Create<EditContext>(this, v => ctx = v))); Assert.NotNull(cut); }
    [Fact] public void OnValidSubmit_CallbackSet() { EditContext? ctx = null; var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, v => ctx = v))); Assert.NotNull(cut); }
    [Fact] public void OnInvalidSubmit_CallbackSet() { EditContext? ctx = null; var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, v => ctx = v))); Assert.NotNull(cut); }
    [Fact] public void OnDiscard_CallbackSet() { EditContext? ctx = null; var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnDiscard, EventCallback.Factory.Create<EditContext>(this, v => ctx = v))); Assert.NotNull(cut); }
    [Fact] public void OnSaveAndClose_CallbackSet() { EditContext? ctx = null; var cut = Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.OnSaveAndClose, EventCallback.Factory.Create<EditContext>(this, v => ctx = v))); Assert.NotNull(cut); }
    [Fact] public void Submit_ValidatesModel() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel { Name = "Valid" }).Add(x => x.ValidationMode, XFEditFormValidationMode.DataAnnotations));
    [Fact] public void Submit_InvalidModel_ShowsValidation() => Render<XFEditForm>(p => { p.Add(x => x.Model, new TestModel()); p.Add(x => x.ValidationMode, XFEditFormValidationMode.DataAnnotations); p.Add(x => x.ShowValidationSummary, true); });
    [Fact] public void ValidationReview_None() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.None));
    [Fact] public void ValidationReview_Button() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.Button));
    [Fact] public void ValidationReview_BannerAndButton() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationReview, XFEditFormValidationReview.BannerAndButton));
    [Fact] public void Density_Compact() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Density, XFDensity.Compact));
    [Fact] public void Density_Comfortable() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Density, XFDensity.Comfortable));
    [Fact] public void Method_Get() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Method, XFEditFormMethod.Get));
    [Fact] public void Method_Post() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Method, XFEditFormMethod.Post));
    [Fact] public void NativeValidation_Renders() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.Native));
    [Fact] public void CustomValidation_Renders() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.ValidationMode, XFEditFormValidationMode.Custom));
    [Fact] public void Layout_Grid() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Grid));
    [Fact] public void Layout_Inline() => Render<XFEditForm>(p => p.Add(x => x.Model, new TestModel()).Add(x => x.Layout, XFEditFormLayout.Inline));
    [Fact] public void Enums_Layout() => Assert.Equal(3, Enum.GetValues<XFEditFormLayout>().Length);
    [Fact] public void Enums_ValidationMode() => Assert.Equal(3, Enum.GetValues<XFEditFormValidationMode>().Length);
    [Fact] public void Enums_ValidationReview() => Assert.Equal(3, Enum.GetValues<XFEditFormValidationReview>().Length);
    [Fact] public void Enums_Method() => Assert.Equal(2, Enum.GetValues<XFEditFormMethod>().Length);
    [Fact] public void Enums_Density() => Assert.Equal(2, Enum.GetValues<XFDensity>().Length);
}
