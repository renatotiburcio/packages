using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Utilities.DragDrop;

/// <summary>
/// Drag-and-drop zone component for reordering and moving items between zones.
/// </summary>
/// <typeparam name="TItem">The type of items in the zone.</typeparam>
public partial class XFDropZone<TItem> : XForgeComponentBase
{
    protected override string BaseClass => "xf-drop-zone rounded-xl border-2 border-dashed border-surface-200 bg-surface-50 p-4 transition-all";

    /// <summary>Items in this zone.</summary>
    [Parameter] public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();

    /// <summary>Callback when items change.</summary>
    [Parameter] public EventCallback<IReadOnlyList<TItem>> ItemsChanged { get; set; }

    /// <summary>Zone identifier for multi-zone scenarios.</summary>
    [Parameter] public object? ZoneValue { get; set; }

    /// <summary>Header text or content.</summary>
    [Parameter] public string? Header { get; set; }

    /// <summary>Footer content.</summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    /// <summary>Template for rendering each item.</summary>
    [Parameter] public RenderFragment<XFDropZoneItemContext<TItem>>? ItemTemplate { get; set; }

    /// <summary>Template for empty state.</summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>Whether items can be reordered within this zone.</summary>
    [Parameter] public bool AllowReorder { get; set; } = true;

    /// <summary>Whether items can be moved to other zones.</summary>
    [Parameter] public bool AllowMove { get; set; } = true;

    /// <summary>Callback when an item is dropped into this zone.</summary>
    [Parameter] public EventCallback<XFDropZoneDropEventArgs<TItem>> ItemDropped { get; set; }

    /// <summary>Callback to validate if an item can be dropped.</summary>
    [Parameter] public Func<TItem, XFDropZone<TItem>, bool>? CanDrop { get; set; }

    private bool _isDragOver;
    private TItem? _draggedItem;

    /// <summary>
    /// Registers this zone with the container (if any).
    /// </summary>
    internal void RegisterZone() { }

    private void OnDragOverAsync(DragEventArgs args)
    {
        var canAccept = CanDrop?.Invoke(_draggedItem!, this) ?? true;
        args.DataTransfer.DropEffect = canAccept ? "move" : "none";
        _isDragOver = canAccept;
    }

    private void OnDragLeaveAsync(DragEventArgs args)
    {
        _isDragOver = false;
    }

    private async Task OnDragStartAsync(DragEventArgs args, TItem item)
    {
        _draggedItem = item;
        args.DataTransfer.EffectAllowed = "move";
    }

    private async Task OnDropAsync(DragEventArgs args)
    {
        _isDragOver = false;

        if (_draggedItem is null) return;

        var canAccept = CanDrop?.Invoke(_draggedItem, this) ?? true;
        if (!canAccept) return;

        await ItemDropped.InvokeAsync(new XFDropZoneDropEventArgs<TItem>(_draggedItem, this));
        _draggedItem = default;
    }

    /// <summary>
    /// Adds an item to this zone.
    /// </summary>
    public async Task AddItemAsync(TItem item)
    {
        var list = Items.ToList();
        list.Add(item);
        Items = list;
        await ItemsChanged.InvokeAsync(Items);
    }

    /// <summary>
    /// Removes an item from this zone.
    /// </summary>
    public async Task RemoveItemAsync(TItem item)
    {
        var list = Items.ToList();
        list.Remove(item);
        Items = list;
        await ItemsChanged.InvokeAsync(Items);
    }
}

/// <summary>
/// Context for rendering items in a drop zone.
/// </summary>
public class XFDropZoneItemContext<TItem>
{
    public TItem Item { get; }
    public XFDropZone<TItem> Zone { get; }

    public XFDropZoneItemContext(TItem item, XFDropZone<TItem> zone)
    {
        Item = item;
        Zone = zone;
    }
}

/// <summary>
/// Event arguments for drop events.
/// </summary>
public class XFDropZoneDropEventArgs<TItem>
{
    public TItem Item { get; }
    public XFDropZone<TItem> TargetZone { get; }

    public XFDropZoneDropEventArgs(TItem item, XFDropZone<TItem> targetZone)
    {
        Item = item;
        TargetZone = targetZone;
    }
}
