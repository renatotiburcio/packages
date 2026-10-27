#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Chat;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFMessageListComponentTest : Bunit.BunitContext
{
    public XFMessageListComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFMessageList>();
        Assert.Contains("xf-message-list", cut.Markup);
    }

    [Fact]
    public void Render_WithEmptyText()
    {
        var cut = Render<XFMessageList>(parameters =>
        {
            parameters.Add(p => p.EmptyText, "No messages yet");
        });
        Assert.Contains("No messages yet", cut.Markup);
    }

    [Fact]
    public void Render_WithMessages()
    {
        var messages = new[]
        {
            new XFChatMessage { Content = "Hello", UserId = "user1", Role = AiRole.User }
        };
        var cut = Render<XFMessageList>(parameters =>
        {
            parameters.Add(p => p.Messages, messages);
            parameters.Add(p => p.CurrentUserId, "user1");
        });
        Assert.Contains("Hello", cut.Markup);
    }

    [Fact]
    public void Render_WithTypingIndicator()
    {
        var cut = Render<XFMessageList>(parameters =>
        {
            parameters.Add(p => p.ShowTypingIndicator, true);
            parameters.Add(p => p.TypingText, "Typing...");
        });
        Assert.Contains("Typing...", cut.Markup);
    }

    [Fact]
    public void Render_WithEmptyContent()
    {
        var cut = Render<XFMessageList>(parameters =>
        {
            parameters.Add(p => p.EmptyContent, (RenderFragment)(builder => builder.AddContent(0, "Custom empty")));
        });
        Assert.Contains("Custom empty", cut.Markup);
    }
}