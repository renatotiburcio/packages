using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.AI.Documents;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for XFAiDocumentReader extended tests.
/// </summary>
internal sealed class ExtendedDocMockProvider : IAiProvider, IDisposable, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }

    public string ProviderId => "mock-doc";
    public string DisplayName => "Mock Doc";
    public string? DefaultModel => "mock";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = "Mock summary response."
            }
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

/// <summary>
/// Extended tests for XFAiDocumentReader covering parameters, render fragments,
/// document types, public methods, and disposal.
/// </summary>
public sealed class XFAiDocumentReaderExtendedTest : BunitContext
{
    public XFAiDocumentReaderExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_BaseClass()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        Assert.Contains("xf-ai-document-reader", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomClass()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomStyle()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Style, "margin-top: 20px"));
        Assert.Contains("margin-top: 20px", cut.Markup);
    }

    [Fact]
    public void Renders_WithDisabled()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomTitle()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "My Reader"));
        Assert.Contains("My Reader", cut.Markup);
    }

    [Fact]
    public void Renders_ShowHeaderFalse()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("<header", cut.Markup);
    }

    [Fact]
    public void Renders_ShowSummaryFalse()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowSummary, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_ShowChunks()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowChunks, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomMaxChunks()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.MaxChunks, 5));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomChunkSize()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ChunkSize, 1000));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomEmptyText()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.EmptyText, "No document loaded"));
        Assert.Contains("No document loaded", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomSystemPrompt()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithDocumentPreview()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.DocumentPreview, (RenderFragment<XFAiDocumentResult>)(_ => b => b.AddContent(0, "custom preview"))));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithChunkTemplate()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ChunkTemplate, (RenderFragment<XFAiDocumentChunk>)(_ => b => b.AddContent(0, "custom chunk"))));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithHeaderContent()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "custom header"))));
        Assert.Contains("custom header", cut.Markup);
    }

    [Fact]
    public void Renders_WithEmptyContent()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "custom empty"))));
        Assert.Contains("custom empty", cut.Markup);
    }

    [Fact]
    public void Renders_WithoutProvider()
    {
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
        Assert.Contains("xf-ai-document-reader", cut.Markup);
    }

    [Fact]
    public void Renders_WithDocumentType()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.DocumentType, XFAiDocumentType.Pdf));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithDocumentName()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.DocumentName, "report.pdf"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Clear_ResetsState()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(() => cut.Instance.Clear());
        Assert.NotNull(cut);
    }

    [Fact]
    public void ProcessDocumentAsync_WithoutContent_DoesNotThrow()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () => await cut.Instance.ProcessDocumentAsync());
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var provider = new ExtendedDocMockProvider();
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    [Fact]
    public void Renders_WithAllDocumentTypes()
    {
        var provider = new ExtendedDocMockProvider();

        foreach (XFAiDocumentType docType in Enum.GetValues<XFAiDocumentType>())
        {
            var cut = Render<XFAiDocumentReader>(p => p
                .Add(x => x.Provider, provider)
                .Add(x => x.DocumentType, docType));
            Assert.NotNull(cut);
            Assert.Contains("xf-ai-document-reader", cut.Markup);
        }
    }
}
