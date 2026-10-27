using System.Text.Json;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Enterprise AI explain button component. Provides contextual AI explanations
/// for fields, reports, errors, metrics, and tax items in grids, forms, dashboards.
/// </summary>
public partial class XFAiExplainButton : XForgeComponentBase, IDisposable
{
    private const string DefaultSystemPrompt = """
        Voce e um assistente especializado em explicar conceitos, campos, erros e metricas de um ERP brasileiro.
        Dado o contexto de um elemento, retorne um JSON com:
        elementName, explanation (string detalhada e didatica),
        details (array de {label, value, icon}),
        confidence (0.0-1.0), sources (array de strings).
        Retorne APENAS o JSON, sem texto adicional.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>AI provider for generating explanations.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>The context or element name to explain.</summary>
    [Parameter]
    public string ElementContext { get; set; } = string.Empty;

    /// <summary>Button label text.</summary>
    [Parameter]
    public string ButtonLabel { get; set; } = "Explicar";

    /// <summary>Panel title text.</summary>
    [Parameter]
    public string Title { get; set; } = "Explicacao";

    /// <summary>Whether to show the text label on the button.</summary>
    [Parameter]
    public bool ShowLabel { get; set; } = true;

    /// <summary>Aria label for accessibility.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Explicar com IA";

    /// <summary>Custom system prompt override.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    /// <summary>External loading state.</summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>Variant for button styling.</summary>
    [Parameter]
    public XFAiExplainVariant Variant { get; set; } = XFAiExplainVariant.Icon;

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when explanation completes.</summary>
    [Parameter]
    public EventCallback<XFAiExplainResult> OnExplained { get; set; }

    /// <summary>Fired when an error occurs.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom explanation template.</summary>
    [Parameter]
    public RenderFragment<XFAiExplainResult>? ExplainTemplate { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private bool _isExpanded;
    private bool _isLoading;
    private XFAiExplainResult? _result;
    private string? _errorText;
    private CancellationTokenSource? _cts;

    private bool IsBusy => _isLoading || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";
    private bool CanExplain => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(ElementContext) && Provider is not null;

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        $"xf-ai-explain-button {(_isExpanded ? "block" : "inline-flex")}",
        Class).Trim();

    private string ButtonClass => Variant switch
    {
        XFAiExplainVariant.Ghost => "inline-flex h-8 items-center gap-1.5 rounded-md px-2.5 text-xs font-medium text-text-secondary outline-none transition-all duration-150 hover:bg-surface-100 hover:text-primary focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50",
        XFAiExplainVariant.Solid => "inline-flex h-8 items-center gap-1.5 rounded-md bg-primary/10 px-2.5 text-xs font-medium text-primary outline-none transition-all duration-150 hover:bg-primary/20 focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50",
        _ => "inline-flex h-7 w-7 items-center justify-center rounded-md text-text-muted outline-none transition-all duration-150 hover:bg-surface-100 hover:text-primary focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50"
    };

    private string PanelClass =>
        "overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06)]";

    private string PanelHeaderClass =>
        "flex items-center justify-between gap-2 border-b border-border-light px-3 py-2.5";

    private string CloseButtonClass =>
        "inline-flex h-6 w-6 items-center justify-center rounded text-text-muted outline-none transition-colors hover:bg-surface-100 hover:text-text-secondary focus-visible:ring-2 focus-visible:ring-border-focus";

    private string ContentClass => "px-3 py-3";

    private string LoadingClass => "animate-pulse px-3 py-4";

    private string ErrorClass =>
        "flex items-center gap-2 px-3 py-2 text-xs text-danger";

    private string ConfidenceTrackClass =>
        "h-1.5 w-20 overflow-hidden rounded-full bg-surface-200";

    private string ConfidenceFillClass => _result?.Confidence switch
    {
        >= 0.8 => "h-full rounded-full transition-all duration-300 bg-emerald-500",
        >= 0.5 => "h-full rounded-full transition-all duration-300 bg-amber-500",
        _ => "h-full rounded-full transition-all duration-300 bg-red-500"
    };

    // ── Events ──────────────────────────────────────────────────────────

    private async Task ExplainAsync()
    {
        if (!CanExplain) return;

        _isExpanded = true;
        _isLoading = true;
        _errorText = null;
        _result = null;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            var prompt = $"Explique o seguinte elemento do sistema: {ElementContext}";
            var request = new AiChatRequest
            {
                Messages = new[]
                {
                    new AiMessage { Role = AiRole.User, Content = prompt }
                },
                SystemPrompt = SystemPrompt ?? DefaultSystemPrompt,
                MaxTokens = 1024
            };

            var response = await Provider!.CompleteAsync(request, _cts.Token);

            if (_cts.IsCancellationRequested) return;

            var responseContent = response.Message?.Content ?? string.Empty;
            var json = ExtractJson(responseContent);
            if (!string.IsNullOrWhiteSpace(json))
            {
                _result = JsonSerializer.Deserialize<XFAiExplainResult>(json, JsonOptions);
            }
            else
            {
                _result = new XFAiExplainResult
                {
                    ElementName = ElementContext,
                    Explanation = responseContent,
                    Confidence = 0.5
                };
            }

            if (OnExplained.HasDelegate)
                await OnExplained.InvokeAsync(_result);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _errorText = $"Erro ao gerar explicacao: {ex.Message}";
            if (OnError.HasDelegate)
                await OnError.InvokeAsync(_errorText);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private Task CollapseAsync()
    {
        _isExpanded = false;
        _result = null;
        _errorText = null;
        return Task.CompletedTask;
    }

    private static string? ExtractJson(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');
        if (start >= 0 && end > start)
            return content[start..(end + 1)];
        return null;
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}

/// <summary>
/// Visual variants for the explain button.
/// </summary>
public enum XFAiExplainVariant
{
    /// <summary>Icon-only button (default).</summary>
    Icon,

    /// <summary>Ghost button with label.</summary>
    Ghost,

    /// <summary>Solid tinted button with label.</summary>
    Solid
}
