using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Documents;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class ReportMockProvider : IAiProvider, IDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-report";
    public string DisplayName => "Mock Report";
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
                Content = """{"title":"Sales Report","description":"Monthly sales","format":"Pdf","aiSummary":"Generated","confidence":0.9}"""
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

public sealed class XFAiReportBuilderTest : BunitContext
{
    public XFAiReportBuilderTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact] public void Report_Renders_Default()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.Title, "Custom Report"));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithoutHeader()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.Placeholder, "Describe report..."));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithLoadingState()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithCustomSystemPrompt()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithAvailableFields()
    {
        var fields = new List<XFAiReportField> { new() { Name = "Revenue", DisplayName = "Revenue", DataType = "number" } };
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.AvailableFields, (IReadOnlyList<XFAiReportField>)fields));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithSupportedFormats()
    {
        var formats = new List<XFAiReportFormat> { XFAiReportFormat.Pdf, XFAiReportFormat.Excel };
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.SupportedFormats, (IReadOnlyList<XFAiReportFormat>)formats));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithCustomClass()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_GenerateReportAsync_WithProvider_CallsProvider()
    {
        var provider = new ReportMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Generate a sales report");
            await cut.Instance.GenerateReportAsync();
        });
        Assert.Equal(1, provider.CallCount);
    }

    [Fact] public void Report_GenerateReportAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, null));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.GenerateReportAsync();
        });
    }

    [Fact] public void Report_GenerateReportAsync_EmptyInput_DoesNotCallProvider()
    {
        var provider = new ReportMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () => await cut.Instance.GenerateReportAsync());
        Assert.Equal(0, provider.CallCount);
    }

    [Fact] public void Report_GenerateReportAsync_Exception_DoesNotThrow()
    {
        var provider = new ReportMockProvider { ThrowException = new Exception("API error") };
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.GenerateReportAsync();
        });
    }

    [Fact] public void Report_GenerateReportAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new ReportMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.GenerateReportAsync();
        });
    }

    [Fact] public void Report_Clear_DoesNotThrow()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()));
        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    [Fact] public void Report_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()));
        cut.Instance.Dispose();
    }

    [Fact] public void Report_Renders_WithDisabledState()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithReportPreview()
    {
        var def = new XFAiReportDefinition { Title = "Test" };
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.ReportPreview, (RenderFragment<XFAiReportDefinition>)(_ => b => b.AddContent(0, "preview"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "header"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Report_Renders_WithEmptyContent()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ReportMockProvider()).Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "empty"))));
        Assert.NotNull(cut);
    }
}
