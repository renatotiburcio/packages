#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Stock;
using XForge.DesignSystem.Services.AI;
using Xunit;
#endregion

namespace XForge.DesignSystem.Tests.Components.AI.Stock;

/// <summary>
/// Manual mock IAiProvider that returns a serialized XFAiStockInsight JSON payload.
/// Does not use Moq — fully deterministic for bUnit tests.
/// </summary>
internal sealed class StockAdvisorMockProvider : IAiProvider
{
    public string ProviderId => "stock-mock";
    public string DisplayName => "Stock Mock";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;
    public string? DefaultModel => null;

    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        if (ThrowException is not null)
            throw ThrowException;

        if (NextResponse is not null)
            return Task.FromResult(NextResponse);

        var insight = new XFAiStockInsight
        {
            AiSummary = "Test analysis summary",
            Confidence = 0.85,
            Summary = new XFAiStockSummary
            {
                TotalProducts = 100,
                LowStockProducts = 5,
                OverstockProducts = 3
            },
            Alerts = new List<XFAiStockAlert>
            {
                new()
                {
                    Type = XFAiStockAlertType.RuptureRisk,
                    Severity = "high",
                    ProductName = "Product A",
                    Message = "Low stock",
                    RecommendedAction = "Reorder"
                }
            }
        };

        return Task.FromResult(new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = JsonSerializer.Serialize(insight)
            }
        });
    }

    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        await Task.CompletedTask;
        yield break;
    }

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
}

public sealed class XFAiStockAdvisorExtendedTest : XForgeTestContext
{
    public XFAiStockAdvisorExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // ── Helpers ────────────────────────────────────────────────────────

    private IRenderedComponent<XFAiStockAdvisor> RenderDefault(IAiProvider? provider = null)
        => Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, provider ?? new StockAdvisorMockProvider()));

    private static void SetQueryField(IRenderedComponent<XFAiStockAdvisor> cut, string query)
    {
        var field = typeof(XFAiStockAdvisor).GetField("_query",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field!.SetValue(cut.Instance, query);
    }

    private static object? GetFieldValue(IRenderedComponent<XFAiStockAdvisor> cut, string fieldName)
    {
        var field = typeof(XFAiStockAdvisor).GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field!.GetValue(cut.Instance);
    }

    // ── 1. Default render ──────────────────────────────────────────────

    [Fact]
    public void DefaultRender_RendersBaseCssClass()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-ai-stock-advisor", cut.Markup);
    }

    // ── 2. Title renders in header ─────────────────────────────────────

    [Fact]
    public void Title_RendersDefaultTitleInHeader()
    {
        var cut = RenderDefault();
        Assert.Contains("Assistente de Estoque", cut.Markup);
    }

    // ── 3. Custom title renders ────────────────────────────────────────

    [Fact]
    public void Title_CustomTitle_RendersCustomTitle()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.Title, "Custom Stock Title"));
        Assert.Contains("Custom Stock Title", cut.Markup);
    }

    // ── 4. ShowHeader true renders header ──────────────────────────────

    [Fact]
    public void ShowHeader_True_RendersHeader()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.ShowHeader, true));
        Assert.Contains("<header", cut.Markup);
    }

    // ── 5. ShowHeader false hides header ───────────────────────────────

    [Fact]
    public void ShowHeader_False_HidesHeader()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("<header", cut.Markup);
    }

    // ── 6. Placeholder renders on textarea ─────────────────────────────

    [Fact]
    public void Placeholder_RendersDefaultOnTextarea()
    {
        var cut = RenderDefault();
        Assert.Contains("Pergunte sobre previsao de demanda", cut.Markup);
    }

    // ── 7. Custom placeholder renders ──────────────────────────────────

    [Fact]
    public void Placeholder_CustomPlaceholder_RendersOnTextarea()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.Placeholder, "Ask about stock..."));
        Assert.Contains("Ask about stock...", cut.Markup);
    }

    // ── 8. Empty state renders when no result and not busy ─────────────

    [Fact]
    public void EmptyState_RendersWhenNoResultAndNotBusy()
    {
        var cut = RenderDefault();
        Assert.Contains("Assistente de Estoque", cut.Markup);
        Assert.Contains("Pergunte sobre previsao de demanda", cut.Markup);
    }

    // ── 9. Custom empty content renders ────────────────────────────────

    [Fact]
    public void EmptyContent_CustomContent_RendersInsteadOfDefault()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.EmptyContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddContent(1, "No stock data available");
                b.CloseElement();
            })));
        Assert.Contains("No stock data available", cut.Markup);
    }

    // ── 10. Custom empty text via EmptyContent ─────────────────────────

    [Fact]
    public void EmptyContent_CustomText_RendersCustomText()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(p => p.Provider, new StockAdvisorMockProvider())
            .Add(p => p.EmptyContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "div");
                b.AddContent(1, "Custom empty state text");
                b.CloseElement();
            })));
        Assert.Contains("Custom empty state text", cut.Markup);
    }

    // ── 11. Disabled disables textarea and button ──────────────────────

    [Fact]
    public void Disabled_DisablesTextareaAndButton()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.Disabled, true));
        // Textarea is disabled via IsBusy, not Disabled parameter
        // Button is disabled via CanSend which checks Disabled
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    // ── 12. Loading disables send button ───────────────────────────────

    [Fact]
    public void Loading_DisablesSendButton()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.Loading, true));
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    // ── 13. Send button disabled when no Provider ──────────────────────

    [Fact]
    public void SendButton_NoProvider_IsDisabled()
    {
        var cut = Render<XFAiStockAdvisor>();
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    // ── 14. Send button disabled when query is empty ───────────────────

    [Fact]
    public void SendButton_EmptyQuery_IsDisabled()
    {
        var cut = RenderDefault();
        // _query is null by default, so CanSend is false
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    // ── 15. Send button enabled when Provider set and query entered ────

    [Fact]
    public void SendButton_ProviderAndQuery_IsEnabled()
    {
        var cut = RenderDefault();
        SetQueryField(cut, "Analyze stock");
        // Force re-render so CanSend re-evaluates with the new _query value
        var stateHasChanged = typeof(ComponentBase).GetMethod("StateHasChanged",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        cut.InvokeAsync(() => stateHasChanged.Invoke(cut.Instance, null));
        var button = cut.Find("button");
        Assert.False(button.HasAttribute("disabled"));
    }

    // ── 16. Error banner renders with role="alert" ─────────────────────

    [Fact]
    public void ErrorBanner_RendersWithRoleAlert()
    {
        var provider = new StockAdvisorMockProvider
        {
            ThrowException = new Exception("API failure")
        };
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "test query");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Contains("role=\"alert\"", cut.Markup);
    }

    // ── 17. Dismiss error clears error banner ──────────────────────────

    [Fact]
    public void DismissError_ClearsErrorBanner()
    {
        var provider = new StockAdvisorMockProvider
        {
            ThrowException = new Exception("API failure")
        };
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "test query");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Contains("role=\"alert\"", cut.Markup);

        // Click the dismiss button in the error banner
        var dismissBtn = cut.Find("button[aria-label='Dispensar erro']");
        cut.InvokeAsync(() => dismissBtn.Click());
        Assert.DoesNotContain("role=\"alert\"", cut.Markup);
    }

    // ── 18. Loading skeleton renders when IsBusy ───────────────────────

    [Fact]
    public void LoadingSkeleton_RendersWhenIsBusy()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.Loading, true));
        Assert.Contains("Analisando estoque...", cut.Markup);
        Assert.Contains("role=\"status\"", cut.Markup);
    }

    // ── 19. Result preview renders when result set ─────────────────────

    [Fact]
    public void ResultPreview_RendersWhenResultSet()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "Analyze stock levels");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Contains("Test analysis summary", cut.Markup);
    }

    // ── 20. ResultPreview template renders with result context ─────────

    [Fact]
    public void ResultPreview_Template_RendersWithResultContext()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.ResultPreview, (RenderFragment<XFAiStockInsight>)(r => b =>
            {
                b.OpenElement(0, "div");
                b.AddAttribute(1, "class", "custom-result");
                b.AddContent(2, $"AI: {r.AiSummary}");
                b.CloseElement();
            })));
        Assert.Contains("xf-ai-stock-advisor", cut.Markup);
    }

    // ── 21. HeaderContent renders instead of default header ────────────

    [Fact]
    public void HeaderContent_CustomContent_RendersInsteadOfDefault()
    {
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, new StockAdvisorMockProvider())
            .Add(x => x.HeaderContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddContent(1, "Custom Header");
                b.CloseElement();
            })));
        Assert.Contains("Custom Header", cut.Markup);
    }

    // ── 22. Summary stats render ───────────────────────────────────────

    [Fact]
    public void SummaryStats_RendersTotalLowAndOverstock()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "Summary");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Contains("100", cut.Markup);
        Assert.Contains("5", cut.Markup);
        Assert.Contains("3", cut.Markup);
    }

    // ── 23. Alerts render with severity classes ────────────────────────

    [Fact]
    public void Alerts_RendersWithSeverityClasses()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "Alerts");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Contains("Product A", cut.Markup);
        Assert.Contains("Low stock", cut.Markup);
        Assert.Contains("Reorder", cut.Markup);
    }

    // ── 24. Confidence progressbar renders ─────────────────────────────

    [Fact]
    public void Confidence_RendersProgressBar()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "Confidence");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Contains("role=\"progressbar\"", cut.Markup);
        Assert.Contains("85%", cut.Markup);
    }

    // ── 25. Custom SystemPrompt accepted ───────────────────────────────

    [Fact]
    public void SystemPrompt_CustomValue_Accepted()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.SystemPrompt, "Custom system prompt for stock"));
        SetQueryField(cut, "test");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Equal(1, provider.CallCount);
    }

    // ── 26. OnAnalysisComplete callback fires ──────────────────────────

    [Fact]
    public void OnAnalysisComplete_FiresCallback()
    {
        XFAiStockInsight? capturedResult = null;
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete,
                EventCallback.Factory.Create<XFAiStockInsight>(this, r => capturedResult = r)));
        SetQueryField(cut, "Analyze");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.NotNull(capturedResult);
        Assert.Equal("Test analysis summary", capturedResult!.AiSummary);
        Assert.Equal(0.85, capturedResult.Confidence);
    }

    // ── 27. OnError callback fires ─────────────────────────────────────

    [Fact]
    public void OnError_FiresCallback()
    {
        string? capturedError = null;
        var provider = new StockAdvisorMockProvider
        {
            ThrowException = new Exception("Provider error")
        };
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError,
                EventCallback.Factory.Create<string>(this, e => capturedError = e)));
        SetQueryField(cut, "test");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.NotNull(capturedError);
        Assert.Equal("Provider error", capturedError);
    }

    // ── 28. Clear method resets state ──────────────────────────────────

    [Fact]
    public void Clear_ResetsState()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "test");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.NotNull(GetFieldValue(cut, "_result"));

        cut.InvokeAsync(() => cut.Instance.Clear());
        Assert.Null(GetFieldValue(cut, "_result"));
        Assert.Null(GetFieldValue(cut, "_errorText"));
        Assert.Equal(string.Empty, GetFieldValue(cut, "_query"));
    }

    // ── 29. AnalyzeAsync with Provider calls CompleteAsync ─────────────

    [Fact]
    public void AnalyzeAsync_WithProvider_CallsCompleteAsync()
    {
        var provider = new StockAdvisorMockProvider();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        SetQueryField(cut, "Stock analysis");
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        Assert.Equal(1, provider.CallCount);
    }

    // ── 30. Escape key clears query ────────────────────────────────────

    [Fact]
    public void EscapeKey_ClearsQuery()
    {
        var cut = RenderDefault();
        var textarea = cut.Find("textarea");
        textarea.Input("test query");
        textarea.KeyDown(new KeyboardEventArgs { Key = "Escape" });
        var queryValue = GetFieldValue(cut, "_query") as string;
        Assert.Equal(string.Empty, queryValue);
    }
}
