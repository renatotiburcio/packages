using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Popover;

namespace XForge.DesignSystem.Tests.Components;

public class XFPopoverKeyboardTest : XForgeTestContext
{
    public XFPopoverKeyboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Escape_Key_Closes_Popover()
    {
        var open = true;
        var cut = Render<XFPopover>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
            parameters.AddChildContent("Popover content");
        });

        Assert.Contains("role=\"dialog\"", cut.Markup);

        var panel = cut.Find("div[role='dialog']");
        panel.KeyDown("Escape");

        Assert.False(open);
    }

    [Fact]
    public void Tab_Key_Traps_Focus_Inside_Popover_When_TrapFocus()
    {
        var open = true;
        var cut = Render<XFPopover>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.TrapFocus, true);
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
            parameters.AddChildContent("<button id='popover-btn'>Action</button>");
        });

        var panel = cut.Find("div[role='dialog']");
        Assert.NotNull(panel);
        Assert.Equal("-1", panel.GetAttribute("tabindex"));
    }

    [Fact]
    public void Click_On_Trigger_Button_Toggles_Popover()
    {
        var open = false;
        var cut = Render<XFPopover>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.ButtonText, "Toggle Popover");
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
            parameters.AddChildContent("Content");
        });

        Assert.False(open);

        cut.Find("button").Click();
        cut.WaitForAssertion(() => Assert.True(open));

        cut.Find("button").Click();
        cut.WaitForAssertion(() => Assert.False(open));
    }
}
