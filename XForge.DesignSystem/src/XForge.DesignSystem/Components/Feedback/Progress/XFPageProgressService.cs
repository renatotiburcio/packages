using XForge.DesignSystem.Components.Feedback.Progress;

namespace XForge.DesignSystem.Components.Feedback.Progress;

/// <summary>
/// Event arguments for page progress changes.
/// </summary>
public class XFPageProgressEventArgs : EventArgs
{
    /// <summary>
    /// Progress percentage (0-100) or null for indeterminate.
    /// </summary>
    public int? Percentage { get; }

    /// <summary>
    /// Progress bar color.
    /// </summary>
    public XFProgressColor Color { get; }

    public XFPageProgressEventArgs(int? percentage, XFProgressColor color = XFProgressColor.Primary)
    {
        Percentage = percentage;
        Color = color;
    }
}

/// <summary>
/// Service for programmatically controlling the page-level progress bar.
/// Register as a scoped service and inject into any component that needs
/// to show route-change or long-operation progress.
/// </summary>
public class XFPageProgressService
{
    /// <summary>
    /// Raised when the page progress state changes.
    /// </summary>
    public event EventHandler<XFPageProgressEventArgs>? ProgressChanged;

    /// <summary>
    /// Trigger a page progress update.
    /// </summary>
    /// <param name="percentage">0-100 for determinate, null for indeterminate, -1 to hide.</param>
    /// <param name="color">Bar color (default Primary).</param>
    public Task Go(int? percentage, XFProgressColor color = XFProgressColor.Primary)
    {
        ProgressChanged?.Invoke(this, new XFPageProgressEventArgs(percentage, color));
        return Task.CompletedTask;
    }
}
