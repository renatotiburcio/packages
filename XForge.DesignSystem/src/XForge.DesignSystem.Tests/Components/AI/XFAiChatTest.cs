using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Chat;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class ChatMockProvider : IAiProvider, IDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-chat";
    public string DisplayName => "Mock Chat";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "Hello!" }
        });
    }

    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
        => AsyncEnumerable.Empty<AiStreamChunk>();
    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });
    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    public void Dispose() { }
}

public sealed class XFAiChatTest : BunitContext
{
    public XFAiChatTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiProvider>(new ChatMockProvider());
    }

    [Fact] public void Chat_Renders_Default()
    {
        var cut = Render<XFAiChat>();
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Title, "My Chat"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithSubtitle()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Subtitle, "AI Assistant"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithoutHeader()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Placeholder, "Type here..."));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithStreamingFalse()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Streaming, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithAttachments()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.AllowAttachments, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithVoice()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.AllowVoice, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithTemperature()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Temperature, 0.7));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithMaxTokens()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.MaxTokens, 2048));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithSystemPrompt()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.SystemPrompt, "You are a helpful assistant"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithModel()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Model, "gpt-4"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithShowTimestampsFalse()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.ShowTimestamps, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithShowCopyButtonFalse()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.ShowCopyButton, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithShowTokenUsage()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.ShowTokenUsage, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithCustomSendLabel()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.SendLabel, "Send"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithCustomCancelLabel()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.CancelLabel, "Stop"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithCustomCopyLabel()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.CopyLabel, "Copy"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithCustomClass()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Class, "custom-chat"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_InitialState_Idle()
    {
        var cut = Render<XFAiChat>();
        Assert.Equal(XFAiChatState.Idle, cut.Instance.State);
        Assert.Equal(0, cut.Instance.TotalTokens);
        Assert.Null(cut.Instance.ErrorText);
    }

    [Fact] public void Chat_SendMessageAsync_WithProvider_CallsProvider()
    {
        var provider = new ChatMockProvider();
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiChat).GetField("_draft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Hello AI");
            await cut.Instance.SendMessageAsync();
        });
        Assert.Equal(1, provider.CallCount);
    }

    [Fact] public void Chat_SendMessageAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, null));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiChat).GetField("_draft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.SendMessageAsync();
        });
    }

    [Fact] public void Chat_SendMessageAsync_EmptyInput_DoesNotCallProvider()
    {
        var provider = new ChatMockProvider();
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () => await cut.Instance.SendMessageAsync());
        Assert.Equal(0, provider.CallCount);
    }

    [Fact] public void Chat_SendMessageAsync_Exception_DoesNotThrow()
    {
        var provider = new ChatMockProvider { ThrowException = new Exception("API error") };
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiChat).GetField("_draft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.SendMessageAsync();
        });
    }

    [Fact] public void Chat_ClearAsync_DoesNotThrow()
    {
        var cut = Render<XFAiChat>();
        cut.InvokeAsync(async () => await cut.Instance.ClearAsync());
    }

    [Fact] public void Chat_CancelAsync_DoesNotThrow()
    {
        var cut = Render<XFAiChat>();
        cut.InvokeAsync(async () => await cut.Instance.CancelAsync());
    }

    [Fact] public void Chat_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiChat>();
        cut.Instance.Dispose();
    }

    [Fact] public void Chat_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "header"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithMessageTemplate()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.MessageTemplate, (RenderFragment<AiMessage>)(msg => b => b.AddContent(0, "msg"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithFooterContent()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.FooterContent, (RenderFragment)(b => b.AddContent(0, "footer"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Chat_Renders_WithEmptyContent()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "empty"))));
        Assert.NotNull(cut);
    }
}
