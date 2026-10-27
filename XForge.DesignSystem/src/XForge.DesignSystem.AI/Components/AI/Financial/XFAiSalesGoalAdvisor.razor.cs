using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Financial;

public partial class XFAiSalesGoalAdvisor : XForgeComponentBase, IDisposable
{
    private const string DefaultSystemPrompt = """
        Voce e um assistente especializado em gestao de metas de vendas para ERP brasileiro.
        Dada uma pergunta sobre metas, vendedores ou comissoes, retorne um JSON com:
        aiSummary, confidence (0.0-1.0),
        rankings (array de {position, salespersonName, totalSales, goalAmount, achievementPercentage, commission, badge: gold|silver|bronze}),
        goals (array de {goalName, targetAmount, currentAmount, progressPercentage, daysRemaining, predictedOutcome: will-achieve|at-risk|unlikely, predictedFinalAmount}),
        recommendations (array de {title, description, type: action|insight|warning, productMix, estimatedImpact}),
        performanceSummary (totalRevenue, averageTicket, totalTransactions, growthRate, bestPerformingRegion, bestPerformingProduct).
        Retorne APENAS o JSON, sem texto adicional.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true };

    [Parameter] public IAiProvider? Provider { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Pergunte sobre ranking, metas, comissoes ou previsoes...";
    [Parameter] public string Title { get; set; } = "Assistente de Metas";
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool Loading { get; set; }
    [Parameter] public string? SystemPrompt { get; set; }
    [Parameter] public EventCallback<XFAiSalesGoalInsight> OnAnalysisComplete { get; set; }
    [Parameter] public EventCallback<string> OnError { get; set; }
    [Parameter] public RenderFragment<XFAiSalesGoalInsight>? ResultPreview { get; set; }
    [Parameter] public RenderFragment? HeaderContent { get; set; }
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    private string? _query;
    private bool _preventEnterDefault;
    private XFAiSalesGoalInsight? _result;
    private string? _errorText;
    private bool _isAnalyzing;
    private CancellationTokenSource? _cts;

    private bool IsBusy => _isAnalyzing || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";
    private bool CanSend => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(_query) && Provider is not null;

    private string RootClass => Join("xf-ai-sales-goal-advisor flex flex-col overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]", Class);
    private string HeaderClass => "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";
    private string HeaderTitleClass => "inline-flex items-center gap-2 text-sm font-semibold text-text-primary";
    private string InputContainerClass => "border-b border-border-light p-4";
    private string TextareaClass => "w-full resize-none rounded-lg border border-surface-200 bg-bg-surface px-3 py-2.5 text-sm leading-6 text-text-primary outline-none transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";
    private string InputActionsClass => "mt-2.5 flex flex-wrap items-center justify-between gap-3";
    private string SendButtonClass => Join("inline-flex h-9 min-w-9 items-center gap-1.5 rounded-lg bg-primary px-3.5 text-sm font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50", IsBusy ? "animate-pulse" : "");
    private string ErrorBannerClass => "flex items-center gap-2 border-t border-danger-200 bg-danger-50 px-4 py-2 text-xs text-danger";
    private string LoadingContainerClass => "animate-pulse px-4 py-5";
    private string SkeletonLineClass => "h-3 rounded bg-surface-200";
    private string PreviewContainerClass => "px-4 py-4";
    private string SummaryBoxClass => "mb-4 rounded-lg bg-primary/5 px-3 py-2.5 ring-1 ring-primary/10";
    private string ConfidenceTrackClass => "h-1.5 w-24 overflow-hidden rounded-full bg-surface-200";
    private string ConfidenceFillClass => _result?.Confidence switch { >= 0.8 => "h-full rounded-full transition-all duration-300 bg-emerald-500", >= 0.5 => "h-full rounded-full transition-all duration-300 bg-amber-500", _ => "h-full rounded-full transition-all duration-300 bg-red-500" };
    private string EmptyContainerClass => "grid min-h-48 flex-1 place-items-center px-6 py-8";
    private string EmptyInnerClass => "flex flex-col items-center gap-3 text-center";
    private string EmptyIconClass => "inline-flex h-12 w-12 items-center justify-center rounded-full bg-surface-100 text-text-muted";
    private string EmptyTextClass => "max-w-xs text-sm text-text-secondary";

    private static string GetBadgeClass(string badge) => badge?.ToLowerInvariant() switch
    {
        "gold" => "inline-flex items-center rounded-full bg-amber-100 px-1.5 py-0.5 text-[10px] font-semibold text-amber-800",
        "silver" => "inline-flex items-center rounded-full bg-slate-100 px-1.5 py-0.5 text-[10px] font-semibold text-slate-800",
        "bronze" => "inline-flex items-center rounded-full bg-orange-100 px-1.5 py-0.5 text-[10px] font-semibold text-orange-800",
        _ => ""
    };

    private static string GetAchievementClass(double pct) => pct switch
    {
        >= 100 => "font-semibold text-emerald-600",
        >= 70 => "font-semibold text-amber-600",
        _ => "font-semibold text-red-600"
    };

    private static string GetOutcomeClass(string outcome) => outcome?.ToLowerInvariant() switch
    {
        "will-achieve" => "text-emerald-600 font-medium",
        "at-risk" => "text-amber-600 font-medium",
        "unlikely" => "text-red-600 font-medium",
        _ => "text-text-muted"
    };

    private static string GetOutcomeLabel(string outcome) => outcome?.ToLowerInvariant() switch
    {
        "will-achieve" => "Alcancara",
        "at-risk" => "Em risco",
        "unlikely" => "Dificil",
        _ => ""
    };

    private static string GetProgressFillClass(double pct) => pct switch
    {
        >= 100 => "h-full rounded-full bg-emerald-500 transition-all duration-300",
        >= 70 => "h-full rounded-full bg-amber-500 transition-all duration-300",
        _ => "h-full rounded-full bg-red-500 transition-all duration-300"
    };

    private void HandleInput(ChangeEventArgs args) { _query = args.Value?.ToString(); StateHasChanged(); }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && !IsBusy) { _query = string.Empty; _errorText = null; StateHasChanged(); return; }
        _preventEnterDefault = args.Key == "Enter" && !args.ShiftKey;
        if (_preventEnterDefault && CanSend) await AnalyzeAsync();
    }

    private void DismissError() { _errorText = null; StateHasChanged(); }

    public async Task AnalyzeAsync()
    {
        if (!CanSend) return;
        _errorText = null; _result = null; _isAnalyzing = true; StateHasChanged();
        _cts?.Dispose(); _cts = new CancellationTokenSource();
        try
        {
            var request = new AiChatRequest { Messages = new[] { new AiMessage { Role = AiRole.User, Content = _query!.Trim(), Timestamp = DateTimeOffset.UtcNow } }, SystemPrompt = SystemPrompt ?? DefaultSystemPrompt, Temperature = 0.3 };
            var response = await Provider!.CompleteAsync(request, _cts.Token);
            var jsonContent = ExtractJson(response.Message.Content);
            var result = JsonSerializer.Deserialize<XFAiSalesGoalInsight>(jsonContent, JsonOptions);
            if (result is not null) { result.NaturalLanguageQuery = _query.Trim(); _result = result; if (OnAnalysisComplete.HasDelegate) await OnAnalysisComplete.InvokeAsync(result); }
            else { _errorText = "Nao foi possivel analisar as metas. Tente novamente."; if (OnError.HasDelegate) await OnError.InvokeAsync(_errorText); }
        }
        catch (OperationCanceledException) { }
        catch (JsonException) { _errorText = "A resposta da IA nao esta no formato esperado."; if (OnError.HasDelegate) await OnError.InvokeAsync(_errorText); }
        catch (Exception ex) { _errorText = ex.Message; if (OnError.HasDelegate) await OnError.InvokeAsync(ex.Message); }
        finally { _isAnalyzing = false; _cts?.Dispose(); _cts = null; StateHasChanged(); }
    }

    public void Clear() { _result = null; _errorText = null; _query = string.Empty; StateHasChanged(); }

    private static string ExtractJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return content;
        var trimmed = content.Trim();
        if (trimmed.StartsWith("```")) { var n = trimmed.IndexOf('\n'); if (n > 0) trimmed = trimmed[(n + 1)..]; if (trimmed.EndsWith("```")) trimmed = trimmed[..^3]; trimmed = trimmed.Trim(); }
        var s = trimmed.IndexOf('{'); var e = trimmed.LastIndexOf('}');
        if (s >= 0 && e > s) trimmed = trimmed[s..(e + 1)];
        return trimmed;
    }

    private static string Join(params string?[] values) => string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));

    public void Dispose() { _cts?.Dispose(); GC.SuppressFinalize(this); }
}
