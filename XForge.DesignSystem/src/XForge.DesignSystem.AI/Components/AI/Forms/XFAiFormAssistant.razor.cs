using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Forms;

public partial class XFAiFormAssistant : XForgeComponentBase, IDisposable
{
    private const string DefaultSystemPrompt = """
        Voce e um assistente inteligente para formularios de ERP brasileiro.
        Dada uma descricao ou dados de formulario, retorne um JSON com:
        aiSummary, confidence (0.0-1.0),
        autofillSuggestions (array de {fieldName, fieldLabel, suggestedValue, source: receita-federal|cnpj|cep|history, confidence}),
        validationFixes (array de {fieldName, currentValue, suggestedFix, validationRule, explanation}),
        inconsistencies (array de {field1, field2, description, severity: low|medium|high|critical, suggestion}).
        Integre com Receita Federal, CEP, CNPJ e IE quando aplicavel.
        Retorne APENAS o JSON, sem texto adicional.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true };

    [Parameter] public IAiProvider? Provider { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Pergunte sobre preenchimento, validacao ou correcao de formulario...";
    [Parameter] public string Title { get; set; } = "Assistente de Formularios";
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool Loading { get; set; }
    [Parameter] public string? SystemPrompt { get; set; }
    [Parameter] public EventCallback<XFAiFormInsight> OnAnalysisComplete { get; set; }
    [Parameter] public EventCallback<string> OnError { get; set; }
    [Parameter] public RenderFragment<XFAiFormInsight>? ResultPreview { get; set; }
    [Parameter] public RenderFragment? HeaderContent { get; set; }
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    private string? _query;
    private bool _preventEnterDefault;
    private XFAiFormInsight? _result;
    private string? _errorText;
    private bool _isAnalyzing;
    private CancellationTokenSource? _cts;

    private bool IsBusy => _isAnalyzing || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";
    private bool CanSend => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(_query) && Provider is not null;

    private string RootClass => Join("xf-ai-form-assistant flex flex-col overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]", Class);
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
            var result = JsonSerializer.Deserialize<XFAiFormInsight>(jsonContent, JsonOptions);
            if (result is not null) { result.NaturalLanguageQuery = _query.Trim(); _result = result; if (OnAnalysisComplete.HasDelegate) await OnAnalysisComplete.InvokeAsync(result); }
            else { _errorText = "Nao foi possivel analisar o formulario. Tente novamente."; if (OnError.HasDelegate) await OnError.InvokeAsync(_errorText); }
        }
        catch (OperationCanceledException) { }
        catch (JsonException) { _errorText = "A resposta da IA nao esta no formato esperado."; if (OnError.HasDelegate) await OnError.InvokeAsync(_errorText); }
        catch (Exception ex) { _errorText = ex.Message; if (OnError.HasDelegate) await OnError.InvokeAsync(ex.Message); }
        finally { _isAnalyzing = false; _cts?.Dispose(); _cts = null; StateHasChanged(); }
    }

    public void Clear() { _result = null; _errorText = null; _query = string.Empty; StateHasChanged(); }
    private static string ExtractJson(string content) { if (string.IsNullOrWhiteSpace(content)) return content; var t = content.Trim(); if (t.StartsWith("```")) { var n = t.IndexOf('\n'); if (n > 0) t = t[(n + 1)..]; if (t.EndsWith("```")) t = t[..^3]; t = t.Trim(); } var s = t.IndexOf('{'); var e = t.LastIndexOf('}'); if (s >= 0 && e > s) t = t[s..(e + 1)]; return t; }
    private static string Join(params string?[] values) => string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    public void Dispose() { _cts?.Dispose(); GC.SuppressFinalize(this); }
}
