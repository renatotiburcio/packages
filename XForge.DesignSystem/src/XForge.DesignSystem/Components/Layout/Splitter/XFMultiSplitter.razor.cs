using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Layout.Splitter;

public partial class XFMultiSplitter : XForgeComponentBase
{
    private readonly List<XFMultiSplitterPane> panes = new();
    private IReadOnlyList<double> currentSizes = Array.Empty<double>();
    private IReadOnlyList<double> initialSizes = Array.Empty<double>();
    private int registeredVersion;
    private int initializedVersion = -1;
    private IReadOnlyList<double>? lastParameterSizes;

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public XFOrientation Orientation { get; set; }
    [Parameter] public IReadOnlyList<double>? Sizes { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<double>> SizesChanged { get; set; }
    [Parameter] public EventCallback<XFMultiSplitterChangedEventArgs> Change { get; set; }
    [Parameter] public double Step { get; set; } = 1;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public string HandleLabel { get; set; } = "Redimensionar paineis";
    [Parameter] public string HandleLabelFormat { get; set; } = "{0} {1}";

    internal IReadOnlyList<XFMultiSplitterPane> Panes => panes;

    private string OrientationValue => Orientation == XFOrientation.Vertical ? "vertical" : "horizontal";

    protected override string BaseClass => "xf-multi-splitter";

    protected override void OnParametersSet()
    {
        if (Sizes is not null && !SequenceEqual(Sizes, lastParameterSizes))
        {
            currentSizes = NormalizeSizes(Sizes);
            initialSizes = currentSizes;
            lastParameterSizes = currentSizes.ToArray();
        }

        Step = Math.Max(0.1, Step);
    }

    protected override string GetComponentClass()
    {
        return GetClass($"{BaseClass} {RootClass}");
    }

    internal void Register(XFMultiSplitterPane pane)
    {
        if (!panes.Contains(pane))
        {
            panes.Add(pane);
            registeredVersion++;
        }

        if (initializedVersion != registeredVersion && panes.Count > 0)
        {
            currentSizes = Sizes is not null && Sizes.Count == panes.Count
                ? NormalizeSizes(Sizes)
                : NormalizeSizes(panes.Select(item => item.Size).ToArray());
            initialSizes = currentSizes.ToArray();
            initializedVersion = registeredVersion;
            _ = InvokeAsync(StateHasChanged);
        }
    }

    internal void Unregister(XFMultiSplitterPane pane)
    {
        if (panes.Remove(pane))
        {
            registeredVersion++;
            initializedVersion = -1;
            _ = InvokeAsync(StateHasChanged);
        }
    }

    private string RootClass => Join(
        "min-w-0 overflow-hidden rounded-lg border border-border-light bg-bg-surface select-none",
        Orientation == XFOrientation.Vertical ? "grid min-h-96" : "grid min-h-72",
        Disabled ? "opacity-60" : "",
        Class);

    private string RootStyle => JoinStyle(
        Orientation == XFOrientation.Vertical
            ? $"grid-template-rows:{GridTracks}"
            : $"grid-template-columns:{GridTracks}",
        Style);

    private string GridTracks
    {
        get
        {
            var tracks = new List<string>();
            for (var index = 0; index < panes.Count; index++)
            {
                tracks.Add(panes[index].Collapsed ? "0fr" : $"{GetSize(index).ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)}fr");
                if (index < panes.Count - 1)
                {
                    tracks.Add("auto");
                }
            }

            return string.Join(" ", tracks);
        }
    }

    private string PaneClass(XFMultiSplitterPane pane) =>
        Join("min-w-0 min-h-0 overflow-auto", pane.Collapsed ? "hidden" : "");

    private static string PaneStyle(XFMultiSplitterPane pane) => pane.Collapsed ? "display:none" : "";

    private string SeparatorWrapClass => Orientation == XFOrientation.Vertical
        ? "relative flex h-4 items-center justify-center bg-surface-50"
        : "relative flex w-4 items-center justify-center bg-surface-50";

    private string HandleClass => Orientation == XFOrientation.Vertical
        ? "group absolute inset-x-0 -top-2 -bottom-2 flex cursor-row-resize items-center justify-center border-0 bg-transparent p-0 text-surface-500 outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-bg-surface disabled:cursor-not-allowed"
        : "group absolute inset-y-0 -left-2 -right-2 flex cursor-col-resize items-center justify-center border-0 bg-transparent p-0 text-surface-500 outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-bg-surface disabled:cursor-not-allowed";

    private string HandleVisualClass => Orientation == XFOrientation.Vertical
        ? "flex h-2 w-16 items-center justify-center rounded-full bg-surface-200 transition-colors group-hover:bg-primary group-focus-visible:bg-primary"
        : "flex h-16 w-2 items-center justify-center rounded-full bg-surface-200 transition-colors group-hover:bg-primary group-focus-visible:bg-primary";

    private string GripClass => Orientation == XFOrientation.Vertical
        ? "block h-px w-8 rounded-full bg-current opacity-70 group-hover:text-white group-focus-visible:text-white"
        : "block h-8 w-px rounded-full bg-current opacity-70 group-hover:text-white group-focus-visible:text-white";

    private string ResolvedHandleLabel(int handleIndex) =>
        string.Format(CultureInfo.CurrentCulture, HandleLabelFormat, HandleLabel, handleIndex + 1);

    private double HandleValue(int handleIndex) => GetSize(handleIndex);

    private double HandleMin(int handleIndex) => panes[handleIndex].Min;

    private double HandleMax(int handleIndex) => panes[handleIndex].Max;

    private async Task HandleKeyDownAsync(KeyboardEventArgs args, int handleIndex)
    {
        if (Disabled)
        {
            return;
        }

        var delta = args.Key switch
        {
            "ArrowRight" or "ArrowDown" => Step,
            "ArrowLeft" or "ArrowUp" => -Step,
            "PageUp" => Step * 10,
            "PageDown" => -Step * 10,
            "Home" => panes[handleIndex].Min - GetSize(handleIndex),
            "End" => panes[handleIndex].Max - GetSize(handleIndex),
            _ => double.NaN
        };

        if (!double.IsNaN(delta))
        {
            await AdjustHandleAsync(handleIndex, delta, notifyChanged: true);
        }
    }

    private Task ResetHandleAsync(int handleIndex)
    {
        if (Disabled || handleIndex < 0 || handleIndex >= panes.Count - 1)
        {
            return Task.CompletedTask;
        }

        var desired = initialSizes.Count == panes.Count ? initialSizes[handleIndex] : panes[handleIndex].Size;
        return AdjustHandleAsync(handleIndex, desired - GetSize(handleIndex), notifyChanged: true);
    }

    private async Task AdjustHandleAsync(int handleIndex, double delta, bool notifyChanged)
    {
        if (handleIndex < 0 || handleIndex >= panes.Count - 1)
        {
            return;
        }

        var nextIndex = handleIndex + 1;
        var sizes = EnsureCurrentSizes().ToArray();
        var left = sizes[handleIndex];
        var right = sizes[nextIndex];
        var total = left + right;

        var nextLeft = Math.Clamp(left + delta, panes[handleIndex].Min, Math.Min(panes[handleIndex].Max, total - panes[nextIndex].Min));
        var nextRight = total - nextLeft;
        nextRight = Math.Clamp(nextRight, panes[nextIndex].Min, panes[nextIndex].Max);
        nextLeft = total - nextRight;

        sizes[handleIndex] = Math.Max(0, nextLeft);
        sizes[nextIndex] = Math.Max(0, nextRight);
        currentSizes = sizes;
        lastParameterSizes = currentSizes.ToArray();

        if (SizesChanged.HasDelegate)
        {
            await SizesChanged.InvokeAsync(currentSizes);
        }

        if (notifyChanged && Change.HasDelegate)
        {
            await Change.InvokeAsync(new XFMultiSplitterChangedEventArgs(currentSizes, handleIndex, Orientation));
        }
    }

    private double GetSize(int index)
    {
        var sizes = EnsureCurrentSizes();
        return index >= 0 && index < sizes.Count ? sizes[index] : 1;
    }

    private IReadOnlyList<double> EnsureCurrentSizes()
    {
        if (currentSizes.Count == panes.Count)
        {
            return currentSizes;
        }

        currentSizes = NormalizeSizes(panes.Select(item => item.Size).ToArray());
        initialSizes = currentSizes.ToArray();
        return currentSizes;
    }

    private IReadOnlyList<double> NormalizeSizes(IReadOnlyList<double> source)
    {
        if (panes.Count == 0)
        {
            return Array.Empty<double>();
        }

        var values = new double[panes.Count];
        for (var index = 0; index < values.Length; index++)
        {
            var raw = index < source.Count ? source[index] : 1;
            values[index] = double.IsFinite(raw) && raw > 0 ? raw : 1;
        }

        return values;
    }

    private static bool SequenceEqual(IReadOnlyList<double>? left, IReadOnlyList<double>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null || left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (Math.Abs(left[index] - right[index]) > double.Epsilon)
            {
                return false;
            }
        }

        return true;
    }

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));

    private static string JoinStyle(params string?[] values) =>
        string.Join(";", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
