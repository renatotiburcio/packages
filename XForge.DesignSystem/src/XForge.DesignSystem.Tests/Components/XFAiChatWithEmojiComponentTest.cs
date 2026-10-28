using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI;

namespace XForge.DesignSystem.Tests.Components;

public class XFAiChatWithEmojiComponentTest : XForgeTestContext
{
    public XFAiChatWithEmojiComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_RendersChatContainer()
    {
        var cut = Render<XFAiChatWithEmoji>();
        Assert.Contains("xf-ai-chat-emoji", cut.Markup);
    }

    [Fact]
    public void Render_HasInputField()
    {
        var cut = Render<XFAiChatWithEmoji>();
        Assert.Contains("Chat input", cut.Markup);
    }

    [Fact]
    public void Render_HasSendButton()
    {
        var cut = Render<XFAiChatWithEmoji>();
        Assert.Contains("Send", cut.Markup);
    }

    [Fact]
    public void Render_HasLogRole()
    {
        var cut = Render<XFAiChatWithEmoji>();
        Assert.Contains("role=\"log\"", cut.Markup);
    }
}