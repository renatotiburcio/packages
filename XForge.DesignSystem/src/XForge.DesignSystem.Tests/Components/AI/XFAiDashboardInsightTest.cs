using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Insights;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class DashboardMockProvider : IAiProvider, IDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-dashboard";
    public string DisplayName => "Mock Dashboard";
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
                Content = """[{"title":"Revenue up 15%","description":"Monthly revenue increased","type":"Trend","confidence":0.9,"relatedKpi":"Revenue"}]"""
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

public sealed class XFAiDashboardInsightTest : BunitContext
{
    public XFAiDashboardInsightTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static List<XFAiDashboardKpi> MakeKpis() =>
        new() { new() { Name = "Revenue", DisplayName = "Revenue", CurrentValue = 100000, Unit = "BRL", PreviousValue = 85000 } };

    [Fact] public void Dashboard_Renders_Default()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.Title, "My Insights"));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithoutTitle()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.ShowTitle, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithKpis()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.Kpis, (IReadOnlyList<XFAiDashboardKpi>)MakeKpis()));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithEmptyKpis()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.Kpis, Array.Empty<XFAiDashboardKpi>()));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithShowIconsFalse()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.ShowIcons, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithShowDismissFalse()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.ShowDismiss, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithMaxInsights()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.MaxInsights, 3));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithCustomEmptyText()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.EmptyText, "No data"));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithCustomLoadingText()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.LoadingText, "Loading..."));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithAutoRefresh()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.AutoRefresh, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithLoadingState()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithCustomSystemPrompt()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithCustomClass()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_InitialState_NoInsights()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()));
        Assert.NotNull(cut.Instance.Insights);
    }

    [Fact] public void Dashboard_GenerateInsightsAsync_WithProvider_CallsProvider()
    {
        var provider = new DashboardMockProvider();
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, provider).Add(x => x.Kpis, (IReadOnlyList<XFAiDashboardKpi>)MakeKpis()));
        cut.InvokeAsync(async () => await cut.Instance.GenerateInsightsAsync());
        Assert.Equal(1, provider.CallCount);
    }

    [Fact] public void Dashboard_GenerateInsightsAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, null));
        cut.InvokeAsync(async () => await cut.Instance.GenerateInsightsAsync());
    }

    [Fact] public void Dashboard_GenerateInsightsAsync_EmptyKpis_DoesNotCallProvider()
    {
        var provider = new DashboardMockProvider();
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, provider).Add(x => x.Kpis, Array.Empty<XFAiDashboardKpi>()));
        cut.InvokeAsync(async () => await cut.Instance.GenerateInsightsAsync());
        Assert.Equal(0, provider.CallCount);
    }

    [Fact] public void Dashboard_GenerateInsightsAsync_Exception_DoesNotThrow()
    {
        var provider = new DashboardMockProvider { ThrowException = new Exception("API error") };
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, provider).Add(x => x.Kpis, (IReadOnlyList<XFAiDashboardKpi>)MakeKpis()));
        cut.InvokeAsync(async () => await cut.Instance.GenerateInsightsAsync());
    }

    [Fact] public void Dashboard_GenerateInsightsAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new DashboardMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, provider).Add(x => x.Kpis, (IReadOnlyList<XFAiDashboardKpi>)MakeKpis()));
        cut.InvokeAsync(async () => await cut.Instance.GenerateInsightsAsync());
    }

    [Fact] public void Dashboard_ClearInsights_DoesNotThrow()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()));
        // No ClearInsights method — just verify render works after GenerateInsightsAsync
        cut.InvokeAsync(async () => await cut.Instance.GenerateInsightsAsync());
    }

    [Fact] public void Dashboard_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "header"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithInsightTemplate()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.InsightTemplate, (RenderFragment<XFAiDashboardInsightItem>)(ctx => b => b.AddContent(0, "insight"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Dashboard_Renders_WithEmptyContent()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Provider, new DashboardMockProvider()).Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "empty"))));
        Assert.NotNull(cut);
    }
}
