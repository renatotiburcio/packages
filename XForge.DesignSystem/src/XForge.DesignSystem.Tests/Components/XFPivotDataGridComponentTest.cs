using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.PivotDataGrid;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFPivotDataGridComponentTest : BunitContext
{
    public XFPivotDataGridComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }
    [Fact]
    public void Render_SumsCellsAndTotals()
    {
        var cut = Render<XFPivotDataGrid<Record>>(parameters =>
        {
            parameters.Add(p => p.Items, CreateRecords());
            parameters.Add(p => p.RowSelector, item => item.Region);
            parameters.Add(p => p.ColumnSelector, item => item.Quarter);
            parameters.Add(p => p.ValueSelector, item => item.Amount);
            parameters.Add(p => p.ValueFormat, "0");
        });

        Assert.Contains(">Norte<", cut.Markup);
        Assert.Contains(">Q1<", cut.Markup);
        Assert.Contains(">30<", cut.Markup);
        Assert.Contains(">42<", cut.Markup);
    }

    [Fact]
    public void Render_CountAggregateCountsRecords()
    {
        var cut = Render<XFPivotDataGrid<Record>>(parameters =>
        {
            parameters.Add(p => p.Items, CreateRecords());
            parameters.Add(p => p.RowSelector, item => item.Region);
            parameters.Add(p => p.ColumnSelector, item => item.Quarter);
            parameters.Add(p => p.Aggregate, XFPivotAggregate.Count);
            parameters.Add(p => p.ValueFormat, "0");
        });

        Assert.Contains(">2<", cut.Markup);
    }

    [Fact]
    public void Render_UsesCultureAndCustomSortComparer()
    {
        var cut = Render<XFPivotDataGrid<Record>>(parameters =>
        {
            parameters.Add(p => p.Items, CreateRecords());
            parameters.Add(p => p.RowSelector, item => item.Region);
            parameters.Add(p => p.ColumnSelector, item => item.Quarter);
            parameters.Add(p => p.ValueSelector, item => item.Amount);
            parameters.Add(p => p.Aggregate, XFPivotAggregate.Average);
            parameters.Add(p => p.ValueFormat, "N2");
            parameters.Add(p => p.Culture, CultureInfo.GetCultureInfo("pt-BR"));
            parameters.Add(p => p.SortComparer, Comparer<string>.Create((left, right) => string.Compare(right, left, StringComparison.Ordinal)));
        });

        Assert.Contains(">Sul<", cut.Markup);
        Assert.Contains(">15,00<", cut.Markup);
    }

    [Fact]
    public void Render_EmptyStateWhenNoData()
    {
        var cut = Render<XFPivotDataGrid<Record>>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<Record>());
            parameters.Add(p => p.RowSelector, item => item.Region);
            parameters.Add(p => p.ColumnSelector, item => item.Quarter);
        });

        Assert.Contains("pivotdatagrid.empty", cut.Markup);
    }

    [Fact]
    public void Render_Caption_UsesDefaultAndAllowsOverride()
    {
        var defaultCut = Render<XFPivotDataGrid<Record>>(parameters =>
        {
            parameters.Add(p => p.Items, CreateRecords());
            parameters.Add(p => p.RowSelector, item => item.Region);
            parameters.Add(p => p.ColumnSelector, item => item.Quarter);
        });
        var customCut = Render<XFPivotDataGrid<Record>>(parameters =>
        {
            parameters.Add(p => p.Items, CreateRecords());
            parameters.Add(p => p.RowSelector, item => item.Region);
            parameters.Add(p => p.ColumnSelector, item => item.Quarter);
            parameters.Add(p => p.Caption, "Resumo fiscal por trimestre");
        });

        Assert.Contains("<caption class=\"sr-only\">pivotdatagrid.caption</caption>", defaultCut.Markup);
        Assert.Contains("<caption class=\"sr-only\">Resumo fiscal por trimestre</caption>", customCut.Markup);
    }

    private static IReadOnlyList<Record> CreateRecords() => new[]
    {
        new Record("Norte", "Q1", 10),
        new Record("Norte", "Q1", 20),
        new Record("Norte", "Q2", 12),
        new Record("Sul", "Q1", 8),
    };

    private sealed record Record(string Region, string Quarter, double Amount);
}
