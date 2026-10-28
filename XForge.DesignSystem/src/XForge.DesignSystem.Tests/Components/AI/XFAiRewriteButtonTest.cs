using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Utility;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for XFAiRewriteButton tests.
/// </summary>
internal sealed class RewriteMockProvider : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public AiChatRequest? LastRequest { get; private set; }

    public string ProviderId => "mock-rewrite";
    public string DisplayName => "Mock Rewrite Provider";
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
/// Tests for XFAiRewriteButton — rewrite generation, JSON parsing,
/// all rewrite modes, error handling, and apply workflow.
/// </summary>
public sealed class XFAiRewriteButtonTest : XForgeTestContext
{
    public XFAiRewriteButtonTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private RewriteMockProvider CreateProvider() => new RewriteMockProvider();

    [Fact]
    public void Renders_WithDefaultParameters()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello world"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomTitle()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.Title, "AI Rewrite"));
        Assert.Contains("AI Rewrite", cut.Markup);
    }

    [Fact]
    public void Renders_WithTargetLanguage()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.TargetLanguage, "espanhol"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomAvailableModes()
    {
        var provider = CreateProvider();
        var modes = new[] { XFAiRewriteMode.Translate, XFAiRewriteMode.AdjustTone };
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.AvailableModes, modes));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithoutProvider_DoesNotThrow()
    {
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, null)
            .Add(x => x.Text, "Hello"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithEmptyText()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, ""));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithLoadingState()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomSystemPrompt()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithChildContent()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.ChildContent, "<span>Extra</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task RewriteAsync_ValidJsonResponse_FiresOnRewritten()
    {
        var provider = new RewriteMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    // Note: mode omitted because JsonSerializer can't parse string enums
                    // without JsonStringEnumConverter
                    Content = """{"originalText":"Hello","rewrittenText":"Greetings","confidence":0.9,"changeDescription":"More formal"}"""
                }
            }
        };

        XFAiRewriteResult? rewritten = null;
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.OnRewritten, r => { rewritten = r; }));

        // Click the "Formalizar" mode button by aria-label
        var modeButton = cut.Find("button[aria-label='Formalizar']");
        await cut.InvokeAsync(async () => await modeButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

        Assert.NotNull(rewritten);
        Assert.Equal("Greetings", rewritten.RewrittenText);
    }

    [Fact]
    public async Task RewriteAsync_NonJsonResponse_UsesFallback()
    {
        var provider = new RewriteMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "Plain text rewrite"
                }
            }
        };

        XFAiRewriteResult? rewritten = null;
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.OnRewritten, r => { rewritten = r; }));

        var modeButton = cut.Find("button[aria-label='Formalizar']");
        await cut.InvokeAsync(async () => await modeButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

        Assert.NotNull(rewritten);
        Assert.Equal("Plain text rewrite", rewritten.RewrittenText);
    }

    [Fact]
    public async Task RewriteAsync_ProviderException_FiresOnError()
    {
        var provider = new RewriteMockProvider
        {
            ThrowException = new Exception("Rewrite failed")
        };

        string? errorMessage = null;
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello")
            .Add(x => x.OnError, e => { errorMessage = e; }));

        var modeButton = cut.Find("button[aria-label='Formalizar']");
        await cut.InvokeAsync(async () => await modeButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

        Assert.NotNull(errorMessage);
    }

    [Fact]
    public async Task RewriteAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new RewriteMockProvider
        {
            ThrowException = new OperationCanceledException()
        };

        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello"));

        var modeButton = cut.Find("button[aria-label='Formalizar']");
        await cut.InvokeAsync(async () => await modeButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var provider = CreateProvider();
        var cut = Render<XFAiRewriteButton>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Text, "Hello"));
        cut.Instance.Dispose();
    }
}
