using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.PdfViewer;

public partial class XFPdfViewer : XForgeComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IXFLocalizer? Localizer { get; set; }
    private string L(string key, string fallback) => Localizer?[key] ?? fallback;

    // --- Existing parameters (preserved) ---
    [Parameter] public string? Url { get; set; }
    [Parameter] public string? Base64 { get; set; }
    [Parameter] public string? Title { get; set; } = "Documento PDF";
    [Parameter] public string? SourceLabel { get; set; }
    [Parameter] public string AriaLabel { get; set; } = default!;
    [Parameter] public string FrameTitle { get; set; } = default!;
    [Parameter] public string EmptyText { get; set; } = default!;
    [Parameter] public bool ShowToolbar { get; set; } = true;
    [Parameter] public int Page { get; set; } = 1;
    [Parameter] public EventCallback<int> PageChanged { get; set; }
    [Parameter] public int? PageCount { get; set; }
    [Parameter] public int Zoom { get; set; } = 100;
    [Parameter] public EventCallback<int> ZoomChanged { get; set; }
    [Parameter] public int MinZoom { get; set; } = 50;
    [Parameter] public int MaxZoom { get; set; } = 200;
    [Parameter] public int ZoomStep { get; set; } = 10;
    [Parameter] public string PreviousLabel { get; set; } = default!;
    [Parameter] public string NextLabel { get; set; } = default!;
    [Parameter] public string ZoomInLabel { get; set; } = default!;
    [Parameter] public string ZoomOutLabel { get; set; } = default!;
    [Parameter] public string PageLabelFormat { get; set; } = default!;
    [Parameter] public string PageCountLabelFormat { get; set; } = default!;

    // --- New parameters for S-C016 ---
    [Parameter] public XFPdfRenderMode RenderMode { get; set; } = XFPdfRenderMode.Iframe;
    [Parameter] public bool ShowDownload { get; set; } = true;
    [Parameter] public bool ShowPrint { get; set; } = true;
    [Parameter] public bool ShowFullscreen { get; set; } = true;
    [Parameter] public string? DownloadFileName { get; set; }
    [Parameter] public EventCallback<XFPdfDocumentLoadedEventArgs> OnDocumentLoaded { get; set; }
    [Parameter] public string DownloadLabel { get; set; } = default!;
    [Parameter] public string PrintLabel { get; set; } = default!;
    [Parameter] public string FullscreenLabel { get; set; } = default!;
    [Parameter] public string LoadingText { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Title = L("pdfviewer.document", "Documento PDF");
        AriaLabel = L("pdfviewer.title", "Visualizador de PDF");
        EmptyText = L("pdfviewer.placeholder", "Informe uma URL ou base64 para visualizar o PDF.");
        PreviousLabel = L("pdfviewer.prev", "Anterior");
        NextLabel = L("pdfviewer.next", "Proxima");
        ZoomInLabel = L("pdfviewer.zoom_in", "Aumentar zoom");
        ZoomOutLabel = L("pdfviewer.zoom_out", "Reduzir zoom");
        PageLabelFormat = L("pdfviewer.page_format", "Pagina {0}");
        DownloadLabel = L("pdfviewer.download", "Baixar");
        PrintLabel = L("pdfviewer.print", "Imprimir");
        FullscreenLabel = L("pdfviewer.fullscreen", "Tela cheia");
        LoadingText = L("pdfviewer.loading", "Carregando PDF...");
    }

    // --- Internal state ---
    private IJSObjectReference? _pdfModule;
    private DotNetObjectReference<XFPdfViewer>? _dotNetRef;
    private readonly string _canvasId = $"xf-pdf-canvas-{Guid.NewGuid():N}";
    private bool _isLoading;
    private int _detectedPageCount;
    private bool _initialized;

    private bool HasSource => !string.IsNullOrWhiteSpace(Url) || !string.IsNullOrWhiteSpace(Base64);
    private string? ResolvedSource => !string.IsNullOrWhiteSpace(Base64)
        ? $"data:application/pdf;base64,{Base64}"
        : Url;
    private int CurrentPage => Math.Max(1, Page);
    private int CurrentZoom => Math.Clamp(Zoom, MinZoom, MaxZoom);
    private int EffectivePageCount => _detectedPageCount > 0 ? _detectedPageCount : (PageCount ?? int.MaxValue);
    private bool CanGoPrevious => CurrentPage > 1 && !Disabled;
    private bool CanGoNext => CurrentPage < EffectivePageCount && !Disabled;
    private bool CanZoomOut => CurrentZoom > MinZoom && !Disabled;
    private bool CanZoomIn => CurrentZoom < MaxZoom && !Disabled;
    private string PageText => _detectedPageCount > 0
        ? string.Format(System.Globalization.CultureInfo.CurrentCulture, PageCountLabelFormat, CurrentPage, _detectedPageCount)
        : PageCount.HasValue
            ? string.Format(System.Globalization.CultureInfo.CurrentCulture, PageCountLabelFormat, CurrentPage, PageCount)
            : string.Format(System.Globalization.CultureInfo.CurrentCulture, PageLabelFormat, CurrentPage);
    private string ZoomText => $"{CurrentZoom}%";
    private string FrameSource => BuildFrameSource();
    private string FrameStyle => $"height:{Math.Max(360, CurrentZoom * 5)}px";

    private string RootClass =>
        GetClass("xf-pdf-viewer overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.035)]");

    private string ToolbarClass =>
        "flex flex-wrap items-center gap-3 border-b border-border-light bg-surface-50 px-4 py-3";

    private string ViewportClass =>
        "bg-surface-100 p-3";

    private string FrameClass =>
        "block w-full rounded-md border border-border-light bg-white";

    private string CanvasClass =>
        "block w-full rounded-md border border-border-light bg-white";

    private string ToolButtonClass =>
        "inline-flex h-8 items-center justify-center rounded-md border border-border-light bg-bg-surface px-3 text-xs font-semibold text-text-primary outline-none hover:text-primary focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-45";

    private string IconButtonClass =>
        "inline-flex h-8 w-8 items-center justify-center rounded-md border border-border-light bg-bg-surface text-sm font-semibold text-text-primary outline-none hover:text-primary focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-45";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (RenderMode == XFPdfRenderMode.Canvas && HasSource && !_initialized)
        {
            try
            {
                _pdfModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/XForge.DesignSystem/js/xf-pdf-viewer.js");

                _dotNetRef ??= DotNetObjectReference.Create(this);
                _isLoading = true;
                StateHasChanged();

                await _pdfModule.InvokeVoidAsync("initialize",
                    Id, _canvasId, _dotNetRef, ResolvedSource, CurrentPage, CurrentZoom);

                _initialized = true;
            }
            catch
            {
                // Graceful degradation — bUnit has no JS interop
                _isLoading = false;
                _initialized = true;
            }
        }
    }

    [JSInvokable]
    public Task NotifyPdfInitialized(XFPdfNotification notification)
    {
        _isLoading = false;
        _detectedPageCount = notification.TotalPages;
        Page = notification.PageNumber;

        if (OnDocumentLoaded.HasDelegate)
        {
            return OnDocumentLoaded.InvokeAsync(new XFPdfDocumentLoadedEventArgs
            {
                PageCount = notification.TotalPages,
                Title = string.IsNullOrWhiteSpace(notification.Title) ? null : notification.Title,
            });
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task NotifyPdfChanged(XFPdfNotification notification)
    {
        Page = notification.PageNumber;
        _detectedPageCount = notification.TotalPages;

        if (PageChanged.HasDelegate)
        {
            return PageChanged.InvokeAsync(Page);
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task PreviousPageAsync()
    {
        if (RenderMode == XFPdfRenderMode.Canvas && _pdfModule is not null)
        {
            await _pdfModule.InvokeVoidAsync("prevPage", Id);
        }
        else
        {
            await SetPageAsync(CurrentPage - 1);
        }
    }

    private async Task NextPageAsync()
    {
        if (RenderMode == XFPdfRenderMode.Canvas && _pdfModule is not null)
        {
            await _pdfModule.InvokeVoidAsync("nextPage", Id);
        }
        else
        {
            await SetPageAsync(CurrentPage + 1);
        }
    }

    private async Task ZoomOutAsync()
    {
        var newZoom = CurrentZoom - ZoomStep;
        if (RenderMode == XFPdfRenderMode.Canvas && _pdfModule is not null)
        {
            Zoom = Math.Clamp(newZoom, MinZoom, MaxZoom);
            await _pdfModule.InvokeVoidAsync("setScale", Id, Zoom);
        }
        else
        {
            await SetZoomAsync(newZoom);
        }
    }

    private async Task ZoomInAsync()
    {
        var newZoom = CurrentZoom + ZoomStep;
        if (RenderMode == XFPdfRenderMode.Canvas && _pdfModule is not null)
        {
            Zoom = Math.Clamp(newZoom, MinZoom, MaxZoom);
            await _pdfModule.InvokeVoidAsync("setScale", Id, Zoom);
        }
        else
        {
            await SetZoomAsync(newZoom);
        }
    }

    private async Task DownloadAsync()
    {
        if (_pdfModule is not null)
        {
            await _pdfModule.InvokeVoidAsync("downloadPdf", Id, DownloadFileName);
        }
    }

    private async Task PrintAsync()
    {
        if (_pdfModule is not null)
        {
            await _pdfModule.InvokeVoidAsync("printPdf", Id);
        }
    }

    private async Task ToggleFullscreenAsync()
    {
        if (_pdfModule is not null)
        {
            await _pdfModule.InvokeVoidAsync("toggleFullscreen", Id);
        }
    }

    private async Task SetPageAsync(int page)
    {
        Page = Math.Max(1, EffectivePageCount < int.MaxValue ? Math.Min(page, EffectivePageCount) : page);
        if (PageChanged.HasDelegate)
        {
            await PageChanged.InvokeAsync(Page);
        }
    }

    private async Task SetZoomAsync(int zoom)
    {
        Zoom = Math.Clamp(zoom, MinZoom, MaxZoom);
        if (ZoomChanged.HasDelegate)
        {
            await ZoomChanged.InvokeAsync(Zoom);
        }
    }

    private string BuildFrameSource()
    {
        var source = ResolvedSource ?? string.Empty;
        return $"{source}#page={CurrentPage}&zoom={CurrentZoom}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_pdfModule is not null)
        {
            try
            {
                await _pdfModule.InvokeVoidAsync("destroy", Id);
                await _pdfModule.DisposeAsync();
            }
            catch
            {
                // Swallow disposal errors (bUnit, already disposed)
            }
        }

        _dotNetRef?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Notification model from JS interop for PDF state changes.
/// </summary>
public record XFPdfNotification
{
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int Scale { get; init; }
    public string Title { get; init; } = "";
}
