using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Range navigator component with draggable selection window for filtering data ranges.
/// </summary>
public partial class XFRangeNavigator : XForgeComponentBase
{
    protected override string BaseClass => "xf-range-nav relative h-20 w-full select-none overflow-hidden rounded-lg border border-surface-200 bg-surface-50";

    /// <summary>Start of the selected range as a fraction (0-1).</summary>
    [Parameter] public double Start { get; set; }

    /// <summary>Callback when Start changes.</summary>
    [Parameter] public EventCallback<double> StartChanged { get; set; }

    /// <summary>End of the selected range as a fraction (0-1).</summary>
    [Parameter] public double End { get; set; } = 1;

    /// <summary>Callback when End changes.</summary>
    [Parameter] public EventCallback<double> EndChanged { get; set; }

    /// <summary>Whether to show handle labels.</summary>
    [Parameter] public bool ShowHandleLabels { get; set; }

    /// <summary>Format string for handle labels.</summary>
    [Parameter] public string? HandleLabelFormat { get; set; }

    /// <summary>Whether to show axis ticks.</summary>
    [Parameter] public bool ShowAxis { get; set; }

    /// <summary>Number of axis ticks.</summary>
    [Parameter] public int TickCount { get; set; } = 5;

    /// <summary>Format string for axis tick labels.</summary>
    [Parameter] public string? AxisFormat { get; set; }

    /// <summary>Minimum axis value.</summary>
    [Parameter] public double AxisMin { get; set; }

    /// <summary>Maximum axis value.</summary>
    [Parameter] public double AxisMax { get; set; } = 100;

    /// <summary>Callback when range selection changes (both Start and End).</summary>
    [Parameter] public EventCallback<XFRangeChangeEventArgs> RangeChanged { get; set; }

    /// <summary>Content to render inside the navigator (e.g., a mini chart).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private enum HandleType { Start, End }
    private HandleType? _activeHandle;
    private double _dragStartX;
    private double _dragStartValue;

    private async Task OnMouseDownAsync(MouseEventArgs e)
    {
        if (_activeHandle.HasValue) return;

        // Calculate percentage from mouse position relative to the element
        // Use offsetX which is relative to the target element
        var pct = Math.Clamp(e.OffsetX / 300, 0, 1); // 300 is approximate viewBox width
        pct = Math.Clamp(pct, 0, 1);

        // Move the nearest handle
        var distToStart = Math.Abs(pct - Start);
        var distToEnd = Math.Abs(pct - End);

        if (distToStart < distToEnd)
        {
            await UpdateStartAsync(pct);
        }
        else
        {
            await UpdateEndAsync(pct);
        }
    }

    private async Task StartDragAsync(HandleType handle)
    {
        _activeHandle = handle;
        _dragStartX = 0;
        _dragStartValue = handle == HandleType.Start ? Start : End;
    }

    private async Task UpdateStartAsync(double value)
    {
        value = Math.Clamp(value, 0, End - 0.01);
        Start = value;
        await StartChanged.InvokeAsync(Start);
        await NotifyRangeChanged();
    }

    private async Task UpdateEndAsync(double value)
    {
        value = Math.Clamp(value, Start + 0.01, 1);
        End = value;
        await EndChanged.InvokeAsync(End);
        await NotifyRangeChanged();
    }

    private async Task NotifyRangeChanged()
    {
        await RangeChanged.InvokeAsync(new XFRangeChangeEventArgs(Start, End));
    }

    private string FormatHandleLabel(double value)
    {
        var axisValue = AxisMin + (AxisMax - AxisMin) * value;
        if (!string.IsNullOrEmpty(HandleLabelFormat))
        {
            return string.Format(HandleLabelFormat, axisValue);
        }
        return axisValue.ToString("0.#");
    }

    private List<string> GetAxisTicks()
    {
        var ticks = new List<string>();
        for (var i = 0; i <= TickCount; i++)
        {
            var pct = (double)i / TickCount;
            var val = AxisMin + (AxisMax - AxisMin) * pct;
            if (!string.IsNullOrEmpty(AxisFormat))
            {
                ticks.Add(string.Format(AxisFormat, val));
            }
            else
            {
                ticks.Add(val.ToString("0.#"));
            }
        }
        return ticks;
    }
}

/// <summary>
/// Event arguments for range change events.
/// </summary>
public record XFRangeChangeEventArgs(double Start, double End);
