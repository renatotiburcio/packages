using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Utilities.FocusTrap;

namespace XForge.DesignSystem.Tests.Components;

public class XFFocusTrapComponentTest : XForgeTestContext
{
    public XFFocusTrapComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_Active_RendersContentAndActiveState()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.AddChildContent("<button>Save</button>");
        });

        Assert.Equal("true", cut.Find(".xf-focus-trap").GetAttribute("data-active"));
        Assert.Contains("Save", cut.Markup);
    }

    [Fact]
    public void Render_WhenInvisible_DoesNotRender()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden content");
        });

        Assert.DoesNotContain("Hidden content", cut.Markup);
    }

    [Fact]
    public async Task ActivateAsync_InvokesJs()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.Add(p => p.InitialFocusSelector, "[data-first]");
            parameters.AddChildContent("<button data-first=\"true\">First</button>");
        });

        await cut.Instance.ActivateAsync();

        Assert.Contains(JSInterop.Invocations, invocation => invocation.Identifier == "xfFocusTrap.activate");
    }

    [Fact]
    public async Task ReleaseAsync_InvokesJs()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.AddChildContent("<button>First</button>");
        });

        await cut.Instance.ActivateAsync();
        await cut.Instance.ReleaseAsync();

        Assert.Contains(JSInterop.Invocations, invocation => invocation.Identifier == "xfFocusTrap.release");
    }

    [Fact]
    public void Escape_RaisesCallbackAndActiveChanged()
    {
        var escaped = false;
        bool? active = null;
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.Add(p => p.EscapeReleases, true);
            parameters.Add(p => p.OnEscape, () => escaped = true);
            parameters.Add(p => p.ActiveChanged, value => active = value);
            parameters.AddChildContent("<button>First</button>");
        });

        cut.Find(".xf-focus-trap").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.True(escaped);
        Assert.False(active);
    }

    [Fact]
    public void Render_Inactive_RendersContentWithInactiveState()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.AddChildContent("<button>Content</button>");
        });

        Assert.Equal("false", cut.Find(".xf-focus-trap").GetAttribute("data-active"));
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void Render_ActiveTrue_DefaultValue()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.AddChildContent("<button>Default</button>");
        });

        Assert.Equal("true", cut.Find(".xf-focus-trap").GetAttribute("data-active"));
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.AddChildContent("<input type=\"text\" />");
        });

        Assert.Contains("<input", cut.Markup);
    }

    [Fact]
    public void Render_WithId()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Id, "trap-123");
            parameters.AddChildContent("<button>Save</button>");
        });

        Assert.Contains("id=\"trap-123\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomClass()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Class, "custom-trap");
            parameters.AddChildContent("<button>Save</button>");
        });

        Assert.Contains("custom-trap", cut.Markup);
    }

    [Fact]
    public void Render_WithStyle()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Style, "width: 300px");
            parameters.AddChildContent("<button>Save</button>");
        });

        Assert.Contains("width: 300px", cut.Markup);
    }

    [Fact]
    public void Render_HasTabindexNegative()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.AddChildContent("<button>Save</button>");
        });

        Assert.Equal("-1", cut.Find(".xf-focus-trap").GetAttribute("tabindex"));
    }

    [Fact]
    public void Render_DataActive_Attribute()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.AddChildContent("<button>Save</button>");
        });

        Assert.Equal("true", cut.Find(".xf-focus-trap").GetAttribute("data-active"));
    }

    [Fact]
    public void Escape_NoDelegate_DoesNotThrow()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.AddChildContent("<button>First</button>");
        });

        cut.Find(".xf-focus-trap").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.True(true);
    }

    [Fact]
    public void Escape_NoEscapeReleases_DoesNotDeactivate()
    {
        bool? active = null;
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.Add(p => p.EscapeReleases, false);
            parameters.Add(p => p.ActiveChanged, value => active = value);
            parameters.AddChildContent("<button>First</button>");
        });

        cut.Find(".xf-focus-trap").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.Null(active);
    }

    [Fact]
    public async Task FocusInitialAsync_InvokesJs()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.InitialFocusSelector, "#first-input");
            parameters.AddChildContent("<input id=\"first-input\" />");
        });

        await cut.Instance.FocusInitialAsync();

        Assert.Contains(JSInterop.Invocations, invocation => invocation.Identifier == "xfFocusTrap.focusInitial");
    }

    [Fact]
    public void Escape_NonEscapeKey_DoesNotTrigger()
    {
        bool? active = null;
        var escaped = false;
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.Add(p => p.EscapeReleases, true);
            parameters.Add(p => p.OnEscape, () => escaped = true);
            parameters.Add(p => p.ActiveChanged, value => active = value);
            parameters.AddChildContent("<button>First</button>");
        });

        cut.Find(".xf-focus-trap").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.False(escaped);
        Assert.Null(active);
    }

    [Fact]
    public async Task ReleaseAsync_WhenNotActive_DoesNotInvokeJs()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.AddChildContent("<button>First</button>");
        });

        await cut.Instance.ReleaseAsync();

        Assert.Empty(JSInterop.Invocations);
    }

    [Fact]
    public async Task ActivateAsync_CalledTwice_DoesNotInvokeJsTwice()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.AddChildContent("<button>First</button>");
        });

        await cut.Instance.ActivateAsync();
        await cut.Instance.ActivateAsync();

        var activateInvocations = JSInterop.Invocations
            .Where(i => i.Identifier == "xfFocusTrap.activate")
            .ToList();
        Assert.Single(activateInvocations);
    }

    [Fact]
    public async Task DisposeAsync_WhenActive_ReleasesTrap()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.AddChildContent("<button>First</button>");
        });

        await cut.Instance.ActivateAsync();
        await cut.Instance.DisposeAsync();

        Assert.Contains(JSInterop.Invocations, invocation => invocation.Identifier == "xfFocusTrap.release");
    }

    [Fact]
    public async Task DisposeAsync_WhenNotActive_DoesNotRelease()
    {
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, false);
            parameters.AddChildContent("<button>First</button>");
        });

        await cut.Instance.DisposeAsync();

        Assert.Empty(JSInterop.Invocations);
    }

    [Fact]
    public void Escape_EscapeReleasesFalse_FiresOnEscapeButKeepsActive()
    {
        var escaped = false;
        bool? active = null;
        var cut = Render<XFFocusTrap>(parameters =>
        {
            parameters.Add(p => p.Active, true);
            parameters.Add(p => p.EscapeReleases, false);
            parameters.Add(p => p.OnEscape, () => escaped = true);
            parameters.Add(p => p.ActiveChanged, value => active = value);
            parameters.AddChildContent("<button>First</button>");
        });

        cut.Find(".xf-focus-trap").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.True(escaped);
        Assert.Null(active);
        Assert.Equal("true", cut.Find(".xf-focus-trap").GetAttribute("data-active"));
    }
}
