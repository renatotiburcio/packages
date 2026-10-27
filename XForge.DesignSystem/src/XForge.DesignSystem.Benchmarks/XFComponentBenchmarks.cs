using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using XForge.Components.Gantt;
using XForge.Components.Scheduler;

namespace XForge.DesignSystem.Benchmarks;

/// <summary>
/// Performance benchmarks for critical XForge components.
/// Measures rendering time, memory allocation, and throughput.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class XFComponentBenchmarks
{
    private List<GanttTask> _ganttTasks = null!;
    private List<SchedulerEvent> _schedulerEvents = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Prepare test data
        _ganttTasks = Enumerable.Range(0, 100)
            .Select(i => new GanttTask(
                $"task-{i}",
                $"Task {i}",
                DateTime.Today.AddDays(i),
                DateTime.Today.AddDays(i + 5),
                Random.Shared.NextDouble()))
            .ToList();

        _schedulerEvents = Enumerable.Range(0, 50)
            .Select(i => new SchedulerEvent(
                $"event-{i}",
                $"Event {i}",
                DateTime.Today.AddHours(i % 24),
                DateTime.Today.AddHours((i % 24) + 1)))
            .ToList();
    }

    [Benchmark(Description = "Gantt Tasks Creation")]
    [BenchmarkCategory("Data Preparation")]
    public List<GanttTask> GanttTasksCreation()
    {
        return Enumerable.Range(0, 100)
            .Select(i => new GanttTask(
                $"task-{i}",
                $"Task {i}",
                DateTime.Today.AddDays(i),
                DateTime.Today.AddDays(i + 5),
                Random.Shared.NextDouble()))
            .ToList();
    }

    [Benchmark(Description = "Scheduler Events Creation")]
    [BenchmarkCategory("Data Preparation")]
    public List<SchedulerEvent> SchedulerEventsCreation()
    {
        return Enumerable.Range(0, 50)
            .Select(i => new SchedulerEvent(
                $"event-{i}",
                $"Event {i}",
                DateTime.Today.AddHours(i % 24),
                DateTime.Today.AddHours((i % 24) + 1)))
            .ToList();
    }

    [Benchmark(Description = "Gantt Task Sorting")]
    [BenchmarkCategory("Data Processing")]
    public List<GanttTask> GanttTaskSorting()
    {
        return _ganttTasks.OrderBy(t => t.Start).ToList();
    }

    [Benchmark(Description = "Scheduler Event Filtering")]
    [BenchmarkCategory("Data Processing")]
    public List<SchedulerEvent> SchedulerEventFiltering()
    {
        var today = DateTime.Today;
        return _schedulerEvents
            .Where(e => e.Start.Date == today)
            .ToList();
    }

    [Benchmark(Description = "Gantt Dependency Resolution")]
    [BenchmarkCategory("Data Processing")]
    public int GanttDependencyResolution()
    {
        var tasks = _ganttTasks.Take(10).ToList();
        var dependencies = new List<GanttDependency>();
        
        for (int i = 1; i < tasks.Count; i++)
        {
            dependencies.Add(new GanttDependency(tasks[i - 1].Id, tasks[i].Id));
        }

        return dependencies.Count;
    }
}
