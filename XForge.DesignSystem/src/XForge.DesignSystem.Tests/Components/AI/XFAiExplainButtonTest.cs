using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Utility;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for XFAiExplainButton tests.
/// </summary>
internal sealed class ExplainMockProvider : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public AiChatRequest? LastRequest { get; private set; }

    public string ProviderId => "mock-explain";
    public string DisplayName => "Mock Explain Provider";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        LastRequest = request;
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
/// Tests for XFAiExplainButton — explain generation, JSON parsing,
/// error handling, variants, accessibility, and dispose lifecycle.
/// </summary>
public sealed class XFAiExplainButtonTest : BunitContext
{
    public XFAiExplainButtonTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_WithDefaultParameters()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Invoice number"));

        // Default button label is "Explicar"
        Assert.Contains("Explicar", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomButtonLabel()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ButtonLabel, "Explain AI"));

        Assert.Contains("Explain AI", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomTitle()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "AI Explanation"));

        Assert.Contains("AI Explanation", cut.Markup);
    }

    [Fact]
    public void Renders_WithShowLabelFalse()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowLabel, false));

        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomAriaLabel()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.AriaLabel, "Get AI explanation"));

        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_GhostVariant()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Variant, XFAiExplainVariant.Ghost));

        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_SolidVariant()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Variant, XFAiExplainVariant.Solid));

        Assert.NotNull(cut);
    }

    [Fact]
    public async Task ExplainAsync_ValidJsonResponse_FiresOnExplained()
    {
        var provider = new ExplainMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = """{"elementName":"Field","explanation":"This field represents...","confidence":0.85,"sources":["docs"]}"""
                }
            }
        };

        XFAiExplainResult? explained = null;
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field")
            .Add(x => x.OnExplained, e => { explained = e; }));

        var explainButton = cut.Find("button");
        await cut.InvokeAsync(async () => await explainButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

        Assert.NotNull(explained);
        Assert.Equal("Field", explained.ElementName);
    }

    [Fact]
    public async Task ExplainAsync_NonJsonResponse_FiresOnExplainedWithFallback()
    {
        var provider = new ExplainMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "This is a plain text explanation."
                }
            }
        };

        XFAiExplainResult? explained = null;
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field")
            .Add(x => x.OnExplained, e => { explained = e; }));

        var explainButton = cut.Find("button");
        await cut.InvokeAsync(async () => await explainButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

        Assert.NotNull(explained);
        Assert.Equal("Field", explained.ElementName);
        Assert.Equal("This is a plain text explanation.", explained.Explanation);
    }

    [Fact]
    public async Task ExplainAsync_ProviderException_FiresOnError()
    {
        var provider = new ExplainMockProvider
        {
            ThrowException = new Exception("Provider failed")
        };

        string? errorMessage = null;
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field")
            .Add(x => x.OnError, e => { errorMessage = e; }));

        var explainButton = cut.Find("button");
        await cut.InvokeAsync(async () => await explainButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

        Assert.NotNull(errorMessage);
        Assert.Contains("Provider failed", errorMessage);
    }

    [Fact]
    public async Task ExplainAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new ExplainMockProvider
        {
            ThrowException = new OperationCanceledException()
        };

        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field"));

        var explainButton = cut.Find("button");
        await cut.InvokeAsync(async () => await explainButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
    }

    [Fact]
    public void Renders_WithoutProvider_DoesNotThrow()
    {
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, null)
            .Add(x => x.ElementContext, "Field"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithEmptyElementContext_DoesNotThrow()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, ""));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithLoadingState()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field")
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomSystemPrompt()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field")
            .Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomContent()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field")
            .Add(x => x.ExplainTemplate, result => builder => { builder.AddContent(0, "Custom template content"); }));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var provider = new ExplainMockProvider();
        var cut = Render<XFAiExplainButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ElementContext, "Field"));
        cut.Instance.Dispose();
    }
}
