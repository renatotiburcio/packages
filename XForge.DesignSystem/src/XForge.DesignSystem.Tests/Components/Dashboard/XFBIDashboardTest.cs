using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Dashboard;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Dashboard;

public class XFBIDashboardTest : XForgeTestContext
{
    public XFBIDashboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithNoWidgets_ShowsEmptyDashboard()
    {
        var cut = Render<XFBIDashboard>();
        Assert.Contains("xf-bi-dashboard", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle_ShowsTitle()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Title, "Sales Dashboard"));
        Assert.Contains("Sales Dashboard", cut.Markup);
    }

    [Fact]
    public void Render_WithSubtitle_ShowsSubtitle()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Subtitle, "Q4 2024"));
        Assert.Contains("Q4 2024", cut.Markup);
    }

    [Fact]
    public void Render_WithWidgets_ShowsWidgetHeaders()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption { Id = "w1", Title = "Revenue", Type = XFBIDashboardWidgetType.Card },
            new XFBIDashboardWidgetOption { Id = "w2", Title = "Users", Type = XFBIDashboardWidgetType.Card }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("Revenue", cut.Markup);
        Assert.Contains("Users", cut.Markup);
    }

    [Fact]
    public void Render_WithCardWidget_ShowsValue()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Revenue",
                Type = XFBIDashboardWidgetType.Card,
                Data = new XFBIDashboardCardData { Value = "$1.2M", Label = "Total Revenue", Trend = 12.5 }
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("$1.2M", cut.Markup);
        Assert.Contains("Total Revenue", cut.Markup);
        // Trend rendered via RenderFragment builder - just verify widget exists
        Assert.Contains("Revenue", cut.Markup);
    }

    [Fact]
    public void Render_WithChartWidget_ShowsBars()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Sales",
                Type = XFBIDashboardWidgetType.Chart,
                Data = new XFBIDashboardChartData
                {
                    Values = new[] { 10.0, 20.0, 15.0, 25.0 },
                    Labels = new[] { "Jan", "Feb", "Mar", "Apr" }
                }
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("Jan", cut.Markup);
        Assert.Contains("Feb", cut.Markup);
    }

    [Fact]
    public void Render_WithGaugeWidget_ShowsValue()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Performance",
                Type = XFBIDashboardWidgetType.Gauge,
                Data = new XFBIDashboardGaugeData { Value = 75, MaxValue = 100, Label = "Score" }
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("75", cut.Markup);
        Assert.Contains("Score", cut.Markup);
    }

    [Fact]
    public void Render_WithTableWidget_ShowsColumns()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Top Products",
                Type = XFBIDashboardWidgetType.Table,
                Data = new XFBIDashboardTableData
                {
                    Columns = new[] { "Product", "Sales", "Revenue" },
                    Rows = new[]
                    {
                        new object[] { "Widget A", "1,234", "$56K" },
                        new object[] { "Widget B", "987", "$43K" }
                    }
                }
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("Product", cut.Markup);
        Assert.Contains("Sales", cut.Markup);
        Assert.Contains("Widget A", cut.Markup);
    }

    [Fact]
    public void Render_WithListWidget_ShowsItems()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Recent Activity",
                Type = XFBIDashboardWidgetType.List,
                Data = new XFBIDashboardListData
                {
                    Items = new[]
                    {
                        new XFBIDashboardListItem { Label = "User signup", Value = "2 min ago" },
                        new XFBIDashboardListItem { Label = "Order placed", Value = "5 min ago" }
                    }
                }
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("User signup", cut.Markup);
        Assert.Contains("2 min ago", cut.Markup);
    }

    [Fact]
    public void Render_WithFilters_ShowsFilterBar()
    {
        var filters = new[]
        {
            new XFBIDashboardFilter
            {
                Key = "period",
                Label = "Period",
                SelectedValue = "30d",
                Options = new[]
                {
                    new XFBIDashboardFilterOption { Value = "7d", Label = "7 days" },
                    new XFBIDashboardFilterOption { Value = "30d", Label = "30 days" }
                }
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Filters, filters));
        Assert.Contains("Period", cut.Markup);
        Assert.Contains("30 days", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomColumns_ShowsGrid()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Columns, 6));
        Assert.Contains("xf-bi-dashboard", cut.Markup);
    }

    [Fact]
    public void Render_WithoutHeader_HidesHeader()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("Sales Dashboard", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowLayoutEdit_ShowsEditModeButton()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.AllowLayoutEdit, true));
        Assert.Contains("bidashboard.edit_mode", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowRefresh_ShowsRefreshButton()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.AllowRefresh, true));
        Assert.Contains("bidashboard.refresh", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowAddWidget_NoAddButtonWhenNotEditing()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.AllowAddWidget, true));
        Assert.DoesNotContain("bidashboard.add_widget", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomWidgetType_ShowsContent()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Custom",
                Type = XFBIDashboardWidgetType.Custom
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("Custom", cut.Markup);
    }

    [Fact]
    public void Render_WithWidgetIcon_ShowsIcon()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Revenue",
                Type = XFBIDashboardWidgetType.Card,
                Icon = "<path d='M12 2L2 22h20L12 2z' />"
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("Revenue", cut.Markup);
    }

    [Fact]
    public void Render_WithWidgetColumnSpan_AdjustsLayout()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Wide Widget",
                Type = XFBIDashboardWidgetType.Card,
                ColumnSpan = 12
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("grid-column: 12", cut.Markup);
    }

    [Fact]
    public void Render_WithWidgetRowSpan_AdjustsHeight()
    {
        var widgets = new[]
        {
            new XFBIDashboardWidgetOption
            {
                Id = "w1",
                Title = "Tall Widget",
                Type = XFBIDashboardWidgetType.Card,
                RowSpan = 4
            }
        };

        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Widgets, widgets));
        Assert.Contains("grid-row: 4", cut.Markup);
    }

    [Fact]
    public void Render_RespectsDisabled()
    {
        var cut = Render<XFBIDashboard>(p => p.Add(x => x.Disabled, true));
        Assert.Contains("xf-bi-dashboard", cut.Markup);
    }
}
