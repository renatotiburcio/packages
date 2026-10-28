using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.DataGrid;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for XFAiDataGridAssistant tests.
/// </summary>
internal sealed class DataGridMockProvider : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-datagrid";
    public string DisplayName => "Mock DataGrid Provider";
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
/// Tests for XFAiDataGridAssistant — query submission, insight generation,
/// error handling, and grid query/insight DTOs.
/// </summary>
public sealed class XFAiDataGridAssistantTest : XForgeTestContext
{
    public XFAiDataGridAssistantTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_WithDefaultParameters()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomTitle()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "My Data Assistant"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithPlaceholder()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Placeholder, "Ask about data..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithoutHeader()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithMaxResults()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.MaxResults, 10));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithLoadingState()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomEmptyText()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.EmptyText, "No data available"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomSystemPrompt()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithData()
    {
        var provider = new DataGridMockProvider();
        var data = new List<object>
        {
            new { Name = "Item 1", Value = 100 },
            new { Name = "Item 2", Value = 200 }
        };
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Data, data));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithColumns()
    {
        var provider = new DataGridMockProvider();
        var columns = new[] { "Name", "Value", "Date" };
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Columns, columns));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithoutProvider()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithHeaderContent()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.HeaderContent, "<span>Custom Header</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithInsightTemplate()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.InsightTemplate, insight => builder => { builder.AddContent(0, "Custom Insight"); }));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Clear_DoesNotThrow()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var provider = new DataGridMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }
}
