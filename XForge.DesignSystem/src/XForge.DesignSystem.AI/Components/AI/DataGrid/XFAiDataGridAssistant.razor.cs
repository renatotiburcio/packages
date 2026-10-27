using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.DataGrid;

/// <summary>
/// AI assistant for DataGrid that provides natural language filtering, data explanation,
/// smart grouping, AI sorting, summarization, pattern/anomaly detection, and KPI extraction.
/// </summary>
public partial class XFAiDataGridAssistant : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>AI provider for completions. When null, only OnQueryGenerated/OnInsightFound are invoked externally.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>Grid data for context (sample or full dataset).</summary>
    [Parameter]
    public IEnumerable<object>? Data { get; set; }

    /// <summary>Column names for context.</summary>
    [Parameter]
    public IReadOnlyList<string>? Columns { get; set; }

    /// <summary>Input placeholder text.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Pergunte sobre seus dados...";

    /// <summary>Header title.</summary>
    [Parameter]
    public string Title { get; set; } = "Assistente de Dados";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Maximum number of insights to display.</summary>
    [Parameter]
    public int MaxResults { get; set; } = 5;

    /// <summary>External loading state (combined with internal loading).</summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Faça uma pergunta sobre seus dados para receber insights.";

    /// <summary>Custom system prompt override for the AI model.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when AI generates a grid query (filter/sort/group).</summary>
    [Parameter]
    public EventCallback<XFAiGridQuery> OnQueryGenerated { get; set; }

    /// <summary>Fired when AI finds an insight.</summary>
    [Parameter]
    public EventCallback<XFAiGridInsight> OnInsightFound { get; set; }

    /// <summary>Fired on error during AI request.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom insight rendering template.</summary>
    [Parameter]
    public RenderFragment<XFAiGridInsight>? InsightTemplate { get; set; }

    /// <summary>Custom header content (replaces default header).</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private string? _queryText;
    private bool _isBusy;
    private string? _errorText;
    private XFAiGridQuery? _currentQuery;
    private CancellationTokenSource? _cts;
    private readonly List<XFAiGridInsight> _insights = new();

    private bool CanSubmit => !Disabled && !_isBusy && !string.IsNullOrWhiteSpace(_queryText);
    private bool HasInsights => _insights.Count > 0;

    private IReadOnlyList<XFAiGridInsight> VisibleInsights =>
        _insights.Take(MaxResults).ToList();

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => Join(
        "xf-ai-datagrid-assistant flex flex-col overflow-hidden rounded-xl border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06),0_8px_24px_rgb(15_23_42/.04)]",
        Class);

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string HeaderTitleClass =>
        "inline-flex items-center gap-2 text-sm font-semibold text-text-primary";

    private string ComposerClass =>
        "flex items-center gap-2 border-b border-border-light bg-surface-50 px-4 py-3";

    private string SearchIconClass =>
        "pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-text-muted";

    private string InputClass =>
        "w-full rounded-lg border border-surface-200 bg-bg-surface py-2 pl-9 pr-3 text-sm leading-6 text-text-primary outline-none transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";

    private string SubmitButtonClass =>
        "inline-flex h-9 min-w-9 items-center justify-center rounded-lg bg-primary px-3 text-sm font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50";

    private string CancelButtonClass =>
        "inline-flex h-9 min-w-9 items-center justify-center rounded-lg border border-border-light bg-bg-surface px-3 text-sm font-semibold text-text-primary outline-none transition-all duration-150 hover:bg-surface-100 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2";

    private string ErrorBannerClass =>
        "flex items-center gap-2 border-t border-danger-200 bg-danger-50 px-4 py-2 text-xs text-danger";

    private string LoadingClass =>
        "animate-pulse";

    private string SkeletonItemClass =>
        "flex items-start gap-3 px-4 py-3";

    private string SkeletonCircleClass =>
        "h-8 w-8 shrink-0 rounded-lg bg-surface-200";

    private string SkeletonLineClass =>
        "h-3 rounded bg-surface-200";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

    private string EmptyIconClass =>
        "h-8 w-8 text-text-muted";

    private string EmptyTextClass =>
        "text-sm text-text-secondary";

    private string ResultsContainerClass =>
        "overflow-y-auto";

    private string QuerySummaryClass =>
        "flex items-center gap-2 border-b border-border-light bg-surface-50 px-4 py-2 text-xs";

    private string ConfidenceBadgeClass =>
        "ml-auto inline-flex items-center rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-semibold text-primary tabular-nums";

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

    private string ConfidenceBarContainerClass =>
        "mt-1.5 flex items-center gap-2";

    private string ConfidenceBarTrackClass =>
        "h-1 w-16 overflow-hidden rounded-full bg-surface-200";

    private string ConfidenceLabelClass =>
        "text-[10px] font-medium text-text-muted tabular-nums";

    private string ActionButtonClass =>
        "mt-1.5 inline-flex items-center rounded-md bg-primary/10 px-2 py-1 text-xs font-medium text-primary outline-none transition-all duration-150 hover:bg-primary/20 focus-visible:ring-2 focus-visible:ring-border-focus";

    private string InsightItemClass(XFAiGridInsight insight) =>
        $"{InsightBaseClass} {InsightDisabledClass} {TypeBorderClass(insight.Type)}";

    private string IconContainerClass(XFAiGridInsightType type) =>
        $"mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-lg {TypeBgClass(type)}";

    private string IconClass(XFAiGridInsightType type) =>
        $"h-4 w-4 {TypeTextClass(type)}";

    private string ConfidenceBarFillClass(XFAiGridInsightType type) =>
        $"h-full rounded-full transition-all duration-300 {TypeConfidenceClass(type)}";

    // ── Type-based styling ──────────────────────────────────────────────

    private static string TypeBorderClass(XFAiGridInsightType type) => type switch
    {
        XFAiGridInsightType.Pattern => "border-l-[3px] border-l-blue-500",
        XFAiGridInsightType.Anomaly => "border-l-[3px] border-l-red-500",
        XFAiGridInsightType.Kpi => "border-l-[3px] border-l-emerald-500",
        XFAiGridInsightType.Trend => "border-l-[3px] border-l-purple-500",
        XFAiGridInsightType.Summary => "border-l-[3px] border-l-slate-400",
        XFAiGridInsightType.Recommendation => "border-l-[3px] border-l-amber-500",
        _ => "border-l-[3px] border-l-slate-400"
    };

    private static string TypeBgClass(XFAiGridInsightType type) => type switch
    {
        XFAiGridInsightType.Pattern => "bg-blue-50",
        XFAiGridInsightType.Anomaly => "bg-red-50",
        XFAiGridInsightType.Kpi => "bg-emerald-50",
        XFAiGridInsightType.Trend => "bg-purple-50",
        XFAiGridInsightType.Summary => "bg-slate-100",
        XFAiGridInsightType.Recommendation => "bg-amber-50",
        _ => "bg-slate-100"
    };

    private static string TypeTextClass(XFAiGridInsightType type) => type switch
    {
        XFAiGridInsightType.Pattern => "text-blue-600",
        XFAiGridInsightType.Anomaly => "text-red-600",
        XFAiGridInsightType.Kpi => "text-emerald-600",
        XFAiGridInsightType.Trend => "text-purple-600",
        XFAiGridInsightType.Summary => "text-slate-500",
        XFAiGridInsightType.Recommendation => "text-amber-600",
        _ => "text-slate-500"
    };

    private static string TypeConfidenceClass(XFAiGridInsightType type) => type switch
    {
        XFAiGridInsightType.Pattern => "bg-blue-500",
        XFAiGridInsightType.Anomaly => "bg-red-500",
        XFAiGridInsightType.Kpi => "bg-emerald-500",
        XFAiGridInsightType.Trend => "bg-purple-500",
        XFAiGridInsightType.Summary => "bg-slate-400",
        XFAiGridInsightType.Recommendation => "bg-amber-500",
        _ => "bg-slate-400"
    };

    private static string DefaultIconForType(XFAiGridInsightType type) => type switch
    {
        XFAiGridInsightType.Pattern => "magnifying-glass-chart",
        XFAiGridInsightType.Anomaly => "triangle-exclamation",
        XFAiGridInsightType.Kpi => "gauge",
        XFAiGridInsightType.Trend => "arrow-trend-up",
        XFAiGridInsightType.Summary => "bars-staggered",
        XFAiGridInsightType.Recommendation => "lightbulb",
        _ => "circle-info"
    };

    private static string ConfidencePercent(double? confidence)
    {
        if (confidence is null) return string.Empty;
        var clamped = Math.Clamp(confidence.Value, 0, 1);
        return $"{clamped:P0}";
    }

    private static double ConfidenceWidth(double? confidence)
    {
        if (confidence is null) return 0;
        return Math.Clamp(confidence.Value, 0, 1) * 100;
    }

    // ── User Interactions ───────────────────────────────────────────────

    private void HandleInput(ChangeEventArgs args)
    {
        _queryText = args.Value?.ToString();
        StateHasChanged();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "Enter" when CanSubmit:
                await SubmitQueryAsync();
                break;

            case "Escape":
                _queryText = string.Empty;
                _errorText = null;
                StateHasChanged();
                break;
        }
    }

    private async Task HandleInsightClickAsync(XFAiGridInsight insight)
    {
        if (Disabled) return;

        if (OnInsightFound.HasDelegate)
        {
            await OnInsightFound.InvokeAsync(insight);
        }
    }

    private async Task HandleInsightKeyDownAsync(KeyboardEventArgs args, XFAiGridInsight insight)
    {
        if (Disabled) return;

        switch (args.Key)
        {
            case "Enter":
            case " ":
                await HandleInsightClickAsync(insight);
                break;
        }
    }

    private async Task HandleInsightActionAsync(XFAiGridInsight insight)
    {
        if (Disabled) return;

        if (OnInsightFound.HasDelegate)
        {
            await OnInsightFound.InvokeAsync(insight);
        }
    }

    // ── Public Methods ──────────────────────────────────────────────────

    /// <summary>
    /// Programmatically submits a natural language query.
    /// Builds an AI request with grid context and processes the response.
    /// </summary>
    public async Task SubmitQueryAsync()
    {
        if (!CanSubmit)
            return;

        var query = _queryText!.Trim();
        _errorText = null;
        _insights.Clear();
        _currentQuery = null;

        if (Provider is null)
        {
            // Without a provider, create a basic query and fire the callback
            var basicQuery = new XFAiGridQuery
            {
                NaturalLanguageQuery = query,
                Summary = "Consulta enviada sem provedor de IA configurado.",
                Confidence = 0
            };

            _currentQuery = basicQuery;

            if (OnQueryGenerated.HasDelegate)
            {
                await OnQueryGenerated.InvokeAsync(basicQuery);
            }

            StateHasChanged();
            return;
        }

        await ProcessQueryAsync(query);
    }

    /// <summary>
    /// Clears the current insights and query state.
    /// </summary>
    public void Clear()
    {
        _insights.Clear();
        _currentQuery = null;
        _errorText = null;
        _queryText = string.Empty;
        StateHasChanged();
    }

    /// <summary>
    /// Cancels the current AI request.
    /// </summary>
    public async Task CancelAsync()
    {
        if (_cts is not null && !_cts.IsCancellationRequested)
        {
            await _cts.CancelAsync();
        }

        _isBusy = false;
        StateHasChanged();
    }

    // ── Internal Processing ─────────────────────────────────────────────

    private async Task ProcessQueryAsync(string query)
    {
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        _isBusy = true;
        StateHasChanged();

        try
        {
            var systemPrompt = SystemPrompt ?? BuildDefaultSystemPrompt();
            var userMessage = BuildUserMessage(query);

            var request = new AiChatRequest
            {
                Messages = new[]
                {
                    new AiMessage { Role = AiRole.System, Content = systemPrompt },
                    new AiMessage { Role = AiRole.User, Content = userMessage }
                },
                Temperature = 0.3,
                MaxTokens = 2048
            };

            var response = await Provider!.CompleteAsync(request, ct);
            var content = response.Message.Content;

            ParseResponse(query, content);

            _isBusy = false;
            StateHasChanged();
        }
        catch (OperationCanceledException)
        {
            _isBusy = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }

        _cts?.Dispose();
        _cts = null;
    }

    private string BuildDefaultSystemPrompt()
    {
        var columns = Columns is { Count: > 0 }
            ? string.Join(", ", Columns)
            : "Nao disponivel";

        var sampleRows = GetSampleDataDescription();

        return $@"Voce e um assistente de analise de dados para uma tabela/grid.
Responda sempre em JSON com a seguinte estrutura:

Para consultas de filtro/ordenacao/agrupamento:
{{
  ""type"": ""query"",
  ""naturalLanguageQuery"": ""<query original>"",
  ""generatedSql"": ""<expressao SQL opcional>"",
  ""filters"": [{{""column"": ""<coluna>"", ""operator"": ""<operador>"", ""value"": ""<valor>""}}],
  ""sorts"": [{{""column"": ""<coluna>"", ""descending"": false}}],
  ""groupBy"": ""<coluna ou null>"",
  ""summary"": ""<resumo da interpretacao>"",
  ""confidence"": 0.0
}}

Para insights/análises:
{{
  ""type"": ""insights"",
  ""insights"": [
    {{""title"": ""<titulo>"", ""description"": ""<descricao>"", ""type"": ""<Pattern|Anomaly|Kpi|Trend|Summary|Recommendation>"", ""confidence"": 0.0, ""actionLabel"": ""<label opcional>""}}
  ]
}}

Colunas disponiveis: {columns}
Dados de exemplo:
{sampleRows}

Responda APENAS com JSON valido, sem texto adicional.";
    }

    private string BuildUserMessage(string query)
    {
        return $"Analise os dados da tabela e responda a seguinte pergunta: {query}";
    }

    private string GetSampleDataDescription()
    {
        if (Data is null) return "Nenhum dado disponivel.";

        var sample = Data.Take(3).ToList();
        if (sample.Count == 0) return "Nenhum dado disponivel.";

        var lines = new List<string>();
        foreach (var item in sample)
        {
            var props = item.GetType().GetProperties()
                .Take(10)
                .Select(p => $"{p.Name}={p.GetValue(item)}");
            lines.Add(string.Join(", ", props));
        }

        return string.Join("\n", lines.Select(l => $"- {l}"));
    }

    private void ParseResponse(string originalQuery, string content)
    {
        try
        {
            // Try to parse as JSON
            var trimmed = content.Trim();
            if (trimmed.StartsWith("```"))
            {
                // Strip markdown code fences
                var firstNewLine = trimmed.IndexOf('\n');
                if (firstNewLine > 0)
                {
                    trimmed = trimmed[(firstNewLine + 1)..];
                }
                if (trimmed.EndsWith("```"))
                {
                    trimmed = trimmed[..^3];
                }
                trimmed = trimmed.Trim();
            }

            using var doc = System.Text.Json.JsonDocument.Parse(trimmed);
            var root = doc.RootElement;

            var type = root.TryGetProperty("type", out var typeEl) ? typeEl.GetString() : null;

            if (type == "query" || root.TryGetProperty("filters", out _))
            {
                ParseQueryResponse(originalQuery, root);
            }
            else if (type == "insights" || root.TryGetProperty("insights", out _))
            {
                ParseInsightsResponse(root);
            }
            else
            {
                // Fallback: treat as summary insight
                _insights.Add(new XFAiGridInsight
                {
                    Title = "Resposta da IA",
                    Description = content.Length > 500 ? content[..500] : content,
                    Type = XFAiGridInsightType.Summary,
                    Confidence = 0.5
                });
            }
        }
        catch (System.Text.Json.JsonException)
        {
            // Not JSON — treat as text summary
            _insights.Add(new XFAiGridInsight
            {
                Title = "Resposta da IA",
                Description = content.Length > 500 ? content[..500] : content,
                Type = XFAiGridInsightType.Summary,
                Confidence = 0.5
            });
        }
    }

    private async void ParseQueryResponse(string originalQuery, System.Text.Json.JsonElement root)
    {
        var query = new XFAiGridQuery
        {
            NaturalLanguageQuery = originalQuery,
            Summary = root.TryGetProperty("summary", out var sumEl) ? sumEl.GetString() : null,
            GeneratedSql = root.TryGetProperty("generatedSql", out var sqlEl) ? sqlEl.GetString() : null,
            GroupBy = root.TryGetProperty("groupBy", out var grpEl) ? grpEl.GetString() : null,
            Confidence = root.TryGetProperty("conf", out var confEl) && confEl.ValueKind == System.Text.Json.JsonValueKind.Number
                ? confEl.GetDouble()
                : 0.7
        };

        // Parse filters
        if (root.TryGetProperty("filters", out var filtersEl) && filtersEl.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            var filters = new List<XFAiGridFilter>();
            foreach (var f in filtersEl.EnumerateArray())
            {
                filters.Add(new XFAiGridFilter
                {
                    Column = f.TryGetProperty("column", out var colEl) ? colEl.GetString() ?? "" : "",
                    Operator = f.TryGetProperty("operator", out var opEl) ? opEl.GetString() ?? "" : "",
                    Value = f.TryGetProperty("value", out var valEl) ? valEl.GetString() : null
                });
            }
            query.Filters = filters;
        }

        // Parse sorts
        if (root.TryGetProperty("sorts", out var sortsEl) && sortsEl.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            var sorts = new List<XFAiGridSort>();
            foreach (var s in sortsEl.EnumerateArray())
            {
                sorts.Add(new XFAiGridSort
                {
                    Column = s.TryGetProperty("column", out var colEl) ? colEl.GetString() ?? "" : "",
                    Descending = s.TryGetProperty("descending", out var descEl) && descEl.GetBoolean()
                });
            }
            query.Sorts = sorts;
        }

        _currentQuery = query;

        if (OnQueryGenerated.HasDelegate)
        {
            await OnQueryGenerated.InvokeAsync(query);
        }
    }

    private async void ParseInsightsResponse(System.Text.Json.JsonElement root)
    {
        if (root.TryGetProperty("insights", out var insightsEl) && insightsEl.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var item in insightsEl.EnumerateArray())
            {
                var typeStr = item.TryGetProperty("type", out var typeEl) ? typeEl.GetString() : "Summary";
                var insightType = Enum.TryParse<XFAiGridInsightType>(typeStr, true, out var parsed) ? parsed : XFAiGridInsightType.Summary;

                var insight = new XFAiGridInsight
                {
                    Title = item.TryGetProperty("title", out var titleEl) ? titleEl.GetString() ?? "" : "",
                    Description = item.TryGetProperty("description", out var descEl) ? descEl.GetString() ?? "" : "",
                    Type = insightType,
                    Confidence = item.TryGetProperty("confidence", out var confEl) && confEl.ValueKind == System.Text.Json.JsonValueKind.Number
                        ? confEl.GetDouble()
                        : null,
                    ActionLabel = item.TryGetProperty("actionLabel", out var actEl) ? actEl.GetString() : null
                };

                _insights.Add(insight);

                if (OnInsightFound.HasDelegate)
                {
                    await OnInsightFound.InvokeAsync(insight);
                }
            }
        }
    }

    private async Task HandleErrorAsync(Exception ex)
    {
        _isBusy = false;
        _errorText = ex.Message;

        if (OnError.HasDelegate)
        {
            await OnError.InvokeAsync(ex.Message);
        }

        StateHasChanged();
    }

    private void DismissError()
    {
        _errorText = null;
        StateHasChanged();
    }

    // ── Dispose ─────────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }
}