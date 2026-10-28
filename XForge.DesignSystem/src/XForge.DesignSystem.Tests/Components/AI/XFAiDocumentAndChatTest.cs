using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Documents;
using XForge.DesignSystem.Components.AI.Chat;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for document and chat tests.
/// </summary>
internal sealed class DocMockProvider : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-doc";
    public string DisplayName => "Mock Doc Provider";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse());
    }

    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
        => AsyncEnumerable.Empty<AiStreamChunk>();

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// Tests for XFAiDocumentReader, XFAiReportBuilder, and XFAiChat.
/// </summary>
public sealed class XFAiDocumentAndChatTest : XForgeTestContext
{
    public XFAiDocumentAndChatTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiDocumentReader

    [Fact]
    public void DocumentReader_Renders_Default()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        Assert.NotNull(cut);
    }

    [Fact]
    public void DocumentReader_Renders_WithTitle()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "Document Reader"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void DocumentReader_Renders_WithoutHeader()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void DocumentReader_Renders_WithoutProvider()
    {
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void DocumentReader_ProcessContentAsync_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () => await cut.Instance.ProcessContentAsync("Sample content", "test.txt"));
    }

    [Fact]
    public void DocumentReader_ProcessDocumentAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, null));
        cut.InvokeAsync(async () => await cut.Instance.ProcessDocumentAsync());
    }

    [Fact]
    public void DocumentReader_Clear_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    [Fact]
    public void DocumentReader_Dispose_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiReportBuilder

    [Fact]
    public void ReportBuilder_Renders_Default()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ReportBuilder_Renders_WithTitle()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "Report Builder"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ReportBuilder_Renders_WithoutHeader()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ReportBuilder_Renders_WithoutProvider()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ReportBuilder_Renders_WithLoadingState()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ReportBuilder_Dispose_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiChat

    [Fact]
    public void Chat_Renders_Default()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Chat_Renders_WithTitle()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiChat>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "My Chat"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Chat_Renders_WithoutProvider()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Chat_Renders_WithDisabledState()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiChat>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task Chat_SendMessageAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, null));
        await cut.InvokeAsync(async () => await cut.Instance.SendMessageAsync());
    }

    [Fact]
    public async Task Chat_ClearAsync_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        await cut.InvokeAsync(async () => await cut.Instance.ClearAsync());
    }

    [Fact]
    public async Task Chat_CancelAsync_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        await cut.InvokeAsync(async () => await cut.Instance.CancelAsync());
    }

    [Fact]
    public void Chat_Dispose_DoesNotThrow()
    {
        var provider = new DocMockProvider();
        var cut = Render<XFAiChat>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    #endregion
}
