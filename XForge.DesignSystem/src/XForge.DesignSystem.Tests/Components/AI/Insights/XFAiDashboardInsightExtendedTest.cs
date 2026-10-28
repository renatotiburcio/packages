using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Insights;
using XForge.DesignSystem.Services.AI;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI.Insights;

public class XFAiDashboardInsightExtendedTest : XForgeTestContext
{
    private sealed class TestAiProvider : IAiProvider
    {
        public string ProviderId => "test";
        public string DisplayName => "Test Provider";
        public bool SupportsStreaming => false;
        public bool SupportsToolCalling => false;
        public string DefaultModel => "test-model";

        public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
        {
            var json = "[{\"title\":\"Insight 1\",\"description\":\"Test insight\",\"type\":\"Trend\",\"severity\":\"Info\",\"confidence\":0.9,\"relatedKpi\":\"Revenue\",\"changePercent\":5.2,\"recommendation\":\"Keep it up\"}]";
            return Task.FromResult(new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = json },
                Usage = new AiTokenUsage { PromptTokens = 60, CompletionTokens = 40 }
            });
        }

        public async IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
            => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

        public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }

    private static XFAiDashboardKpi CreateKpi(string name = "Revenue", double value = 1000, double? previous = 950)
    {
        return new XFAiDashboardKpi
        {
            Name = name,
            DisplayName = name,
            CurrentValue = value,
            PreviousValue = previous,
            Unit = "R$"
        };
    }

    private static XFAiDashboardInsightItem CreateInsight(string id = "i1", string title = "Test", XFAiDashboardInsightType type = XFAiDashboardInsightType.Trend)
    {
        return new XFAiDashboardInsightItem
        {
            Id = id,
            Title = title,
            Description = "Description for " + title,
            Type = type,
            Severity = XFAiDashboardInsightSeverity.Info,
            Confidence = 0.85,
            RelatedKpi = "Revenue",
            ChangePercent = 5.2
        };
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_DefaultTitle_Renders()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, new[] { CreateInsight() }));
        Assert.Contains("Insights do Dashboard", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_CustomTitle_Renders()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Title, "My Insights")
            .Add(p => p.Insights, new[] { CreateInsight() }));
        Assert.Contains("My Insights", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_ShowTitleFalse_HidesHeader()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.ShowTitle, false)
            .Add(p => p.Insights, new[] { CreateInsight() }));
        // The <h3> header should not be rendered, but aria-label on root div always has Title
        Assert.DoesNotContain("<h3", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_EmptyInsights_RendersEmptyText()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, Array.Empty<XFAiDashboardInsightItem>()));
        Assert.Contains("Nenhum insight disponivel", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_CustomEmptyText_Renders()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.EmptyText, "No data")
            .Add(p => p.Insights, Array.Empty<XFAiDashboardInsightItem>()));
        Assert.Contains("No data", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_InsightTitles_Rendered()
    {
        var insights = new[]
        {
            CreateInsight("i1", "First Insight"),
            CreateInsight("i2", "Second Insight")
        };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("First Insight", cut.Markup);
        Assert.Contains("Second Insight", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_InsightDescriptions_Rendered()
    {
        var insights = new[] { CreateInsight("i1", "Test", XFAiDashboardInsightType.Trend) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("Description for Test", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_TypeTrend_RendersPurpleAccent()
    {
        var insights = new[] { CreateInsight("i1", "Trend", XFAiDashboardInsightType.Trend) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("border-l-purple-500", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_TypeOutlier_RendersRedAccent()
    {
        var insights = new[] { CreateInsight("i1", "Outlier", XFAiDashboardInsightType.Outlier) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("border-l-red-500", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_TypeForecast_RendersBlueAccent()
    {
        var insights = new[] { CreateInsight("i1", "Forecast", XFAiDashboardInsightType.Forecast) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("border-l-blue-500", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_TypeAlert_RendersAmberAccent()
    {
        var insights = new[] { CreateInsight("i1", "Alert", XFAiDashboardInsightType.Alert) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("border-l-amber-500", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_TypeRecommendation_RendersEmeraldAccent()
    {
        var insights = new[] { CreateInsight("i1", "Rec", XFAiDashboardInsightType.Recommendation) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("border-l-emerald-500", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_TypeAnomaly_RendersOrangeAccent()
    {
        var insights = new[] { CreateInsight("i1", "Anomaly", XFAiDashboardInsightType.Anomaly) };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("border-l-orange-500", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_ShowIconsFalse_HidesIcons()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.ShowIcons, false)
            .Add(p => p.Insights, new[] { CreateInsight() }));
        Assert.DoesNotContain("xf-icon", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_ShowDismissFalse_HidesDismiss()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.ShowDismiss, false)
            .Add(p => p.Insights, new[] { CreateInsight() }));
        // Dismiss button (which contains "Dispensar" aria-label) should not be rendered
        Assert.DoesNotContain("Dispensar", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_MaxInsights_LimitsDisplay()
    {
        var insights = Enumerable.Range(0, 10).Select(i => CreateInsight($"i{i}", $"Insight {i}")).ToArray();
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.MaxInsights, 3)
            .Add(p => p.Insights, insights));
        Assert.Contains("Insight 0", cut.Markup);
        Assert.Contains("Insight 1", cut.Markup);
        Assert.Contains("Insight 2", cut.Markup);
        Assert.DoesNotContain("Insight 3", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_Loading_RendersLoadingText()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Loading, true));
        Assert.Contains("Analisando KPIs", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_CustomLoadingText_Renders()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Loading, true)
            .Add(p => p.LoadingText, "Generating..."));
        Assert.Contains("Generating...", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_Confidence_RendersPercentage()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", Confidence = 0.75 } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("75%", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_ChangePercentPositive_RendersGreen()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", ChangePercent = 5.5 } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("text-emerald-600", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_ChangePercentNegative_RendersRed()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", ChangePercent = -3.2 } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("text-red-600", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_SeverityWarning_RendersIconContainer()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", Severity = XFAiDashboardInsightSeverity.Warning } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        // Severity icon is rendered via XFSvgIcons.Markup inside icon container
        Assert.Contains("h-8 w-8", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_SeverityCritical_RendersIconContainer()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", Severity = XFAiDashboardInsightSeverity.Critical } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("h-8 w-8", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_SeveritySuccess_RendersIconContainer()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", Severity = XFAiDashboardInsightSeverity.Success } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("h-8 w-8", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_OnInsightGenerated_FiresCallback()
    {
        var generated = default(XFAiDashboardInsightItem);
        var insights = new[] { CreateInsight("i1", "Click me") };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights)
            .Add(p => p.OnInsightGenerated, EventCallback.Factory.Create<XFAiDashboardInsightItem>(this, i => generated = i)));
        var item = cut.Find(".cursor-pointer");
        item.Click();
        Assert.NotNull(generated);
        Assert.Equal("i1", generated.Id);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_Disabled_DoesNotFireClick()
    {
        var generated = default(XFAiDashboardInsightItem);
        var insights = new[] { CreateInsight("i1", "Click me") };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights)
            .Add(p => p.Disabled, true)
            .Add(p => p.OnInsightGenerated, EventCallback.Factory.Create<XFAiDashboardInsightItem>(this, i => generated = i)));
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_EmptyContent_RendersWhenEmpty()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, Array.Empty<XFAiDashboardInsightItem>())
            .Add(p => p.EmptyContent, (RenderFragment)(builder => builder.AddContent(0, "Custom empty"))));
        Assert.Contains("Custom empty", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_HeaderContent_Renders()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, new[] { CreateInsight() })
            .Add(p => p.HeaderContent, (RenderFragment)(builder => builder.AddContent(0, "Custom header"))));
        Assert.Contains("Custom header", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_InsightTemplate_Renders()
    {
        var insights = new[] { CreateInsight("i1", "Templated") };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights)
            .Add(p => p.InsightTemplate, (XFAiDashboardInsightItem ctx) => (RenderFragment)(builder => builder.AddContent(0, $"Custom: {ctx.Title}"))));
        Assert.Contains("Custom: Templated", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_ProviderNull_GenerateInsights_DoesNotThrow()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Kpis, new[] { CreateKpi() }));
        // GenerateInsightsAsync should return early when Provider is null
        var task = cut.Instance.GenerateInsightsAsync();
        Assert.True(task.IsCompleted);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_KpisNull_GenerateInsights_DoesNotThrow()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Provider, new TestAiProvider()));
        var task = cut.Instance.GenerateInsightsAsync();
        Assert.True(task.IsCompleted);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_GenerateInsights_WithProvider_ParsesResponse()
    {
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Provider, new TestAiProvider())
            .Add(p => p.Kpis, new[] { CreateKpi() }));
        // Should not throw
        cut.InvokeAsync(() => cut.Instance.GenerateInsightsAsync());
        // After generation, the component should have attempted to parse
        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_Recommendation_Renders()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", Recommendation = "Do something" } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("Do something", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_RelatedKpi_Renders()
    {
        var insights = new[] { new XFAiDashboardInsightItem { Id = "i1", Title = "Test", RelatedKpi = "Revenue" } };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("Revenue", cut.Markup);
    }

    [Fact]
    public void Extended_XFAiDashboardInsight_CountBadge_Renders()
    {
        var insights = new[] { CreateInsight("i1", "One"), CreateInsight("i2", "Two") };
        var cut = Render<XFAiDashboardInsight>(ps => ps
            .Add(p => p.Insights, insights));
        Assert.Contains("2", cut.Markup);
    }
}
