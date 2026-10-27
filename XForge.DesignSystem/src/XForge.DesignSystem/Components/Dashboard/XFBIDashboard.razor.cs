using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Dashboard
{
    public partial class XFBIDashboard : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-bi-dashboard";

        private readonly List<XFBIDashboardWidget> _visibleWidgets = new();
        private bool _isRefreshing;
        private bool _isEditMode;
        private string? _selectedWidgetId;
        private XFBIDashboardWidget? _menuWidget;
        private double _menuX;
        private double _menuY;

        // Drag state
        private XFBIDashboardWidget? _draggedWidget;

        // Resize state
        private XFBIDashboardWidget? _resizingWidget;
        private double _resizeStartX;
        private double _resizeStartY;
        private int _resizeOriginalColSpan;
        private int _resizeOriginalRowSpan;

        #region Parameters

        /// <summary>Dashboard title.</summary>
        [Parameter]
        public string Title { get; set; } = "Dashboard";

        /// <summary>Dashboard subtitle.</summary>
        [Parameter]
        public string? Subtitle { get; set; }

        /// <summary>Custom title template.</summary>
        [Parameter]
        public RenderFragment? TitleTemplate { get; set; }

        /// <summary>Header actions template.</summary>
        [Parameter]
        public RenderFragment? HeaderActions { get; set; }

        /// <summary>Whether to show the header.</summary>
        [Parameter]
        public bool ShowHeader { get; set; } = true;

        /// <summary>Widget definitions.</summary>
        [Parameter]
        public IReadOnlyList<XFBIDashboardWidgetOption> Widgets { get; set; } = Array.Empty<XFBIDashboardWidgetOption>();

        /// <summary>Filter definitions.</summary>
        [Parameter]
        public IReadOnlyList<XFBIDashboardFilter> Filters { get; set; } = Array.Empty<XFBIDashboardFilter>();

        /// <summary>Number of grid columns.</summary>
        [Parameter]
        public int Columns { get; set; } = 12;

        /// <summary>Grid row height in pixels.</summary>
        [Parameter]
        public int RowHeight { get; set; } = 100;

        /// <summary>Whether layout editing is allowed.</summary>
        [Parameter]
        public bool AllowLayoutEdit { get; set; } = true;

        /// <summary>Whether to allow adding widgets.</summary>
        [Parameter]
        public bool AllowAddWidget { get; set; } = true;

        /// <summary>Whether to allow refresh.</summary>
        [Parameter]
        public bool AllowRefresh { get; set; } = true;

        /// <summary>Widget context menu template.</summary>
        [Parameter]
        public RenderFragment<XFBIDashboardWidget>? WidgetMenuTemplate { get; set; }

        /// <summary>Widget added callback.</summary>
        [Parameter]
        public EventCallback<XFBIDashboardWidget> OnWidgetAdded { get; set; }

        /// <summary>Widget removed callback.</summary>
        [Parameter]
        public EventCallback<XFBIDashboardWidget> OnWidgetRemoved { get; set; }

        /// <summary>Widget clicked callback.</summary>
        [Parameter]
        public EventCallback<XFBIDashboardWidget> OnWidgetClick { get; set; }

        /// <summary>Widget resized callback.</summary>
        [Parameter]
        public EventCallback<XFBIDashboardWidgetResizeEventArgs> OnWidgetResize { get; set; }

        /// <summary>Widget reordered callback.</summary>
        [Parameter]
        public EventCallback<XFBIDashboardWidgetReorderEventArgs> OnWidgetReorder { get; set; }

        /// <summary>Filter changed callback.</summary>
        [Parameter]
        public EventCallback<XFBIDashboardFilterEventArgs> OnFilterChanged { get; set; }

        /// <summary>Refresh requested callback.</summary>
        [Parameter]
        public EventCallback OnRefresh { get; set; }

        #endregion

        #region Labels

        private string AriaLabel => L("bidashboard.aria_label", "BI Dashboard");
        private string EditModeText => L("bidashboard.edit_mode", "Edit layout");
        private string RefreshText => L("bidashboard.refresh", "Refresh");
        private string RemoveWidgetText => L("bidashboard.remove_widget", "Remove widget");
        private string AddWidgetText => L("bidashboard.add_widget", "Add widget");
        private string MaximizeText => L("bidashboard.maximize", "Maximize");
        private string ExportText => L("bidashboard.export", "Export");

        #endregion

        #region CSS

        private bool IsEditMode => _isRefreshing ? false : _isEditMode;

        private string GridClass => string.Join(" ",
            "grid gap-4 auto-rows-min",
            IsEditMode ? "ring-2 ring-[var(--color-primary)]/30 ring-offset-2 rounded-xl" : "");

        private string HeaderButtonClass => string.Join(" ",
            "inline-flex h-8 w-8 items-center justify-center rounded-lg",
            "border border-border-light bg-bg-surface text-text-secondary",
            "hover:border-[var(--color-primary)] hover:text-primary transition-colors");

        private string WidgetContainerClass(XFBIDashboardWidget widget, bool isActive)
        {
            return string.Join(" ",
                "relative rounded-xl border bg-bg-surface overflow-hidden transition-all duration-200",
                isActive ? "border-[var(--color-primary)] shadow-md" : "border-border-light hover:border-[var(--color-primary)]/50",
                IsEditMode ? "cursor-move" : "cursor-pointer",
                widget.CssClass);
        }

        #endregion

        #region Lifecycle

        protected override void OnParametersSet()
        {
            SyncWidgets();
        }

        private void SyncWidgets()
        {
            _visibleWidgets.Clear();
            foreach (var option in Widgets)
            {
                _visibleWidgets.Add(new XFBIDashboardWidget
                {
                    Id = option.Id,
                    Title = option.Title,
                    Type = option.Type,
                    ColumnSpan = option.ColumnSpan,
                    RowSpan = option.RowSpan,
                    ShowHeader = option.ShowHeader,
                    Icon = option.Icon,
                    Content = option.Content,
                    Data = option.Data,
                    Resizable = option.Resizable,
                    Draggable = option.Draggable,
                    CssClass = option.CssClass
                });
            }
        }

        #endregion

        #region Edit Mode

        private void ToggleEditMode()
        {
            _isEditMode = !_isEditMode;
            StateHasChanged();
        }

        #endregion

        #region Widget Interaction

        private async Task SelectWidgetAsync(XFBIDashboardWidget widget)
        {
            if (IsEditMode) return;

            _selectedWidgetId = widget.Id;
            await OnWidgetClick.InvokeAsync(widget);
        }

        private async Task RefreshAsync()
        {
            _isRefreshing = true;
            StateHasChanged();

            await OnRefresh.InvokeAsync();
            _isRefreshing = false;
            StateHasChanged();
        }

        private void OnWidgetDragStart(XFBIDashboardWidget widget)
        {
            _draggedWidget = widget;
        }

        private async Task OnWidgetDrop(XFBIDashboardWidget targetWidget)
        {
            if (_draggedWidget == null || _draggedWidget.Id == targetWidget.Id)
            {
                _draggedWidget = null;
                return;
            }

            var fromIndex = _visibleWidgets.IndexOf(_draggedWidget);
            var toIndex = _visibleWidgets.IndexOf(targetWidget);

            if (fromIndex >= 0 && toIndex >= 0)
            {
                _visibleWidgets.RemoveAt(fromIndex);
                _visibleWidgets.Insert(toIndex, _draggedWidget);

                await OnWidgetReorder.InvokeAsync(new XFBIDashboardWidgetReorderEventArgs
                {
                    WidgetId = _draggedWidget.Id,
                    FromIndex = fromIndex,
                    ToIndex = toIndex
                });
            }

            _draggedWidget = null;
            StateHasChanged();
        }

        private async Task ToggleWidgetMenuAsync(XFBIDashboardWidget widget)
        {
            _menuWidget = _menuWidget?.Id == widget.Id ? null : widget;
            StateHasChanged();
        }

        private async Task RefreshWidgetAsync(XFBIDashboardWidget widget)
        {
            _menuWidget = null;
            await OnRefresh.InvokeAsync();
        }

        private void MaximizeWidgetAsync(XFBIDashboardWidget widget)
        {
            _menuWidget = null;
            // Toggle between default and full width
            if (widget.ColumnSpan == Columns)
                widget.ColumnSpan = widget.Type == XFBIDashboardWidgetType.Chart ? 6 : 4;
            else
                widget.ColumnSpan = Columns;

            StateHasChanged();
        }

        private void ExportWidgetAsync(XFBIDashboardWidget widget)
        {
            _menuWidget = null;
            // Export would be handled by consumer
        }

        #endregion

        #region Widget Resize

        private void StartResize(XFBIDashboardWidget widget)
        {
            _resizingWidget = widget;
            _resizeOriginalColSpan = widget.ColumnSpan;
            _resizeOriginalRowSpan = widget.RowSpan;
        }

        private void OnResizePointerMove(PointerEventArgs e)
        {
            if (_resizingWidget == null) return;
            // Resize would be handled via JS interop in production
            // For now, this is a placeholder
        }

        private void OnResizePointerUp(PointerEventArgs e)
        {
            if (_resizingWidget != null)
            {
                OnWidgetResize.InvokeAsync(new XFBIDashboardWidgetResizeEventArgs
                {
                    WidgetId = _resizingWidget.Id,
                    NewColumnSpan = _resizingWidget.ColumnSpan,
                    NewRowSpan = _resizingWidget.RowSpan
                });
            }
            _resizingWidget = null;
        }

        #endregion

        #region Widget CRUD

        private async Task AddWidgetAsync()
        {
            var newWidget = new XFBIDashboardWidget
            {
                Id = $"widget-{Guid.NewGuid():N}",
                Title = "New Widget",
                Type = XFBIDashboardWidgetType.Card,
                ColumnSpan = 4,
                RowSpan = 2
            };

            _visibleWidgets.Add(newWidget);
            await OnWidgetAdded.InvokeAsync(newWidget);
            StateHasChanged();
        }

        private async Task RemoveWidgetAsync(XFBIDashboardWidget widget)
        {
            _visibleWidgets.Remove(widget);
            await OnWidgetRemoved.InvokeAsync(widget);
            StateHasChanged();
        }

        #endregion

        #region Filters

        private async Task OnFilterChangedAsync(XFBIDashboardFilter filter, string? value)
        {
            filter.SelectedValue = value;
            await OnFilterChanged.InvokeAsync(new XFBIDashboardFilterEventArgs
            {
                Filter = filter,
                Value = value
            });
            StateHasChanged();
        }

        private async Task RemoveFilterAsync(XFBIDashboardFilter filter)
        {
            await OnFilterChanged.InvokeAsync(new XFBIDashboardFilterEventArgs
            {
                Filter = filter,
                Value = null
            });
        }

        #endregion

        #region Widget Renderers

        private RenderFragment RenderCardWidget(XFBIDashboardWidget widget) => builder =>
        {
            var data = widget.Data as XFBIDashboardCardData;
            if (data == null) return;

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "text-center");

            // Value
            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", "text-3xl font-bold text-text-primary");
            builder.AddContent(4, data.Value);
            builder.CloseElement();

            // Label
            if (!string.IsNullOrEmpty(data.Label))
            {
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "text-sm text-text-secondary mt-1");
                builder.AddContent(7, data.Label);
                builder.CloseElement();
            }

            // Trend
            if (data.Trend.HasValue)
            {
                var trendClass = data.Trend > 0 ? "text-green-600" : data.Trend < 0 ? "text-red-600" : "text-text-muted";
                var trendIcon = data.Trend > 0 ? "↑" : data.Trend < 0 ? "↓" : "→";
                builder.OpenElement(8, "div");
                builder.AddAttribute(9, "class", $"text-sm font-semibold mt-2 {trendClass}");
                builder.AddContent(10, $"{trendIcon} {Math.Abs(data.Trend.Value):F1}%");
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private RenderFragment RenderChartWidget(XFBIDashboardWidget widget) => builder =>
        {
            var data = widget.Data as XFBIDashboardChartData;
            if (data == null) return;

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "flex items-end justify-around h-full gap-1 pt-4");

            var maxValue = data.Values.Any() ? data.Values.Max() : 1;
            for (int i = 0; i < data.Values.Count; i++)
            {
                var height = maxValue > 0 ? (data.Values[i] / maxValue * 100) : 0;
                var label = i < data.Labels.Count ? data.Labels[i] : "";

                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "flex flex-col items-center gap-1 flex-1");

                // Bar
                builder.OpenElement(4, "div");
                builder.AddAttribute(5, "class", "w-full rounded-t bg-[var(--color-primary)] transition-all duration-300");
                builder.AddAttribute(6, "style", $"height: {height}%");
                builder.CloseElement();

                // Label
                builder.OpenElement(7, "div");
                builder.AddAttribute(8, "class", "text-[10px] text-text-muted truncate w-full text-center");
                builder.AddContent(9, label);
                builder.CloseElement();

                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private RenderFragment RenderTableWidget(XFBIDashboardWidget widget) => builder =>
        {
            var data = widget.Data as XFBIDashboardTableData;
            if (data == null) return;

            builder.OpenElement(0, "table");
            builder.AddAttribute(1, "class", "w-full text-sm");

            // Header
            builder.OpenElement(2, "thead");
            builder.OpenElement(3, "tr");
            foreach (var column in data.Columns)
            {
                builder.OpenElement(4, "th");
                builder.AddAttribute(5, "class", "text-left text-xs font-semibold text-text-secondary pb-2 border-b border-border-light");
                builder.AddContent(6, column);
                builder.CloseElement();
            }
            builder.CloseElement();
            builder.CloseElement();

            // Body
            builder.OpenElement(7, "tbody");
            foreach (var row in data.Rows)
            {
                builder.OpenElement(8, "tr");
                builder.AddAttribute(9, "class", "border-b border-border-light last:border-0");
                foreach (var cell in row)
                {
                    builder.OpenElement(10, "td");
                    builder.AddAttribute(11, "class", "py-1.5 text-text-primary");
                    builder.AddContent(12, cell);
                    builder.CloseElement();
                }
                builder.CloseElement();
            }
            builder.CloseElement();

            builder.CloseElement();
        };

        private RenderFragment RenderGaugeWidget(XFBIDashboardWidget widget) => builder =>
        {
            var data = widget.Data as XFBIDashboardGaugeData;
            if (data == null) return;

            var percentage = data.MaxValue > 0 ? (data.Value / data.MaxValue * 100) : 0;
            var circumference = 2 * Math.PI * 45;
            var offset = circumference - (percentage / 100 * circumference);

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "flex flex-col items-center justify-center h-full");

            // SVG gauge
            builder.OpenElement(2, "svg");
            builder.AddAttribute(3, "width", "120");
            builder.AddAttribute(4, "height", "120");
            builder.AddAttribute(5, "viewBox", "0 0 100 100");

            // Background circle
            builder.OpenElement(6, "circle");
            builder.AddAttribute(7, "cx", "50");
            builder.AddAttribute(8, "cy", "50");
            builder.AddAttribute(9, "r", "45");
            builder.AddAttribute(10, "fill", "none");
            builder.AddAttribute(11, "stroke", "#e5e7eb");
            builder.AddAttribute(12, "stroke-width", "8");
            builder.CloseElement();

            // Progress circle
            builder.OpenElement(13, "circle");
            builder.AddAttribute(14, "cx", "50");
            builder.AddAttribute(15, "cy", "50");
            builder.AddAttribute(16, "r", "45");
            builder.AddAttribute(17, "fill", "none");
            builder.AddAttribute(18, "stroke", "var(--color-primary)");
            builder.AddAttribute(19, "stroke-width", "8");
            builder.AddAttribute(20, "stroke-linecap", "round");
            builder.AddAttribute(21, "stroke-dasharray", $"{circumference}");
            builder.AddAttribute(22, "stroke-dashoffset", $"{offset}");
            builder.AddAttribute(23, "transform", "rotate(-90 50 50)");
            builder.CloseElement();

            // Value text
            builder.OpenElement(24, "text");
            builder.AddAttribute(25, "x", "50");
            builder.AddAttribute(26, "y", "50");
            builder.AddAttribute(27, "text-anchor", "middle");
            builder.AddAttribute(28, "dominant-baseline", "central");
            builder.AddAttribute(29, "class", "text-xl font-bold fill-text-primary");
            builder.AddContent(30, data.Value.ToString("F0"));
            builder.CloseElement();

            builder.CloseElement();

            // Label
            if (!string.IsNullOrEmpty(data.Label))
            {
                builder.OpenElement(31, "div");
                builder.AddAttribute(32, "class", "text-sm text-text-secondary mt-2");
                builder.AddContent(33, data.Label);
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private RenderFragment RenderListWidget(XFBIDashboardWidget widget) => builder =>
        {
            var data = widget.Data as XFBIDashboardListData;
            if (data == null) return;

            builder.OpenElement(0, "ul");
            builder.AddAttribute(1, "class", "space-y-2");

            foreach (var item in data.Items)
            {
                builder.OpenElement(2, "li");
                builder.AddAttribute(3, "class", "flex items-center justify-between py-1 border-b border-border-light last:border-0");

                builder.OpenElement(4, "span");
                builder.AddAttribute(5, "class", "text-sm text-text-primary truncate");
                builder.AddContent(6, item.Label);
                builder.CloseElement();

                builder.OpenElement(7, "span");
                builder.AddAttribute(8, "class", "text-sm font-semibold text-text-secondary");
                builder.AddContent(9, item.Value);
                builder.CloseElement();

                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private RenderFragment RenderCustomWidget(XFBIDashboardWidget widget) => builder =>
        {
            if (widget.Content != null)
            {
                builder.AddContent(0, widget.Content);
            }
        };

        #endregion
    }

    #region Supporting types

    public enum XFBIDashboardWidgetType
    {
        Card,
        Chart,
        Table,
        Gauge,
        List,
        Custom
    }

    public class XFBIDashboardWidgetOption
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public XFBIDashboardWidgetType Type { get; set; }
        public int ColumnSpan { get; set; } = 4;
        public int RowSpan { get; set; } = 2;
        public bool ShowHeader { get; set; } = true;
        public string? Icon { get; set; }
        public RenderFragment? Content { get; set; }
        public object? Data { get; set; }
        public bool Resizable { get; set; } = true;
        public bool Draggable { get; set; } = true;
        public string? CssClass { get; set; }
    }

    public class XFBIDashboardWidget
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public XFBIDashboardWidgetType Type { get; set; }
        public int ColumnSpan { get; set; } = 4;
        public int RowSpan { get; set; } = 2;
        public bool ShowHeader { get; set; } = true;
        public bool ShowMenu { get; set; } = true;
        public string? Icon { get; set; }
        public RenderFragment? Content { get; set; }
        public object? Data { get; set; }
        public bool Resizable { get; set; } = true;
        public bool Draggable { get; set; } = true;
        public string? CssClass { get; set; }
    }

    public class XFBIDashboardCardData
    {
        public string Value { get; set; } = "";
        public string? Label { get; set; }
        public double? Trend { get; set; }
    }

    public class XFBIDashboardChartData
    {
        public IReadOnlyList<double> Values { get; set; } = Array.Empty<double>();
        public IReadOnlyList<string> Labels { get; set; } = Array.Empty<string>();
    }

    public class XFBIDashboardTableData
    {
        public IReadOnlyList<string> Columns { get; set; } = Array.Empty<string>();
        public IReadOnlyList<IReadOnlyList<object>> Rows { get; set; } = Array.Empty<IReadOnlyList<object>>();
    }

    public class XFBIDashboardGaugeData
    {
        public double Value { get; set; }
        public double MaxValue { get; set; } = 100;
        public string? Label { get; set; }
    }

    public class XFBIDashboardListData
    {
        public IReadOnlyList<XFBIDashboardListItem> Items { get; set; } = Array.Empty<XFBIDashboardListItem>();
    }

    public class XFBIDashboardListItem
    {
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class XFBIDashboardFilter
    {
        public string Key { get; set; } = "";
        public string Label { get; set; } = "";
        public string? SelectedValue { get; set; }
        public bool HasOptions => Options?.Any() == true;
        public bool IsRemovable { get; set; }
        public IReadOnlyList<XFBIDashboardFilterOption> Options { get; set; } = Array.Empty<XFBIDashboardFilterOption>();
    }

    public class XFBIDashboardFilterOption
    {
        public string Value { get; set; } = "";
        public string Label { get; set; } = "";
    }

    public class XFBIDashboardWidgetResizeEventArgs
    {
        public string WidgetId { get; set; } = "";
        public int NewColumnSpan { get; set; }
        public int NewRowSpan { get; set; }
    }

    public class XFBIDashboardWidgetReorderEventArgs
    {
        public string WidgetId { get; set; } = "";
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }

    public class XFBIDashboardFilterEventArgs
    {
        public XFBIDashboardFilter Filter { get; set; } = null!;
        public string? Value { get; set; }
    }

    #endregion
}
