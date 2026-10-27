using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.MultiModal;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class ExtendedMultiModalMockProvider : IAiProvider, IDisposable, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-multimodal";
    public string DisplayName => "Mock MultiModal";
    public string? DefaultModel => "mock";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "Mock response" }
        });
    }

    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
        => AsyncEnumerable.Empty<AiStreamChunk>();

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public void Dispose() { }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public sealed class XFAiMultiModalChatExtendedTest : BunitContext
{
    private readonly ExtendedMultiModalMockProvider _mockProvider;

    public XFAiMultiModalChatExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        _mockProvider = new ExtendedMultiModalMockProvider();
        Services.AddSingleton<IAiProvider>(_mockProvider);
        Services.AddSingleton<IAiVisionProvider>(new InMemoryAiVisionProvider());
    }

    private IRenderedComponent<XFAiMultiModalChat> RenderChat(
        Action<ComponentParameterCollectionBuilder<XFAiMultiModalChat>>? parameters = null)
    {
        return parameters is not null
            ? Render(parameters)
            : Render<XFAiMultiModalChat>();
    }

    [Fact]
    public void Extended_Inherits_XForgeComponentBase()
    {
        var cut = RenderChat();
        Assert.IsAssignableFrom<XForge.DesignSystem.Core.Base.XForgeComponentBase>(cut.Instance);
    }

    [Fact]
    public void Extended_CustomClass_AppearsOnRootElement()
    {
        var cut = RenderChat(p => p.Add(x => x.Class, "my-custom-class"));
        var div = cut.Find(".xf-ai-multimodal-chat");
        Assert.Contains("my-custom-class", div.GetAttribute("class"));
    }

    [Fact]
    public void Extended_CustomStyle_AppearsOnRootElement()
    {
        var cut = RenderChat(p => p.Add(x => x.Style, "margin-top: 20px"));
        var div = cut.Find(".xf-ai-multimodal-chat");
        Assert.Contains("margin-top: 20px", div.GetAttribute("style"));
    }

    [Fact]
    public void Extended_Disabled_IsTrue()
    {
        var cut = RenderChat(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Instance.Disabled);
    }

    [Fact]
    public void Extended_Title_RendersInHeader()
    {
        var cut = RenderChat(p => p.Add(x => x.Title, "My AI Assistant"));
        var header = cut.Find("h3");
        Assert.Contains("My AI Assistant", header.TextContent);
    }

    [Fact]
    public void Extended_Title_Null_ShowsDefault()
    {
        var cut = RenderChat();
        var header = cut.Find("h3");
        Assert.Contains("Chat Multi-modal", header.TextContent);
    }

    [Fact]
    public void Extended_ShowHeader_False_HidesHeader()
    {
        var cut = RenderChat(p => p.Add(x => x.ShowHeader, false));
        var headers = cut.FindAll("h3");
        Assert.Empty(headers);
    }

    [Fact]
    public void Extended_Placeholder_RendersOnTextarea()
    {
        var cut = RenderChat(p => p.Add(x => x.Placeholder, "Ask me anything..."));
        var textarea = cut.Find("textarea");
        Assert.Equal("Ask me anything...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void Extended_Placeholder_Null_ShowsDefault()
    {
        var cut = RenderChat();
        var textarea = cut.Find("textarea");
        Assert.Equal("Envie uma mensagem...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void Extended_AriaLabel_RendersOnRootElement()
    {
        var cut = RenderChat(p => p.Add(x => x.AriaLabel, "Custom Chat Label"));
        var div = cut.Find(".xf-ai-multimodal-chat");
        Assert.Equal("Custom Chat Label", div.GetAttribute("aria-label"));
    }

    [Fact]
    public void Extended_AriaLabel_DefaultValue()
    {
        var cut = RenderChat();
        var div = cut.Find(".xf-ai-multimodal-chat");
        Assert.Equal("Chat multi-modal AI", div.GetAttribute("aria-label"));
    }

    [Fact]
    public void Extended_MaxMessages_StoresValue()
    {
        var cut = RenderChat(p => p.Add(x => x.MaxMessages, 200));
        Assert.Equal(200, cut.Instance.MaxMessages);
    }

    [Fact]
    public void Extended_HeaderContent_RendersInsideHeader()
    {
        var cut = RenderChat(p => p.Add(x => x.HeaderContent, (RenderFragment)(builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddContent(1, "Custom Header");
            builder.CloseElement();
        })));
        var header = cut.Find(".xf-ai-multimodal-chat > div");
        Assert.Contains("Custom Header", header.InnerHtml);
    }

    [Fact]
    public void Extended_EmptyState_ShowsPlaceholderMessage()
    {
        var cut = RenderChat();
        var emptyText = cut.Find(".text-center p");
        Assert.Contains("Envie imagens", emptyText.TextContent);
    }

    [Fact]
    public void Extended_Messages_InitiallyEmpty()
    {
        var cut = RenderChat();
        Assert.Empty(cut.Instance.Messages);
    }

    [Fact]
    public void Extended_AddAttachment_IncreasesPendingCount()
    {
        var cut = RenderChat();
        var attachment = new XFAiAttachment
        {
            Name = "photo.png",
            Type = XFAiAttachmentType.Image,
            Data = "data:image/png;base64,abc"
        };
        cut.InvokeAsync(() => cut.Instance.AddAttachment(attachment));
        cut.Render();
        var previewImgs = cut.FindAll("img.w-12");
        Assert.Single(previewImgs);
    }

    [Fact]
    public void Extended_ClearMessages_RemovesAllMessages()
    {
        var cut = RenderChat();
        cut.InvokeAsync(() => cut.Instance.ClearMessages());
        Assert.Empty(cut.Instance.Messages);
    }

    [Fact]
    public void Extended_SendMessage_TriggersOnMessageSent()
    {
        var messageSent = false;
        var messageReceived = false;
        var errorReceived = false;

        var cut = RenderChat(p =>
        {
            p.Add(x => x.OnMessageSent, EventCallback.Factory.Create<XFAiMultiModalMessage>(this, _ => messageSent = true));
            p.Add(x => x.OnResponseReceived, EventCallback.Factory.Create<XFAiMultiModalMessage>(this, _ => messageReceived = true));
            p.Add(x => x.OnError, EventCallback.Factory.Create<string>(this, _ => errorReceived = true));
        });

        var textarea = cut.Find("textarea");
        textarea.Input("Hello AI");

        var sendButton = cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Enviar mensagem");
        cut.InvokeAsync(() => sendButton.Click());

        Assert.True(messageSent);
        Assert.True(messageReceived);
        Assert.False(errorReceived);
    }

    [Fact]
    public void Extended_SendMessage_EnterKey_TriggersSend()
    {
        var messageSent = false;
        var cut = RenderChat(p =>
        {
            p.Add(x => x.OnMessageSent, EventCallback.Factory.Create<XFAiMultiModalMessage>(this, _ => messageSent = true));
        });

        var textarea = cut.Find("textarea");
        textarea.Input("Test message");
        textarea.KeyDown(new KeyboardEventArgs { Key = "Enter", ShiftKey = false });

        Assert.True(messageSent);
    }

    [Fact]
    public void Extended_SendMessage_EnterWithShift_DoesNotTriggerSend()
    {
        var messageSent = false;
        var cut = RenderChat(p =>
        {
            p.Add(x => x.OnMessageSent, EventCallback.Factory.Create<XFAiMultiModalMessage>(this, _ => messageSent = true));
        });

        var textarea = cut.Find("textarea");
        textarea.Input("Test message");
        textarea.KeyDown(new KeyboardEventArgs { Key = "Enter", ShiftKey = true });

        Assert.False(messageSent);
    }

    [Fact]
    public void Extended_Error_TriggersOnError()
    {
        _mockProvider.ThrowException = new InvalidOperationException("Provider failure");
        var errorReceived = false;
        string? errorMessage = null;

        var cut = RenderChat(p =>
        {
            p.Add(x => x.OnError, EventCallback.Factory.Create<string>(this, e => { errorReceived = true; errorMessage = e; }));
        });

        var textarea = cut.Find("textarea");
        textarea.Input("Trigger error");

        var sendButton = cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Enviar mensagem");
        cut.InvokeAsync(() => sendButton.Click());

        Assert.True(errorReceived);
        Assert.NotNull(errorMessage);
        Assert.Contains("Provider failure", errorMessage);
    }

    [Fact]
    public void Extended_Disabled_DisablesTextarea()
    {
        var cut = RenderChat(p => p.Add(x => x.Disabled, true));
        var textarea = cut.Find("textarea");
        Assert.True(textarea.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_Disabled_DisablesSendButton()
    {
        var cut = RenderChat(p => p.Add(x => x.Disabled, true));
        var sendButton = cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Enviar mensagem");
        Assert.True(sendButton.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_Disabled_DisablesAttachmentButton()
    {
        var cut = RenderChat(p => p.Add(x => x.Disabled, true));
        var attachButton = cut.FindAll("button").First(b => b.GetAttribute("aria-label") == "Anexar arquivo");
        Assert.True(attachButton.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_RootElement_HasRoleLog()
    {
        var cut = RenderChat();
        var div = cut.Find(".xf-ai-multimodal-chat");
        Assert.Equal("log", div.GetAttribute("role"));
    }

    [Fact]
    public void Extended_RootElement_HasAriaLivePolite()
    {
        var cut = RenderChat();
        var div = cut.Find(".xf-ai-multimodal-chat");
        Assert.Equal("polite", div.GetAttribute("aria-live"));
    }

    [Fact]
    public void Extended_Dispose_DoesNotThrow()
    {
        var cut = RenderChat();
        var exception = Record.Exception(() => cut.Instance.Dispose());
        Assert.Null(exception);
    }
}
