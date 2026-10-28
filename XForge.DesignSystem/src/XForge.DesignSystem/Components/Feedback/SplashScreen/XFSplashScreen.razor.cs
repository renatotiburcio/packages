using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Feedback.SplashScreen;

public partial class XFSplashScreen : XForgeComponentBase
{
    [Inject] private IXFLocalizer? Localizer { get; set; }
    private string L(string key, string fallback) => Localizer?[key] ?? fallback;

    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public bool FullScreen { get; set; } = true;
    [Parameter] public string? Title { get; set; } = "Carregando";
    [Parameter] public string? Message { get; set; }
    [Parameter] public string Initials { get; set; } = default!;
    [Parameter] public double? Progress { get; set; }
    [Parameter] public bool ShowProgressValue { get; set; }
    [Parameter] public string? AriaLabel { get; set; } = "Carregando aplicacao";
    [Parameter] public string ProgressValueFormat { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Title)) Title = L("splash.loading", Title);
        if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("splash.loading_app", AriaLabel);
        base.OnInitialized();
    }
    [Parameter] public RenderFragment? Logo { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string Role => FullScreen ? "status" : "region";
    private string AriaLive => FullScreen ? "polite" : "off";
    private double ClampedProgress => Math.Clamp(Progress ?? 0, 0, 100);
    private string ProgressStyle => $"width:{ClampedProgress.ToString("0.###", CultureInfo.InvariantCulture)}%;";
    private string ProgressValueText => string.Format(CultureInfo.CurrentCulture, ProgressValueFormat, ClampedProgress.ToString("0", CultureInfo.CurrentCulture));

    private string RootClass => Join(
        "xf-splash-screen grid place-items-center bg-bg-canvas text-text-primary",
        FullScreen ? "fixed inset-0 z-50 min-h-screen" : "min-h-80 rounded-lg border border-border-light",
        Class);

    private string PanelClass =>
        "grid w-full max-w-md justify-items-center gap-5 px-6 py-8";

    private string LogoClass =>
        "grid h-16 w-16 place-items-center rounded-2xl bg-primary text-primary-contrast shadow-[0_16px_34px_rgb(15_23_42/.18)]";

    private string ProgressBarClass =>
        "h-full rounded-full bg-primary transition-[width] duration-300 ease-out";

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
