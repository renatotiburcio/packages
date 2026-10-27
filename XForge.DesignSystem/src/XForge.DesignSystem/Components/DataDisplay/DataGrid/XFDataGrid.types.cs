namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
public enum XFDataGridAlign
    {
        Start,
        Center,
        End
    }

    public enum XFDataGridSortDirection
    {
        None,
        Ascending,
        Descending
    }

    public enum XFDataGridStickySide
    {
        Start,
        End
    }

    public enum XFDataGridStickyOffset
    {
        None,
        Sm,
        Md,
        Lg,
        Xl
    }

    public enum XFDataGridColumnWidth
    {
        Auto,
        Xs,
        Sm,
        Md,
        Lg,
        Xl,
        Actions
    }

    public enum XFDataGridSelectionMode
    {
        None,
        Single,
        Multiple
    }

    public enum XFDataGridFilterMode
    {
        None,
        Row,
        Advanced
    }

    public enum XFDataGridFilterOperator
    {
        Contains,
        NotContains,
        Equals,
        NotEquals,
        StartsWith,
        EndsWith,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        IsNull,
        IsNotNull,
        IsEmpty,
        IsNotEmpty,
        In,
        NotIn
    }

    public enum XFDataGridFilterType
    {
        Auto,
        Text,
        Number,
        Date,
        Boolean,
        Enum
    }

    public enum XFDataGridAggregateFunction
    {
        Sum,
        Average,
        Count,
        Min,
        Max
    }

    public sealed record XFDataGridSortDescriptor(string Key, XFDataGridSortDirection Direction, int Index);

    public sealed record XFDataGridAggregate(string ColumnKey, XFDataGridAggregateFunction Function, string? FormatString = null);
}
