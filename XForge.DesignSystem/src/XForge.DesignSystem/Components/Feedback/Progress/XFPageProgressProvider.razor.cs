using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.Progress;

/// <summary>
/// Thin page-level progress bar that responds to <see cref="XFPageProgressService"/>.
/// Place once in the app shell layout (e.g. AppShell or MainLayout).
/// Renders a fixed bar at the top of the viewport.
/// </summary>
public partial class XFPageProgressProvider : XForgeComponentBase, IDisposable
{
    [Inject] private XFPageProgressService ProgressService { get; set; } = default!;

    /// <summary>Accessibility label for the progress bar.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Progresso da página";

    /// <summary>Accessible value text when loading (indeterminate).</summary>
    [Parameter] public string LoadingText { get; set; } = "Carregando";

    private int? _percentage;
    private bool _visible;
    private XFProgressColor _color = XFProgressColor.Primary;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ProgressService.ProgressChanged += OnProgressChanged;
    }

    private void OnProgressChanged(object? sender, XFPageProgressEventArgs e)
    {
        if (e.Percentage is null)
        {
            // Indeterminate
            _percentage = null;
            _visible = true;
            _color = e.Color;
        }
        else if (e.Percentage < 0)
        {
            // Hide
            _visible = false;
            _percentage = null;
        }
        else
        {
            // Determinate (0-100)
            _percentage = Math.Clamp(e.Percentage.Value, 0, 100);
            _visible = true;
            _color = e.Color;
        }

        InvokeAsync(StateHasChanged);
    }

    private string GetBarClass()
    {
        var color = _color switch
        {
            XFProgressColor.Secondary => "bg-secondary",
            XFProgressColor.Success => "bg-success",
            XFProgressColor.Warning => "bg-warning",
            XFProgressColor.Danger => "bg-danger",
            XFProgressColor.Neutral => "bg-surface-700",
            XFProgressColor.Info => "bg-info",
            XFProgressColor.Light => "bg-surface-200",
            XFProgressColor.Dark => "bg-surface-900",
            _ => "bg-primary"
        };

        var animation = _percentage is null
            ? "motion-safe:xf-progress-indeterminate-animate"
            : "transition-[width] duration-300 ease-out";

        return $"xf-page-progress-bar {color} {animation}";
    }

    private string GetBarStyle()
    {
        if (_percentage is null)
            return string.Empty;

        return $"width: {_percentage.Value}%;";
    }

    private string GetAriaValueText()
    {
        if (_percentage is null)
            return LoadingText;

        return $"{_percentage.Value}%";
    }

    public void Dispose()
    {
        ProgressService.ProgressChanged -= OnProgressChanged;
    }
}
