using System.Text.Json;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Enterprise AI validation summary component. Displays validation items with
/// AI-powered diagnostics, explanations, and fix suggestions.
/// </summary>
public partial class XFAiValidationSummary : XForgeComponentBase, IDisposable
{
    private const string DefaultSystemPrompt = """
        Voce e um assistente especializado em validacao de dados de ERP brasileiro.
        Dado um array de itens de validacao, analise cada um e retorne um JSON com:
        items (array de {fieldName, message, aiExplanation, aiFixSuggestion, confidence}).
        Para cada item, explique o motivo do erro e sugira uma correcao pratica.
        Retorne APENAS o JSON, sem texto adicional.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Validation items to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiValidationItem> Items { get; set; } = Array.Empty<XFAiValidationItem>();

    /// <summary>Event callback when items are updated.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<XFAiValidationItem>> ItemsChanged { get; set; }

    /// <summary>AI provider for diagnostics.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>Panel title.</summary>
    [Parameter]
    public string Title { get; set; } = "Validacao";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show dismiss/resolve buttons.</summary>
    [Parameter]
    public bool ShowDismiss { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhuma validacao pendente.";

    /// <summary>Custom system prompt override.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    /// <summary>External loading state.</summary>
    [Parameter]
    public bool Loading { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when AI diagnostics complete.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<XFAiValidationItem>> OnDiagnosed { get; set; }

    /// <summary>Fired when an item is resolved.</summary>
    [Parameter]
    public EventCallback<XFAiValidationItem> OnResolved { get; set; }

    /// <summary>Fired when an error occurs.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private bool _isDiagnosing;
    private CancellationTokenSource? _cts;

    private bool IsBusy => _isDiagnosing || Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";

    private IEnumerable<XFAiValidationItem> SortedItems =>
        Items.OrderByDescending(i => i.Severity).ThenBy(i => i.FieldName);

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-validation-summary overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class).Trim();

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string CountBadgeClass =>
        "inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-danger/10 px-1.5 text-[10px] font-bold text-danger";

    private string DiagnoseButtonClass =>
        "inline-flex h-7 items-center gap-1.5 rounded-md bg-primary/10 px-2.5 text-xs font-medium text-primary outline-none transition-all duration-150 hover:bg-primary/20 focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50";

    private string ListClass => "divide-y divide-border-light";

    private string ItemClass(XFAiValidationItem item) => item.Severity switch
    {
        XFAiValidationSeverity.Critical => "border-l-[3px] border-l-red-500 px-4 py-3",
        XFAiValidationSeverity.Error => "border-l-[3px] border-l-orange-500 px-4 py-3",
        XFAiValidationSeverity.Warning => "border-l-[3px] border-l-amber-500 px-4 py-3",
        _ => "border-l-[3px] border-l-blue-400 px-4 py-3"
    };

    private string SeverityIconClass(XFAiValidationSeverity severity) => severity switch
    {
        XFAiValidationSeverity.Critical => "mt-0.5 inline-flex h-4 w-4 shrink-0 items-center justify-center text-red-500",
        XFAiValidationSeverity.Error => "mt-0.5 inline-flex h-4 w-4 shrink-0 items-center justify-center text-orange-500",
        XFAiValidationSeverity.Warning => "mt-0.5 inline-flex h-4 w-4 shrink-0 items-center justify-center text-amber-500",
        _ => "mt-0.5 inline-flex h-4 w-4 shrink-0 items-center justify-center text-blue-500"
    };

    private string SeverityBadgeClass(XFAiValidationSeverity severity) => severity switch
    {
        XFAiValidationSeverity.Critical => "inline-flex items-center rounded-full bg-red-50 px-1.5 py-0.5 text-[10px] font-semibold text-red-700 ring-1 ring-red-200",
        XFAiValidationSeverity.Error => "inline-flex items-center rounded-full bg-orange-50 px-1.5 py-0.5 text-[10px] font-semibold text-orange-700 ring-1 ring-orange-200",
        XFAiValidationSeverity.Warning => "inline-flex items-center rounded-full bg-amber-50 px-1.5 py-0.5 text-[10px] font-semibold text-amber-700 ring-1 ring-amber-200",
        _ => "inline-flex items-center rounded-full bg-blue-50 px-1.5 py-0.5 text-[10px] font-semibold text-blue-700 ring-1 ring-blue-200"
    };

    private string DismissButtonClass =>
        "mt-0.5 inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md text-text-muted outline-none transition-all duration-150 hover:bg-emerald-50 hover:text-emerald-600 focus-visible:ring-2 focus-visible:ring-border-focus";

    private string LoadingClass => "animate-pulse px-4 py-4";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

    private string EmptyTextClass => "text-sm text-text-secondary";

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetSeverityIcon(XFAiValidationSeverity severity) => severity switch
    {
        XFAiValidationSeverity.Critical => "circle-xmark",
        XFAiValidationSeverity.Error => "circle-exclamation",
        XFAiValidationSeverity.Warning => "triangle-exclamation",
        _ => "circle-info"
    };

    private static string GetSeverityLabel(XFAiValidationSeverity severity) => severity switch
    {
        XFAiValidationSeverity.Critical => "Critico",
        XFAiValidationSeverity.Error => "Erro",
        XFAiValidationSeverity.Warning => "Aviso",
        _ => "Info"
    };

    // ── Events ──────────────────────────────────────────────────────────

    private async Task DiagnoseAsync()
    {
        if (Provider is null || Items.Count == 0) return;

        _isDiagnosing = true;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            var itemsJson = JsonSerializer.Serialize(Items.Select(i => new
            {
                i.FieldName,
                i.Message,
                Severity = i.Severity.ToString(),
                i.TriggerValue
            }));

            var prompt = $"Analise os seguintes itens de validacao e forneça explicacoes e sugestoes de correcao:\n{itemsJson}";
            var request = new AiChatRequest
            {
                Messages = new[] { new AiMessage { Role = AiRole.User, Content = prompt } },
                SystemPrompt = SystemPrompt ?? DefaultSystemPrompt,
                MaxTokens = 2048
            };

            var response = await Provider.CompleteAsync(request, _cts.Token);

            if (_cts.IsCancellationRequested) return;

            var responseContent = response.Message?.Content ?? string.Empty;
            var json = ExtractJson(responseContent);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var diagResult = JsonSerializer.Deserialize<XFAiDiagResult>(json, JsonOptions);
                if (diagResult?.Items is { Count: > 0 })
                {
                    var updated = new List<XFAiValidationItem>();
                    for (var i = 0; i < Items.Count; i++)
                    {
                        var diag = i < diagResult.Items.Count ? diagResult.Items[i] : null;
                        updated.Add(Items[i] with
                        {
                            AiExplanation = diag?.AiExplanation ?? Items[i].AiExplanation,
                            AiFixSuggestion = diag?.AiFixSuggestion ?? Items[i].AiFixSuggestion
                        });
                    }

                    if (ItemsChanged.HasDelegate)
                        await ItemsChanged.InvokeAsync(updated);

                    if (OnDiagnosed.HasDelegate)
                        await OnDiagnosed.InvokeAsync(updated);
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (OnError.HasDelegate)
                await OnError.InvokeAsync($"Erro no diagnostico: {ex.Message}");
        }
        finally
        {
            _isDiagnosing = false;
        }
    }

    private async Task ResolveItemAsync(XFAiValidationItem item)
    {
        var updated = Items.Select(i => i.Id == item.Id ? i with { IsResolved = true } : i).ToList();

        if (ItemsChanged.HasDelegate)
            await ItemsChanged.InvokeAsync(updated);

        if (OnResolved.HasDelegate)
            await OnResolved.InvokeAsync(item);
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

/// <summary>Internal diagnostic result model.</summary>
internal sealed record XFAiDiagResult
{
    public IReadOnlyList<XFAiDiagItem> Items { get; init; } = Array.Empty<XFAiDiagItem>();
}

internal sealed record XFAiDiagItem
{
    public string? FieldName { get; init; }
    public string? AiExplanation { get; init; }
    public string? AiFixSuggestion { get; init; }
    public double Confidence { get; init; }
}
