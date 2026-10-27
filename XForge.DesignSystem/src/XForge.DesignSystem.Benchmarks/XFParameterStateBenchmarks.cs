// T055: Benchmarks for parameter state change tracking.
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Weavers;

namespace XForge.DesignSystem.Benchmarks;

[MemoryDiagnoser]
public class XFParameterStateBenchmarks
{
    private XFParameterChangeTracker _tracker = null!;

    [GlobalSetup]
    public void Setup()
    {
        _tracker = new XFParameterChangeTracker(new XFParameterDirtyWeaver(), typeof(DemoBenchmarkComponent));
    }

    [Benchmark(Description = "MarkDirty 1000 times")]
    public int MarkDirty_1000()
    {
        _tracker.Reset();
        for (var i = 0; i < 1000; i++) _tracker.MarkDirty(i % 2 == 0 ? "Name" : "Age");
        return _tracker.DirtyCount;
    }
}

[XFDirtyTracked]
public class DemoBenchmarkComponent
{
    [Parameter] [XFDirty] public string? Name { get; set; }
    [Parameter] public int Age { get; set; }
}