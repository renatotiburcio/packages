#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Chat;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFChatComponentTest : Bunit.BunitContext
{
    public XFChatComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFChat>();
        Assert.Contains("xf-chat", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle()
    {
        var cut = Render<XFChat>(parameters =>
        {
            parameters.Add(p => p.Title, "Support Chat");
        });
        Assert.Contains("Support Chat", cut.Markup);
    }

    [Fact]
    public void Render_WithSubtitle()
    {
        var cut = Render<XFChat>(parameters =>
        {
            parameters.Add(p => p.Title, "Chat");
            parameters.Add(p => p.Subtitle, "3 participants");
        });
        Assert.Contains("3 participants", cut.Markup);
    }

    [Fact]
    public void Render_WithPlaceholder()
    {
        var cut = Render<XFChat>(parameters =>
        {
            parameters.Add(p => p.Placeholder, "Type a message...");
        });
        Assert.Contains("Type a message...", cut.Markup);
    }

    [Fact]
    public void Render_LoadingState()
    {
        var cut = Render<XFChat>(parameters =>
        {
            parameters.Add(p => p.Loading, true);
        });
        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }
}
