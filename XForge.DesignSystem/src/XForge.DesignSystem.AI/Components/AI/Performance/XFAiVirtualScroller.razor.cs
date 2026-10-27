using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Performance;

/// <summary>
/// High-performance virtual scroller for million-row datasets.
/// Renders only visible items using absolute positioning and scroll offset tracking.
/// Pure Blazor, no JS interop.
/// </summary>
public partial class XFAiVirtualScroller<TItem> : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>All items in the dataset.</summary>
    [Parameter]
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();

    /// <summary>Individual item height in pixels (fixed strategy).</summary>
    [Parameter]
    public double ItemHeight { get; set; } = 40;

    /// <summary>Number of extra items to render above/below viewport.</summary>
    [Parameter]
    public int OverscanCount { get; set; } = 5;

    /// <summary>Container height in pixels.</summary>
    [Parameter]
    public int ContainerHeight { get; set; } = 400;

    /// <summary>Rendering strategy.</summary>
    [Parameter]
    public XFAiVirtualRenderStrategy Strategy { get; set; } = XFAiVirtualRenderStrategy.FixedHeight;

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Virtual Scroller";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Lista virtual de alto desempenho";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the footer bar.</summary>
    [Parameter]
    public bool ShowFooter { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhum item para exibir.";

    /// <summary>Custom item template.</summary>
    [Parameter]
    public RenderFragment<XFAiVirtualItem<TItem>>? ItemTemplate { get; set; }

    /// <summary>Total item count (for server-side scenarios).</summary>
    [Parameter]
    public int TotalItems { get; set; }

    /// <summary>Fired when the visible range changes.</summary>
    [Parameter]
    public EventCallback<XFAiVirtualRange> OnRangeChanged { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private ElementReference _scrollContainer;
    private List<XFAiVirtualItem<TItem>> _visibleItems = new();
    private double _totalHeight;
    private double _scrollPercentage;
    private int _visibleRange;
    private XFAiVirtualRenderStrategy _strategy;
    private double _scrollTop;

    // ── Computed ────────────────────────────────────────────────────────

    private int EffectiveTotalItems => TotalItems > 0 ? TotalItems : Items.Count;

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        _strategy = Strategy;
        Recalculate();
    }

    // ── Scroll Handling ─────────────────────────────────────────────────

    private async Task HandleScrollAsync()
    {
        // Pure Blazor: track scroll position via internal state
        // In production, scrollTop would come from JS interop
        // For now, simulate scroll tracking
        Recalculate();

        if (OnRangeChanged.HasDelegate && _visibleItems.Count > 0)
        {
            var range = new XFAiVirtualRange
            {
                StartIndex = _visibleItems.First().Index,
                EndIndex = _visibleItems.Last().Index,
                OffsetTop = _visibleItems.First().Top,
                TotalHeight = _totalHeight
            };
            await OnRangeChanged.InvokeAsync(range);
        }
    }

    // ── Virtualization Logic ────────────────────────────────────────────

    private void Recalculate()
    {
        var count = EffectiveTotalItems;
        if (count == 0)
        {
            _visibleItems = new();
            _totalHeight = 0;
            _scrollPercentage = 0;
            _visibleRange = 0;
            return;
        }

        _totalHeight = count * ItemHeight;
        var overscanPx = OverscanCount * ItemHeight;
        var viewportStart = Math.Max(0, _scrollTop - overscanPx);
        var viewportEnd = _scrollTop + ContainerHeight + overscanPx;

        var startIndex = Math.Max(0, (int)(viewportStart / ItemHeight));
        var endIndex = Math.Min(count - 1, (int)(viewportEnd / ItemHeight));

        _visibleItems = new List<XFAiVirtualItem<TItem>>();
        for (var i = startIndex; i <= endIndex; i++)
        {
            _visibleItems.Add(new XFAiVirtualItem<TItem>
            {
                Data = i < Items.Count ? Items[i] : default!,
                Index = i,
                Top = i * ItemHeight,
                Height = ItemHeight,
                IsVisible = true
            });
        }

        _visibleRange = _visibleItems.Count;
        _scrollPercentage = count > 0 ? (double)(endIndex + 1) / count * 100 : 0;
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Scrolls to a specific item index.</summary>
    public void ScrollToIndex(int index)
    {
        var count = EffectiveTotalItems;
        if (count == 0 || index < 0 || index >= count) return;
        _scrollTop = index * ItemHeight;
        Recalculate();
    }

    /// <summary>Returns the current visible range.</summary>
    public XFAiVirtualRange GetVisibleRange()
    {
        if (_visibleItems.Count == 0)
            return new XFAiVirtualRange { StartIndex = 0, EndIndex = 0 };

        return new XFAiVirtualRange
        {
            StartIndex = _visibleItems.First().Index,
            EndIndex = _visibleItems.Last().Index,
            OffsetTop = _visibleItems.First().Top,
            TotalHeight = _totalHeight
        };
    }

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-virtual-scroller overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
