using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Grid;

public class GridDataProviderRequest<T>
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public string? SortColumn { get; init; }
    public bool SortDescending { get; init; }
    public string? Filter { get; init; }
    public CancellationToken CancellationToken { get; init; }
}

public class GridDataProviderResult<T>
{
    public IEnumerable<T> Data { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
}

public delegate Task<GridDataProviderResult<T>> GridDataProviderDelegate<T>(GridDataProviderRequest<T> request);

public abstract class HxGridBase<T> : XForgeComponentBase
{
    [Parameter] public GridDataProviderDelegate<T>? DataProvider { get; set; }
    [Parameter] public int PageSize { get; set; } = 20;
    [Parameter] public RenderFragment<T>? Columns { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    protected int PageIndex { get; set; }
    protected IEnumerable<T> Data { get; set; } = Array.Empty<T>();
    protected int TotalCount { get; set; }

    protected async Task LoadAsync(CancellationToken ct = default)
    {
        if (DataProvider is null) return;
        var req = new GridDataProviderRequest<T> { PageIndex = PageIndex, PageSize = PageSize, CancellationToken = ct };
        var res = await DataProvider(req);
        Data = res.Data;
        TotalCount = res.TotalCount;
    }

    protected Task NextPageAsync() { PageIndex++; return LoadAsync(); }
    protected Task PrevPageAsync() { if (PageIndex > 0) PageIndex--; return LoadAsync(); }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}
