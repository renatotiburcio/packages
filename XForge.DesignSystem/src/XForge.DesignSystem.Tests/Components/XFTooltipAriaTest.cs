using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Shared;

namespace XForge.DesignSystem.Tests.Components;

public class XFTooltipAriaTest : XForgeTestContext
{
    public XFTooltipAriaTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void AriaDescribedBy_PointsToTooltipPopupId()
    {
        var cut = Render<XFTooltip>(parameters =>
        {
            parameters.Add(p => p.Text, "Helpful text");
            parameters.AddChildContent("Hover me");
        });

        var trigger = cut.Find("[data-tooltip-id]");
        var tooltipId = trigger.GetAttribute("data-tooltip-id");
        Assert.NotNull(tooltipId);
        Assert.Equal(tooltipId, trigger.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void DataTooltipId_Attribute_IsPresent()
    {
        var cut = Render<XFTooltip>(parameters =>
        {
            parameters.Add(p => p.Text, "Helpful text");
            parameters.AddChildContent("Hover me");
        });

        Assert.Contains("data-tooltip-id", cut.Markup);
    }

    [Fact]
    public void DataTooltipText_Attribute_IsPresent()
    {
        var cut = Render<XFTooltip>(parameters =>
        {
            parameters.Add(p => p.Text, "Helpful text");
            parameters.AddChildContent("Hover me");
        });

        Assert.Contains("data-tooltip-text=\"Helpful text\"", cut.Markup);
    }

    [Fact]
    public void DataTooltipPosition_Attribute_IsPresent()
    {
        var cut = Render<XFTooltip>(parameters =>
        {
            parameters.Add(p => p.Text, "Helpful text");
            parameters.Add(p => p.Position, XFTooltipPosition.Bottom);
            parameters.AddChildContent("Hover me");
        });

        Assert.Contains("data-tooltip-position=\"bottom\"", cut.Markup);
    }

    [Fact]
    public void DataTooltipTrigger_WhenHoverFocus_ContainsFocus()
    {
        var cut = Render<XFTooltip>(parameters =>
        {
            parameters.Add(p => p.Text, "Helpful text");
            parameters.Add(p => p.Trigger, XFTooltipTrigger.HoverFocus);
            parameters.AddChildContent("Hover me");
        });

        var trigger = cut.Find("[data-tooltip-trigger]");
        var triggerValue = trigger.GetAttribute("data-tooltip-trigger");
        Assert.Contains("focus", triggerValue);
    }

    [Fact]
    public void KeyDown_WhenEscape_CallsHideAsync()
    {
        var cut = Render<XFTooltip>(parameters =>
        {
            parameters.Add(p => p.Text, "Helpful text");
            parameters.AddChildContent("Hover me");
        });

        // Verify JS init was called on render
        JSInterop.VerifyInvoke("xfTooltip.init");

        var trigger = cut.Find("span.xf-tooltip");
        trigger.KeyDown("Escape");

        // Escape triggers HideAsync which calls xfTooltip.hide JS interop
        JSInterop.VerifyInvoke("xfTooltip.hide");
    }
}
