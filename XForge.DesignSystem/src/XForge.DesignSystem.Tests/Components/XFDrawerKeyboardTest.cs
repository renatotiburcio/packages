using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Drawer;

namespace XForge.DesignSystem.Tests.Components;

public class XFDrawerKeyboardTest : BunitContext
{
    public XFDrawerKeyboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Escape_Key_Closes_Drawer()
    {
        var open = true;
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        cut.WaitForAssertion(() => Assert.Contains("role=\"dialog\"", cut.Markup));

        var drawer = cut.Find("aside");
        drawer.KeyDown("Escape");

        cut.WaitForAssertion(() => Assert.False(open));
    }

    [Fact]
    public void CloseOnEscape_False_Escape_Does_Not_Close()
    {
        var open = true;
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.CloseOnEscape, false);
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var drawer = cut.Find("aside");
        drawer.KeyDown("Escape");

        Assert.True(open);
    }

    [Fact]
    public void Tab_Key_Inside_Drawer()
    {
        var open = true;
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Title, "Drawer Title");
            parameters.AddChildContent("<button id='inner-btn'>Inner</button>");
        });

        var panel = cut.Find("aside");
        Assert.NotNull(panel);
    }

    [Fact]
    public void Drawer_Has_TabIndex_Negative_One()
    {
        var open = true;
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Title, "Focus Test");
        });

        Assert.Contains("tabindex=\"-1\"", cut.Markup);
    }

    [Fact]
    public void Drawer_Role_Is_Dialog()
    {
        var open = true;
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, open);
        });

        Assert.Contains("role=\"dialog\"", cut.Markup);
        Assert.Contains("aria-modal=\"true\"", cut.Markup);
    }
}
