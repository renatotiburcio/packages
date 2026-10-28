using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Feedback.Chat;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public sealed class XFChatExtendedTest : XForgeTestContext
{
    private static readonly XFChatUser User = new() { Id = "user", Name = "Renato" };
    private static readonly XFChatUser Bot = new() { Id = "bot", Name = "Assistant" };

    private static readonly IReadOnlyList<XFChatMessage> SampleMessages = new[]
    {
        new XFChatMessage { Id = "m1", UserId = "user", Content = "Hello", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
        new XFChatMessage { Id = "m2", UserId = "bot", Content = "Hi there!", Timestamp = DateTime.UtcNow.AddMinutes(-4) }
    };

    private IRenderedComponent<XFChat> RenderChat(
        Action<ComponentParameterCollectionBuilder<XFChat>>? configure = null)
    {
        return Render<XFChat>(p =>
        {
            p.Add(x => x.Messages, SampleMessages);
            p.Add(x => x.CurrentUserId, "user");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderChat();
        Assert.Contains("xf-chat", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Messages()
    {
        var cut = RenderChat();
        Assert.Contains("Hello", cut.Markup);
        Assert.Contains("Hi there!", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Title()
    {
        var cut = RenderChat(p => p.Add(x => x.Title, "Chat Title"));
        Assert.Contains("Chat Title", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Subtitle()
    {
        var cut = RenderChat(p => p.Add(x => x.Subtitle, "Online"));
        Assert.Contains("Online", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Composer()
    {
        var cut = RenderChat();
        Assert.Contains("Digite sua mensagem", cut.Markup);
    }

    [Fact]
    public void Extended_CustomPlaceholder_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.Placeholder, "Type here..."));
        Assert.Contains("Type here...", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_SendButton()
    {
        var cut = RenderChat();
        Assert.Contains("Enviar mensagem", cut.Markup);
    }

    [Fact]
    public void Extended_CustomSendLabel_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.SendLabel, "Send"));
        Assert.Contains("Send", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyMessages_RendersEmptyText()
    {
        var cut = Render<XFChat>(p => p.Add(x => x.CurrentUserId, "user"));
        Assert.Contains("Nenhuma mensagem ainda.", cut.Markup);
    }

    [Fact]
    public void Extended_CustomEmptyText_Renders()
    {
        var cut = Render<XFChat>(p => p
            .Add(x => x.CurrentUserId, "user")
            .Add(x => x.EmptyText, "No messages"));
        Assert.Contains("No messages", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyContent_Renders()
    {
        var cut = Render<XFChat>(p => p
            .Add(x => x.CurrentUserId, "user")
            .Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "custom empty"))));
        Assert.Contains("custom empty", cut.Markup);
    }

    [Fact]
    public void Extended_Loading_RendersLoadingState()
    {
        var cut = RenderChat(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_ReadOnly_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.ReadOnly, true));
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Extended_ShowTypingIndicator_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.ShowTypingIndicator, true));
        Assert.Contains("Digitando...", cut.Markup);
    }

    [Fact]
    public void Extended_CustomTypingText_Renders()
    {
        var cut = RenderChat(p => p
            .Add(x => x.ShowTypingIndicator, true)
            .Add(x => x.TypingText, "User is typing..."));
        Assert.Contains("User is typing...", cut.Markup);
    }

    [Fact]
    public void Extended_SendOnEnterFalse_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.SendOnEnter, false));
        Assert.Contains("xf-chat", cut.Markup);
    }

    [Fact]
    public void Extended_SizeSm_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("xf-chat", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    [Fact]
    public void Extended_CustomRows_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.Rows, 5));
        Assert.Contains("xf-chat", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderContent_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "header"))));
        Assert.Contains("header", cut.Markup);
    }

    [Fact]
    public void Extended_ComposerStart_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.ComposerStart, (RenderFragment)(b => b.AddContent(0, "start"))));
        Assert.Contains("start", cut.Markup);
    }

    [Fact]
    public void Extended_ComposerEnd_Renders()
    {
        var cut = RenderChat(p => p.Add(x => x.ComposerEnd, (RenderFragment)(b => b.AddContent(0, "end"))));
        Assert.Contains("end", cut.Markup);
    }

    [Fact]
    public void Extended_MessageTemplate_Renders()
    {
        var cut = RenderChat(p => p.Add(
            x => x.MessageTemplate,
            (RenderFragment<XFChatMessage>)(msg => b => b.AddContent(0, $"msg:{msg.Content}"))));
        Assert.Contains("msg:Hello", cut.Markup);
    }

    [Fact]
    public async Task Extended_Send_FiresMessageSent()
    {
        XFChatSendEventArgs? sent = null;
        var cut = RenderChat(p => p.Add(
            x => x.MessageSent,
            EventCallback.Factory.Create<XFChatSendEventArgs>(this, args => sent = args)));
        await cut.Find("textarea").InputAsync(new ChangeEventArgs { Value = "Test message" });
        await cut.Find("button").ClickAsync();
        Assert.NotNull(sent);
    }

    [Fact]
    public async Task Extended_DraftChanged_Fires()
    {
        string? draft = null;
        var cut = RenderChat(p => p.Add(
            x => x.DraftChanged,
            EventCallback.Factory.Create<string?>(this, d => draft = d)));
        await cut.Find("textarea").InputAsync(new ChangeEventArgs { Value = "draft text" });
        Assert.Equal("draft text", draft);
    }

    [Fact]
    public void Extended_XFChatMessage_CanCreate()
    {
        var msg = new XFChatMessage
        {
            Id = "m1",
            UserId = "user",
            Content = "Hello",
            Timestamp = DateTime.UtcNow
        };
        Assert.Equal("m1", msg.Id);
        Assert.Equal("Hello", msg.Content);
    }

    [Fact]
    public void Extended_XFChatUser_CanCreate()
    {
        var user = new XFChatUser { Id = "u1", Name = "Test User", AvatarUrl = "avatar.png" };
        Assert.Equal("u1", user.Id);
        Assert.Equal("Test User", user.Name);
        Assert.Equal("avatar.png", user.AvatarUrl);
    }

    [Fact]
    public void Extended_XFChatSendEventArgs_CanCreate()
    {
        var args = new XFChatSendEventArgs("Hello world", "user", DateTime.UtcNow);
        Assert.Equal("Hello world", args.Content);
        Assert.Equal("user", args.CurrentUserId);
    }
}
