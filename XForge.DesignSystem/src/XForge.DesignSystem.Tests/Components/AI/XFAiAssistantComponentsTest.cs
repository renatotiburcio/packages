using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Accounting;
using XForge.DesignSystem.Components.AI.Financial;
using XForge.DesignSystem.Components.AI.Forms;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for assistant component tests.
/// </summary>
internal sealed class AssistantMockProvider : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public AiChatRequest? LastRequest { get; private set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-assistant";
    public string DisplayName => "Mock Assistant Provider";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
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
/// Tests for XFAiAccountingAssistant, XFAiFormAssistant, and XFAiFinancialConciliator.
/// All three follow the same pattern: query input + AnalyzeAsync + JSON result.
/// </summary>
public sealed class XFAiAssistantComponentsTest : BunitContext
{
    public XFAiAssistantComponentsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiAccountingAssistant

    [Fact]
    public void Accounting_Renders_WithDefaultParameters()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Provider, provider));
        Assert.Contains("Assistente Contabil", cut.Markup);
    }

    [Fact]
    public void Accounting_Renders_WithCustomTitle()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "My Accountant"));
        Assert.Contains("My Accountant", cut.Markup);
    }

    [Fact]
    public void Accounting_Renders_WithHeaderHidden()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Accounting_Renders_WithCustomPlaceholder()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Placeholder, "Ask about accounting..."));
        var textarea = cut.Find("textarea");
        Assert.Equal("Ask about accounting...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void Accounting_Renders_WithoutProvider()
    {
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Accounting_Renders_WithLoadingState()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task Accounting_AnalyzeAsync_ValidJson_FiresOnAnalysisComplete()
    {
        var provider = new AssistantMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = """{"aiSummary":"Analysis complete","confidence":0.85}"""
                }
            }
        };

        XFAiAccountingInsight? result = null;
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, r => { result = r; }));

        // Set query via reflection
        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiAccountingAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "Analyze Q1 revenue");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.NotNull(result);
        Assert.Equal("Analysis complete", result.AiSummary);
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task Accounting_AnalyzeAsync_InvalidJson_FiresOnError()
    {
        var provider = new AssistantMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = "not json" }
            }
        };

        string? error = null;
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, e => { error = e; }));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiAccountingAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.NotNull(error);
    }

    [Fact]
    public async Task Accounting_AnalyzeAsync_Exception_FiresOnError()
    {
        var provider = new AssistantMockProvider { ThrowException = new Exception("Provider error") };
        string? error = null;
        var cut = Render<XFAiAccountingAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, e => { error = e; }));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiAccountingAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.Equal("Provider error", error);
    }

    [Fact]
    public async Task Accounting_AnalyzeAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new AssistantMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiAccountingAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact]
    public async Task Accounting_AnalyzeAsync_NoProvider_ReturnsEarly()
    {
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Provider, null));
        await cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
    }

    [Fact]
    public void Accounting_Dispose_DoesNotThrow()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiFormAssistant

    [Fact]
    public void Form_Renders_WithDefaultParameters()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Provider, provider));
        Assert.Contains("Assistente de Formularios", cut.Markup);
    }

    [Fact]
    public void Form_Renders_WithCustomTitle()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFormAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "Form Helper"));
        Assert.Contains("Form Helper", cut.Markup);
    }

    [Fact]
    public void Form_Renders_WithoutProvider()
    {
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Form_Renders_WithLoadingState()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFormAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task Form_AnalyzeAsync_ValidJson_FiresOnAnalysisComplete()
    {
        var provider = new AssistantMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = """{"aiSummary":"Form analyzed","confidence":0.9}"""
                }
            }
        };

        XFAiFormInsight? result = null;
        var cut = Render<XFAiFormAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, r => { result = r; }));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiFormAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "Analyze this form");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.NotNull(result);
        Assert.Equal("Form analyzed", result.AiSummary);
    }

    [Fact]
    public async Task Form_AnalyzeAsync_Exception_FiresOnError()
    {
        var provider = new AssistantMockProvider { ThrowException = new Exception("Form error") };
        string? error = null;
        var cut = Render<XFAiFormAssistant>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, e => { error = e; }));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiFormAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.Equal("Form error", error);
    }

    [Fact]
    public async Task Form_AnalyzeAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new AssistantMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiFormAssistant).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact]
    public async Task Form_AnalyzeAsync_NoProvider_ReturnsEarly()
    {
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Provider, null));
        await cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
    }

    [Fact]
    public void Form_Dispose_DoesNotThrow()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiFinancialConciliator

    [Fact]
    public void Financial_Renders_WithDefaultParameters()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Provider, provider));
        Assert.Contains("Conciliador Financeiro", cut.Markup);
    }

    [Fact]
    public void Financial_Renders_WithCustomTitle()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFinancialConciliator>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Title, "Bank Reconciler"));
        Assert.Contains("Bank Reconciler", cut.Markup);
    }

    [Fact]
    public void Financial_Renders_WithoutProvider()
    {
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Financial_Renders_WithLoadingState()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFinancialConciliator>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task Financial_AnalyzeAsync_ValidJson_FiresOnReconciled()
    {
        var provider = new AssistantMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = """{"aiSummary":"Reconciliation complete","confidence":0.88}"""
                }
            }
        };

        XFAiReconciliationResult? result = null;
        var cut = Render<XFAiFinancialConciliator>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnReconciled, r => { result = r; }));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiFinancialConciliator).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "Reconcile bank statement");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.NotNull(result);
        Assert.Equal("Reconciliation complete", result.AiSummary);
    }

    [Fact]
    public async Task Financial_AnalyzeAsync_Exception_FiresOnError()
    {
        var provider = new AssistantMockProvider { ThrowException = new Exception("Finance error") };
        string? error = null;
        var cut = Render<XFAiFinancialConciliator>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, e => { error = e; }));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiFinancialConciliator).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });

        Assert.Equal("Finance error", error);
    }

    [Fact]
    public async Task Financial_AnalyzeAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new AssistantMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiFinancialConciliator).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact]
    public async Task Financial_AnalyzeAsync_NoProvider_ReturnsEarly()
    {
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Provider, null));
        await cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
    }

    [Fact]
    public void Financial_Dispose_DoesNotThrow()
    {
        var provider = new AssistantMockProvider();
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Provider, provider));
        cut.Instance.Dispose();
    }

    #endregion
}
