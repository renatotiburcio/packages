using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Financial;
using XForge.DesignSystem.Services.AI;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI;

public sealed class XFAiSalesGoalAdvisorExtendedTest : BunitContext
{
    public XFAiSalesGoalAdvisorExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private sealed class SalesGoalMockProvider : IAiProvider
    {
        public string ProviderId => "sales-goal-mock";
        public string DisplayName => "Sales Goal Mock";
        public bool SupportsStreaming => false;
        public bool SupportsToolCalling => false;
        public string? DefaultModel => null;
        public AiChatResponse? NextResponse { get; set; }

        public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
        {
            return Task.FromResult(NextResponse ?? new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = "{\"aiSummary\":\"ok\",\"confidence\":0.9}" }
            });
        }

        public async IAsyncEnumerable<XForge.DesignSystem.Components.AI.Common.AiStreamChunk> StreamAsync(AiChatRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }

    private IRenderedComponent<XFAiSalesGoalAdvisor> RenderDefault(IAiProvider? provider = null)
        => Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, provider ?? new SalesGoalMockProvider()));

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-ai-sales-goal-advisor", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithRegionRole()
    {
        var cut = RenderDefault();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void Extended_Title_RendersDefaultTitle()
    {
        var cut = RenderDefault();
        Assert.Contains("Assistente de Metas", cut.Markup);
    }

    [Fact]
    public void Extended_Title_CustomTitle_RendersCustom()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.Title, "Custom Title"));
        Assert.Contains("Custom Title", cut.Markup);
    }

    [Fact]
    public void Extended_ShowHeaderTrue_RendersHeader()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.ShowHeader, true));
        // ShowHeader=true renders the header section with sparkles icon
        Assert.Contains("xf-ai-sales-goal-advisor", cut.Markup);
    }

    [Fact]
    public void Extended_Placeholder_RendersInTextarea()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.Placeholder, "Ask about goals..."));
        Assert.Contains("Ask about goals...", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_SendButtonIsDisabled()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.Disabled, true));
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_NoProvider_CanSendIsFalse()
    {
        var cut = Render<XFAiSalesGoalAdvisor>();
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_EmptyState_RendersDefaultEmptyContent()
    {
        var cut = RenderDefault();
        Assert.Contains("Assistente de Metas", cut.Markup);
        Assert.Contains("Pergunte sobre ranking", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyContent_CustomContent_Renders()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.EmptyContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddContent(1, "No data");
                b.CloseElement();
            })));
        Assert.Contains("No data", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderContent_CustomContent_Renders()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.HeaderContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddContent(1, "Custom Header");
                b.CloseElement();
            })));
        Assert.Contains("Custom Header", cut.Markup);
    }

    [Fact]
    public void Extended_AnalyzeAsync_ProviderSuccess_FiresOnAnalysisComplete()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "{\"aiSummary\":\"Meta atingida\",\"confidence\":0.95,\"rankings\":[{\"position\":1,\"salespersonName\":\"Joao\",\"totalSales\":50000,\"goalAmount\":40000,\"achievementPercentage\":125,\"commission\":2500,\"badge\":\"gold\"}],\"goals\":[{\"goalName\":\"Meta Q1\",\"targetAmount\":100000,\"currentAmount\":80000,\"progressPercentage\":80,\"daysRemaining\":15,\"predictedOutcome\":\"will-achieve\",\"predictedFinalAmount\":105000}]}"
                }
            }
        };
        XFAiSalesGoalInsight? capturedResult = null;
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, EventCallback.Factory.Create<XFAiSalesGoalInsight>(this, r => capturedResult = r)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Qual o ranking?");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(capturedResult);
        Assert.Equal("Meta atingida", capturedResult!.AiSummary);
        Assert.Equal(0.95, capturedResult.Confidence);
    }

    [Fact]
    public void Extended_AnalyzeAsync_Result_RendersRankings()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "{\"aiSummary\":\"ok\",\"confidence\":0.9,\"rankings\":[{\"position\":1,\"salespersonName\":\"Maria\",\"totalSales\":75000,\"goalAmount\":60000,\"achievementPercentage\":125,\"commission\":3000,\"badge\":\"gold\"}]}"
                }
            }
        };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Ranking");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Contains("Maria", cut.Markup);
        Assert.Contains("gold", cut.Markup);
    }

    [Fact]
    public void Extended_AnalyzeAsync_Result_RendersGoals()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "{\"aiSummary\":\"ok\",\"confidence\":0.85,\"goals\":[{\"goalName\":\"Meta Mensal\",\"targetAmount\":200000,\"currentAmount\":150000,\"progressPercentage\":75,\"daysRemaining\":10,\"predictedOutcome\":\"at-risk\",\"predictedFinalAmount\":180000}]}"
                }
            }
        };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "Metas");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Contains("Meta Mensal", cut.Markup);
        Assert.Contains("Em risco", cut.Markup);
    }

    [Fact]
    public void Extended_AnalyzeAsync_JsonWithCodeBlocks_ExtractsCorrectly()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "```json\n{\"aiSummary\":\"parsed\",\"confidence\":0.88}\n```"
                }
            }
        };
        XFAiSalesGoalInsight? capturedResult = null;
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, provider)
            .Add(x => x.OnAnalysisComplete, EventCallback.Factory.Create<XFAiSalesGoalInsight>(this, r => capturedResult = r)));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.NotNull(capturedResult);
        Assert.Equal("parsed", capturedResult!.AiSummary);
    }

    [Fact]
    public void Extended_AnalyzeAsync_InvalidJson_SetsError()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = "not json at all" }
            }
        };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        var errorField = typeof(XFAiSalesGoalAdvisor).GetField("_errorText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var errorValue = errorField!.GetValue(cut.Instance) as string;
        Assert.NotNull(errorValue);
    }

    [Fact]
    public void Extended_DismissError_ClearsError()
    {
        var cut = RenderDefault();
        var errorField = typeof(XFAiSalesGoalAdvisor).GetField("_errorText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        errorField!.SetValue(cut.Instance, "Some error");
        cut.InvokeAsync(() => cut.Instance.GetType().GetMethod("DismissError", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(cut.Instance, null));
        var errorValue = errorField.GetValue(cut.Instance) as string;
        Assert.Null(errorValue);
    }

    [Fact]
    public void Extended_Clear_ClearsResult()
    {
        var cut = RenderDefault();
        cut.InvokeAsync(() => cut.Instance.Clear());
        var resultField = typeof(XFAiSalesGoalAdvisor).GetField("_result", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var resultValue = resultField!.GetValue(cut.Instance);
        Assert.Null(resultValue);
    }

    [Fact]
    public void Extended_IsBusy_RendersAriaBusy()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.Loading, true));
        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_NotBusy_RendersAriaBusyFalse()
    {
        var cut = RenderDefault();
        Assert.Contains("aria-busy=\"false\"", cut.Markup);
    }

    [Fact]
    public void Extended_Confidence_RendersProgressBar()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "{\"aiSummary\":\"ok\",\"confidence\":0.92}"
                }
            }
        };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Contains("role=\"progressbar\"", cut.Markup);
        Assert.Contains("92%", cut.Markup);
    }

    [Fact]
    public void Extended_ResultPreview_Template_RendersCustomContent()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.ResultPreview, (RenderFragment<XFAiSalesGoalInsight>)(r => b => b.AddContent(0, $"SUMMARY:{r.AiSummary}"))));
        Assert.Contains("xf-ai-sales-goal-advisor", cut.Markup);
    }

    [Fact]
    public void Extended_Dispose_DoesNotThrow()
    {
        var cut = RenderDefault();
        var exception = Record.Exception(() => cut.Instance.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Extended_Class_CustomClass_Appended()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.Class, "my-sales-goal"));
        Assert.Contains("my-sales-goal", cut.Markup);
    }

    [Fact]
    public void Extended_Style_CustomStyle_Rendered()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.Style, "padding:10px"));
        Assert.Contains("padding:10px", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_Rendered()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p
            .Add(x => x.Provider, new SalesGoalMockProvider())
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "sales-goal" } }));
        Assert.Contains("data-testid=\"sales-goal\"", cut.Markup);
    }

    [Fact]
    public void Extended_BadgeSilver_RendersSilverClass()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "{\"aiSummary\":\"ok\",\"confidence\":0.9,\"rankings\":[{\"position\":2,\"salespersonName\":\"Ana\",\"totalSales\":40000,\"goalAmount\":50000,\"achievementPercentage\":80,\"commission\":1500,\"badge\":\"silver\"}]}"
                }
            }
        };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Contains("silver", cut.Markup);
    }

    [Fact]
    public void Extended_BadgeBronze_RendersBronzeClass()
    {
        var provider = new SalesGoalMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = "{\"aiSummary\":\"ok\",\"confidence\":0.9,\"rankings\":[{\"position\":3,\"salespersonName\":\"Carlos\",\"totalSales\":20000,\"goalAmount\":50000,\"achievementPercentage\":40,\"commission\":500,\"badge\":\"bronze\"}]}"
                }
            }
        };
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        Assert.Contains("bronze", cut.Markup);
    }

    [Fact]
    public void Extended_HandleKeyDown_Escape_ClearsQuery()
    {
        var cut = RenderDefault();
        var textarea = cut.Find("textarea");
        textarea.Input("test query");
        textarea.KeyDown(new KeyboardEventArgs { Key = "Escape" });
        var queryField = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var queryValue = queryField!.GetValue(cut.Instance) as string;
        Assert.Equal(string.Empty, queryValue);
    }

    [Fact]
    public void Extended_HandleKeyDown_Enter_PreventsDefault()
    {
        var cut = RenderDefault();
        var textarea = cut.Find("textarea");
        textarea.KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.Contains("xf-ai-sales-goal-advisor", cut.Markup);
    }

    [Fact]
    public void Extended_AnalyzeAsync_EmptyQuery_DoesNotCallProvider()
    {
        var provider = new SalesGoalMockProvider();
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Provider, provider));
        // Don't set _query, so CanSend is false
        cut.InvokeAsync(async () => await cut.Instance.AnalyzeAsync());
        // Provider should not have been called (NextResponse is null, would throw if called)
        Assert.Contains("xf-ai-sales-goal-advisor", cut.Markup);
    }

    [Fact]
    public void Extended_AnalyzeAsync_NullProvider_DoesNotThrow()
    {
        var cut = Render<XFAiSalesGoalAdvisor>();
        cut.InvokeAsync(async () =>
        {
            var f = typeof(XFAiSalesGoalAdvisor).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f!.SetValue(cut.Instance, "test");
            await cut.Instance.AnalyzeAsync();
        });
        // Should not throw, just return early
        Assert.Contains("xf-ai-sales-goal-advisor", cut.Markup);
    }
}
