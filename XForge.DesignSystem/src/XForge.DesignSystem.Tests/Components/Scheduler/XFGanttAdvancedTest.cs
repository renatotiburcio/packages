using Bunit;
using Xunit;
using XForge.Components.Gantt;

namespace XForge.DesignSystem.Tests.Components.Scheduler;

public class XFGanttAdvancedTest : Bunit.BunitContext
{
    public XFGanttAdvancedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFGanttAdvanced>();
        Assert.Contains("xf-gantt-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithTasks_ShowsBars()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Task 1", DateTime.Today, DateTime.Today.AddDays(5), 0.5),
            new("t2", "Task 2", DateTime.Today.AddDays(3), DateTime.Today.AddDays(10), 0.3, IsCritical: true)
        };

        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.Tasks, tasks));
        Assert.Contains("xf-gantt-advanced", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Render_ShowsToolbar()
    {
        var cut = Render<XFGanttAdvanced>();
        Assert.Contains("xf-gantt-toolbar", cut.Markup);
        Assert.Contains("Add Task", cut.Markup);
    }

    [Fact]
    public void Render_WithTreePanel_ShowsTaskNames()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Design Phase", DateTime.Today, DateTime.Today.AddDays(5)),
            new("t2", "Development", DateTime.Today.AddDays(5), DateTime.Today.AddDays(15))
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.ShowTreePanel, true);
        });
        Assert.Contains("Design Phase", cut.Markup);
        Assert.Contains("Development", cut.Markup);
    }

    [Fact]
    public void Render_WithDependencies_ShowsConnections()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Task 1", DateTime.Today, DateTime.Today.AddDays(5)),
            new("t2", "Task 2", DateTime.Today.AddDays(5), DateTime.Today.AddDays(10))
        };
        var deps = new List<GanttDependency>
        {
            new("t1", "t2")
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.Dependencies, deps);
            p.Add(x => x.ShowDependencies, true);
        });
        Assert.Contains("arrowhead", cut.Markup);
    }

    [Fact]
    public void Render_WithMilestones_ShowsDiamond()
    {
        var tasks = new List<GanttTask>
        {
            new("m1", "Milestone", DateTime.Today.AddDays(5), DateTime.Today.AddDays(5), IsMilestone: true)
        };

        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.Tasks, tasks));
        Assert.Contains("milestone", cut.Markup);
    }

    [Fact]
    public void Render_WithCriticalPath_ShowsRed()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Critical Task", DateTime.Today, DateTime.Today.AddDays(5), IsCritical: true)
        };

        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.Tasks, tasks));
        Assert.Contains("critical", cut.Markup);
    }

    [Fact]
    public void Render_ViewModeButtons_ShowsAllModes()
    {
        var cut = Render<XFGanttAdvanced>();
        Assert.Contains("Day", cut.Markup);
        Assert.Contains("Week", cut.Markup);
        Assert.Contains("Month", cut.Markup);
    }

    [Fact]
    public void Render_WithHierarchy_ShowsNestedTasks()
    {
        var tasks = new List<GanttTask>
        {
            new("parent", "Parent Task", DateTime.Today, DateTime.Today.AddDays(10)),
            new("child1", "Child 1", DateTime.Today, DateTime.Today.AddDays(5), ParentId: "parent"),
            new("child2", "Child 2", DateTime.Today.AddDays(5), DateTime.Today.AddDays(10), ParentId: "parent")
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.ShowTreePanel, true);
            p.Add(x => x.AllowCollapse, true);
        });
        Assert.Contains("Parent Task", cut.Markup);
        Assert.Contains("Child 1", cut.Markup);
    }

    [Fact]
    public void Render_WithDayView_ShowsDayColumns()
    {
        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.ViewMode, GanttViewMode.Day));
        Assert.Contains("xf-gantt-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithMonthView_ShowsMonthGrid()
    {
        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.ViewMode, GanttViewMode.Month));
        Assert.Contains("xf-gantt-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowInlineEdit_ShowsEditOptions()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Editable Task", DateTime.Today, DateTime.Today.AddDays(5))
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.AllowInlineEdit, true);
        });
        Assert.Contains("Editable Task", cut.Markup);
    }

    [Fact]
    public void Render_WithResourcePanel_ShowsResources()
    {
        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.ShowResourcePanel, true));
        Assert.Contains("xf-gantt-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithExport_ShowsExportButton()
    {
        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.AllowExport, true));
        Assert.Contains("Export", cut.Markup);
    }

    [Fact]
    public void Render_WithDragResize_ShowsResizeHandles()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Resizable Task", DateTime.Today, DateTime.Today.AddDays(5))
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.AllowDragResize, true);
        });
        Assert.Contains("Resizable Task", cut.Markup);
        Assert.Contains("xf-gantt-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithProgress_ShowsProgressBars()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Task with Progress", DateTime.Today, DateTime.Today.AddDays(5), Progress: 0.7)
        };

        var cut = Render<XFGanttAdvanced>(p => p.Add(x => x.Tasks, tasks));
        Assert.Contains("Task with Progress", cut.Markup);
    }

    [Fact]
    public void Render_WithBaseline_ShowsBaselineBars()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Baseline Task", DateTime.Today, DateTime.Today.AddDays(5))
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.ShowBaseline, true);
        });
        Assert.Contains("Baseline Task", cut.Markup);
    }

    [Fact]
    public void Render_WithDragResize_AllowsResize()
    {
        var tasks = new List<GanttTask>
        {
            new("t1", "Resizable Task", DateTime.Today, DateTime.Today.AddDays(5))
        };

        var cut = Render<XFGanttAdvanced>(p =>
        {
            p.Add(x => x.Tasks, tasks);
            p.Add(x => x.AllowDragResize, true);
            p.Add(x => x.ShowTreePanel, true);
        });
        Assert.Contains("Resizable Task", cut.Markup);
        Assert.Contains("xf-gantt-advanced", cut.Markup);
    }
}
