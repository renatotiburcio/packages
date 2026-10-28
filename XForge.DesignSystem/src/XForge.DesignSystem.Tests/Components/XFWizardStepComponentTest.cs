using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Steps;

namespace XForge.DesignSystem.Tests.Components;

public class XFWizardStepComponentTest : XForgeTestContext
{
    public XFWizardStepComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_AsChildOfWizard_RegistersStep()
    {
        var cut = Render<XFWizard>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFWizardStep>(0);
            builder.AddAttribute(1, nameof(XFWizardStep.Label), "Step 1");
            builder.CloseComponent();
        }));
        Assert.Contains("xf-wizard", cut.Markup);
    }

    [Fact]
    public void Render_WithLabel_ShowsLabel()
    {
        var cut = Render<XFWizard>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFWizardStep>(0);
            builder.AddAttribute(1, nameof(XFWizardStep.Label), "Personal Info");
            builder.CloseComponent();
        }));
        Assert.Contains("Personal Info", cut.Markup);
    }

    [Fact]
    public void Render_WithDescription_ShowsDescription()
    {
        var cut = Render<XFWizard>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFWizardStep>(0);
            builder.AddAttribute(1, nameof(XFWizardStep.Label), "Step");
            builder.AddAttribute(2, nameof(XFWizardStep.Description), "Fill your details");
            builder.CloseComponent();
        }));
        Assert.Contains("Fill your details", cut.Markup);
    }

    [Fact]
    public void Render_Complete_StepIsCompleted()
    {
        var cut = Render<XFWizard>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFWizardStep>(0);
            builder.AddAttribute(1, nameof(XFWizardStep.Label), "Done");
            builder.AddAttribute(2, nameof(XFWizardStep.Complete), true);
            builder.CloseComponent();
        }));
        Assert.Contains("Done", cut.Markup);
    }

    [Fact]
    public void Render_MultipleSteps_AllRegistered()
    {
        var cut = Render<XFWizard>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFWizardStep>(0);
            builder.AddAttribute(1, nameof(XFWizardStep.Label), "First");
            builder.CloseComponent();
            builder.OpenComponent<XFWizardStep>(2);
            builder.AddAttribute(3, nameof(XFWizardStep.Label), "Second");
            builder.CloseComponent();
        }));
        Assert.Contains("First", cut.Markup);
        Assert.Contains("Second", cut.Markup);
    }
}
