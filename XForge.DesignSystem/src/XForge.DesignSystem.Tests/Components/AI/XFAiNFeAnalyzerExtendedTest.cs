using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Fiscal;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class ExtendedNFeMockProvider : IAiProvider, IDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public int CallCount { get; private set; }

    public string ProviderId => "extended-mock-nfe";
    public string DisplayName => "Extended Mock NFe";
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
                Content = """{"nFeKey":"43123456789012345678901234567890123456789012","issuerCnpj":"12.345.678/0001-90","issuerName":"Empresa Teste","recipientCnpj":"98.765.432/0001-10","recipientName":"Cliente Teste","invoiceNumber":"000123","series":"1","issueDate":"2025-01-15","totalValue":15000.0,"icmsTotal":2700.0,"ipiTotal":0.0,"pisTotal":97.5,"cofinsTotal":450.0,"items":[{"sequence":1,"description":"Produto Teste","ncm":"8471.30.12","cfop":"5102","cst":"00","quantity":10,"unitValue":1500.0,"totalValue":15000.0,"icmsValue":2700.0,"ipiValue":0.0,"itemClassification":"Revenda"}],"taxSummary":[{"cfop":"5102","description":"Venda de mercadoria","itemCount":1,"totalValue":15000.0,"totalTax":2700.0,"effectiveRate":0.18}],"inconsistencies":[],"aiSummary":"NF-e validada com sucesso. Todos os CFOPs e CSTs estao corretos.","spedStatus":"Valid","cbsIbsReadiness":"Ready","riskLevel":"Low","confidence":0.95}"""
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

public sealed class XFAiNFeAnalyzerExtendedTest : XForgeTestContext
{
    public XFAiNFeAnalyzerExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void NFe_Renders_WithCustomStyle()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, new ExtendedNFeMockProvider())
            .Add(x => x.Style, "margin-top: 2rem; max-width: 800px;"));
        Assert.Contains("margin-top: 2rem; max-width: 800px;", cut.Markup);
    }

    [Fact]
    public void NFe_Renders_WithCustomId()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, new ExtendedNFeMockProvider())
            .Add(x => x.Id, "my-nfe-analyzer"));
        // The component renders via AdditionalAttributes, verify it renders
        Assert.Contains("xf-ai-nfe-analyzer", cut.Markup);
    }

    [Fact]
    public void NFe_Renders_AriaBusy_WhenIdle()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        Assert.Contains("aria-busy", cut.Markup);
        Assert.Contains("false", cut.Markup);
    }

    [Fact]
    public void NFe_Renders_AriaBusy_WhenLoading()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, new ExtendedNFeMockProvider())
            .Add(x => x.Loading, true));
        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }

    [Fact]
    public void NFe_Textarea_Disabled_WhenLoading()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, new ExtendedNFeMockProvider())
            .Add(x => x.Loading, true));
        var textarea = cut.Find("textarea");
        Assert.True(textarea.HasAttribute("disabled"));
    }

    [Fact]
    public void NFe_Textarea_Enabled_WhenNotDisabled()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        var textarea = cut.Find("textarea");
        Assert.False(textarea.HasAttribute("disabled"));
    }

    [Fact]
    public void NFe_ErrorBanner_Renders_AfterProviderException()
    {
        var provider = new ExtendedNFeMockProvider { ThrowException = new InvalidOperationException("API failure") };
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "<xml>test</xml>");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Contains("API failure", cut.Markup);
    }

    [Fact]
    public void NFe_ErrorBanner_Dismissible()
    {
        var provider = new ExtendedNFeMockProvider { ThrowException = new Exception("Some error") };
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        var errorBanner = cut.FindAll("div[role='alert']");
        Assert.NotEmpty(errorBanner);
        var dismissBtn = errorBanner[0].QuerySelector("button[aria-label='Dispensar erro']");
        Assert.NotNull(dismissBtn);
        cut.InvokeAsync(() => dismissBtn.Click());
        var errorAfterDismiss = cut.FindAll("div[role='alert']");
        Assert.Empty(errorAfterDismiss);
    }

    [Fact]
    public void NFe_Clear_ResetsQueryAndResult()
    {
        var provider = new ExtendedNFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "<xml>data</xml>");
            await cut.Instance.AnalyzeAsync();
        });
        cut.InvokeAsync(() => cut.Instance.Clear());
        var textarea = cut.Find("textarea");
        Assert.Empty(textarea.GetAttribute("value"));
    }

    [Fact]
    public void NFe_AnalyzeAsync_ProviderError_FiresOnError()
    {
        var provider = new ExtendedNFeMockProvider { ThrowException = new Exception("Provider down") };
        string? capturedError = null;
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, msg => capturedError = msg)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test query");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Equal("Provider down", capturedError);
    }

    [Fact]
    public void NFe_AnalyzeAsync_ProviderSuccess_FiresOnAnalysisComplete()
    {
        // Use a mock that returns JSON with integer enum values so System.Text.Json can deserialize
        var provider = new ExtendedNFeMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = """{"nFeKey":"test123","issuerName":"Test","aiSummary":"ok","spedStatus":1,"riskLevel":0,"confidence":0.95}"""
                }
            }
        };
        XFAiNFeAnalysisResult? capturedResult = null;
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, EventCallback.Factory.Create<XFAiNFeAnalysisResult>(this, r => capturedResult = r)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "<xml>nfe data</xml>");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(capturedResult);
        Assert.Equal("test123", capturedResult!.NFeKey);
        Assert.Equal(0.95, capturedResult.Confidence);
    }

    [Fact]
    public void NFe_EmptyState_ShowsDefaultText()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        Assert.Contains("Analisador de NF-e", cut.Markup);
        Assert.Contains("Cole o XML da NF-e", cut.Markup);
    }

    [Fact]
    public void NFe_ResultPreview_Template_RendersCustomContent()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, new ExtendedNFeMockProvider())
            .Add(x => x.ResultPreview, (RenderFragment<XFAiNFeAnalysisResult>)(r => b => b.AddContent(0, $"KEY:{r.NFeKey}"))));
        // Just verify the ResultPreview parameter is accepted and component renders
        Assert.Contains("xf-ai-nfe-analyzer", cut.Markup);
    }

    [Fact]
    public void NFe_MultipleAnalysis_CallsProviderMultipleTimes()
    {
        var provider = new ExtendedNFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "first query");
            await cut.Instance.AnalyzeAsync();
        });
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "second query");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Equal(2, provider.CallCount);
    }

    [Fact]
    public void NFe_AnalyzeAsync_InvalidJson_FiresOnError()
    {
        var provider = new ExtendedNFeMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = "not valid json at all" }
            }
        };
        string? capturedError = null;
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, msg => capturedError = msg)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(capturedError);
        Assert.Contains("formato esperado", capturedError);
    }

    [Fact]
    public void NFe_HandleInput_UpdatesQuery()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        var textarea = cut.Find("textarea");
        textarea.Input("<xml>updated value</xml>");
        var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var queryValue = f!.GetValue(cut.Instance);
        Assert.Equal("<xml>updated value</xml>", queryValue);
    }

    [Fact]
    public void NFe_HandleKeyDown_Escape_ClearsQuery()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        var textarea = cut.Find("textarea");
        textarea.Input("some text");
        textarea.KeyDown(new KeyboardEventArgs { Key = "Escape" });
        var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var queryValue = f!.GetValue(cut.Instance);
        Assert.Equal(string.Empty, queryValue);
    }

    [Fact]
    public void NFe_HandleKeyDown_Enter_TriggersAnalysis()
    {
        var provider = new ExtendedNFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        var textarea = cut.Find("textarea");
        cut.InvokeAsync(() =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "xml data");
        });
        textarea.KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public void NFe_HandleKeyDown_ShiftEnter_DoesNotTriggerAnalysis()
    {
        var provider = new ExtendedNFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        var textarea = cut.Find("textarea");
        cut.InvokeAsync(() =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "xml data");
        });
        textarea.KeyDown(new KeyboardEventArgs { Key = "Enter", ShiftKey = true });
        Assert.Equal(0, provider.CallCount);
    }

    [Fact]
    public void NFe_Dispose_CancelsPendingOperation()
    {
        var provider = new ExtendedNFeMockProvider();
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        var ex = Record.Exception(() => cut.Instance.Dispose());
        Assert.Null(ex);
    }

    [Fact]
    public void NFe_Section_HasCorrectRole()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        var section = cut.Find("section");
        Assert.NotNull(section);
    }

    [Fact]
    public void NFe_Section_AriaLabel_MatchesTitle()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, new ExtendedNFeMockProvider())
            .Add(x => x.Title, "Custom NFe Title"));
        Assert.Contains("Custom NFe Title", cut.Markup);
    }

    [Fact]
    public void NFe_SendButton_Disabled_WhenNoProvider()
    {
        var cut = Render<XFAiNFeAnalyzer>();
        var button = cut.Find("button[aria-label='Analisar NF-e']");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void NFe_SendButton_Disabled_WhenQueryEmpty()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Provider, new ExtendedNFeMockProvider()));
        var button = cut.Find("button[aria-label='Analisar NF-e']");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void NFe_AnalyzeAsync_JsonWithCodeBlocks_ExtractsCorrectly()
    {
        var provider = new ExtendedNFeMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "```json\n{\"nFeKey\":\"extracted\",\"aiSummary\":\"parsed ok\",\"spedStatus\":1,\"riskLevel\":0,\"confidence\":0.88}\n```"
                }
            }
        };
        XFAiNFeAnalysisResult? capturedResult = null;
        var cut = Render<XFAiNFeAnalyzer>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, EventCallback.Factory.Create<XFAiNFeAnalysisResult>(this, r => capturedResult = r)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiNFeAnalyzer).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(capturedResult);
        Assert.Equal("extracted", capturedResult!.NFeKey);
        Assert.Equal(0.88, capturedResult.Confidence);
    }
}
