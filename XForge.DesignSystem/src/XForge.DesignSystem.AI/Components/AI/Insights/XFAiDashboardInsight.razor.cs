using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Insights
{
    public partial class XFAiDashboardInsight : XForgeComponentBase
    {
        protected override string BaseClass => "xf-ai-dashboard-insight";

        private bool _isGenerating;
        private CancellationTokenSource? _cts;

        // ── Parameters ──────────────────────────────────────────────────────

        /// <summary>AI provider for generating insights from KPI data.</summary>
        [Parameter]
        public IAiProvider? Provider { get; set; }

        /// <summary>KPI data to analyze for insights.</summary>
        [Parameter]
        public IReadOnlyList<XFAiDashboardKpi>? Kpis { get; set; }

        /// <summary>Header title text.</summary>
        [Parameter]
        public string Title { get; set; } = "Insights do Dashboard";

        /// <summary>Whether to display the title header.</summary>
        [Parameter]
        public bool ShowTitle { get; set; } = true;

        /// <summary>Whether to display type-based icons on each insight.</summary>
        [Parameter]
        public bool ShowIcons { get; set; } = true;

        /// <summary>Whether to display the dismiss button on each insight.</summary>
        [Parameter]
        public bool ShowDismiss { get; set; } = true;

        /// <summary>Maximum number of insights to display.</summary>
        [Parameter]
        public int MaxInsights { get; set; } = 5;

        /// <summary>External loading state override.</summary>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>Text displayed when no insights are available.</summary>
        [Parameter]
        public string EmptyText { get; set; } = "Nenhum insight disponivel. Adicione KPIs para analise.";

        /// <summary>Text displayed during insight generation.</summary>
        [Parameter]
        public string LoadingText { get; set; } = "Analisando KPIs...";

        /// <summary>Whether to auto-generate insights when KPIs change.</summary>
        [Parameter]
        public bool AutoRefresh { get; set; }

        /// <summary>Custom system prompt for the AI provider.</summary>
        [Parameter]
        public string? SystemPrompt { get; set; }

        /// <summary>Current list of generated insights.</summary>
        [Parameter]
        public IReadOnlyList<XFAiDashboardInsightItem> Insights { get; set; } = Array.Empty<XFAiDashboardInsightItem>();

        /// <summary>Two-way binding for the insights list.</summary>
        [Parameter]
        public EventCallback<IReadOnlyList<XFAiDashboardInsightItem>> InsightsChanged { get; set; }

        /// <summary>Fired when a single insight is generated.</summary>
        [Parameter]
        public EventCallback<XFAiDashboardInsightItem> OnInsightGenerated { get; set; }

        /// <summary>Fired when all insights are generated.</summary>
        [Parameter]
        public EventCallback<IReadOnlyList<XFAiDashboardInsightItem>> OnInsightsGenerated { get; set; }

        /// <summary>Fired when an error occurs during analysis.</summary>
        [Parameter]
        public EventCallback<string> OnError { get; set; }

        /// <summary>Custom template for rendering each insight.</summary>
        [Parameter]
        public RenderFragment<XFAiDashboardInsightItem>? InsightTemplate { get; set; }

        /// <summary>Custom header content.</summary>
        [Parameter]
        public RenderFragment? HeaderContent { get; set; }

        /// <summary>Custom empty state content.</summary>
        [Parameter]
        public RenderFragment? EmptyContent { get; set; }

        // ── Computed ────────────────────────────────────────────────────────

        private bool IsLoading => Loading || _isGenerating;

        private IReadOnlyList<XFAiDashboardInsightItem> VisibleInsights =>
            Insights.Take(MaxInsights).ToList();

        private bool HasInsights => VisibleInsights.Count > 0;

        // ── Lifecycle ───────────────────────────────────────────────────────

        protected override async Task OnParametersSetAsync()
        {
            if (AutoRefresh && Kpis is { Count: > 0 } && Provider is not null && !_isGenerating)
            {
                await GenerateInsightsAsync();
            }
        }

        // ── Public Methods ──────────────────────────────────────────────────

        /// <summary>
        /// Triggers AI insight generation from the current KPI data.
        /// </summary>
        public async Task GenerateInsightsAsync()
        {
            if (Provider is null || Kpis is null || Kpis.Count == 0 || _isGenerating) return;

            _isGenerating = true;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            StateHasChanged();

            try
            {
                var prompt = BuildPrompt();
                var systemMsg = SystemPrompt ?? BuildDefaultSystemPrompt();

                var request = new AiChatRequest
                {
                    Messages = new[]
                    {
                        new AiMessage { Role = AiRole.User, Content = prompt }
                    },
                    SystemPrompt = systemMsg,
                    Temperature = 0.3,
                    MaxTokens = 2048
                };

                var response = await Provider.CompleteAsync(request, _cts.Token);
                var parsed = ParseInsights(response.Message.Content);

                var updated = parsed.Take(MaxInsights).ToList();
                Insights = updated;

                if (InsightsChanged.HasDelegate)
                {
                    await InsightsChanged.InvokeAsync(updated);
                }

                foreach (var insight in updated)
                {
                    if (OnInsightGenerated.HasDelegate)
                    {
                        await OnInsightGenerated.InvokeAsync(insight);
                    }
                }

                if (OnInsightsGenerated.HasDelegate)
                {
                    await OnInsightsGenerated.InvokeAsync(updated);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation is expected — no error to report
            }
            catch (Exception ex)
            {
                if (OnError.HasDelegate)
                {
                    await OnError.InvokeAsync(ex.Message);
                }
            }
            finally
            {
                _isGenerating = false;
                StateHasChanged();
            }
        }

        // ── Styling ─────────────────────────────────────────────────────────

        protected override string GetComponentClass()
        {
            return GetClass(
                $"{BaseClass} overflow-hidden rounded-xl border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06),0_8px_24px_rgb(15_23_42/.04)]");
        }

        private string HeaderClass =>
            "flex items-center justify-between gap-2 border-b border-border-light px-4 py-3";

        private string TitleClass =>
            "text-sm font-semibold text-text-primary";

        private string CountBadgeClass =>
            "inline-flex items-center justify-center min-w-[20px] h-5 px-1.5 rounded-full bg-surface-200 text-[10px] font-semibold text-text-secondary tabular-nums";

        private string ListClass =>
            "divide-y divide-border-light";

        private string InsightBaseClass =>
            "group relative flex items-start gap-3 px-4 py-3 outline-none transition-colors duration-150 hover:bg-surface-50 focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-border-focus";

        private string InsightDisabledClass =>
            Disabled ? "pointer-events-none opacity-50" : "cursor-pointer";

        private string InsightTitleClass =>
            "text-sm font-medium text-text-primary leading-5";

        private string InsightDescriptionClass =>
            "mt-0.5 text-xs text-text-secondary leading-4";

        private string KpiBadgeClass =>
            "inline-flex shrink-0 items-center rounded-md bg-surface-100 px-1.5 py-0.5 text-[10px] font-medium text-text-muted";

        private string RecommendationClass =>
            "mt-1 text-xs text-text-secondary italic leading-4";

        private string ActionButtonClass =>
            "mt-1.5 inline-flex items-center gap-1 rounded-md bg-primary-50 px-2 py-1 text-[11px] font-medium text-primary-700 outline-none transition-colors duration-150 hover:bg-primary-100 focus-visible:ring-2 focus-visible:ring-border-focus active:scale-95";

        private string ConfidenceBarContainerClass =>
            "mt-1.5 flex items-center gap-2";

        private string ConfidenceBarTrackClass =>
            "h-1 w-16 overflow-hidden rounded-full bg-surface-200";

        private string ConfidenceBarFillClass(XFAiDashboardInsightType type) =>
            $"h-full rounded-full transition-all duration-300 {TypeConfidenceClass(type)}";

        private string ConfidenceLabelClass =>
            "text-[10px] font-medium text-text-muted tabular-nums";

        private string DismissButtonClass =>
            "absolute right-3 top-3 inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md text-text-muted outline-none transition-all duration-150 opacity-0 group-hover:opacity-100 group-focus-within:opacity-100 hover:bg-surface-200 hover:text-text-secondary focus-visible:ring-2 focus-visible:ring-border-focus active:scale-95";

        private string EmptyClass =>
            "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

        private string EmptyIconClass =>
            "h-8 w-8 text-text-muted";

        private string EmptyTextClass =>
            "text-sm text-text-secondary";

        private string LoadingSkeletonClass =>
            "animate-pulse";

        private string SkeletonItemClass =>
            "flex items-start gap-3 px-4 py-3";

        private string SkeletonIconClass =>
            "h-8 w-8 shrink-0 rounded-lg bg-surface-200";

        private string SkeletonLineClass =>
            "h-3 rounded bg-surface-200";

        // ── Type-based Styling ──────────────────────────────────────────────

        private string IconContainerClass(XFAiDashboardInsightType type, XFAiDashboardInsightSeverity severity) =>
            $"mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-lg {TypeBgClass(type)}";

        private string IconClass(XFAiDashboardInsightType type) =>
            $"h-4 w-4 {TypeTextClass(type)}";

        private string TypeBorderClass(XFAiDashboardInsightType type) => type switch
        {
            XFAiDashboardInsightType.Trend => "border-l-[3px] border-l-purple-500",
            XFAiDashboardInsightType.Outlier => "border-l-[3px] border-l-red-500",
            XFAiDashboardInsightType.Forecast => "border-l-[3px] border-l-blue-500",
            XFAiDashboardInsightType.Alert => "border-l-[3px] border-l-amber-500",
            XFAiDashboardInsightType.Recommendation => "border-l-[3px] border-l-emerald-500",
            XFAiDashboardInsightType.Anomaly => "border-l-[3px] border-l-orange-500",
            _ => "border-l-[3px] border-l-slate-400"
        };

        private string TypeBgClass(XFAiDashboardInsightType type) => type switch
        {
            XFAiDashboardInsightType.Trend => "bg-purple-50",
            XFAiDashboardInsightType.Outlier => "bg-red-50",
            XFAiDashboardInsightType.Forecast => "bg-blue-50",
            XFAiDashboardInsightType.Alert => "bg-amber-50",
            XFAiDashboardInsightType.Recommendation => "bg-emerald-50",
            XFAiDashboardInsightType.Anomaly => "bg-orange-50",
            _ => "bg-slate-100"
        };

        private string TypeTextClass(XFAiDashboardInsightType type) => type switch
        {
            XFAiDashboardInsightType.Trend => "text-purple-600",
            XFAiDashboardInsightType.Outlier => "text-red-600",
            XFAiDashboardInsightType.Forecast => "text-blue-600",
            XFAiDashboardInsightType.Alert => "text-amber-600",
            XFAiDashboardInsightType.Recommendation => "text-emerald-600",
            XFAiDashboardInsightType.Anomaly => "text-orange-600",
            _ => "text-slate-500"
        };

        private string TypeConfidenceClass(XFAiDashboardInsightType type) => type switch
        {
            XFAiDashboardInsightType.Trend => "bg-purple-500",
            XFAiDashboardInsightType.Outlier => "bg-red-500",
            XFAiDashboardInsightType.Forecast => "bg-blue-500",
            XFAiDashboardInsightType.Alert => "bg-amber-500",
            XFAiDashboardInsightType.Recommendation => "bg-emerald-500",
            XFAiDashboardInsightType.Anomaly => "bg-orange-500",
            _ => "bg-slate-400"
        };

        private string SeverityIcon(XFAiDashboardInsightSeverity severity) => severity switch
        {
            XFAiDashboardInsightSeverity.Warning => "triangle-exclamation",
            XFAiDashboardInsightSeverity.Critical => "circle-exclamation",
            XFAiDashboardInsightSeverity.Success => "circle-check",
            _ => "circle-info"
        };

        private string ChangePercentClass(double change) =>
            $"mt-1 inline-flex items-center gap-0.5 text-[11px] font-semibold {(change >= 0 ? "text-emerald-600" : "text-red-600")}";

        private string ChangePercentText(double change)
        {
            var arrow = change >= 0 ? "\u25B2" : "\u25BC";
            return $"{arrow} {Math.Abs(change):F1}%";
        }

        private string ConfidencePercent(double? confidence)
        {
            if (confidence is null) return string.Empty;
            var clamped = Math.Clamp(confidence.Value, 0, 1);
            return $"{clamped:P0}";
        }

        private double ConfidenceWidth(double? confidence)
        {
            if (confidence is null) return 0;
            return Math.Clamp(confidence.Value, 0, 1) * 100;
        }

        // ── Prompt Building ─────────────────────────────────────────────────

        private string BuildDefaultSystemPrompt() =>
            "Voce e um assistente de analise de dados de dashboard. " +
            "Analise os KPIs fornecidos e gere insights em formato JSON. " +
            "Para cada insight, forne\u00e7a: title, description, type (Trend|Outlier|Forecast|Alert|Recommendation|Anomaly), " +
            "severity (Info|Warning|Critical|Success), confidence (0-1), relatedKpi, changePercent, recommendation. " +
            "Responda APENAS com um array JSON valido, sem texto adicional.";

        private string BuildPrompt()
        {
            if (Kpis is null || Kpis.Count == 0) return string.Empty;

            var lines = new List<string> { "Analise os seguintes KPIs e gere insights:" };

            foreach (var kpi in Kpis)
            {
                var change = kpi.PreviousValue.HasValue
                    ? $", variacao: {((kpi.CurrentValue - kpi.PreviousValue.Value) / Math.Max(Math.Abs(kpi.PreviousValue.Value), 1) * 100):F1}%"
                    : "";
                var target = kpi.Target.HasValue ? $", meta: {kpi.Target.Value}" : "";
                var unit = !string.IsNullOrEmpty(kpi.Unit) ? $" ({kpi.Unit})" : "";
                lines.Add($"- {kpi.DisplayName}{unit}: valor atual={kpi.CurrentValue}{change}{target}{(kpi.Category != null ? $", categoria={kpi.Category}" : "")}");
            }

            return string.Join("\n", lines);
        }

        // ── Response Parsing ────────────────────────────────────────────────

        private static List<XFAiDashboardInsightItem> ParseInsights(string content)
        {
            var result = new List<XFAiDashboardInsightItem>();

            if (string.IsNullOrWhiteSpace(content)) return result;

            try
            {
                // Extract JSON array from response (handle markdown code blocks)
                var json = content.Trim();
                var startIdx = json.IndexOf('[');
                var endIdx = json.LastIndexOf(']');

                if (startIdx < 0 || endIdx < 0 || endIdx <= startIdx) return result;

                json = json[startIdx..(endIdx + 1)];

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var insight = new XFAiDashboardInsightItem
                    {
                        Title = item.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "",
                        Description = item.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "",
                        Type = item.TryGetProperty("type", out var tp) ? ParseInsightType(tp.GetString()) : XFAiDashboardInsightType.Recommendation,
                        Severity = item.TryGetProperty("severity", out var sv) ? ParseSeverity(sv.GetString()) : XFAiDashboardInsightSeverity.Info,
                        Confidence = item.TryGetProperty("confidence", out var cf) && cf.TryGetDouble(out var cfVal) ? cfVal : null,
                        RelatedKpi = item.TryGetProperty("relatedKpi", out var rk) ? rk.GetString() : null,
                        ChangePercent = item.TryGetProperty("changePercent", out var cp) && cp.TryGetDouble(out var cpVal) ? cpVal : null,
                        Recommendation = item.TryGetProperty("recommendation", out var rc) ? rc.GetString() : null
                    };

                    result.Add(insight);
                }
            }
            catch
            {
                // Parsing failed — return empty list; error handling is done by caller
            }

            return result;
        }

        private static XFAiDashboardInsightType ParseInsightType(string? value) => value?.ToLowerInvariant() switch
        {
            "trend" => XFAiDashboardInsightType.Trend,
            "outlier" => XFAiDashboardInsightType.Outlier,
            "forecast" => XFAiDashboardInsightType.Forecast,
            "alert" => XFAiDashboardInsightType.Alert,
            "recommendation" => XFAiDashboardInsightType.Recommendation,
            "anomaly" => XFAiDashboardInsightType.Anomaly,
            _ => XFAiDashboardInsightType.Recommendation
        };

        private static XFAiDashboardInsightSeverity ParseSeverity(string? value) => value?.ToLowerInvariant() switch
        {
            "warning" => XFAiDashboardInsightSeverity.Warning,
            "critical" => XFAiDashboardInsightSeverity.Critical,
            "success" => XFAiDashboardInsightSeverity.Success,
            _ => XFAiDashboardInsightSeverity.Info
        };

        // ── Events ──────────────────────────────────────────────────────────

        private async Task HandleInsightClickAsync(XFAiDashboardInsightItem insight)
        {
            if (Disabled) return;

            if (OnInsightGenerated.HasDelegate)
            {
                await OnInsightGenerated.InvokeAsync(insight);
            }
        }

        private async Task HandleDismissAsync(XFAiDashboardInsightItem insight, MouseEventArgs _)
        {
            if (Disabled) return;

            var updated = Insights.Where(i => i.Id != insight.Id).ToList();

            if (InsightsChanged.HasDelegate)
            {
                await InsightsChanged.InvokeAsync(updated);
            }
        }

        private async Task HandleInsightKeyDownAsync(KeyboardEventArgs args, XFAiDashboardInsightItem insight)
        {
            if (Disabled) return;

            switch (args.Key)
            {
                case "Enter":
                case " ":
                    await HandleInsightClickAsync(insight);
                    break;

                case "Delete":
                case "Backspace":
                    if (ShowDismiss)
                    {
                        await HandleDismissAsync(insight, default!);
                    }
                    break;
            }
        }
    }
}
