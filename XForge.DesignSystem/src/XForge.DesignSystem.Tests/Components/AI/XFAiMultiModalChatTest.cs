using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.MultiModal;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for MultiModalChat tests.
/// </summary>
internal sealed class MultiModalMockProvider : IAiProvider, IDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-multimodal";
    public string DisplayName => "Mock MultiModal";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "AI response text" }
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

/// <summary>
/// Tests for XFAiMultiModalChat component.
/// </summary>
public sealed class XFAiMultiModalChatTest : BunitContext
{
    public XFAiMultiModalChatTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiProvider>(new MultiModalMockProvider());
        Services.AddSingleton<IAiVisionProvider>(new InMemoryAiVisionProvider());
    }

    [Fact]
    public void MultiModal_Renders_Default()
    {
        var cut = Render<XFAiMultiModalChat>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithTitle()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.Title, "AI Chat"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithoutHeader()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.Placeholder, "Type a message..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithCustomAriaLabel()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.AriaLabel, "AI Assistant"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithMaxMessages()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.MaxMessages, 50));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.HeaderContent, (RenderFragment)(builder => { builder.AddContent(0, "header"); })));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithCustomClass()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.Class, "custom-chat"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithDisabledState()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_InitialState_NoMessages()
    {
        var cut = Render<XFAiMultiModalChat>();
        Assert.Empty(cut.Instance.Messages);
    }

    [Fact]
    public void MultiModal_AddAttachment_DoesNotThrow()
    {
        var cut = Render<XFAiMultiModalChat>();
        var attachment = new XFAiAttachment { Name = "test.png", Type = XFAiAttachmentType.Image, Data = "base64data" };
        cut.InvokeAsync(() => cut.Instance.AddAttachment(attachment));
    }

    [Fact]
    public void MultiModal_ClearMessages_DoesNotThrow()
    {
        var cut = Render<XFAiMultiModalChat>();
        cut.InvokeAsync(() => cut.Instance.ClearMessages());
    }

    [Fact]
    public void MultiModal_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiMultiModalChat>();
        cut.Instance.Dispose();
    }
}
