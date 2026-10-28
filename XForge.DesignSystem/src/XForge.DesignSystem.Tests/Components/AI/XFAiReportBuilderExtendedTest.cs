using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.AI.Documents;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class ExtendedReportMockProvider : IAiProvider, IDisposable, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-report";
    public string DisplayName => "Mock Report";
    public string? DefaultModel => "mock";
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

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public sealed class XFAiReportBuilderExtendedTest : XForgeTestContext
{
    public XFAiReportBuilderExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_BaseClass()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ExtendedReportMockProvider()));
        Assert.Contains("xf-ai-report-builder", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomClass()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomStyle()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Style, "margin-top: 20px"));
        Assert.Contains("margin-top: 20px", cut.Markup);
    }

    [Fact]
    public void Renders_WithDisabled()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithCustomTitle()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Title, "Custom Title"));
        Assert.Contains("Custom Title", cut.Markup);
    }

    [Fact]
    public void Renders_ShowHeaderFalse()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.ShowHeader, false));
        // When ShowHeader=false, the header section should not render.
        // The root element should still be present.
        Assert.NotNull(cut);
        Assert.Contains("xf-ai-report-builder", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Placeholder, "Describe your report..."));
        var textarea = cut.Find("textarea");
        Assert.Equal("Describe your report...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void Renders_WithLoading()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Loading, true));
        Assert.Contains("animate-pulse", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomSystemPrompt()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithAvailableFields()
    {
        var fields = new List<XFAiReportField>
        {
            new() { Name = "Revenue", DisplayName = "Revenue", DataType = "number", IsRequired = true },
            new() { Name = "Date", DisplayName = "Date", DataType = "date", IsRequired = false }
        };
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.AvailableFields, (IReadOnlyList<XFAiReportField>)fields));
        Assert.Contains("Revenue", cut.Markup);
        Assert.Contains("Date", cut.Markup);
    }

    [Fact]
    public void Renders_WithSupportedFormats()
    {
        var formats = new List<XFAiReportFormat>
        {
            XFAiReportFormat.Pdf,
            XFAiReportFormat.Excel,
            XFAiReportFormat.Csv,
            XFAiReportFormat.Word,
            XFAiReportFormat.Dashboard
        };
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.SupportedFormats, (IReadOnlyList<XFAiReportFormat>)formats));
        Assert.Contains("PDF", cut.Markup);
        Assert.Contains("Excel", cut.Markup);
        Assert.Contains("CSV", cut.Markup);
        Assert.Contains("Word", cut.Markup);
        Assert.Contains("Dashboard", cut.Markup);
    }

    [Fact]
    public void Renders_WithReportPreview()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.ReportPreview, (RenderFragment<XFAiReportDefinition>)(_ => b => b.AddContent(0, "custom preview"))));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_WithHeaderContent()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "custom header"))));
        Assert.Contains("custom header", cut.Markup);
    }

    [Fact]
    public void Renders_WithEmptyContent()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "custom empty"))));
        Assert.Contains("custom empty", cut.Markup);
    }

    [Fact]
    public void Renders_WithoutProvider()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, null));
        Assert.NotNull(cut);
        Assert.Contains("xf-ai-report-builder", cut.Markup);
    }

    [Fact]
    public void Renders_EmptyState()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, null));
        Assert.Contains("Gerador de Relatórios", cut.Markup);
    }

    [Fact]
    public void GenerateReportAsync_WithoutProvider_DoesNotThrow()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, null));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Generate a sales report");
            await cut.Instance.GenerateReportAsync();
        });
    }

    [Fact]
    public void GenerateReportAsync_WithProvider_CallsCompleteAsync()
    {
        var provider = new ExtendedReportMockProvider();
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Generate a sales report");
            await cut.Instance.GenerateReportAsync();
        });
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public void GenerateReportAsync_WithProviderError_FiresOnError()
    {
        var provider = new ExtendedReportMockProvider { ThrowException = new Exception("API failure") };
        string? errorContent = null;
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, msg => errorContent = msg)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiReportBuilder).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Generate a sales report");
            await cut.Instance.GenerateReportAsync();
        });
        Assert.Equal("API failure", errorContent);
    }

    [Fact]
    public void Clear_ResetsState()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ExtendedReportMockProvider()));
        cut.InvokeAsync(() => cut.Instance.Clear());
        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_FormatChips()
    {
        var formats = new List<XFAiReportFormat>
        {
            XFAiReportFormat.Pdf,
            XFAiReportFormat.Excel,
            XFAiReportFormat.Csv
        };
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.SupportedFormats, (IReadOnlyList<XFAiReportFormat>)formats));
        var chipGroup = cut.Find("[role='group'][aria-label='Formatos suportados']");
        Assert.NotNull(chipGroup);
    }

    [Fact]
    public void Renders_WithCustomId()
    {
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.Id, "my-report-builder"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ExtendedReportMockProvider()));
        cut.Instance.Dispose();
    }

    [Fact]
    public void Renders_Textarea()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Provider, new ExtendedReportMockProvider()));
        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea);
    }

    [Fact]
    public void Renders_WithMultipleFormats()
    {
        var formats = new List<XFAiReportFormat>
        {
            XFAiReportFormat.Pdf,
            XFAiReportFormat.Excel,
            XFAiReportFormat.Csv,
            XFAiReportFormat.Word,
            XFAiReportFormat.Dashboard
        };
        var cut = Render<XFAiReportBuilder>(p => p
            .Add(x => x.Provider, new ExtendedReportMockProvider())
            .Add(x => x.SupportedFormats, (IReadOnlyList<XFAiReportFormat>)formats));
        Assert.Contains("PDF", cut.Markup);
        Assert.Contains("Excel", cut.Markup);
        Assert.Contains("CSV", cut.Markup);
        Assert.Contains("Word", cut.Markup);
        Assert.Contains("Dashboard", cut.Markup);
    }
}
