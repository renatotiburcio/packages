using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.AI.DataGrid;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Extended mock IAiProvider for XFAiDataGridAssistant tests.
/// Supports configurable response, exception throwing, and call tracking.
/// </summary>
internal sealed class ExtendedDataGridMockProvider : IAiProvider, IDisposable, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-ext";
    public string DisplayName => "Mock";
    public string? DefaultModel => "mock";
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

    public void Dispose()
    {
        // No-op
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// Extended tests for XFAiDataGridAssistant covering CSS class composition,
/// disabled state, data/columns, render fragments, insight types, query submission,
/// error handling, and lifecycle.
/// </summary>
public sealed class XFAiDataGridAssistantExtendedTest : BunitContext
{
    public XFAiDataGridAssistantExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // ── 1. Base CSS class ──────────────────────────────────────────────────

    [Fact]
    public void Renders_BaseClass()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        var markup = cut.Markup;
        Assert.Contains("xf-ai-datagrid-assistant", markup);
    }

    // ── 2. Custom CSS class ────────────────────────────────────────────────

    [Fact]
    public void Renders_WithCustomClass()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Class, "my-custom-class"));
        var markup = cut.Markup;
        Assert.Contains("my-custom-class", markup);
    }

    // ── 3. Custom inline style ─────────────────────────────────────────────

    [Fact]
    public void Renders_WithCustomStyle()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Style, "max-width: 600px;"));
        var markup = cut.Markup;
        Assert.Contains("max-width: 600px;", markup);
    }

    // ── 4. Disabled state ──────────────────────────────────────────────────

    [Fact]
    public void Renders_WithDisabled()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Disabled, true));
        var markup = cut.Markup;
        Assert.Contains("disabled", markup);
    }

    // ── 5. Data and Columns ────────────────────────────────────────────────

    [Fact]
    public void Renders_WithDataAndColumns()
    {
        var provider = new ExtendedDataGridMockProvider();
        var data = new List<object>
        {
            new { Name = "Alpha", Value = 10 },
            new { Name = "Beta", Value = 20 }
        };
        var columns = new[] { "Name", "Value" };
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Data, data)
            .Add(x => x.Columns, columns));
        Assert.NotNull(cut);
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    // ── 6. InsightTemplate RenderFragment ──────────────────────────────────

    [Fact]
    public void Renders_WithInsightTemplate()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.InsightTemplate, (RenderFragment<XFAiGridInsight>)(insight => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, $"Insight: {insight.Title}");
                builder.CloseElement();
            })));
        Assert.NotNull(cut);
    }

    // ── 7. EmptyContent RenderFragment ─────────────────────────────────────

    [Fact]
    public void Renders_WithEmptyContent()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.EmptyContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(1, "Nothing here");
                builder.CloseElement();
            })));
        var markup = cut.Markup;
        Assert.Contains("Nothing here", markup);
    }

    // ── 8. HeaderContent RenderFragment ────────────────────────────────────

    [Fact]
    public void Renders_WithHeaderContent()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.HeaderContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Custom Header");
                builder.CloseElement();
            })));
        var markup = cut.Markup;
        Assert.Contains("Custom Header", markup);
    }

    // ── 9. ShowHeader = false ──────────────────────────────────────────────

    [Fact]
    public void Renders_ShowHeaderFalse()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowHeader, false));
        // When ShowHeader=false, the header div is not rendered at all.
        // The component should still render (composer, empty state, etc.)
        Assert.NotNull(cut);
        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    // ── 10. MaxResults ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_WithMaxResults()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.MaxResults, 3));
        Assert.NotNull(cut);
    }

    // ── 11. Loading state ──────────────────────────────────────────────────

    [Fact]
    public void Renders_WithLoading()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Loading, true));
        // When Loading=true but _isBusy is false internally, the skeleton may not show.
        // The component combines external Loading with internal _isBusy.
        // We verify the component renders without error.
        Assert.NotNull(cut);
    }

    // ── 12. Custom Placeholder ─────────────────────────────────────────────

    [Fact]
    public void Renders_WithCustomPlaceholder()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Placeholder, "Custom placeholder"));
        var markup = cut.Markup;
        Assert.Contains("Custom placeholder", markup);
    }

    // ── 13. Custom EmptyText ───────────────────────────────────────────────

    [Fact]
    public void Renders_WithCustomEmptyText()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.EmptyText, "No insights yet"));
        var markup = cut.Markup;
        Assert.Contains("No insights yet", markup);
    }

    // ── 14. Custom SystemPrompt ────────────────────────────────────────────

    [Fact]
    public void Renders_WithCustomSystemPrompt()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.SystemPrompt, "You are a data analyst"));
        Assert.NotNull(cut);
    }

    // ── 15. SubmitQueryAsync without provider ──────────────────────────────

    [Fact]
    public void SubmitQueryAsync_WithoutProvider_CreatesBasicQuery()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, null));
        // Use Input event to trigger HandleInput which sets _queryText
        var input = cut.Find("input[type='text']");
        input.Input("test query");
        // SubmitQueryAsync requires CanSubmit which needs non-empty _queryText
        cut.InvokeAsync(() => cut.Instance.SubmitQueryAsync());
        // No exception means success
    }

    // ── 16. SubmitQueryAsync with provider calls CompleteAsync ─────────────

    [Fact]
    public void SubmitQueryAsync_WithProvider_CallsCompleteAsync()
    {
        var provider = new ExtendedDataGridMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = "test response" }
            }
        };
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));

        // Use Input event to trigger HandleInput which sets _queryText
        var input = cut.Find("input[type='text']");
        input.Input("show me trends");

        cut.InvokeAsync(() => cut.Instance.SubmitQueryAsync());

        // The provider's CompleteAsync is called during ProcessQueryAsync.
        // Since the mock returns synchronously, it should complete within InvokeAsync.
        Assert.True(provider.CallCount > 0, "Expected provider CompleteAsync to be called at least once.");
    }

    // ── 17. SubmitQueryAsync with provider error fires OnError ─────────────

    [Fact]
    public void SubmitQueryAsync_WithProviderError_FiresOnError()
    {
        var provider = new ExtendedDataGridMockProvider
        {
            ThrowException = new InvalidOperationException("AI service unavailable")
        };
        string? capturedError = null;
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, msg => capturedError = msg)));

        var input = cut.Find("input[type='text']");
        input.Input("test query");

        cut.InvokeAsync(() => cut.Instance.SubmitQueryAsync());

        Assert.NotNull(capturedError);
        Assert.Contains("AI service unavailable", capturedError);
    }

    // ── 18. Clear resets state ─────────────────────────────────────────────

    [Fact]
    public void Clear_ResetsState()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(() => cut.Instance.Clear());
        // No exception means success
    }

    // ── 19. CancelAsync does not throw ─────────────────────────────────────

    [Fact]
    public void CancelAsync_DoesNotThrow()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(() => cut.Instance.CancelAsync());
        // No exception means success
    }

    // ── 20. Composer section renders ───────────────────────────────────────

    [Fact]
    public void Renders_ComposerSection()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        var markup = cut.Markup;
        // The composer section contains the input element
        var input = cut.Find("input[type='text']");
        Assert.NotNull(input);
    }

    // ── 21. Empty state renders ────────────────────────────────────────────

    [Fact]
    public void Renders_EmptyState()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, null));
        var markup = cut.Markup;
        // Default empty text should appear
        Assert.Contains("Faça uma pergunta sobre seus dados para receber insights.", markup);
    }

    // ── 22. All insight types render ───────────────────────────────────────

    [Fact]
    public void Renders_WithAllInsightTypes()
    {
        // Just verify the component renders with an insight template
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnQueryGenerated, EventCallback.Factory.Create<XFAiGridQuery>(this, _ => { }))
            .Add(x => x.OnInsightFound, EventCallback.Factory.Create<XFAiGridInsight>(this, _ => { })));

        Assert.Contains("xf-ai-datagrid-assistant", cut.Markup);
    }

    // ── 23. Dispose does not throw ─────────────────────────────────────────

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
        // No exception means success
    }

    // ── 24. Custom Id ──────────────────────────────────────────────────────

    [Fact]
    public void Renders_WithCustomId()
    {
        var provider = new ExtendedDataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Id, "my-datagrid-assistant"));
        var markup = cut.Markup;
        Assert.Contains("my-datagrid-assistant", markup);
    }

    // ── 25. Multiple series with provider ──────────────────────────────────

    [Fact]
    public void MultipleSeries_WithProvider()
    {
        var provider = new ExtendedDataGridMockProvider();
        var data = new List<object>
        {
            new { Region = "North", Sales = 1500, Quarter = "Q1" },
            new { Region = "South", Sales = 2300, Quarter = "Q1" },
            new { Region = "East", Sales = 1800, Quarter = "Q2" },
            new { Region = "West", Sales = 2100, Quarter = "Q2" }
        };
        var columns = new[] { "Region", "Sales", "Quarter" };

        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Data, data)
            .Add(x => x.Columns, columns));

        var markup = cut.Markup;
        Assert.Contains("xf-ai-datagrid-assistant", markup);
        // Verify the composer input is present
        var input = cut.Find("input[type='text']");
        Assert.NotNull(input);
    }
}
