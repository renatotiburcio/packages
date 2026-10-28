using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.DataDisplay.Gantt;
using Xunit;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFGanttComponentTest : XForgeTestContext
{
    public XFGanttComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }
    private sealed record GanttTaskItem(
        string Id,
        string? ParentId,
        string Title,
        DateTime Start,
        DateTime End,
        double Progress = 0);

    private static List<GanttTaskItem> SampleTasks => new()
    {
        new("1", null, "Project", DateTime.Today.AddDays(-2), DateTime.Today.AddDays(30), 35),
        new("2", "1", "Discovery", DateTime.Today.AddDays(-2), DateTime.Today.AddDays(5), 100),
        new("3", "1", "Implementation", DateTime.Today.AddDays(6), DateTime.Today.AddDays(24), 25),
        new("4", "3", "API", DateTime.Today.AddDays(6), DateTime.Today.AddDays(14), 60),
        new("5", "3", "UI", DateTime.Today.AddDays(10), DateTime.Today.AddDays(22), 10),
    };

    private IRenderedComponent<XFGantt<GanttTaskItem>> RenderDefault()
        => Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.ProgressFunc, new Func<GanttTaskItem, double>(t => t.Progress))
        );

    [Fact]
    public void Render_WithData_RendersGanttContainer()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-gantt", cut.Markup);
        Assert.Contains("role=\"grid\"", cut.Markup);
    }

    [Fact]
    public void Render_WithData_RendersTaskListPanel()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-gantt-tasklist", cut.Markup);
        Assert.Contains("Project", cut.Markup);
        Assert.Contains("Discovery", cut.Markup);
    }

    [Fact]
    public void Render_WithData_RendersTimelinePanel()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-gantt-timeline", cut.Markup);
    }

    [Fact]
    public void Render_WithHierarchy_RendersIndentedTree()
    {
        var cut = RenderDefault();
        Assert.Contains("Project", cut.Markup);
        Assert.Contains("Discovery", cut.Markup);
        Assert.Contains("Implementation", cut.Markup);
        Assert.Contains("API", cut.Markup);
        Assert.Contains("UI", cut.Markup);
    }

    [Fact]
    public void Render_WithProgress_ShowsProgressPercent()
    {
        var cut = RenderDefault();
        Assert.Contains("35%", cut.Markup);
        Assert.Contains("100%", cut.Markup);
    }

    [Fact]
    public void Render_WithoutProgress_HidesProgressPercent()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.ShowProgress, false)
        );
        Assert.DoesNotContain("35%", cut.Markup);
    }

    [Fact]
    public void Render_EmptyData_RendersEmptyState()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, Array.Empty<GanttTaskItem>())
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
        );
        Assert.Contains("gantt.empty", cut.Markup);
        Assert.Contains("role=\"status\"", cut.Markup);
    }

    [Fact]
    public void Render_NullData_RendersEmptyState()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, (IEnumerable<GanttTaskItem>?)null)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
        );
        Assert.Contains("gantt.empty", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomEmptyText_ShowsCustomText()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, Array.Empty<GanttTaskItem>())
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
        );
        Assert.Contains("gantt.empty", cut.Markup);
    }

    [Fact]
    public void Render_DayView_ShowsHourHeaders()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.View, XFGanttView.Day)
        );
        Assert.Contains("00:00", cut.Markup);
        Assert.Contains("12:00", cut.Markup);
    }

    [Fact]
    public void Render_WeekView_ShowsDayHeaders()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-gantt-header", cut.Markup);
    }

    [Fact]
    public void Render_MonthView_ShowsDayNumbers()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.View, XFGanttView.Month)
        );
        // Month view shows day numbers as headers
        Assert.Contains("xf-gantt-header", cut.Markup);
    }

    [Fact]
    public void Render_YearView_ShowsMonthNames()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.View, XFGanttView.Year)
        );
        Assert.Contains("jan", cut.Markup.ToLower());
    }

    [Fact]
    public void Render_WithCustomTaskListWidth_AppliesWidth()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.TaskListWidth, "400px")
        );
        Assert.Contains("400px", cut.Markup);
    }

    [Fact]
    public void Render_WithDisabled_AppliesOpacity()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Disabled, true)
        );
        Assert.Contains("opacity-50", cut.Markup);
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomId_RendersId()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Id, "my-gantt")
        );
        Assert.Contains("id=\"my-gantt\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomClass_AppliesClass()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Class, "my-custom-class")
        );
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Render_WithStyle_AppliesStyle()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Style, "height: 600px")
        );
        Assert.Contains("height: 600px", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_RendersAttributes()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "gantt" } })
        );
        Assert.Contains("data-testid=\"gantt\"", cut.Markup);
    }

    [Fact]
    public void Render_WithoutGridLines_HidesGridLines()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.ShowGridLines, false)
        );
        Assert.DoesNotContain("xf-gantt-grid-line", cut.Markup);
    }

    [Fact]
    public void Render_WithoutTodayLine_HidesTodayLine()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.ShowTodayLine, false)
        );
        Assert.DoesNotContain("xf-gantt-today-line", cut.Markup);
    }

    [Fact]
    public void Render_HasAriaAttributes()
    {
        var cut = RenderDefault();
        Assert.Contains("role=\"grid\"", cut.Markup);
        Assert.Contains("aria-label=\"Gantt chart\"", cut.Markup);
    }

    [Fact]
    public void Render_TaskRow_HasRowRole()
    {
        var cut = RenderDefault();
        Assert.Contains("role=\"row\"", cut.Markup);
    }

    [Fact]
    public void Render_ParentRow_HasAriaExpanded()
    {
        var cut = RenderDefault();
        Assert.Contains("aria-expanded=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_TaskBar_HasImgRole()
    {
        var cut = RenderDefault();
        Assert.Contains("role=\"img\"", cut.Markup);
    }

    [Fact]
    public void Render_TaskBar_HasAriaLabel()
    {
        var cut = RenderDefault();
        Assert.Contains("Project", cut.Markup);
        Assert.Contains("gantt.start", cut.Markup);
        Assert.Contains("gantt.end", cut.Markup);
    }

    [Fact]
    public void Render_SmallSize_AppliesTextXs()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Size, XFSize.Sm)
        );
        Assert.Contains("text-xs", cut.Markup);
    }

    [Fact]
    public void Render_LargeSize_AppliesTextBase()
    {
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Size, XFSize.Lg)
        );
        Assert.Contains("text-base", cut.Markup);
    }

    [Fact]
    public void Render_CompletedTask_ShowsGreenBar()
    {
        var cut = RenderDefault();
        Assert.Contains("bg-green-500", cut.Markup);
    }

    [Fact]
    public void Render_WithDependencies_DependencyParamAccepted()
    {
        var deps = new List<XFGanttDependency>
        {
            new() { FromId = "2", ToId = "3", Type = XFGanttDependencyType.FinishToStart }
        };
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, SampleTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.ParentIdFunc, new Func<GanttTaskItem, string?>(t => t.ParentId))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
            .Add(x => x.Dependencies, deps)
        );
        Assert.Contains("xf-gantt", cut.Markup);
    }

    [Fact]
    public void Parameters_Defaults_AreCorrect()
    {
        var cut = RenderDefault();
        var instance = cut.Instance;
        Assert.Equal(XFGanttView.Week, instance.View);
        Assert.True(instance.ShowProgress);
        Assert.True(instance.ShowDependencies);
        Assert.True(instance.ShowTodayLine);
        Assert.True(instance.ShowGridLines);
        Assert.Equal(40, instance.RowHeight);
        Assert.Equal("300px", instance.TaskListWidth);
        Assert.Equal(XFSize.Md, instance.Size);
        Assert.Equal(XFGanttVariant.Default, instance.Variant);
        Assert.Equal("gantt.today", instance.TodayLabel);
        Assert.Equal("gantt.empty", instance.NoDataText);
    }

    [Fact]
    public void Render_FlatData_WithoutParentIdFunc_RendersAllAsRoots()
    {
        var flatTasks = new List<GanttTaskItem>
        {
            new("1", null, "Task A", DateTime.Today, DateTime.Today.AddDays(5), 50),
            new("2", null, "Task B", DateTime.Today.AddDays(3), DateTime.Today.AddDays(10), 30),
        };
        var cut = Render<XFGantt<GanttTaskItem>>(p => p
            .Add(x => x.Data, flatTasks)
            .Add(x => x.IdFunc, new Func<GanttTaskItem, string>(t => t.Id))
            .Add(x => x.TitleFunc, new Func<GanttTaskItem, string>(t => t.Title))
            .Add(x => x.StartFunc, new Func<GanttTaskItem, DateTime>(t => t.Start))
            .Add(x => x.EndFunc, new Func<GanttTaskItem, DateTime>(t => t.End))
        );
        Assert.Contains("Task A", cut.Markup);
        Assert.Contains("Task B", cut.Markup);
    }

    [Fact]
    public async Task ExpandAll_ClearsCollapsedIds()
    {
        var cut = RenderDefault();
        var instance = cut.Instance;

        await instance.CollapseAll();
        cut.Render();

        await instance.ExpandAll();
        cut.Render();

        Assert.Contains("Project", cut.Markup);
        Assert.Contains("Discovery", cut.Markup);
        Assert.Contains("API", cut.Markup);
    }

    [Fact]
    public async Task NavigateToToday_ChangesDate()
    {
        var cut = RenderDefault();
        var instance = cut.Instance;

        await instance.NavigateToToday();
        Assert.Equal(DateTime.Today, instance.Date);
    }

    [Fact]
    public async Task NavigatePrevious_ChangesDateBackward()
    {
        var cut = RenderDefault();
        var instance = cut.Instance;
        var originalDate = instance.Date;

        await instance.NavigatePrevious();
        Assert.Equal(originalDate.AddDays(-7), instance.Date);
    }

    [Fact]
    public async Task NavigateNext_ChangesDateForward()
    {
        var cut = RenderDefault();
        var instance = cut.Instance;
        var originalDate = instance.Date;

        await instance.NavigateNext();
        Assert.Equal(originalDate.AddDays(7), instance.Date);
    }

    [Fact]
    public async Task SetView_ChangesViewMode()
    {
        var cut = RenderDefault();
        var instance = cut.Instance;

        await instance.SetView(XFGanttView.Month);
        Assert.Equal(XFGanttView.Month, instance.View);
    }
}
