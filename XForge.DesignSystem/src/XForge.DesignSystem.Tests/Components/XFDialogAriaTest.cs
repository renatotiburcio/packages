using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Dialog;

namespace XForge.DesignSystem.Tests.Components;

public class XFDialogAriaTest : XForgeTestContext
{
    public XFDialogAriaTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Role_Default_IsDialog()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
        });

        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void Role_WhenAlert_IsAlertdialog()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Alert, true);
        });

        Assert.Contains("role=\"alertdialog\"", cut.Markup);
    }

    [Fact]
    public void AriaModal_IsTrue()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
        });

        Assert.Contains("aria-modal=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaLabelledBy_PointsToTitle()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Title, "Publish");
        });

        var dialog = cut.Find("[role='dialog']");
        var labelledBy = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(labelledBy);
        Assert.Contains(labelledBy, cut.Markup);
        Assert.Contains("Publish", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_PointsToDescription()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Title, "Publish");
            parameters.Add(p => p.Description, "Review changes");
        });

        var dialog = cut.Find("[role='dialog']");
        var describedBy = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains(describedBy, cut.Markup);
        Assert.Contains("Review changes", cut.Markup);
    }

    [Fact]
    public void AriaReferences_WhenTitleAndDescriptionMissing_AreOmitted()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.ShowCloseButton, false);
        });

        var dialog = cut.Find("[role='dialog']");
        Assert.Null(dialog.GetAttribute("aria-labelledby"));
        Assert.Null(dialog.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void CloseButton_HasAriaLabel()
    {
        var cut = Render<XFDialog>(parameters =>
        {
            parameters.Add(p => p.Open, true);
        });

        var closeButton = cut.Find("button[aria-label='Fechar']");
        Assert.NotNull(closeButton);
    }
}
