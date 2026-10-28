using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Fiscal;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class NFeMockProvider : IAiProvider, IDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-nfe";
    public string DisplayName => "Mock NFe";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = """{"nFeKey":"123","issuerCnpj":"12.345.678/0001-90","issuerName":"Empresa Teste","totalValue":1000.0,"aiSummary":"Test","spedStatus":"Valid","riskLevel":"Low","confidence":0.95}"""
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
}

public sealed class XFAiNFeAnalyzerTest : XForgeTestContext
{
    public XFAiNFeAnalyzerTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact] public void NFe_Renders_Default()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.Title, "Custom NFe"));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithoutHeader()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.Placeholder, "Paste XML here..."));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithCustomSystemPrompt()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithLoadingState()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithResultPreview()
    {
        var result = new XFAiNFeAnalysisResult { NFeKey = "123", AiSummary = "Test" };
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.ResultPreview, (RenderFragment<XFAiNFeAnalysisResult>)(_ => b => b.AddContent(0, "preview"))));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "header"))));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithEmptyContent()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "empty"))));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_Renders_WithCustomClass()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact] public void NFe_AnalyzeAsync_WithProvider_CallsProvider()
    {
        var provider = new NFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "<xml>test</xml>");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Equal(1, provider.CallCount);
    }

    [Fact] public void NFe_AnalyzeAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, null));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact] public void NFe_AnalyzeAsync_EmptyInput_DoesNotCallProvider()
    {
        var provider = new NFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Equal(0, provider.CallCount);
    }

    [Fact] public void NFe_AnalyzeAsync_Exception_DoesNotThrow()
    {
        var provider = new NFeMockProvider { ThrowException = new Exception("API error") };
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact] public void NFe_AnalyzeAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new NFeMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact] public void NFe_Clear_DoesNotThrow()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()));
        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    [Fact] public void NFe_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()));
        cut.Instance.Dispose();
    }

    [Fact] public void NFe_Renders_WithDisabledState()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new NFeMockProvider()).Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }
}
