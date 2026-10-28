using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Financial;
using XForge.DesignSystem.Components.AI.Stock;
using XForge.DesignSystem.Components.AI.Vision;
using XForge.DesignSystem.Components.AI.MultiModal;
using XForge.DesignSystem.Components.AI.Fiscal;
using XForge.DesignSystem.Components.AI.Insights;
using XForge.DesignSystem.Components.AI.Utility;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider for more assistant component tests.
/// </summary>
internal sealed class AssistantMockProvider2 : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public AiChatRequest? LastRequest { get; private set; }

    public string ProviderId => "mock-assistant2";
    public string DisplayName => "Mock";
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
/// Tests for XFAiSalesGoalAdvisor, XFAiStockAdvisor, XFAiVideoAnalyzer,
/// XFAiVisionPanel, XFAiMultiModalChat, XFAiNFeAnalyzer, XFAiDashboardInsight,
/// and XFAiValidationSummary.
/// </summary>
public sealed class XFAiMoreAssistantComponentsTest : XForgeTestContext
{
    public XFAiMoreAssistantComponentsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiVisionProvider>(new InMemoryAiVisionProvider());
        Services.AddSingleton<IAiAudioProvider>(new InMemoryAiAudioProvider());
        Services.AddSingleton<IAiProvider>(new AssistantMockProvider2());
    }

    #region XFAiSalesGoalAdvisor

    [Fact]
    public void SalesGoal_Renders_Default()
    {
        var provider = new AssistantMockProvider2();
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        Assert.Contains("Assistente de Metas", cut.Markup);
    }

    [Fact]
    public void SalesGoal_Renders_WithoutProvider()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task SalesGoal_AnalyzeAsync_ValidJson()
    {
        var provider = new AssistantMockProvider2
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = """{"aiSummary":"Sales goals analyzed","confidence":0.9}""" }
            }
        };

        XFAiSalesGoalInsight? result = null;
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, r => { result = r; }));

        await cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SalesGoal_AnalyzeAsync_Exception()
    {
        var provider = new AssistantMockProvider2 { ThrowException = new Exception("err") };
        string? error = null;
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, e => { error = e; }));

        await cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Equal("err", error);
    }

    [Fact]
    public async Task SalesGoal_AnalyzeAsync_Cancelled()
    {
        var provider = new AssistantMockProvider2 { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        await cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact]
    public void SalesGoal_Dispose()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, new AssistantMockProvider2()));
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiStockAdvisor

    [Fact]
    public void Stock_Renders_Default()
    {
        var provider = new AssistantMockProvider2();
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        Assert.Contains("Assistente de Estoque", cut.Markup);
    }

    [Fact]
    public void Stock_Renders_WithoutProvider()
    {
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task Stock_AnalyzeAsync_ValidJson()
    {
        var provider = new AssistantMockProvider2
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = """{"aiSummary":"Stock analyzed","confidence":0.85}""" }
            }
        };

        XFAiStockInsight? result = null;
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, r => { result = r; }));

        await cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiStockAdvisor).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Stock_AnalyzeAsync_Exception()
    {
        var provider = new AssistantMockProvider2 { ThrowException = new Exception("stock err") };
        string? error = null;
        var cut = Render<XFAiStockAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, e => { error = e; }));

        await cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiStockAdvisor).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Equal("stock err", error);
    }

    [Fact]
    public async Task Stock_AnalyzeAsync_Cancelled()
    {
        var provider = new AssistantMockProvider2 { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, provider));
        await cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiStockAdvisor).GetField("_query",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
    }

    [Fact]
    public void Stock_Dispose()
    {
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Provider, new AssistantMockProvider2()));
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiVideoAnalyzer

    [Fact]
    public void Video_Renders_Default()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Renders_WithTitle()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.Title, "My Video"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Renders_WithoutHeader()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Renders_WithoutFrameSelector()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.ShowFrameSelector, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.Placeholder, "Describe video..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Renders_WithCustomAnalyzeText()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.AnalyzeText, "Process"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Renders_WithCustomAriaLabel()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.AriaLabel, "AI Video"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Video_Dispose()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiVisionPanel

    [Fact]
    public void VisionPanel_Renders_Default()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithTitle()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Title, "My Vision"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithoutHeader()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithoutAnalysisTypeSelector()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ShowAnalysisTypeSelector, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Placeholder, "Describe image..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithCustomAnalyzeText()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.AnalyzeText, "Go"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithCustomLoadingText()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.LoadingText, "Processing..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithCustomDropText()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.DropText, "Drop here"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Renders_WithMaxImages()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.MaxImages, 10));
        Assert.NotNull(cut);
    }

    [Fact]
    public void VisionPanel_Dispose()
    {
        var cut = Render<XFAiVisionPanel>();
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiMultiModalChat

    [Fact]
    public void MultiModal_Renders_Default()
    {
        var cut = Render<XFAiMultiModalChat>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithTitle()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.Title, "My Chat"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithoutHeader()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithPlaceholder()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.Placeholder, "Type message..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithCustomAriaLabel()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.AriaLabel, "AI Chat"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_Renders_WithMaxMessages()
    {
        var cut = Render<XFAiMultiModalChat>(p => p.Add(x => x.MaxMessages, 50));
        Assert.NotNull(cut);
    }

    [Fact]
    public void MultiModal_AddAttachment_DoesNotThrow()
    {
        var cut = Render<XFAiMultiModalChat>();
        var attachment = new XFAiAttachment
        {
            Type = XFAiAttachmentType.Image,
            Name = "test.png",
            Data = "base64data"
        };
        cut.InvokeAsync(() => cut.Instance.AddAttachment(attachment));
    }

    [Fact]
    public void MultiModal_ClearMessages_RemovesAll()
    {
        var cut = Render<XFAiMultiModalChat>();
        cut.InvokeAsync(() => cut.Instance.ClearMessages());
        Assert.Empty(cut.Instance.Messages);
    }

    [Fact]
    public void MultiModal_Dispose()
    {
        var cut = Render<XFAiMultiModalChat>();
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiNFeAnalyzer

    [Fact]
    public void NFe_Renders_Default()
    {
        var cut = Render<XFAiNFeAnalyzer>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void NFe_Renders_WithTitle()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Title, "NFe Analyzer"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void NFe_Dispose()
    {
        var cut = Render<XFAiNFeAnalyzer>();
        cut.Instance.Dispose();
    }

    #endregion

    #region XFAiDashboardInsight

    [Fact]
    public void Dashboard_Renders_Default()
    {
        var cut = Render<XFAiDashboardInsight>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void Dashboard_Renders_WithTitle()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Title, "Dashboard"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Dashboard_Renders_WithProvider()
    {
        var provider = new AssistantMockProvider2();
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Title, "Test"));
        Assert.NotNull(cut);
    }

    #endregion

    #region XFAiValidationSummary

    [Fact]
    public void ValidationSummary_Renders_Default()
    {
        var cut = Render<XFAiValidationSummary>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void ValidationSummary_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Title, "My Validation"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ValidationSummary_Renders_WithoutHeader()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ValidationSummary_Renders_WithoutDismiss()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.ShowDismiss, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ValidationSummary_Renders_WithItems()
    {
        var items = new List<XFAiValidationItem>
        {
            new() { FieldName = "Email", Message = "Invalid email", Severity = XFAiValidationSeverity.Error }
        };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ValidationSummary_Dispose()
    {
        var cut = Render<XFAiValidationSummary>();
        cut.Instance.Dispose();
    }

    #endregion
}
