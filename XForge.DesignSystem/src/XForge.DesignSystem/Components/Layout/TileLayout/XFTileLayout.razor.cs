using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.TileLayout
{
    public partial class XFTileLayout : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-tile-layout";

        private readonly List<XFTileLayoutItem> _tiles = new();
        private bool _isEditMode;
        private string? _selectedTileId;
        private XFTileLayoutItem? _menuTile;
        private XFTileLayoutItem? _draggedTile;
        private XFTileLayoutItem? _resizingTile;

        #region Parameters

        /// <summary>Title.</summary>
        [Parameter] public string Title { get; set; } = "";

        /// <summary>Custom title template.</summary>
        [Parameter] public RenderFragment? TitleTemplate { get; set; }

        /// <summary>Header actions template.</summary>
        [Parameter] public RenderFragment? HeaderActions { get; set; }

        /// <summary>Whether to show the header.</summary>
        [Parameter] public bool ShowHeader { get; set; } = true;

        /// <summary>Tile definitions.</summary>
        [Parameter] public IReadOnlyList<XFTileLayoutOption> Tiles { get; set; } = Array.Empty<XFTileLayoutOption>();

        /// <summary>Number of grid columns.</summary>
        [Parameter] public int Columns { get; set; } = 12;

        /// <summary>Grid gap in pixels.</summary>
        [Parameter] public int Gap { get; set; } = 16;

        /// <summary>Row height in pixels.</summary>
        [Parameter] public int RowHeight { get; set; } = 100;

        /// <summary>Whether edit mode is allowed.</summary>
        [Parameter] public bool AllowEdit { get; set; } = true;

        /// <summary>Whether to allow adding tiles.</summary>
        [Parameter] public bool AllowAdd { get; set; } = true;

        /// <summary>Whether to allow removing tiles.</summary>
        [Parameter] public bool AllowRemove { get; set; } = true;

        /// <summary>Tile context menu template.</summary>
        [Parameter] public RenderFragment<XFTileLayoutItem>? TileMenuTemplate { get; set; }

        /// <summary>Tile clicked callback.</summary>
        [Parameter] public EventCallback<XFTileLayoutItem> OnTileClick { get; set; }

        /// <summary>Tile double-clicked callback.</summary>
        [Parameter] public EventCallback<XFTileLayoutItem> OnTileDoubleClick { get; set; }

        /// <summary>Tile added callback.</summary>
        [Parameter] public EventCallback<XFTileLayoutItem> OnTileAdded { get; set; }

        /// <summary>Tile removed callback.</summary>
        [Parameter] public EventCallback<XFTileLayoutItem> OnTileRemoved { get; set; }

        /// <summary>Tile reordered callback.</summary>
        [Parameter] public EventCallback<XFTileReorderEventArgs> OnTileReorder { get; set; }

        /// <summary>Tile resized callback.</summary>
        [Parameter] public EventCallback<XFTileResizeEventArgs> OnTileResize { get; set; }

        /// <summary>Edit mode changed callback.</summary>
        [Parameter] public EventCallback<bool> OnEditModeChanged { get; set; }

        #endregion

        #region Labels

        private string AriaLabel => L("tilelayout.aria_label", "Tile layout");
        private string EditModeText => L("tilelayout.edit_mode", "Edit layout");
        private string AddTileText => L("tilelayout.add_tile", "Add tile");
        private string RemoveTileText => L("tilelayout.remove_tile", "Remove tile");

        #endregion

        #region CSS

        private bool IsEditMode => _isEditMode;

        private string GridClass => string.Join(" ",
            "grid auto-rows-min",
            IsEditMode ? "ring-2 ring-[var(--color-primary)]/20 rounded-xl p-2" : "");

        private string HeaderButtonClass => string.Join(" ",
            "inline-flex h-8 w-8 items-center justify-center rounded-lg",
            "border border-border-light bg-bg-surface text-text-secondary",
            "hover:border-[var(--color-primary)] hover:text-primary transition-colors");

        private string TileClass(XFTileLayoutItem tile, bool isActive) => string.Join(" ",
            "relative rounded-xl border bg-bg-surface overflow-hidden transition-all duration-200",
            isActive ? "border-[var(--color-primary)] shadow-md ring-1 ring-[var(--color-primary)]/20" : "border-border-light hover:border-[var(--color-primary)]/50",
            IsEditMode ? "cursor-move" : "cursor-pointer",
            tile.CssClass);

        private string TileHeaderClass => "flex items-center justify-between px-3 py-2 border-b border-border-light";

        private string TileContentClass => "flex-1 overflow-auto p-3";

        private string TileMenuButtonClass => string.Join(" ",
            "inline-flex h-6 w-6 items-center justify-center rounded",
            "text-text-muted hover:bg-surface-100 hover:text-text-primary transition-colors");

        private string ResizeHandleClass => string.Join(" ",
            "absolute right-0 bottom-0 w-4 h-4 cursor-se-resize",
            "bg-[var(--color-primary)]/20 hover:bg-[var(--color-primary)]/40 rounded-tl transition-colors");

        private string AddTileClass => string.Join(" ",
            "flex flex-col items-center justify-center rounded-xl",
            "border-2 border-dashed border-border-light",
            "hover:border-[var(--color-primary)] cursor-pointer transition-colors min-h-[120px]");

        #endregion

        #region Lifecycle

        protected override void OnParametersSet()
        {
            SyncTiles();
        }

        private void SyncTiles()
        {
            _tiles.Clear();
            foreach (var option in Tiles)
            {
                _tiles.Add(new XFTileLayoutItem
                {
                    Id = option.Id,
                    Title = option.Title,
                    ColumnSpan = option.ColumnSpan,
                    RowSpan = option.RowSpan,
                    ShowHeader = option.ShowHeader,
                    Icon = option.Icon,
                    IconColor = option.IconColor,
                    Content = option.Content,
                    Resizable = option.Resizable,
                    Draggable = option.Draggable,
                    ShowMenu = option.ShowMenu,
                    CssClass = option.CssClass
                });
            }
        }

        #endregion

        #region Edit Mode

        private async Task ToggleEditMode()
        {
            _isEditMode = !_isEditMode;
            await OnEditModeChanged.InvokeAsync(_isEditMode);
            StateHasChanged();
        }

        #endregion

        #region Tile Interaction

        private async Task HandleTileClick(XFTileLayoutItem tile)
        {
            if (_isEditMode) return;
            _selectedTileId = tile.Id;
            await OnTileClick.InvokeAsync(tile);
            StateHasChanged();
        }

        private async Task HandleTileDoubleClick(XFTileLayoutItem tile)
        {
            await OnTileDoubleClick.InvokeAsync(tile);
        }

        private void OnTileDragStart(XFTileLayoutItem tile)
        {
            _draggedTile = tile;
        }

        private async Task OnTileDrop(XFTileLayoutItem targetTile)
        {
            if (_draggedTile == null || _draggedTile.Id == targetTile.Id)
            {
                _draggedTile = null;
                return;
            }

            var fromIndex = _tiles.IndexOf(_draggedTile);
            var toIndex = _tiles.IndexOf(targetTile);

            if (fromIndex >= 0 && toIndex >= 0)
            {
                _tiles.RemoveAt(fromIndex);
                _tiles.Insert(toIndex, _draggedTile);

                await OnTileReorder.InvokeAsync(new XFTileReorderEventArgs
                {
                    TileId = _draggedTile.Id,
                    FromIndex = fromIndex,
                    ToIndex = toIndex
                });
            }

            _draggedTile = null;
            StateHasChanged();
        }

        private void ToggleTileMenu(XFTileLayoutItem tile)
        {
            _menuTile = _menuTile?.Id == tile.Id ? null : tile;
            StateHasChanged();
        }

        #endregion

        #region Tile CRUD

        private async Task AddTile()
        {
            var newTile = new XFTileLayoutItem
            {
                Id = $"tile-{Guid.NewGuid():N}",
                Title = "New Tile",
                ColumnSpan = 4,
                RowSpan = 2
            };

            _tiles.Add(newTile);
            await OnTileAdded.InvokeAsync(newTile);
            StateHasChanged();
        }

        private async Task RemoveTile(XFTileLayoutItem tile)
        {
            _tiles.Remove(tile);
            await OnTileRemoved.InvokeAsync(tile);
            StateHasChanged();
        }

        #endregion

        #region Resize

        private void StartResize(XFTileLayoutItem tile)
        {
            _resizingTile = tile;
        }

        #endregion
    }

    #region Supporting types

    public class XFTileLayoutOption
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public int ColumnSpan { get; set; } = 4;
        public int RowSpan { get; set; } = 2;
        public bool ShowHeader { get; set; } = true;
        public string? Icon { get; set; }
        public string? IconColor { get; set; }
        public RenderFragment? Content { get; set; }
        public bool Resizable { get; set; } = true;
        public bool Draggable { get; set; } = true;
        public bool ShowMenu { get; set; } = true;
        public string? CssClass { get; set; }
    }

    public class XFTileLayoutItem
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public int ColumnSpan { get; set; } = 4;
        public int RowSpan { get; set; } = 2;
        public bool ShowHeader { get; set; } = true;
        public string? Icon { get; set; }
        public string? IconColor { get; set; }
        public RenderFragment? Content { get; set; }
        public bool Resizable { get; set; } = true;
        public bool Draggable { get; set; } = true;
        public bool ShowMenu { get; set; } = true;
        public string? CssClass { get; set; }
    }

    public class XFTileReorderEventArgs
    {
        public string TileId { get; set; } = "";
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }

    public class XFTileResizeEventArgs
    {
        public string TileId { get; set; } = "";
        public int NewColumnSpan { get; set; }
        public int NewRowSpan { get; set; }
    }

    #endregion
}
