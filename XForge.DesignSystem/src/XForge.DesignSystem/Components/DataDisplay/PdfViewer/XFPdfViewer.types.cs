namespace XForge.DesignSystem.Components.DataDisplay.PdfViewer;

/// <summary>
/// Rendering mode for the PDF viewer.
/// </summary>
public enum XFPdfRenderMode
{
    /// <summary>
    /// Browser-native iframe rendering (default). Delegates to the browser's built-in PDF viewer.
    /// </summary>
    Iframe,

    /// <summary>
    /// Canvas-based rendering via PDF.js. Provides consistent cross-browser rendering,
    /// automatic page count detection, print, download, and fullscreen support.
    /// </summary>
    Canvas
}

/// <summary>
/// Event args raised when a PDF document is loaded in Canvas mode.
/// </summary>
public record XFPdfDocumentLoadedEventArgs
{
    /// <summary>
    /// Total number of pages in the document.
    /// </summary>
    public int PageCount { get; init; }

    /// <summary>
    /// Document title extracted from PDF metadata, if available.
    /// </summary>
    public string? Title { get; init; }
}
