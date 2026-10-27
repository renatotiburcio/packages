#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using XForge.DesignSystem.Components.Forms.Validation;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFValidationSummaryComponentTest : Bunit.BunitContext
{
    public XFValidationSummaryComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithManualItems()
    {
        var cut = RenderWithItems(new[] { "Field is required", "Invalid format" });
        Assert.Contains("xf-validation-summary", cut);
        Assert.Contains("Field is required", cut);
        Assert.Contains("Invalid format", cut);
    }

    [Fact]
    public void Render_WithDefaultTitle()
    {
        var cut = RenderWithItems(new[] { "Error occurred" });
        Assert.Contains("Revise os campos destacados", cut);
    }

    [Fact]
    public void Render_WithCustomTitle()
    {
        var cut = Render<XFValidationSummary>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "Error" });
            parameters.Add(p => p.Title, "Please fix the following");
        });
        Assert.Contains("Please fix the following", cut.Markup);
    }

    [Fact]
    public void Render_WithDangerTone()
    {
        var cut = Render<XFValidationSummary>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "Error" });
            parameters.Add(p => p.Tone, XFSemanticColor.Danger);
        });
        Assert.Contains("border-danger", cut.Markup);
    }

    [Fact]
    public void Render_WithSuccessTone()
    {
        var cut = Render<XFValidationSummary>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "All good" });
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
        });
        Assert.Contains("border-success", cut.Markup);
    }

    [Fact]
    public void Render_WithEditContextMessages()
    {
        var model = new SummaryModel();
        var editContext = new EditContext(model);
        var store = new ValidationMessageStore(editContext);
        store.Add(editContext.Field(nameof(SummaryModel.Name)), "Name required.");
        editContext.NotifyValidationStateChanged();

        var markup = RenderWithEditContext(editContext, builder =>
        {
            builder.OpenComponent<XFValidationSummary>(0);
            builder.CloseComponent();
        });

        Assert.Contains("Revise os campos destacados", markup);
        Assert.Contains("Name required.", markup);
    }

    [Fact]
    public void Render_EmptyItems_NoOutput()
    {
        var cut = Render<XFValidationSummary>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<string>());
        });
        Assert.DoesNotContain("xf-validation-summary", cut.Markup);
    }

    private string RenderWithItems(string[] items)
    {
        var cut = Render<XFValidationSummary>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });
        return cut.Markup;
    }

    private string RenderWithEditContext(EditContext editContext, RenderFragment childContent) =>
        Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", childContent);
            builder.CloseComponent();
        }).Markup;

    public class SummaryModel
    {
        public string? Name { get; set; }
    }
}
