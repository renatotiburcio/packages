using System.Text.Json;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Enterprise AI rewrite button component. Provides intelligent text rewriting
/// with multiple modes: formalize, summarize, expand, fix grammar, adjust tone, translate.
/// </summary>
public partial class XFAiRewriteButton : XForgeComponentBase, IDisposable
{
    private const string DefaultSystemPrompt = """
        Voce e um assistente especializado em reescrita de textos para um ERP brasileiro.
        Dado o texto original e o modo de reescrita, retorne um JSON com:
        originalText, rewrittenText, mode (Formalize|Summarize|Expand|FixGrammar|AdjustTone|Translate),
        confidence (0.0-1.0), changeDescription (breve descricao do que mudou).
        Retorne APENAS o JSON, sem texto adicional.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>AI provider for text rewriting.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>The text to rewrite.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Available rewrite modes to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiRewriteMode> AvailableModes { get; set; } = new[]
    {
        XFAiRewriteMode.Formalize,
        XFAiRewriteMode.Summarize,
        XFAiRewriteMode.Expand,
        XFAiRewriteMode.FixGrammar
    };

    /// <summary>Panel title.</summary>
    [Parameter]
    public string Title { get; set; } = "Reescrita IA";

    /// <summary>Target language for Translate mode.</summary>
    [Parameter]
    public string TargetLanguage { get; set; } = "ingles";

    /// <summary>Custom system prompt override.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    /// <summary>External loading state.</summary>
    [Parameter]
    public bool Loading { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when rewrite completes.</summary>
    [Parameter]
    public EventCallback<XFAiRewriteResult> OnRewritten { get; set; }

    /// <summary>Fired when user clicks Apply.</summary>
    [Parameter]
    public EventCallback<string> OnApply { get; set; }

    /// <summary>Fired when an error occurs.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom rewrite result template.</summary>
    [Parameter]
    public RenderFragment<XFAiRewriteResult>? RewriteTemplate { get; set; }

    /// <summary>Content to render above the mode buttons.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private bool _isExpanded;
    private bool _isLoading;
    private XFAiRewriteResult? _result;
    private string? _errorText;
    private CancellationTokenSource? _cts;

    private bool IsBusy => _isLoading || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";
    private bool CanRewrite => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(Text) && Provider is not null;

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ", "xf-ai-rewrite-button", Class).Trim();

    private string TriggerClass => "space-y-2";

    private string ModeBarClass => "flex flex-wrap gap-1.5";

    private string ModeButtonClass(XFAiRewriteMode mode) =>
        "inline-flex h-7 items-center gap-1.5 rounded-md bg-surface-50 px-2.5 text-xs font-medium text-text-secondary outline-none transition-all duration-150 ring-1 ring-border-light hover:bg-primary/10 hover:text-primary hover:ring-primary/30 focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50";

    private string PanelClass =>
        "overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06)]";

    private string PanelHeaderClass =>
        "flex items-center justify-between gap-2 border-b border-border-light px-3 py-2.5";

    private string ModeBadgeClass =>
        "inline-flex items-center rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-semibold text-primary";

    private string ApplyButtonClass =>
        "inline-flex h-7 items-center gap-1 rounded-md bg-emerald-500 px-2.5 text-xs font-semibold text-white outline-none transition-all hover:bg-emerald-600 focus-visible:ring-2 focus-visible:ring-border-focus";

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

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetModeLabel(XFAiRewriteMode mode) => mode switch
    {
        XFAiRewriteMode.Formalize => "Formalizar",
        XFAiRewriteMode.Summarize => "Resumir",
        XFAiRewriteMode.Expand => "Expandir",
        XFAiRewriteMode.FixGrammar => "Corrigir",
        XFAiRewriteMode.AdjustTone => "Tom",
        XFAiRewriteMode.Translate => "Traduzir",
        _ => mode.ToString()
    };

    private static string GetModeIcon(XFAiRewriteMode mode) => mode switch
    {
        XFAiRewriteMode.Formalize => "briefcase",
        XFAiRewriteMode.Summarize => "compress",
        XFAiRewriteMode.Expand => "expand",
        XFAiRewriteMode.FixGrammar => "spell-check",
        XFAiRewriteMode.AdjustTone => "sliders",
        XFAiRewriteMode.Translate => "language",
        _ => "wand-magic-sparkles"
    };

    // ── Events ──────────────────────────────────────────────────────────

    private async Task RewriteAsync(XFAiRewriteMode mode)
    {
        if (!CanRewrite) return;

        _isExpanded = true;
        _isLoading = true;
        _errorText = null;
        _result = null;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            var modeInstruction = mode switch
            {
                XFAiRewriteMode.Formalize => "Reescreva o texto de forma mais formal e profissional.",
                XFAiRewriteMode.Summarize => "Resuma o texto mantendo as informacoes principais.",
                XFAiRewriteMode.Expand => "Expanda o texto com mais detalhes e contexto.",
                XFAiRewriteMode.FixGrammar => "Corrija erros gramaticais e de ortografia.",
                XFAiRewriteMode.AdjustTone => "Ajuste o tom do texto para ser mais amigavel e profissional.",
                XFAiRewriteMode.Translate => $"Traduza o texto para {TargetLanguage}.",
                _ => "Reescreva o texto."
            };

            var prompt = $"{modeInstruction}\n\nTexto original:\n{Text}";
            var request = new AiChatRequest
            {
                Messages = new[] { new AiMessage { Role = AiRole.User, Content = prompt } },
                SystemPrompt = SystemPrompt ?? DefaultSystemPrompt,
                MaxTokens = 2048
            };

            var response = await Provider!.CompleteAsync(request, _cts.Token);

            if (_cts.IsCancellationRequested) return;

            var responseContent = response.Message?.Content ?? string.Empty;
            var json = ExtractJson(responseContent);
            if (!string.IsNullOrWhiteSpace(json))
            {
                _result = JsonSerializer.Deserialize<XFAiRewriteResult>(json, JsonOptions);
            }
            else
            {
                _result = new XFAiRewriteResult
                {
                    OriginalText = Text,
                    RewrittenText = responseContent,
                    Mode = mode,
                    Confidence = 0.5
                };
            }

            if (OnRewritten.HasDelegate)
                await OnRewritten.InvokeAsync(_result);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _errorText = $"Erro ao reescrever: {ex.Message}";
            if (OnError.HasDelegate)
                await OnError.InvokeAsync(_errorText);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task ApplyAsync()
    {
        if (_result is not null && OnApply.HasDelegate)
        {
            await OnApply.InvokeAsync(_result.RewrittenText);
        }
        _isExpanded = false;
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
