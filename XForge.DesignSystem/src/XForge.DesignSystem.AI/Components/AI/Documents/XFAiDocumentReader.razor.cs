using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Documents;

/// <summary>
/// Intelligent document reader with AI summarization, semantic search, and content extraction.
/// Supports PDF, DOCX, XLSX, XML, TXT, CSV, JPG, PNG.
/// </summary>
public partial class XFAiDocumentReader : XForgeComponentBase
{
    protected override string BaseClass => "xf-ai-document-reader";

    private XFAiDocumentResult? _result;
    private string? _errorMessage;
    private bool _isSummaryExpanded = true;
    private CancellationTokenSource? _cts;

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>AI provider for summarization and extraction.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>Raw document content to process.</summary>
    [Parameter]
    public string? DocumentContent { get; set; }

    /// <summary>Document filename for context.</summary>
    [Parameter]
    public string? DocumentName { get; set; }

    /// <summary>Document format type.</summary>
    [Parameter]
    public XFAiDocumentType? DocumentType { get; set; }

    /// <summary>Header title text.</summary>
    [Parameter]
    public string Title { get; set; } = "Leitor de Documentos";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the AI summary section.</summary>
    [Parameter]
    public bool ShowSummary { get; set; } = true;

    /// <summary>Whether to show the document chunks list.</summary>
    [Parameter]
    public bool ShowChunks { get; set; }

    /// <summary>Maximum number of chunks to display.</summary>
    [Parameter]
    public int MaxChunks { get; set; } = 10;

    /// <summary>Characters per chunk when splitting document content.</summary>
    [Parameter]
    public int ChunkSize { get; set; } = 500;

    /// <summary>External loading state override.</summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>Text displayed when no document is loaded.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Envie um documento para analise.";

    /// <summary>Custom system prompt for AI summarization.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when the document has been fully processed.</summary>
    [Parameter]
    public EventCallback<XFAiDocumentResult> OnDocumentProcessed { get; set; }

    /// <summary>Fired when the AI summary is generated.</summary>
    [Parameter]
    public EventCallback<string> OnSummaryGenerated { get; set; }

    /// <summary>Fired on error during processing.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    // ── Render Fragments ────────────────────────────────────────────────

    /// <summary>Custom document preview template.</summary>
    [Parameter]
    public RenderFragment<XFAiDocumentResult>? DocumentPreview { get; set; }

    /// <summary>Custom chunk rendering template.</summary>
    [Parameter]
    public RenderFragment<XFAiDocumentChunk>? ChunkTemplate { get; set; }

    /// <summary>Custom header content (replaces default header).</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Computed ────────────────────────────────────────────────────────

    private bool IsProcessing => Loading;

    private IReadOnlyList<XFAiDocumentChunk> VisibleChunks =>
        _result is not null ? _result.Chunks.Take(MaxChunks).ToList() : (IReadOnlyList<XFAiDocumentChunk>)Array.Empty<XFAiDocumentChunk>();

    private string DocumentTypeLabel => (_result?.DocumentType ?? DocumentType ?? XFAiDocumentType.Txt) switch
    {
        XFAiDocumentType.Pdf => "PDF",
        XFAiDocumentType.Docx => "DOCX",
        XFAiDocumentType.Xlsx => "XLSX",
        XFAiDocumentType.Xml => "XML",
        XFAiDocumentType.Txt => "TXT",
        XFAiDocumentType.Csv => "CSV",
        XFAiDocumentType.Jpg => "JPG",
        XFAiDocumentType.Png => "PNG",
        _ => "DOC"
    };

    private string DocumentTypeIcon => (_result?.DocumentType ?? DocumentType ?? XFAiDocumentType.Txt) switch
    {
        XFAiDocumentType.Pdf => "file-pdf",
        XFAiDocumentType.Docx => "file-word",
        XFAiDocumentType.Xlsx => "file-excel",
        XFAiDocumentType.Xml => "file-code",
        XFAiDocumentType.Csv => "file-csv",
        XFAiDocumentType.Jpg => "file-image",
        XFAiDocumentType.Png => "file-image",
        _ => "file-lines"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    protected override string GetComponentClass()
    {
        return GetClass(
            $"{BaseClass} overflow-hidden rounded-xl border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06),0_8px_24px_rgb(15_23_42/.04)]");
    }

    private string HeaderClass =>
        "flex items-center justify-between gap-2 border-b border-border-light px-4 py-3";

    private string HeaderTitleClass =>
        "text-sm font-semibold text-text-primary";

    private string LoadingContainerClass =>
        "animate-pulse p-4 space-y-3";

    private string SkeletonInfoBarClass =>
        "flex items-center gap-3";

    private string SkeletonSummaryClass =>
        "mt-3 rounded-lg bg-surface-50 p-4 space-y-3";

    private string SkeletonChunkClass =>
        "rounded-lg bg-surface-50 p-3 space-y-2";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-3 px-4 py-12 text-center";

    private string EmptyIconClass =>
        "inline-flex h-12 w-12 items-center justify-center rounded-full bg-surface-100 text-text-muted";

    private string EmptyTextClass =>
        "text-sm text-text-secondary max-w-xs";

    private string InfoBarClass =>
        "flex flex-wrap items-center gap-3 border-b border-border-light px-4 py-2.5";

    private string DocumentTypeBadgeClass =>
        $"inline-flex items-center rounded-md px-2 py-0.5 text-xs font-semibold {TypeBadgeColorClass}";

    private string TypeBadgeColorClass => (_result?.DocumentType ?? DocumentType ?? XFAiDocumentType.Txt) switch
    {
        XFAiDocumentType.Pdf => "bg-red-50 text-red-700 ring-1 ring-red-200",
        XFAiDocumentType.Docx => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        XFAiDocumentType.Xlsx => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        XFAiDocumentType.Xml => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        XFAiDocumentType.Csv => "bg-teal-50 text-teal-700 ring-1 ring-teal-200",
        XFAiDocumentType.Jpg or XFAiDocumentType.Png => "bg-purple-50 text-purple-700 ring-1 ring-purple-200",
        _ => "bg-slate-50 text-slate-700 ring-1 ring-slate-200"
    };

    private string InfoMetricClass =>
        "text-xs text-text-muted tabular-nums";

    private string SummaryContainerClass =>
        "border-b border-border-light";

    private string SummaryToggleClass =>
        "flex w-full items-center gap-2 px-4 py-3 text-left outline-none transition-colors duration-150 hover:bg-surface-50 focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-border-focus";

    private string SummaryContentClass =>
        "px-4 pb-4 pt-0";

    private string ChunksContainerClass =>
        "px-4 py-3";

    private string ChunksHeaderClass =>
        "flex items-center justify-between mb-2";

    private string ChunkItemClass =>
        "py-2.5";

    private string ChunkIndexBadgeClass =>
        "inline-flex items-center rounded bg-surface-100 px-1.5 py-0.5 text-[10px] font-semibold text-text-muted tabular-nums";

    private string RelevanceBarTrackClass =>
        "h-1.5 w-14 overflow-hidden rounded-full bg-surface-200 ml-auto";

    private string RelevanceBarFillClass =>
        "h-full rounded-full bg-blue-500 transition-all duration-300";

    private string ErrorBannerClass =>
        "flex items-center gap-2 border-t border-danger-200 bg-danger-50 px-4 py-2 text-xs text-danger";

    // ── Lifecycle ───────────────────────────────────────────────────────

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        // Auto-process when DocumentContent changes and we have enough info
        if (!string.IsNullOrWhiteSpace(DocumentContent)
            && !IsProcessing
            && _result is null
            && Provider is not null)
        {
            await ProcessDocumentAsync();
        }
    }

    // ── Public Methods ──────────────────────────────────────────────────

    /// <summary>
    /// Processes the current DocumentContent: splits into chunks,
    /// generates AI summary, and fires OnDocumentProcessed.
    /// </summary>
    public async Task ProcessDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(DocumentContent))
            return;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        _errorMessage = null;
        _result = null;
        StateHasChanged();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // 1. Chunk the document
            var chunks = ChunkContent(DocumentContent, ChunkSize);

            // 2. Build result
            var docType = DocumentType ?? DetectDocumentType(DocumentName);
            var result = new XFAiDocumentResult
            {
                DocumentName = DocumentName ?? "Documento",
                DocumentType = docType,
                Chunks = chunks,
                TotalCharacters = DocumentContent.Length,
                TotalChunks = chunks.Count,
                Metadata = new Dictionary<string, string>
                {
                    ["chunkSize"] = ChunkSize.ToString(),
                    ["provider"] = Provider?.ProviderId ?? "none"
                }
            };

            // 3. Generate AI summary if provider is available
            if (Provider is not null && ShowSummary)
            {
                result.AiSummary = await GenerateSummaryAsync(DocumentContent, DocumentName, ct);
            }

            stopwatch.Stop();
            result.ProcessingDuration = stopwatch.Elapsed;
            _result = result;

            // 4. Fire callbacks
            if (OnDocumentProcessed.HasDelegate)
            {
                await OnDocumentProcessed.InvokeAsync(_result);
            }

            if (!string.IsNullOrWhiteSpace(_result.AiSummary) && OnSummaryGenerated.HasDelegate)
            {
                await OnSummaryGenerated.InvokeAsync(_result.AiSummary);
            }
        }
        catch (OperationCanceledException)
        {
            // Silently handle cancellation
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;

            if (OnError.HasDelegate)
            {
                await OnError.InvokeAsync(ex.Message);
            }
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Processes arbitrary content with a given document name and type.
    /// </summary>
    public async Task ProcessContentAsync(string content, string? name = null, XFAiDocumentType? type = null)
    {
        DocumentContent = content;
        if (name is not null) DocumentName = name;
        if (type is not null) DocumentType = type;
        _result = null;
        await ProcessDocumentAsync();
    }

    /// <summary>
    /// Clears the current document result and resets to empty state.
    /// </summary>
    public void Clear()
    {
        _result = null;
        _errorMessage = null;
        _isSummaryExpanded = true;
        StateHasChanged();
    }

    // ── Internal Processing ─────────────────────────────────────────────

    private async Task<string> GenerateSummaryAsync(string content, string? fileName, CancellationToken ct)
    {
        var truncatedContent = content.Length > 4000
            ? content[..4000] + "\n[...truncado...]"
            : content;

        var contextHint = fileName is not null ? $" O arquivo se chama '{fileName}'." : "";
        var docTypeHint = DocumentType.HasValue ? $" O formato do documento e {DocumentTypeLabel}." : "";

        var prompt = SystemPrompt
            ?? $"Resuma o seguinte documento de forma concisa e estruturada.{contextHint}{docTypeHint} Destaque os pontos principais, dados numericos relevantes e qualquer informacao critica.\n\n---\n{truncatedContent}";

        var request = new AiChatRequest
        {
            Messages = new[]
            {
                new AiMessage { Role = AiRole.User, Content = prompt }
            },
            SystemPrompt = "Voce e um assistente especializado em analise de documentos corporativos brasileiros. Responda sempre em portugues.",
            MaxTokens = 1000
        };

        var response = await Provider!.CompleteAsync(request, ct);
        return response.Message.Content;
    }

    private static List<XFAiDocumentChunk> ChunkContent(string content, int chunkSize)
    {
        var chunks = new List<XFAiDocumentChunk>();
        var index = 0;
        var position = 0;

        while (position < content.Length)
        {
            var length = Math.Min(chunkSize, content.Length - position);
            var chunkContent = content.Substring(position, length);

            chunks.Add(new XFAiDocumentChunk
            {
                Index = index,
                Content = chunkContent,
                StartChar = position,
                EndChar = position + length
            });

            position += length;
            index++;
        }

        return chunks;
    }

    private static XFAiDocumentType DetectDocumentType(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return XFAiDocumentType.Txt;

        var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => XFAiDocumentType.Pdf,
            ".docx" or ".doc" => XFAiDocumentType.Docx,
            ".xlsx" or ".xls" => XFAiDocumentType.Xlsx,
            ".xml" => XFAiDocumentType.Xml,
            ".csv" => XFAiDocumentType.Csv,
            ".jpg" or ".jpeg" => XFAiDocumentType.Jpg,
            ".png" => XFAiDocumentType.Png,
            _ => XFAiDocumentType.Txt
        };
    }

    private void ToggleSummary()
    {
        _isSummaryExpanded = !_isSummaryExpanded;
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalSeconds >= 1)
            return $"{duration.TotalSeconds:F1}s";
        return $"{duration.TotalMilliseconds:F0}ms";
    }

    // ── Dispose ─────────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}

