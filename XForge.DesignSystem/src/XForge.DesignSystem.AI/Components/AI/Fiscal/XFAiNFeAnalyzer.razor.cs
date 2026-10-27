using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Fiscal;

/// <summary>
/// Enterprise AI NF-e analyzer component. Parses NF-e XML content, validates CFOP/CST/NCM codes,
/// detects fiscal inconsistencies, assesses SPED compliance, and evaluates CBS/IBS readiness.
/// </summary>
public partial class XFAiNFeAnalyzer : XForgeComponentBase, IDisposable
{
    private const string DefaultSystemPrompt = """
        Voce e um assistente especializado em analise fiscal de NF-e (Nota Fiscal Eletronica).
        Dado o conteudo XML ou descricao de uma NF-e, analise e retorne um JSON com:
        nFeKey, issuerCnpj, issuerName, recipientCnpj, recipientName, invoiceNumber, series,
        issueDate, totalValue, icmsTotal, ipiTotal, pisTotal, cofinsTotal,
        items (array de {sequence, description, ncm, cfop, cst, quantity, unitValue, totalValue, icmsValue, ipiValue, itemClassification}),
        taxSummary (array de {cfop, description, itemCount, totalValue, totalTax, effectiveRate}),
        inconsistencies (array de {severity, code, message, field, suggestion}),
        aiSummary, spedStatus (NotValidated|Valid|WithWarnings|Invalid),
        cbsIbsReadiness, riskLevel (Low|Medium|High|Critical), confidence (0.0-1.0).
        Valide CFOPs conhecidos, CSTs validos, NCMs na tabela. Detecte possiveis problemas fiscais.
        Retorne APENAS o JSON, sem texto adicional.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    // -- Parameters ------------------------------------------------------

    /// <summary>AI provider for fiscal analysis.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>Input placeholder text.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Cole o XML da NF-e ou descreva o que deseja analisar...";

    /// <summary>Header title.</summary>
    [Parameter]
    public string Title { get; set; } = "Analisador de NF-e";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>External loading state.</summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>Custom system prompt override for the AI provider.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    // -- Events ----------------------------------------------------------

    /// <summary>Fired when analysis completes successfully.</summary>
    [Parameter]
    public EventCallback<XFAiNFeAnalysisResult> OnAnalysisComplete { get; set; }

    /// <summary>Fired when an error occurs during analysis.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    // -- RenderFragments -------------------------------------------------

    /// <summary>Custom result preview template.</summary>
    [Parameter]
    public RenderFragment<XFAiNFeAnalysisResult>? ResultPreview { get; set; }

    /// <summary>Custom header content (replaces default header).</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // -- Internal State --------------------------------------------------

    private string? _query;
    private bool _preventEnterDefault;
    private XFAiNFeAnalysisResult? _result;
    private string? _errorText;
    private bool _isAnalyzing;
    private CancellationTokenSource? _cts;

    private bool IsBusy => _isAnalyzing || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";
    private bool CanSend => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(_query) && Provider is not null;

    // -- CSS Classes -----------------------------------------------------

    private string RootClass => Join(
        "xf-ai-nfe-analyzer flex flex-col overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string HeaderTitleClass =>
        "inline-flex items-center gap-2 text-sm font-semibold text-text-primary";

    private string InputContainerClass =>
        "border-b border-border-light p-4";

    private string TextareaClass =>
        "w-full resize-none rounded-lg border border-surface-200 bg-bg-surface px-3 py-2.5 text-sm leading-6 text-text-primary outline-none transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";

    private string InputActionsClass =>
        "mt-2.5 flex flex-wrap items-center justify-between gap-3";

    private string SendButtonClass =>
        Join(
            "inline-flex h-9 min-w-9 items-center gap-1.5 rounded-lg bg-primary px-3.5 text-sm font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50",
            IsBusy ? "animate-pulse" : "");

    private string ErrorBannerClass =>
        "flex items-center gap-2 border-t border-danger-200 bg-danger-50 px-4 py-2 text-xs text-danger";

    private string LoadingContainerClass =>
        "animate-pulse px-4 py-5";

    private string SkeletonLineClass =>
        "h-3 rounded bg-surface-200";

    private string SkeletonChipClass =>
        "h-6 rounded-full bg-surface-200";

    private string PreviewContainerClass =>
        "px-4 py-4";

    private string SummaryBoxClass =>
        "mb-4 rounded-lg bg-primary/5 px-3 py-2.5 ring-1 ring-primary/10";

    private string ConfidenceTrackClass =>
        "h-1.5 w-24 overflow-hidden rounded-full bg-surface-200";

    private string ConfidenceFillClass
    {
        get
        {
            var color = _result?.Confidence switch
            {
                >= 0.8 => "bg-emerald-500",
                >= 0.5 => "bg-amber-500",
                _ => "bg-red-500"
            };
            return $"h-full rounded-full transition-all duration-300 {color}";
        }
    }

    private string EmptyContainerClass =>
        "grid min-h-48 flex-1 place-items-center px-6 py-8";

    private string EmptyInnerClass =>
        "flex flex-col items-center gap-3 text-center";

    private string EmptyIconClass =>
        "inline-flex h-12 w-12 items-center justify-center rounded-full bg-surface-100 text-text-muted";

    private string EmptyTextClass =>
        "max-w-xs text-sm text-text-secondary";

    // -- Risk/Sped Badge Classes ----------------------------------------

    private string RiskBadgeClass => _result?.RiskLevel switch
    {
        XFAiNFeRiskLevel.Critical => "inline-flex items-center gap-1 rounded-full bg-red-50 px-2.5 py-1 text-xs font-semibold text-red-700 ring-1 ring-red-200",
        XFAiNFeRiskLevel.High => "inline-flex items-center gap-1 rounded-full bg-orange-50 px-2.5 py-1 text-xs font-semibold text-orange-700 ring-1 ring-orange-200",
        XFAiNFeRiskLevel.Medium => "inline-flex items-center gap-1 rounded-full bg-amber-50 px-2.5 py-1 text-xs font-semibold text-amber-700 ring-1 ring-amber-200",
        _ => "inline-flex items-center gap-1 rounded-full bg-emerald-50 px-2.5 py-1 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-200"
    };

    private string SpedBadgeClass => _result?.SpedStatus switch
    {
        XFAiNFeSpedStatus.Valid => "inline-flex items-center gap-1 rounded-full bg-emerald-50 px-2.5 py-1 text-xs font-medium text-emerald-700 ring-1 ring-emerald-200",
        XFAiNFeSpedStatus.WithWarnings => "inline-flex items-center gap-1 rounded-full bg-amber-50 px-2.5 py-1 text-xs font-medium text-amber-700 ring-1 ring-amber-200",
        XFAiNFeSpedStatus.Invalid => "inline-flex items-center gap-1 rounded-full bg-red-50 px-2.5 py-1 text-xs font-medium text-red-700 ring-1 ring-red-200",
        _ => "inline-flex items-center gap-1 rounded-full bg-surface-100 px-2.5 py-1 text-xs font-medium text-text-muted ring-1 ring-border-light"
    };

    // -- Helper Methods --------------------------------------------------

    private static string GetRiskLabel(XFAiNFeRiskLevel level) => level switch
    {
        XFAiNFeRiskLevel.Critical => "Risco Critico",
        XFAiNFeRiskLevel.High => "Risco Alto",
        XFAiNFeRiskLevel.Medium => "Risco Medio",
        _ => "Risco Baixo"
    };

    private static string GetSpedLabel(XFAiNFeSpedStatus status) => status switch
    {
        XFAiNFeSpedStatus.Valid => "SPED Valido",
        XFAiNFeSpedStatus.WithWarnings => "SPED com Alertas",
        XFAiNFeSpedStatus.Invalid => "SPED Invalido",
        _ => "SPED Nao Validado"
    };

    private static string GetInconsistencyClass(XFAiNFeInconsistencySeverity severity) => severity switch
    {
        XFAiNFeInconsistencySeverity.Critical => "flex items-start gap-2 rounded-md bg-red-50 px-3 py-2 text-xs text-red-800 ring-1 ring-red-200",
        XFAiNFeInconsistencySeverity.Error => "flex items-start gap-2 rounded-md bg-orange-50 px-3 py-2 text-xs text-orange-800 ring-1 ring-orange-200",
        XFAiNFeInconsistencySeverity.Warning => "flex items-start gap-2 rounded-md bg-amber-50 px-3 py-2 text-xs text-amber-800 ring-1 ring-amber-200",
        _ => "flex items-start gap-2 rounded-md bg-blue-50 px-3 py-2 text-xs text-blue-800 ring-1 ring-blue-200"
    };

    private static string GetInconsistencyIcon(XFAiNFeInconsistencySeverity severity) => severity switch
    {
        XFAiNFeInconsistencySeverity.Critical => "triangle-exclamation",
        XFAiNFeInconsistencySeverity.Error => "circle-exclamation",
        XFAiNFeInconsistencySeverity.Warning => "exclamation",
        _ => "circle-info"
    };

    // -- User Interactions -----------------------------------------------

    private void HandleInput(ChangeEventArgs args)
    {
        _query = args.Value?.ToString();
        StateHasChanged();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && !IsBusy)
        {
            _query = string.Empty;
            _errorText = null;
            StateHasChanged();
            return;
        }

        _preventEnterDefault = args.Key == "Enter" && !args.ShiftKey;

        if (_preventEnterDefault && CanSend)
        {
            await AnalyzeAsync();
        }
    }

    private void DismissError()
    {
        _errorText = null;
        StateHasChanged();
    }

    // -- Analysis --------------------------------------------------------

    /// <summary>Analyzes the current NF-e content or query.</summary>
    public async Task AnalyzeAsync()
    {
        if (!CanSend)
            return;

        _errorText = null;
        _result = null;
        _isAnalyzing = true;
        StateHasChanged();

        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        try
        {
            var systemPrompt = SystemPrompt ?? DefaultSystemPrompt;

            var request = new AiChatRequest
            {
                Messages = new[]
                {
                    new AiMessage
                    {
                        Role = AiRole.User,
                        Content = _query!.Trim(),
                        Timestamp = DateTimeOffset.UtcNow
                    }
                },
                SystemPrompt = systemPrompt,
                Temperature = 0.2
            };

            var response = await Provider!.CompleteAsync(request, ct);
            var jsonContent = ExtractJson(response.Message.Content);

            var result = JsonSerializer.Deserialize<XFAiNFeAnalysisResult>(jsonContent, JsonOptions);

            if (result is not null)
            {
                result.NaturalLanguageQuery = _query.Trim();
                _result = result;

                if (OnAnalysisComplete.HasDelegate)
                {
                    await OnAnalysisComplete.InvokeAsync(result);
                }
            }
            else
            {
                _errorText = "Nao foi possivel analisar a NF-e. Tente reformular sua solicitacao.";
                if (OnError.HasDelegate)
                {
                    await OnError.InvokeAsync(_errorText);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // User cancelled
        }
        catch (JsonException)
        {
            _errorText = "A resposta da IA nao esta no formato esperado. Tente novamente.";
            if (OnError.HasDelegate)
            {
                await OnError.InvokeAsync(_errorText);
            }
        }
        catch (Exception ex)
        {
            _errorText = ex.Message;
            if (OnError.HasDelegate)
            {
                await OnError.InvokeAsync(ex.Message);
            }
        }
        finally
        {
            _isAnalyzing = false;
            _cts?.Dispose();
            _cts = null;
            StateHasChanged();
        }
    }

    /// <summary>Clears the current result and resets the component.</summary>
    public void Clear()
    {
        _result = null;
        _errorText = null;
        _query = string.Empty;
        StateHasChanged();
    }

    // -- Helpers ---------------------------------------------------------

    private static string ExtractJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return content;

        var trimmed = content.Trim();

        if (trimmed.StartsWith("```"))
        {
            var firstNewLine = trimmed.IndexOf('\n');
            if (firstNewLine > 0)
                trimmed = trimmed[(firstNewLine + 1)..];
            if (trimmed.EndsWith("```"))
                trimmed = trimmed[..^3];
            trimmed = trimmed.Trim();
        }

        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');

        if (start >= 0 && end > start)
            trimmed = trimmed[start..(end + 1)];

        return trimmed;
    }

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    // -- Dispose ---------------------------------------------------------

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
