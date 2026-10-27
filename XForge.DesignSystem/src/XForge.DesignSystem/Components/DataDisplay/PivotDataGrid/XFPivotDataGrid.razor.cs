using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.PivotDataGrid;

public partial class XFPivotDataGrid<TItem> : XForgeComponentBase
{
    [Inject] private IXFLocalizer? Localizer { get; set; }
    private string L(string key, string fallback) => Localizer?[key] ?? fallback;

    [Parameter] public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
    [Parameter, EditorRequired] public Func<TItem, string>? RowSelector { get; set; }
    [Parameter, EditorRequired] public Func<TItem, string>? ColumnSelector { get; set; }
    [Parameter] public Func<TItem, double>? ValueSelector { get; set; }
    [Parameter] public XFPivotAggregate Aggregate { get; set; } = XFPivotAggregate.Sum;
    [Parameter] public bool ShowTotals { get; set; } = true;
    [Parameter] public string RowHeader { get; set; } = "Linha";
    [Parameter] public string TotalHeader { get; set; } = "Total";
    [Parameter] public string EmptyText { get; set; } = "Nenhum dado para exibir.";
    [Parameter] public string? Caption { get; set; }
    [Parameter] public string DefaultCaption { get; set; } = "Tabela dinamica de dados";
    [Parameter] public string? ValueFormat { get; set; } = "N0";
    [Parameter] public CultureInfo? Culture { get; set; }
    [Parameter] public IComparer<string>? SortComparer { get; set; }

    protected override void OnInitialized()
    {
        RowHeader = L("pivotdatagrid.row_header", RowHeader);
        TotalHeader = L("pivotdatagrid.total_header", TotalHeader);
        EmptyText = L("pivotdatagrid.empty", EmptyText);
        DefaultCaption = L("pivotdatagrid.caption", DefaultCaption);
        base.OnInitialized();
    }

    private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;
    private IComparer<string> EffectiveSortComparer => SortComparer ?? StringComparer.CurrentCultureIgnoreCase;

    private string? EffectiveCaption => !string.IsNullOrWhiteSpace(Caption) ? Caption : DefaultCaption;

    private IReadOnlyList<string> Rows => RowSelector is null
        ? Array.Empty<string>()
        : Items.Select(RowSelector).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.Ordinal).OrderBy(value => value, EffectiveSortComparer).ToArray();

    private IReadOnlyList<string> Columns => ColumnSelector is null
        ? Array.Empty<string>()
        : Items.Select(ColumnSelector).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.Ordinal).OrderBy(value => value, EffectiveSortComparer).ToArray();

    private int EmptyColspan => 1 + Columns.Count + (ShowTotals ? 1 : 0);

    private string RootClass =>
        GetClass("xf-pivot-data-grid overflow-x-auto rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.035)]");

    private string CornerHeaderClass =>
        "sticky left-0 z-10 border-b border-r border-border-light bg-surface-50 px-4 py-3 text-xs font-semibold uppercase tracking-wide text-text-secondary";

    private string ColumnHeaderClass =>
        "border-b border-border-light bg-surface-50 px-4 py-3 text-right text-xs font-semibold uppercase tracking-wide text-text-secondary";

    private string RowHeaderClass =>
        "sticky left-0 z-10 border-r border-border-light bg-bg-surface px-4 py-3 font-semibold text-text-primary";

    private string CellClass =>
        "border-b border-border-light px-4 py-3 text-right tabular-nums text-text-primary";

    private string TotalCellClass =>
        "border-b border-border-light bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] px-4 py-3 text-right font-semibold tabular-nums text-text-primary";

    private string FooterHeaderClass =>
        "sticky left-0 z-10 border-r border-border-light bg-surface-100 px-4 py-3 text-sm font-semibold text-text-primary";

    private string FooterCellClass =>
        "bg-surface-100 px-4 py-3 text-right font-semibold tabular-nums text-text-primary";

    private double GetCellValue(string row, string column)
    {
        if (RowSelector is null || ColumnSelector is null)
        {
            return 0;
        }

        var values = Items
            .Where(item => string.Equals(RowSelector(item), row, StringComparison.Ordinal)
                && string.Equals(ColumnSelector(item), column, StringComparison.Ordinal))
            .Select(GetValue)
            .ToArray();

        return AggregateValues(values);
    }

    private double GetRowTotal(string row)
    {
        if (RowSelector is null)
        {
            return 0;
        }

        return AggregateValues(Items.Where(item => string.Equals(RowSelector(item), row, StringComparison.Ordinal)).Select(GetValue).ToArray());
    }

    private double GetColumnTotal(string column)
    {
        if (ColumnSelector is null)
        {
            return 0;
        }

        return AggregateValues(Items.Where(item => string.Equals(ColumnSelector(item), column, StringComparison.Ordinal)).Select(GetValue).ToArray());
    }

    private double GetGrandTotal() => AggregateValues(Items.Select(GetValue).ToArray());

    private double GetValue(TItem item) => Aggregate == XFPivotAggregate.Count ? 1 : ValueSelector?.Invoke(item) ?? 0;

    private double AggregateValues(IReadOnlyList<double> values)
    {
        if (values.Count == 0)
        {
            return 0;
        }

        return Aggregate switch
        {
            XFPivotAggregate.Count => values.Count,
            XFPivotAggregate.Average => values.Average(),
            XFPivotAggregate.Min => values.Min(),
            XFPivotAggregate.Max => values.Max(),
            _ => values.Sum()
        };
    }

    private string FormatValue(double value)
    {
        return string.IsNullOrWhiteSpace(ValueFormat)
            ? value.ToString(EffectiveCulture)
            : value.ToString(ValueFormat, EffectiveCulture);
    }
}
