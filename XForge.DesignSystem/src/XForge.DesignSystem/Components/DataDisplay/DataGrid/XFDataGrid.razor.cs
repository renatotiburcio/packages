using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public partial class XFDataGrid<TItem> : XForgeComponentBase, IAsyncDisposable
    {
        private readonly List<XFDataGridColumn<TItem>> columns = new();

        [Inject] private IXFLocalizer? Localizer { get; set; }

        private string L(string key, string fallback) => Localizer?[key] ?? fallback;
        private string L(string key, string fallback, params object[] args) => Localizer?.Format(key, args) ?? string.Format(fallback, args);

        private string? internalSortKey;
        private XFDataGridSortDirection internalSortDirection = XFDataGridSortDirection.None;
        private int internalPage = 1;
        private int internalPageSize = 10;
        private TItem? internalSelectedItem;
        private IReadOnlyList<TItem> internalSelectedItems = Array.Empty<TItem>();
        private string? previousFilterText;
        private IReadOnlyList<TItem> providerItems = Array.Empty<TItem>();
        private int providerTotalItems;
        private XFDataGridDataRequest? previousProviderRequest;
        private bool providerLoading;
        private readonly Dictionary<string, bool> columnVisibility = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> columnWidths = new(StringComparer.Ordinal);
        private TItem? internalEditingItem;
        private readonly List<XFDataGridSortDescriptor> sortDescriptors = new();
        private readonly Dictionary<string, string?> columnFilterValues = new(StringComparer.Ordinal);
        private System.Threading.Timer? filterDelayTimer;
        private string? pendingFilterColumnKey;
        private string? pendingFilterValue;

        protected override string BaseClass => "xf-datagrid";

        [Parameter] public IEnumerable<TItem>? Items { get; set; }
        [Parameter] public XFDataGridItemsProvider<TItem>? ItemsProvider { get; set; }
        [Parameter] public RenderFragment? Columns { get; set; }
        [Parameter] public string? Caption { get; set; }
        [Parameter] public RenderFragment? EmptyTemplate { get; set; }
        [Parameter] public RenderFragment? LoadingTemplate { get; set; }
        [Parameter] public RenderFragment<TItem>? DetailTemplate { get; set; }
        [Parameter] public RenderFragment<TItem>? EditTemplate { get; set; }
        [Parameter] public string? GroupBy { get; set; }
        [Parameter] public Func<TItem, object?>? GroupKeySelector { get; set; }
        [Parameter] public RenderFragment<XFDataGridGroup<TItem>>? GroupHeaderTemplate { get; set; }
        [Parameter] public RenderFragment<XFDataGridGroup<TItem>>? GroupFooterTemplate { get; set; }
        [Parameter] public string EmptyText { get; set; } = "Nenhum registro encontrado.";
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Loading { get; set; }
        [Parameter] public bool Striped { get; set; }
        [Parameter] public bool Hoverable { get; set; } = true;
        [Parameter] public bool StickyHeader { get; set; } = true;
        [Parameter] public bool StickyFooter { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool AllowSorting { get; set; } = true;
        [Parameter] public bool AllowPaging { get; set; }
        [Parameter] public bool ShowPageSize { get; set; }
        [Parameter] public int Page { get; set; } = 1;
        [Parameter] public EventCallback<int> PageChanged { get; set; }
        [Parameter] public int PageSize { get; set; } = 10;
        [Parameter] public EventCallback<int> PageSizeChanged { get; set; }
        [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = Array.Empty<int>();
        [Parameter] public bool ServerSide { get; set; }
        [Parameter] public int? TotalItems { get; set; }
        [Parameter] public bool Virtualize { get; set; }
        [Parameter] public float ItemSize { get; set; } = 44;
        [Parameter] public int OverscanCount { get; set; } = 3;
        [Parameter] public bool ShowColumnChooser { get; set; }
        [Parameter] public string ColumnChooserLabel { get; set; } = "Colunas";
        [Parameter] public string ShowColumnLabelFormat { get; set; } = "Mostrar {0}";
        [Parameter] public string HideColumnLabelFormat { get; set; } = "Ocultar {0}";
        [Parameter] public bool AllowColumnReorder { get; set; }
        [Parameter] public bool AllowColumnResize { get; set; }
        [Parameter] public string MoveColumnLeftLabelFormat { get; set; } = "Mover {0} para esquerda";
        [Parameter] public string MoveColumnRightLabelFormat { get; set; } = "Mover {0} para direita";
        [Parameter] public string DecreaseColumnWidthLabelFormat { get; set; } = "Reduzir largura de {0}";
        [Parameter] public string IncreaseColumnWidthLabelFormat { get; set; } = "Aumentar largura de {0}";
        [Parameter] public EventCallback<XFDataGridColumnVisibilityChangedEventArgs> ColumnVisibilityChanged { get; set; }
        [Parameter] public EventCallback<XFDataGridColumnReorderedEventArgs> ColumnReordered { get; set; }
        [Parameter] public EventCallback<XFDataGridColumnResizedEventArgs> ColumnResized { get; set; }
        [Parameter] public bool AllowEditing { get; set; }
        [Parameter] public string EditColumnTitle { get; set; } = "Acoes";
        [Parameter] public string EditLabel { get; set; } = "Editar";
        [Parameter] public string SaveLabel { get; set; } = "Salvar";
        [Parameter] public string CancelLabel { get; set; } = "Cancelar";
        [Parameter] public string EditActionLabelFormat { get; set; } = "{0} {1}";
        [Parameter] public string SaveActionLabelFormat { get; set; } = "{0} {1}";
        [Parameter] public string CancelActionLabelFormat { get; set; } = "{0} {1}";
        [Parameter] public TItem? EditingItem { get; set; }
        [Parameter] public EventCallback<TItem?> EditingItemChanged { get; set; }
        [Parameter] public EventCallback<TItem> RowEditStarted { get; set; }
        [Parameter] public EventCallback<TItem> RowEditCommitted { get; set; }
        [Parameter] public EventCallback<TItem> RowEditCanceled { get; set; }
        [Parameter] public string? SortKey { get; set; }
        [Parameter] public XFDataGridSortDirection SortDirection { get; set; } = XFDataGridSortDirection.None;
        [Parameter] public string SortAscendingLabelFormat { get; set; } = "Ordenar {0} crescente";
        [Parameter] public string SortDescendingLabelFormat { get; set; } = "Ordenar {0} decrescente";
        [Parameter] public string ClearSortLabelFormat { get; set; } = "Remover ordenacao de {0}";
        [Parameter] public EventCallback<XFDataGridSortChangedEventArgs> SortChanged { get; set; }
        [Parameter] public string? FilterText { get; set; }
        [Parameter] public IReadOnlyList<string>? FilterKeys { get; set; }
        [Parameter] public Func<TItem, string, bool>? FilterPredicate { get; set; }
        [Parameter] public XFDataGridSelectionMode SelectionMode { get; set; } = XFDataGridSelectionMode.None;
        [Parameter] public TItem? SelectedItem { get; set; }
        [Parameter] public EventCallback<TItem?> SelectedItemChanged { get; set; }
        [Parameter] public IReadOnlyList<TItem>? SelectedItems { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<TItem>> SelectedItemsChanged { get; set; }
        [Parameter] public EventCallback<TItem> RowClicked { get; set; }
        [Parameter] public int? ViewportRows { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFListVariant Variant { get; set; } = XFListVariant.Bordered;
        [Parameter] public string EmptyGroupLabel { get; set; } = "Sem grupo";
        [Parameter] public XFDataGridFilterMode FilterMode { get; set; } = XFDataGridFilterMode.None;
        [Parameter] public int FilterDelay { get; set; } = 400;
        [Parameter] public bool AllowMultiColumnSorting { get; set; } = true;
        [Parameter] public bool ShowMultiColumnSortingIndex { get; set; } = true;
        [Parameter] public bool GotoFirstPageOnSort { get; set; } = true;
        [Parameter] public bool ShowAggregates { get; set; } = false;
        [Parameter] public string AggregateRowLabel { get; set; } = "Total:";
        [Parameter] public EventCallback<XFDataGridColumnFilterEventArgs> ColumnFilterChanged { get; set; }
        [Parameter] public bool AllowExport { get; set; }
        [Parameter] public string ExportFileName { get; set; } = "export";
        [Parameter] public string ExportCsvLabel { get; set; } = "Exportar CSV";
        [Parameter] public string ExportExcelLabel { get; set; } = "Exportar Excel";
        [Parameter] public string ExportDelimiter { get; set; } = ";";
        [Parameter] public bool ExportAllPages { get; set; } = true;

        // assinatura de render: grid grande — evita re-render de centenas de celulas quando
        // o componente pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Caption, EmptyText, GroupBy, ColumnChooserLabel,
                ShowColumnLabelFormat, HideColumnLabelFormat, ShowColumnChooser, Loading, ExportAllPages);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        internal IReadOnlyList<XFDataGridColumn<TItem>> VisibleColumns => columns.Where(IsColumnVisible).ToArray();

        private IReadOnlyList<XFDataGridColumn<TItem>> OperationalColumns => columns.ToArray();

        private bool UsesItemsProvider => ItemsProvider is not null;

        private IReadOnlyList<TItem> SourceItems => UsesItemsProvider ? providerItems : Items?.ToArray() ?? Array.Empty<TItem>();

        private string EffectiveSortKey => SortKey ?? internalSortKey ?? "";

        private XFDataGridSortDirection EffectiveSortDirection => SortKey is not null ? SortDirection : internalSortDirection;

        private int CurrentPage => Math.Max(1, PageChanged.HasDelegate ? Page : internalPage);

        private int CurrentPageSize => Math.Max(1, PageSizeChanged.HasDelegate ? PageSize : internalPageSize);

        private bool IsServerDriven => ServerSide || UsesItemsProvider;

        private bool EffectiveLoading => Loading || providerLoading;

        private int TotalCount => IsServerDriven ? Math.Max(0, UsesItemsProvider ? providerTotalItems : TotalItems ?? SourceItems.Count) : FilteredItems.Count;

        private IReadOnlyList<TItem> FilteredItems => IsServerDriven ? SourceItems : ApplyFiltering(SourceItems).ToArray();

        private IReadOnlyList<TItem> ProcessedItems => IsServerDriven ? SourceItems : ApplySorting(FilteredItems).ToArray();

        private IReadOnlyList<TItem> PagedItems => AllowPaging && !IsServerDriven
            ? ProcessedItems.Skip((CurrentPage - 1) * CurrentPageSize).Take(CurrentPageSize).ToArray()
            : ProcessedItems;

        private bool HasGrouping => GroupKeySelector is not null || !string.IsNullOrWhiteSpace(GroupBy);

        private IReadOnlyList<XFDataGridGroup<TItem>> GroupedItems => CreateGroups(PagedItems).ToArray();

        private int VisibleColumnSpan => Math.Max(1, VisibleColumns.Count + (AllowEditing ? 1 : 0));

        private ICollection<TItem> VirtualizedItems => PagedItems as ICollection<TItem> ?? PagedItems.ToArray();

        private int EffectiveOverscanCount => Math.Max(0, OverscanCount);

        private string? VirtualizedAttribute => Virtualize ? "true" : null;

        private TItem? EffectiveSelectedItem => SelectedItem is not null ? SelectedItem : internalSelectedItem;

        private IReadOnlyList<TItem> EffectiveSelectedItems => SelectedItems ?? internalSelectedItems;

        private bool IsMultiColumnSortActive => sortDescriptors.Count(d => d.Direction != XFDataGridSortDirection.None) > 1;

        private string WrapperClass => Join("w-full overflow-auto rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.035)]", ViewportClass);

        private string FilterRowClass => "bg-surface-50/50";

        private string FilterCellClass(XFDataGridColumn<TItem> column) =>
            Join("border-b border-border-light px-2 py-1.5", AlignClass(column.Align));

        private string FilterInputClass =>
            "w-full rounded border border-border-light bg-bg-surface px-2 py-1 text-xs text-text-primary outline-none transition-colors focus:border-primary focus:ring-1 focus:ring-primary/20 placeholder:text-text-muted";

        private string AggregateRowClass =>
            "bg-surface-80 text-xs font-semibold text-text-primary";

        private string AggregateFooterClass =>
            "bg-surface-50/95 font-semibold backdrop-blur supports-[backdrop-filter]:bg-surface-50/85";

        protected override void OnInitialized()
        {
            internalPage = Math.Max(1, Page);
            internalPageSize = Math.Max(1, PageSize);
            internalSortKey = SortKey;
            internalSortDirection = SortDirection;
            internalSelectedItem = SelectedItem;
            internalSelectedItems = SelectedItems ?? Array.Empty<TItem>();
            internalEditingItem = EditingItem;
            previousFilterText = FilterText;
        }

        protected override void OnParametersSet()
        {
            if (!string.Equals(previousFilterText, FilterText, StringComparison.Ordinal))
            {
                internalPage = 1;
                previousFilterText = FilterText;
            }

            if (PageSize > 0 && !PageSizeChanged.HasDelegate)
            {
                internalPageSize = PageSize;
            }

            if (SelectedItem is not null)
            {
                internalSelectedItem = SelectedItem;
            }

            if (SelectedItems is not null)
            {
                internalSelectedItems = SelectedItems;
            }

            if (EditingItem is not null)
            {
                internalEditingItem = EditingItem;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadProviderIfNeededAsync();
        }

        private async Task LoadProviderIfNeededAsync()
        {
            if (ItemsProvider is null)
            {
                previousProviderRequest = null;
                providerItems = Array.Empty<TItem>();
                providerTotalItems = 0;
                providerLoading = false;
                return;
            }

            var request = CreateProviderRequest();
            if (RequestsEqual(request, previousProviderRequest))
            {
                return;
            }

            previousProviderRequest = request;
            providerLoading = true;

            try
            {
                var result = await ItemsProvider.Invoke(request);
                providerItems = result.Items;
                providerTotalItems = Math.Max(0, result.TotalItems);
            }
            finally
            {
                providerLoading = false;
            }
        }

        private async Task ForceProviderReloadAsync()
        {
            if (ItemsProvider is null)
            {
                return;
            }

            previousProviderRequest = null;
            await LoadProviderIfNeededAsync();
        }

        private XFDataGridDataRequest CreateProviderRequest()
        {
            return new XFDataGridDataRequest(
                CurrentPage,
                CurrentPageSize,
                EffectiveSortKey,
                EffectiveSortDirection,
                FilterText,
                ResolveFilterKeys());
        }

        private static bool RequestsEqual(XFDataGridDataRequest request, XFDataGridDataRequest? previous)
        {
            return previous is not null
                && request.Page == previous.Page
                && request.PageSize == previous.PageSize
                && string.Equals(request.SortKey, previous.SortKey, StringComparison.Ordinal)
                && request.SortDirection == previous.SortDirection
                && string.Equals(request.FilterText, previous.FilterText, StringComparison.Ordinal)
                && request.FilterKeys.SequenceEqual(previous.FilterKeys, StringComparer.Ordinal);
        }

        protected override string GetComponentClass()
        {
            var width = FullWidth ? "w-full" : "w-auto";
            return GetClass($"{BaseClass} border-separate border-spacing-0 text-left text-sm {width} {VariantClass}");
        }

        internal void Register(XFDataGridColumn<TItem> column)
        {
            if (!columns.Contains(column))
            {
                columns.Add(column);
                var key = column.ResolvedKey;
                columnVisibility.TryAdd(key, column.Visible);
                if (column.WidthPixels is > 0)
                {
                    columnWidths[key] = NormalizeColumnWidth(column, column.WidthPixels.Value);
                }

                _ = InvokeAsync(StateHasChanged);
            }
        }

        internal void Unregister(XFDataGridColumn<TItem> column)
        {
            if (columns.Remove(column))
            {
                _ = InvokeAsync(StateHasChanged);
            }
        }

        private bool IsColumnVisible(XFDataGridColumn<TItem> column)
        {
            return columnVisibility.TryGetValue(column.ResolvedKey, out var visible) ? visible : column.Visible;
        }

        private string GetColumnVisibilityLabel(XFDataGridColumn<TItem> column, bool visible)
        {
            return FormatColumnLabel(visible ? HideColumnLabelFormat : ShowColumnLabelFormat, column);
        }

        private string FormatColumnLabel(string format, XFDataGridColumn<TItem> column)
        {
            return string.Format(CultureInfo.CurrentCulture, format, column.ResolvedTitle);
        }

        private async Task ToggleColumnVisibilityAsync(XFDataGridColumn<TItem> column)
        {
            if (!column.Hideable || VisibleColumns.Count == 1 && IsColumnVisible(column))
            {
                return;
            }

            var key = column.ResolvedKey;
            var visible = !IsColumnVisible(column);
            columnVisibility[key] = visible;

            if (ColumnVisibilityChanged.HasDelegate)
            {
                await ColumnVisibilityChanged.InvokeAsync(new XFDataGridColumnVisibilityChangedEventArgs(key, visible));
            }
        }

        private async Task MoveColumnAsync(XFDataGridColumn<TItem> column, int direction)
        {
            if (!AllowColumnReorder)
            {
                return;
            }

            var oldIndex = columns.IndexOf(column);
            var newIndex = Math.Clamp(oldIndex + direction, 0, columns.Count - 1);
            if (oldIndex < 0 || oldIndex == newIndex)
            {
                return;
            }

            columns.RemoveAt(oldIndex);
            columns.Insert(newIndex, column);

            if (ColumnReordered.HasDelegate)
            {
                await ColumnReordered.InvokeAsync(new XFDataGridColumnReorderedEventArgs(column.ResolvedKey, oldIndex, newIndex));
            }
        }

        private async Task ResizeColumnAsync(XFDataGridColumn<TItem> column, int direction)
        {
            if (!AllowColumnResize || !column.Resizable)
            {
                return;
            }

            var current = GetColumnWidthPixels(column) ?? DefaultColumnWidthPixels(column.Width);
            var next = NormalizeColumnWidth(column, current + direction * Math.Max(1, column.ResizeStepPixels));
            columnWidths[column.ResolvedKey] = next;

            if (ColumnResized.HasDelegate)
            {
                await ColumnResized.InvokeAsync(new XFDataGridColumnResizedEventArgs(column.ResolvedKey, next));
            }
        }

        private int? GetColumnWidthPixels(XFDataGridColumn<TItem> column)
        {
            return columnWidths.TryGetValue(column.ResolvedKey, out var width) ? width : column.WidthPixels;
        }

        private static int NormalizeColumnWidth(XFDataGridColumn<TItem> column, int width)
        {
            return Math.Clamp(width, Math.Max(1, column.MinWidthPixels), Math.Max(column.MinWidthPixels, column.MaxWidthPixels));
        }

        private static int DefaultColumnWidthPixels(XFDataGridColumnWidth width) => width switch
        {
            XFDataGridColumnWidth.Xs => 80,
            XFDataGridColumnWidth.Sm => 112,
            XFDataGridColumnWidth.Md => 160,
            XFDataGridColumnWidth.Lg => 224,
            XFDataGridColumnWidth.Xl => 288,
            XFDataGridColumnWidth.Actions => 96,
            _ => 128
        };

        private bool CanMoveColumn(XFDataGridColumn<TItem> column, int direction)
        {
            var index = columns.IndexOf(column);
            return AllowColumnReorder && index >= 0 && index + direction >= 0 && index + direction < columns.Count;
        }

        private bool CanResizeColumn(XFDataGridColumn<TItem> column, int direction)
        {
            if (!AllowColumnResize || !column.Resizable)
            {
                return false;
            }

            var current = GetColumnWidthPixels(column) ?? DefaultColumnWidthPixels(column.Width);
            var next = current + direction * Math.Max(1, column.ResizeStepPixels);
            return next >= column.MinWidthPixels && next <= column.MaxWidthPixels;
        }

        public XFDataGridState CaptureState()
        {
            return new XFDataGridState(
                CurrentPage,
                CurrentPageSize,
                EffectiveSortKey,
                EffectiveSortDirection,
                FilterText,
                GroupBy,
                columns.Select(column => column.ResolvedKey).ToArray(),
                OperationalColumns.ToDictionary(column => column.ResolvedKey, IsColumnVisible, StringComparer.Ordinal),
                new Dictionary<string, int>(columnWidths, StringComparer.Ordinal));
        }

        public async Task ApplyStateAsync(XFDataGridState state)
        {
            internalPage = Math.Max(1, state.Page);
            internalPageSize = Math.Max(1, state.PageSize);
            internalSortKey = string.IsNullOrWhiteSpace(state.SortKey) ? null : state.SortKey;
            internalSortDirection = state.SortDirection;

            columnVisibility.Clear();
            foreach (var pair in state.ColumnVisibility)
            {
                columnVisibility[pair.Key] = pair.Value;
            }

            columnWidths.Clear();
            foreach (var pair in state.ColumnWidths)
            {
                columnWidths[pair.Key] = Math.Max(1, pair.Value);
            }

            if (state.ColumnOrder.Count > 0)
            {
                var order = state.ColumnOrder
                    .Select((key, index) => new { key, index })
                    .ToDictionary(item => item.key, item => item.index, StringComparer.Ordinal);

                columns.Sort((left, right) =>
                    (order.TryGetValue(left.ResolvedKey, out var leftIndex) ? leftIndex : int.MaxValue)
                        .CompareTo(order.TryGetValue(right.ResolvedKey, out var rightIndex) ? rightIndex : int.MaxValue));
            }

            await ForceProviderReloadAsync();
            await InvokeAsync(StateHasChanged);
        }

        public async Task ExportToCsvAsync()
        {
            var data = ExportAllPages ? ProcessedItems : PagedItems;
            var columns = VisibleColumns;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Join(ExportDelimiter, columns.Select(c => c.ResolvedTitle)));

            foreach (var item in data)
            {
                var values = columns.Select(c =>
                {
                    var value = GetPropertyValue(item, c.Property);
                    var text = value?.ToString() ?? "";
                    return text.Contains(ExportDelimiter) || text.Contains("\"") || text.Contains("\n")
                        ? $"\"{text.Replace("\"", "\"\"")}\""
                        : text;
                });
                sb.AppendLine(string.Join(ExportDelimiter, values));
            }

            var csvBytes = System.Text.Encoding.UTF8.GetPreamble()
                .Concat(System.Text.Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

            await DownloadFileAsync($"{ExportFileName}.csv", csvBytes, "text/csv;charset=utf-8");
        }

        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        private static object? GetPropertyValue(TItem item, string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            return item?.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(item);
        }

        private async Task DownloadFileAsync(string fileName, byte[] data, string contentType)
        {
            var jsRuntime = (IJSRuntime)JS;
            var base64 = Convert.ToBase64String(data);
            await jsRuntime.InvokeVoidAsync("eval", $@"
                const link = document.createElement('a');
                link.href = 'data:{contentType};base64,{base64}';
                link.download = '{fileName}';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            ");
        }

        private TItem? EffectiveEditingItem => EditingItem is not null ? EditingItem : internalEditingItem;

        private bool IsEditing(TItem item)
        {
            return EffectiveEditingItem is not null && EqualityComparer<TItem>.Default.Equals(EffectiveEditingItem, item);
        }

        private async Task StartEditAsync(TItem item)
        {
            if (!AllowEditing)
            {
                return;
            }

            internalEditingItem = item;
            if (EditingItemChanged.HasDelegate)
            {
                await EditingItemChanged.InvokeAsync(item);
            }

            if (RowEditStarted.HasDelegate)
            {
                await RowEditStarted.InvokeAsync(item);
            }
        }

        private async Task CommitEditAsync(TItem item)
        {
            if (RowEditCommitted.HasDelegate)
            {
                await RowEditCommitted.InvokeAsync(item);
            }

            await ClearEditingItemAsync();
        }

        private async Task CancelEditAsync(TItem item)
        {
            if (RowEditCanceled.HasDelegate)
            {
                await RowEditCanceled.InvokeAsync(item);
            }

            await ClearEditingItemAsync();
        }

        private async Task ClearEditingItemAsync()
        {
            internalEditingItem = default;
            if (EditingItemChanged.HasDelegate)
            {
                await EditingItemChanged.InvokeAsync(default);
            }
        }

        private string GetEditActionLabel(TItem item)
        {
            var firstColumn = VisibleColumns.FirstOrDefault();
            if (firstColumn is null)
            {
                return EditLabel;
            }

            return string.Format(CultureInfo.CurrentCulture, EditActionLabelFormat, EditLabel, FormatValue(GetValue(item, firstColumn), null));
        }

        private string GetSaveActionLabel(TItem item)
        {
            return GetContextualActionLabel(SaveLabel, SaveActionLabelFormat, item);
        }

        private string GetCancelActionLabel(TItem item)
        {
            return GetContextualActionLabel(CancelLabel, CancelActionLabelFormat, item);
        }

        private string GetContextualActionLabel(string action, string format, TItem item)
        {
            var firstColumn = VisibleColumns.FirstOrDefault();
            if (firstColumn is null)
            {
                return action;
            }

            return string.Format(CultureInfo.CurrentCulture, format, action, FormatValue(GetValue(item, firstColumn), null));
        }

        private IEnumerable<TItem> ApplyFiltering(IEnumerable<TItem> items)
        {
            IEnumerable<TItem> result = items;

            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                var filter = FilterText.Trim();

                if (FilterPredicate is not null)
                {
                    result = result.Where(item => FilterPredicate(item, filter));
                }
                else
                {
                    var keys = ResolveFilterKeys();
                    if (keys.Count > 0)
                    {
                        result = result.Where(item => keys.Any(key => ValueMatchesFilter(GetNestedValue(item, key), filter)));
                    }
                }
            }

            if (columnFilterValues.Count > 0 && FilterMode != XFDataGridFilterMode.None)
            {
                result = result.Where(item => ColumnFiltersMatch(item));
            }

            return result;
        }

        private bool ColumnFiltersMatch(TItem item)
        {
            foreach (var (columnKey, filterValue) in columnFilterValues)
            {
                if (string.IsNullOrWhiteSpace(filterValue)) continue;
                var column = columns.FirstOrDefault(c => c.ResolvedKey == columnKey);
                if (column is null || !column.Filterable) continue;
                if (!ColumnFilterMatches(item, column, filterValue))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ColumnFilterMatches(TItem item, XFDataGridColumn<TItem> column, string filterValue)
        {
            var value = GetValue(item, column);
            var valueStr = value?.ToString() ?? "";
            var op = column.FilterOperator;

            if (value is IConvertible conv && double.TryParse(filterValue, NumberStyles.Any, CultureInfo.CurrentCulture, out var numFilter))
            {
                try
                {
                    var numValue = conv.ToDouble(CultureInfo.CurrentCulture);
                    return op switch
                    {
                        XFDataGridFilterOperator.Equals => Math.Abs(numValue - numFilter) < 0.0001,
                        XFDataGridFilterOperator.NotEquals => Math.Abs(numValue - numFilter) >= 0.0001,
                        XFDataGridFilterOperator.LessThan => numValue < numFilter,
                        XFDataGridFilterOperator.LessThanOrEquals => numValue <= numFilter,
                        XFDataGridFilterOperator.GreaterThan => numValue > numFilter,
                        XFDataGridFilterOperator.GreaterThanOrEquals => numValue >= numFilter,
                        _ => valueStr.Contains(filterValue, StringComparison.OrdinalIgnoreCase)
                    };
                }
                catch (InvalidCastException) { }
                catch (FormatException) { }
            }

            return op switch
            {
                XFDataGridFilterOperator.Equals => valueStr.Equals(filterValue, StringComparison.OrdinalIgnoreCase),
                XFDataGridFilterOperator.NotEquals => !valueStr.Equals(filterValue, StringComparison.OrdinalIgnoreCase),
                XFDataGridFilterOperator.Contains => valueStr.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
                XFDataGridFilterOperator.NotContains => !valueStr.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
                XFDataGridFilterOperator.StartsWith => valueStr.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase),
                XFDataGridFilterOperator.EndsWith => valueStr.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase),
                XFDataGridFilterOperator.IsNull => value is null,
                XFDataGridFilterOperator.IsNotNull => value is not null,
                XFDataGridFilterOperator.IsEmpty => string.IsNullOrEmpty(valueStr),
                XFDataGridFilterOperator.IsNotEmpty => !string.IsNullOrEmpty(valueStr),
                _ => valueStr.Contains(filterValue, StringComparison.OrdinalIgnoreCase)
            };
        }

        private IEnumerable<TItem> ApplySorting(IEnumerable<TItem> items)
        {
            var activeDescs = sortDescriptors
                .Where(d => d.Direction != XFDataGridSortDirection.None)
                .OrderBy(d => d.Index)
                .ToList();

            if (activeDescs.Count == 0)
            {
                if (SortKey is not null && EffectiveSortDirection != XFDataGridSortDirection.None && !string.IsNullOrWhiteSpace(EffectiveSortKey))
                {
                    return EffectiveSortDirection == XFDataGridSortDirection.Descending
                        ? items.OrderByDescending(item => GetComparableValue(item, EffectiveSortKey))
                        : items.OrderBy(item => GetComparableValue(item, EffectiveSortKey));
                }
                return items;
            }

            var first = activeDescs[0];
            var ordered = first.Direction == XFDataGridSortDirection.Descending
                ? items.OrderByDescending(item => GetComparableValue(item, first.Key))
                : items.OrderBy(item => GetComparableValue(item, first.Key));

            for (int i = 1; i < activeDescs.Count; i++)
            {
                var desc = activeDescs[i];
                if (desc.Direction == XFDataGridSortDirection.Descending)
                {
                    var localOrdered = ordered;
                    ordered = localOrdered.ThenByDescending(item => GetComparableValue(item, desc.Key));
                }
                else
                {
                    var localOrdered = ordered;
                    ordered = localOrdered.ThenBy(item => GetComparableValue(item, desc.Key));
                }
            }

            return ordered;
        }

        private async Task ToggleSortAsync(XFDataGridColumn<TItem> column, bool shiftKey = false)
        {
            if (!HeaderCanSort(column))
            {
                return;
            }

            var key = column.ResolvedSortKey!;

            if (AllowMultiColumnSorting && shiftKey)
            {
                var existing = sortDescriptors.FindIndex(d => string.Equals(d.Key, key, StringComparison.Ordinal));

                if (existing >= 0)
                {
                    var current = sortDescriptors[existing];
                    var nextDirection = current.Direction switch
                    {
                        XFDataGridSortDirection.Ascending => XFDataGridSortDirection.Descending,
                        XFDataGridSortDirection.Descending => XFDataGridSortDirection.None,
                        _ => XFDataGridSortDirection.Ascending
                    };

                    if (nextDirection == XFDataGridSortDirection.None)
                    {
                        sortDescriptors.RemoveAt(existing);
                        ReindexSortDescriptors();
                    }
                    else
                    {
                        sortDescriptors[existing] = current with { Direction = nextDirection };
                    }
                }
                else
                {
                    var newIndex = sortDescriptors.Count > 0 ? sortDescriptors.Max(d => d.Index) + 1 : 1;
                    sortDescriptors.Add(new XFDataGridSortDescriptor(key, XFDataGridSortDirection.Ascending, newIndex));
                }

                SyncPrimarySortFromDescriptors();
            }
            else
            {
                var nextDirection = string.Equals(EffectiveSortKey, key, StringComparison.Ordinal)
                    ? EffectiveSortDirection switch
                    {
                        XFDataGridSortDirection.Ascending => XFDataGridSortDirection.Descending,
                        XFDataGridSortDirection.Descending => XFDataGridSortDirection.None,
                        _ => XFDataGridSortDirection.Ascending
                    }
                    : XFDataGridSortDirection.Ascending;

                if (nextDirection == XFDataGridSortDirection.Ascending)
                {
                    sortDescriptors.Clear();
                    sortDescriptors.Add(new XFDataGridSortDescriptor(key, XFDataGridSortDirection.Ascending, 1));
                }
                else if (nextDirection == XFDataGridSortDirection.Descending)
                {
                    sortDescriptors.Clear();
                    sortDescriptors.Add(new XFDataGridSortDescriptor(key, XFDataGridSortDirection.Descending, 1));
                }
                else
                {
                    sortDescriptors.Clear();
                }

                internalSortKey = nextDirection == XFDataGridSortDirection.None ? null : key;
                internalSortDirection = nextDirection;
            }

            if (GotoFirstPageOnSort)
            {
                internalPage = 1;
            }

            if (SortChanged.HasDelegate)
            {
                await SortChanged.InvokeAsync(new XFDataGridSortChangedEventArgs(
                    sortDescriptors.FirstOrDefault(d => d.Direction != XFDataGridSortDirection.None)?.Key,
                    sortDescriptors.FirstOrDefault(d => d.Direction != XFDataGridSortDirection.None)?.Direction ?? XFDataGridSortDirection.None));
            }

            await ForceProviderReloadAsync();
        }

        private void ReindexSortDescriptors()
        {
            var ordered = sortDescriptors.OrderBy(d => d.Index).ToList();
            sortDescriptors.Clear();
            for (var i = 0; i < ordered.Count; i++)
            {
                sortDescriptors.Add(ordered[i] with { Index = i + 1 });
            }
        }

        private void SyncPrimarySortFromDescriptors()
        {
            var first = sortDescriptors.FirstOrDefault(d => d.Direction != XFDataGridSortDirection.None);
            internalSortKey = first?.Key;
            internalSortDirection = first?.Direction ?? XFDataGridSortDirection.None;
        }

        internal XFDataGridSortDescriptor? GetSortDescriptor(string sortKey)
        {
            return sortDescriptors.FirstOrDefault(d => string.Equals(d.Key, sortKey, StringComparison.Ordinal));
        }

        internal string? GetSortIndexBadge(XFDataGridColumn<TItem> column)
        {
            if (!IsMultiColumnSortActive || !ShowMultiColumnSortingIndex)
            {
                return null;
            }

            var descriptor = GetSortDescriptor(column.ResolvedSortKey!);
            if (descriptor is null || descriptor.Direction == XFDataGridSortDirection.None)
            {
                return null;
            }

            return descriptor.Index.ToString(CultureInfo.InvariantCulture);
        }

        private async Task HandlePageChangedAsync(int page)
        {
            internalPage = Math.Max(1, page);
            if (PageChanged.HasDelegate)
            {
                await PageChanged.InvokeAsync(internalPage);
            }

            await ForceProviderReloadAsync();
        }

        private async Task HandlePageSizeChangedAsync(int pageSize)
        {
            internalPageSize = Math.Max(1, pageSize);
            internalPage = 1;
            if (PageSizeChanged.HasDelegate)
            {
                await PageSizeChanged.InvokeAsync(internalPageSize);
            }

            await ForceProviderReloadAsync();
        }

        private async Task HandleRowSelectedAsync(TItem item)
        {
            if (RowClicked.HasDelegate)
            {
                await RowClicked.InvokeAsync(item);
            }

            if (SelectionMode == XFDataGridSelectionMode.None)
            {
                return;
            }

            if (SelectionMode == XFDataGridSelectionMode.Single)
            {
                internalSelectedItem = item;
                internalSelectedItems = [item];

                if (SelectedItemChanged.HasDelegate)
                {
                    await SelectedItemChanged.InvokeAsync(item);
                }

                if (SelectedItemsChanged.HasDelegate)
                {
                    await SelectedItemsChanged.InvokeAsync(internalSelectedItems);
                }

                return;
            }

            var selectedItems = EffectiveSelectedItems.ToList();
            var existingIndex = selectedItems.FindIndex(selected => EqualityComparer<TItem>.Default.Equals(selected, item));

            if (existingIndex >= 0)
            {
                selectedItems.RemoveAt(existingIndex);
            }
            else
            {
                selectedItems.Add(item);
            }

            internalSelectedItems = selectedItems;
            internalSelectedItem = selectedItems.LastOrDefault();

            if (SelectedItemsChanged.HasDelegate)
            {
                await SelectedItemsChanged.InvokeAsync(internalSelectedItems);
            }

            if (SelectedItemChanged.HasDelegate)
            {
                await SelectedItemChanged.InvokeAsync(internalSelectedItem);
            }
        }

        private async Task HandleRowKeyDownAsync(KeyboardEventArgs args, TItem item)
        {
            if (args.Key is "Enter" or " ")
            {
                await HandleRowSelectedAsync(item);
            }
        }

        private bool HeaderCanSort(XFDataGridColumn<TItem> column)
        {
            return AllowSorting && column.Sortable && !string.IsNullOrWhiteSpace(column.ResolvedSortKey);
        }

        private string GetAriaSort(XFDataGridColumn<TItem> column)
        {
            if (sortDescriptors.Count > 0)
            {
                var descriptor = GetSortDescriptor(column.ResolvedSortKey!);
                if (descriptor is null || descriptor.Direction == XFDataGridSortDirection.None)
                {
                    return "none";
                }
                return descriptor.Direction switch
                {
                    XFDataGridSortDirection.Ascending => "ascending",
                    XFDataGridSortDirection.Descending => "descending",
                    _ => "none"
                };
            }

            if (!string.Equals(EffectiveSortKey, column.ResolvedSortKey, StringComparison.Ordinal))
            {
                return "none";
            }

            return EffectiveSortDirection switch
            {
                XFDataGridSortDirection.Ascending => "ascending",
                XFDataGridSortDirection.Descending => "descending",
                _ => "none"
            };
        }

        private string GetSortLabel(XFDataGridColumn<TItem> column)
        {
            var title = column.ResolvedTitle;
            if (!string.Equals(EffectiveSortKey, column.ResolvedSortKey, StringComparison.Ordinal))
            {
                return string.Format(CultureInfo.CurrentCulture, SortAscendingLabelFormat, title);
            }

            return EffectiveSortDirection switch
            {
                XFDataGridSortDirection.Ascending => string.Format(CultureInfo.CurrentCulture, SortDescendingLabelFormat, title),
                XFDataGridSortDirection.Descending => string.Format(CultureInfo.CurrentCulture, ClearSortLabelFormat, title),
                _ => string.Format(CultureInfo.CurrentCulture, SortAscendingLabelFormat, title)
            };
        }

        internal string? ComputeAggregate(XFDataGridColumn<TItem> column, IReadOnlyList<TItem> items)
        {
            if (!column.Aggregatable || items.Count == 0)
            {
                return null;
            }

            var function = column.AggregateFunction;
            var values = items
                .Select(item => GetValue(item, column))
                .Where(v => v is not null)
                .ToList();

            if (values.Count == 0)
            {
                return "-";
            }

            try
            {
                object? result = function switch
                {
                    XFDataGridAggregateFunction.Count => values.Count,
                    _ => ComputeNumericAggregate(values, function)
                };

                if (result is null)
                {
                    return "-";
                }

                return FormatValue(result, column.FormatString);
            }
            catch
            {
                return "-";
            }
        }

        private static object? ComputeNumericAggregate(IList<object?> values, XFDataGridAggregateFunction function)
        {
            var numbers = new List<double>();
            foreach (var v in values)
            {
                if (v is IConvertible convertible)
                {
                    try { numbers.Add(convertible.ToDouble(CultureInfo.CurrentCulture)); }
                    catch (InvalidCastException) { }
                catch (FormatException) { }
                }
                else if (double.TryParse(v?.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var parsed))
                {
                    numbers.Add(parsed);
                }
            }

            if (numbers.Count == 0) return null;

            return function switch
            {
                XFDataGridAggregateFunction.Sum => numbers.Sum(),
                XFDataGridAggregateFunction.Average => numbers.Average(),
                XFDataGridAggregateFunction.Min => numbers.Min(),
                XFDataGridAggregateFunction.Max => numbers.Max(),
                _ => null
            };
        }

        internal string GetAggregateFunctionLabel(XFDataGridAggregateFunction function) => function switch
        {
            XFDataGridAggregateFunction.Sum => "Total:",
            XFDataGridAggregateFunction.Average => "Média:",
            XFDataGridAggregateFunction.Count => "Qtd:",
            XFDataGridAggregateFunction.Min => "Mín:",
            XFDataGridAggregateFunction.Max => "Máx:",
            _ => "Total:"
        };

        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property path resolution; consumers must preserve TItem properties at AOT deployment.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property path resolution; consumers must preserve TItem properties at AOT deployment.")]
        private static object? GetValue(TItem item, XFDataGridColumn<TItem> column)
        {
            return string.IsNullOrWhiteSpace(column.Property) ? item : GetNestedValue(item, column.Property!);
        }

        private IEnumerable<XFDataGridGroup<TItem>> CreateGroups(IEnumerable<TItem> items)
        {
            if (!HasGrouping)
            {
                yield break;
            }

            foreach (var group in items.GroupBy(GetGroupKey))
            {
                var groupItems = group.ToArray();
                yield return new XFDataGridGroup<TItem>(
                    group.Key,
                    FormatGroupLabel(group.Key),
                    groupItems,
                    groupItems.Length);
            }
        }

        private object? GetGroupKey(TItem item)
        {
            if (GroupKeySelector is not null)
            {
                return GroupKeySelector(item);
            }

            return string.IsNullOrWhiteSpace(GroupBy) ? null : GetNestedValue(item, GroupBy!);
        }

        private string FormatGroupLabel(object? key)
        {
            if (key is null)
            {
                return EmptyGroupLabel;
            }

            return key is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.CurrentCulture)
                : key.ToString() ?? "";
        }

        private static object? GetComparableValue(TItem item, string key)
        {
            var value = GetNestedValue(item, key);
            return value is IComparable ? value : value?.ToString();
        }

        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on current.GetType() is required for property path resolution; consumers must preserve source object properties at AOT deployment.")]
        private static object? GetNestedValue(object? source, string path)
        {
            var current = source;
            foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (current is null)
                {
                    return null;
                }

                var property = current.GetType().GetProperty(segment, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                current = property?.GetValue(current);
            }

            return current;
        }

        private static string FormatValue(object? value, string? formatString)
        {
            if (value is null)
            {
                return "-";
            }

            if (!string.IsNullOrWhiteSpace(formatString) && value is IFormattable formattable)
            {
                return formattable.ToString(formatString, CultureInfo.CurrentCulture);
            }

            return value.ToString() ?? "";
        }

        private IReadOnlyList<string> ResolveFilterKeys()
        {
            if (FilterKeys is not null && FilterKeys.Count > 0)
            {
                return FilterKeys.Where(key => !string.IsNullOrWhiteSpace(key)).ToArray();
            }

            return VisibleColumns
                .Select(column => column.Property)
                .Where(property => !string.IsNullOrWhiteSpace(property))
                .Cast<string>()
                .ToArray();
        }

        private static bool ValueMatchesFilter(object? value, string filter)
        {
            if (value is null)
            {
                return false;
            }

            var text = value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.CurrentCulture)
                : value.ToString();

            return text?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true;
        }

        private RenderFragment<XFDataGridColumn<TItem>> RenderFilterInput => column =>
        {
            var filterType = GetColumnFilterType(column);
            var currentValue = GetColumnFilterValue(column.ResolvedKey);
            var ariaLabel = "Filtrar " + column.ResolvedTitle;

            return builder =>
            {
                if (filterType == XFDataGridFilterType.Boolean)
                {
                    builder.OpenElement(0, "select");
                    builder.AddAttribute(1, "class", FilterInputClass);
                    builder.AddAttribute(2, "aria-label", ariaLabel);
                    builder.AddAttribute(3, "value", currentValue ?? "");
                    builder.AddAttribute(4, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.OpenElement(5, "option");
                    builder.AddAttribute(6, "value", "");
                    builder.AddContent(7, L("datagrid.filter_all", "All"));
                    builder.CloseElement();
                    builder.OpenElement(8, "option");
                    builder.AddAttribute(9, "value", "true");
                    builder.AddContent(10, L("datagrid.filter_yes", "Yes"));
                    builder.CloseElement();
                    builder.OpenElement(11, "option");
                    builder.AddAttribute(12, "value", "false");
                    builder.AddContent(13, L("datagrid.filter_no", "No"));
                    builder.CloseElement();
                    builder.CloseElement();
                }
                else if (filterType == XFDataGridFilterType.Enum)
                {
                    builder.OpenElement(0, "select");
                    builder.AddAttribute(1, "class", FilterInputClass);
                    builder.AddAttribute(2, "aria-label", ariaLabel);
                    builder.AddAttribute(3, "value", currentValue ?? "");
                    builder.AddAttribute(4, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.OpenElement(5, "option");
                    builder.AddAttribute(6, "value", "");
                    builder.AddContent(7, L("datagrid.filter_all", "All"));
                    builder.CloseElement();
                    var enumType = ResolvePropertyType(column.Property ?? "");
                    if (enumType != null && enumType.IsEnum)
                    {
                        foreach (var name in Enum.GetNames(enumType))
                        {
                            builder.OpenElement(8, "option");
                            builder.AddAttribute(9, "value", name);
                            builder.AddContent(10, name);
                            builder.CloseElement();
                        }
                    }
                    builder.CloseElement();
                }
                else if (filterType == XFDataGridFilterType.Date)
                {
                    builder.OpenElement(0, "input");
                    builder.AddAttribute(1, "type", "date");
                    builder.AddAttribute(2, "class", FilterInputClass);
                    builder.AddAttribute(3, "aria-label", ariaLabel);
                    builder.AddAttribute(4, "placeholder", L("datagrid.filter_placeholder", "Filter..."));
                    builder.AddAttribute(5, "value", currentValue ?? "");
                    builder.AddAttribute(6, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.CloseElement();
                }
                else if (filterType == XFDataGridFilterType.Number)
                {
                    builder.OpenElement(0, "input");
                    builder.AddAttribute(1, "type", "number");
                    builder.AddAttribute(2, "class", FilterInputClass);
                    builder.AddAttribute(3, "aria-label", ariaLabel);
                    builder.AddAttribute(4, "placeholder", L("datagrid.filter_placeholder", "Filter..."));
                    builder.AddAttribute(5, "value", currentValue ?? "");
                    builder.AddAttribute(6, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(7, "onblur", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterSubmittedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(0, "input");
                    builder.AddAttribute(1, "type", "text");
                    builder.AddAttribute(2, "class", FilterInputClass);
                    builder.AddAttribute(3, "aria-label", ariaLabel);
                    builder.AddAttribute(4, "placeholder", L("datagrid.filter_placeholder", "Filter..."));
                    builder.AddAttribute(5, "value", currentValue ?? "");
                    builder.AddAttribute(6, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(7, "onblur", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterSubmittedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
                    {
                        if (e.Key == "Enter")
                        {
                            _ = OnColumnFilterSubmittedAsync(column, GetColumnFilterValue(column.ResolvedKey));
                        }
                    }));
                    builder.CloseElement();
                }
            };
        };

        private string? GetColumnFilterValue(string columnKey)
        {
            return columnFilterValues.TryGetValue(columnKey, out var value) ? value : null;
        }

        private XFDataGridFilterType GetColumnFilterType(XFDataGridColumn<TItem> column)
        {
            if (column.FilterType != XFDataGridFilterType.Auto) return column.FilterType;
            var propType = ResolvePropertyType(column.Property ?? "");
            if (propType is null) return XFDataGridFilterType.Text;
            if (propType == typeof(bool) || propType == typeof(bool?)) return XFDataGridFilterType.Boolean;
            if (propType.IsEnum) return XFDataGridFilterType.Enum;
            if (propType == typeof(DateTime) || propType == typeof(DateTime?) || propType == typeof(DateOnly) || propType == typeof(DateOnly?)) return XFDataGridFilterType.Date;
            if (propType == typeof(int) || propType == typeof(long) || propType == typeof(double) || propType == typeof(decimal) || propType == typeof(float)) return XFDataGridFilterType.Number;
            return XFDataGridFilterType.Text;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2090", Justification = "Reflection on typeof(TItem) is required for property path resolution; consumers must preserve TItem properties at AOT deployment.")]
        [UnconditionalSuppressMessage("Trimming", "IL2090", Justification = "Reflection on typeof(TItem) is required for property path resolution; consumers must preserve TItem properties at AOT deployment.")]
        private Type? ResolvePropertyType(string propertyPath)
        {
            if (string.IsNullOrWhiteSpace(propertyPath)) return null;
            var firstSegment = propertyPath.Split('.')[0].Trim();
            return typeof(TItem).GetProperty(firstSegment, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?.PropertyType;
        }

        private async Task OnColumnFilterValueChangedAsync(XFDataGridColumn<TItem> column, string? value)
        {
            var key = column.ResolvedKey;
            columnFilterValues[key] = value;

            if (FilterDelay <= 0)
            {
                await ApplyColumnFilterAndNotifyAsync(key, value);
                return;
            }

            pendingFilterColumnKey = key;
            pendingFilterValue = value;
            filterDelayTimer?.Dispose();
            filterDelayTimer = new System.Threading.Timer(async _ =>
            {
                await InvokeAsync(async () =>
                {
                    await ApplyColumnFilterAndNotifyAsync(pendingFilterColumnKey, pendingFilterValue);
                    pendingFilterColumnKey = null;
                    pendingFilterValue = null;
                });
            }, null, FilterDelay, Timeout.Infinite);
        }

        private async Task OnColumnFilterSubmittedAsync(XFDataGridColumn<TItem> column, string? value)
        {
            filterDelayTimer?.Dispose();
            filterDelayTimer = null;
            await ApplyColumnFilterAndNotifyAsync(column.ResolvedKey, value);
        }

        private async Task ApplyColumnFilterAndNotifyAsync(string? columnKey, string? value)
        {
            if (columnKey is null) return;

            if (ColumnFilterChanged.HasDelegate)
            {
                var column = columns.FirstOrDefault(c => c.ResolvedKey == columnKey);
                var op = column?.FilterOperator ?? XFDataGridFilterOperator.Contains;
                await ColumnFilterChanged.InvokeAsync(new XFDataGridColumnFilterEventArgs(columnKey, value, op));
            }

            await InvokeAsync(StateHasChanged);
        }

        private string VariantClass => Variant switch
        {
            XFListVariant.Plain => "bg-transparent",
            XFListVariant.Soft => "bg-surface-50",
            _ => "bg-bg-surface"
        };

        private string HeaderClass => "bg-surface-50";

        private string HeaderRowClass => "group";

        private string? BodyClass => null;

        private string BaseRowClass => Join(
            "group transition-colors duration-150",
            Striped ? "odd:bg-bg-surface even:bg-surface-50/65" : null,
            Hoverable ? "hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]" : null);

        private string GroupHeaderRowClass =>
            "bg-surface-100 text-xs font-semibold uppercase tracking-wide text-text-secondary";

        private string GroupFooterRowClass =>
            "bg-surface-50 text-xs font-medium text-text-secondary";

        private string DetailRowClass =>
            "bg-surface-50/70";

        private string EditRowClass =>
            "bg-[color-mix(in_oklab,var(--color-primary)_5%,var(--color-bg-surface))]";

        private string EditActionCellClass =>
            Join(CellBaseClass, "border-b border-border-light text-right text-sm");

        private string EditHeaderCellClass =>
            Join(CellBaseClass, "border-b border-border-light text-right text-xs font-semibold uppercase tracking-wide text-text-secondary");

        private string EditActionButtonClass =>
            "inline-flex items-center justify-center rounded-md border border-border-light bg-bg-surface px-2.5 py-1 text-xs font-medium text-text-secondary transition-colors hover:border-primary/30 hover:text-primary focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]";

        private string RowClass(TItem item)
        {
            var selected = IsSelected(item);

            return Join(
                BaseRowClass,
                SelectionMode == XFDataGridSelectionMode.None && !RowClicked.HasDelegate ? null : "cursor-pointer outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]",
                selected ? "bg-[color-mix(in_oklab,var(--color-primary)_8%,var(--color-bg-surface))]" : null);
        }

        private bool IsSelected(TItem item)
        {
            return SelectionMode switch
            {
                XFDataGridSelectionMode.Single => EffectiveSelectedItem is not null && EqualityComparer<TItem>.Default.Equals(EffectiveSelectedItem, item),
                XFDataGridSelectionMode.Multiple => EffectiveSelectedItems.Any(selected => EqualityComparer<TItem>.Default.Equals(selected, item)),
                _ => false
            };
        }

        private string CellBaseClass => Size switch
        {
            XFSize.Sm => "px-3 py-2",
            XFSize.Lg => "px-5 py-4",
            _ => "px-4 py-3"
        };

        private string HeaderCellClass(XFDataGridColumn<TItem> column)
        {
            return Join(
                CellBaseClass,
                "border-b border-border-light text-xs font-semibold uppercase tracking-wide text-text-secondary",
                StickyHeader ? "sticky top-0 z-20 bg-surface-50/95 backdrop-blur supports-[backdrop-filter]:bg-surface-50/80" : null,
                AlignClass(column.Align),
                WidthClass(column.Width),
                StickyClass(column, true));
        }

        private string BodyCellClass(XFDataGridColumn<TItem> column)
        {
            return Join(
                CellBaseClass,
                "border-b border-border-light text-sm text-text-primary",
                AlignClass(column.Align),
                WidthClass(column.Width),
                StickyClass(column, false));
        }

        private string FooterCellClass(XFDataGridColumn<TItem> column)
        {
            return Join(
                CellBaseClass,
                "border-t border-border-light text-sm font-semibold text-text-primary",
                StickyFooter ? "sticky bottom-0 z-20 bg-surface-50/95 backdrop-blur supports-[backdrop-filter]:bg-surface-50/85" : null,
                AlignClass(column.Align),
                WidthClass(column.Width),
                StickyClass(column, false));
        }

        private string HeaderButtonClass(XFDataGridColumn<TItem> column)
        {
            return Join(
                "inline-flex w-full min-w-0 items-center gap-1.5 rounded-md outline-none transition-colors duration-150 hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
                column.Align == XFDataGridAlign.End ? "justify-end" : column.Align == XFDataGridAlign.Center ? "justify-center" : "justify-start");
        }

        private string ColumnOperationButtonClass =>
            "inline-flex size-5 items-center justify-center rounded border border-border-light bg-bg-surface text-text-muted transition-colors hover:border-primary/30 hover:text-primary disabled:cursor-not-allowed disabled:opacity-40";

        private string ColumnChooserButtonClass(bool visible)
        {
            return Join(
                "rounded-md border px-2.5 py-1 text-xs font-medium transition-colors disabled:cursor-not-allowed disabled:opacity-50",
                visible
                    ? "border-primary/30 bg-[color-mix(in_oklab,var(--color-primary)_8%,var(--color-bg-surface))] text-primary"
                    : "border-border-light bg-bg-surface text-text-muted hover:text-text-primary");
        }

        private string SortIconClass(XFDataGridColumn<TItem> column)
        {
            return string.Equals(EffectiveSortKey, column.ResolvedSortKey, StringComparison.Ordinal) && EffectiveSortDirection != XFDataGridSortDirection.None
                ? "text-primary"
                : "text-text-muted opacity-60";
        }

        private MarkupString GetSortIcon(XFDataGridColumn<TItem> column)
        {
            if (!string.Equals(EffectiveSortKey, column.ResolvedSortKey, StringComparison.Ordinal))
            {
                return new MarkupString(IconMarkup("chevron-down", 10));
            }

            var icon = EffectiveSortDirection switch
            {
                XFDataGridSortDirection.Ascending => "chevron-up",
                XFDataGridSortDirection.Descending => "chevron-down",
                _ => "chevron-down"
            };
            return new MarkupString(IconMarkup(icon, 10));
        }

        private string? ColumnStyle(XFDataGridColumn<TItem> column)
        {
            var width = GetColumnWidthPixels(column);
            return width is null ? null : $"width: {width.Value}px; min-width: {width.Value}px;";
        }

        private static string IconMarkup(string name, int size)
        {
            return XFSvgIcons.GetWithSize(name, size);
        }

        private static string AlignClass(XFDataGridAlign align) => align switch
        {
            XFDataGridAlign.Center => "text-center",
            XFDataGridAlign.End => "text-right",
            _ => "text-left"
        };

        private static string WidthClass(XFDataGridColumnWidth width) => width switch
        {
            XFDataGridColumnWidth.Xs => "w-20 min-w-20",
            XFDataGridColumnWidth.Sm => "w-28 min-w-28",
            XFDataGridColumnWidth.Md => "w-40 min-w-40",
            XFDataGridColumnWidth.Lg => "w-56 min-w-56",
            XFDataGridColumnWidth.Xl => "w-72 min-w-72",
            XFDataGridColumnWidth.Actions => "w-24 min-w-24",
            _ => "min-w-32"
        };

        private static string StickyClass(XFDataGridColumn<TItem> column, bool header)
        {
            if (!column.Sticky)
            {
                return "";
            }

            return Join(
                "sticky bg-bg-surface shadow-[1px_0_0_var(--color-border-light)] group-hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,var(--color-bg-surface))]",
                header ? "z-[30] bg-surface-50 group-hover:bg-surface-50" : "z-[25]",
                column.StickySide == XFDataGridStickySide.End ? StickyEndClass(column.StickyOffset) : StickyStartClass(column.StickyOffset));
        }

        private static string StickyStartClass(XFDataGridStickyOffset offset) => offset switch
        {
            XFDataGridStickyOffset.Sm => "left-24",
            XFDataGridStickyOffset.Md => "left-32",
            XFDataGridStickyOffset.Lg => "left-40",
            XFDataGridStickyOffset.Xl => "left-56",
            _ => "left-0"
        };

        private static string StickyEndClass(XFDataGridStickyOffset offset) => offset switch
        {
            XFDataGridStickyOffset.Sm => "right-24",
            XFDataGridStickyOffset.Md => "right-32",
            XFDataGridStickyOffset.Lg => "right-40",
            XFDataGridStickyOffset.Xl => "right-56",
            _ => "right-0"
        };

        private string CaptionClass => "caption-bottom px-4 py-3 text-left text-sm text-text-secondary";

        private string FooterClass => "bg-surface-50/95 font-semibold backdrop-blur supports-[backdrop-filter]:bg-surface-50/85";

        private string FooterRowClass => "group";

        private string EmptyClass => $"{CellBaseClass} text-center text-sm text-text-muted";

        private string PagerClass => "mt-3";

        private string? ViewportClass => (NormalizeViewportRows(), Size) switch
        {
            (5, XFSize.Sm) => "max-h-52",
            (5, XFSize.Lg) => "max-h-80",
            (5, _) => "max-h-64",
            (10, XFSize.Sm) => "max-h-96",
            (10, XFSize.Lg) => "max-h-[42rem]",
            (10, _) => "max-h-[34rem]",
            (15, XFSize.Sm) => "max-h-[34rem]",
            (15, XFSize.Lg) => "max-h-[56rem]",
            (15, _) => "max-h-[46rem]",
            (20, XFSize.Sm) => "max-h-[44rem]",
            (20, XFSize.Lg) => "max-h-[72rem]",
            (20, _) => "max-h-[60rem]",
            _ => null
        };

        private int? NormalizeViewportRows()
        {
            if (ViewportRows is null or <= 0)
            {
                return null;
            }

            return ViewportRows <= 5 ? 5 :
                ViewportRows <= 10 ? 10 :
                ViewportRows <= 15 ? 15 :
                20;
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));

        public async ValueTask DisposeAsync()
        {
            if (filterDelayTimer is not null)
            {
                await filterDelayTimer.DisposeAsync();
                filterDelayTimer = null;
            }
        }
    }
}
