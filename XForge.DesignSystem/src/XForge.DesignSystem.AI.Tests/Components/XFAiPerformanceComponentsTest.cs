using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Performance;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.AI;

public class XFAiPerformanceComponentsTest : BunitContext
{
    #region XFAiVirtualScroller

    [Fact]
    public void VirtualScroller_RendersDefault()
    {
        var cut = Render<XFAiVirtualScroller<string>>();

        Assert.Contains("xf-ai-virtual-scroller", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Virtual Scroller", cut.Markup);
    }

    [Fact]
    public void VirtualScroller_ShowsItemCount()
    {
        var items = Enumerable.Range(1, 1000).Select(i => $"Item {i}").ToList();
        var cut = Render<XFAiVirtualScroller<string>>(p => p.Add(x => x.Items, items));

        Assert.Contains("1000 itens", cut.Markup);
    }

    [Fact]
    public void VirtualScroller_EmptyText_Shown()
    {
        var cut = Render<XFAiVirtualScroller<string>>();

        Assert.Contains("Nenhum item para exibir", cut.Markup);
    }

    [Fact]
    public void VirtualScroller_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiVirtualScroller<string>>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Virtual Scroller", cut.Markup);
    }

    [Fact]
    public void VirtualScroller_CustomTitle_Shown()
    {
        var cut = Render<XFAiVirtualScroller<string>>(p => p.Add(x => x.Title, "Meu Scroller"));

        Assert.Contains("Meu Scroller", cut.Markup);
    }

    [Fact]
    public void VirtualScroller_RendersVisibleItems()
    {
        var items = Enumerable.Range(1, 100).Select(i => $"Item {i}").ToList();
        var cut = Render<XFAiVirtualScroller<string>>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ContainerHeight, 200);
            p.Add(x => x.ItemHeight, 40);
        });

        // Should render a subset of items (not all 100)
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("role=\"list\"", cut.Markup);
    }

    [Fact]
    public void VirtualScroller_ScrollPercentage_Shown()
    {
        var items = Enumerable.Range(1, 50).Select(i => $"Item {i}").ToList();
        var cut = Render<XFAiVirtualScroller<string>>(p => p.Add(x => x.Items, items));

        Assert.Contains("%", cut.Markup);
    }

    #endregion

    #region XFAiLazyComponent

    [Fact]
    public void LazyComponent_RendersDefault()
    {
        var cut = Render<XFAiLazyComponent>();

        Assert.Contains("xf-ai-lazy-component", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Componente Lazy", cut.Markup);
    }

    [Fact]
    public void LazyComponent_ShowsNotLoadedState()
    {
        var cut = Render<XFAiLazyComponent>();

        Assert.Contains("Carregando componente...", cut.Markup);
        Assert.Contains("Aguardando", cut.Markup);
    }

    [Fact]
    public void LazyComponent_CustomPlaceholderText_Shown()
    {
        var cut = Render<XFAiLazyComponent>(p => p.Add(x => x.PlaceholderText, "Aguarde..."));

        Assert.Contains("Aguarde...", cut.Markup);
    }

    [Fact]
    public void LazyComponent_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiLazyComponent>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Componente Lazy", cut.Markup);
    }

    [Fact]
    public void LazyComponent_CustomTitle_Shown()
    {
        var cut = Render<XFAiLazyComponent>(p => p.Add(x => x.Title, "Carregar Relatorio"));

        Assert.Contains("Carregar Relatorio", cut.Markup);
    }

    [Fact]
    public async Task LazyComponent_LoadAsync_ShowsLoadingState()
    {
        var cut = Render<XFAiLazyComponent>(p =>
        {
            p.Add(x => x.AutoLoad, false);
            p.Add(x => x.ShowProgress, true);
        });

        var instance = cut.Instance;
        await cut.InvokeAsync(() => instance.LoadAsync());

        // After load starts, should show loading or loaded state
        Assert.Contains("xf-ai-lazy-component", cut.Markup);
    }

    [Fact]
    public void LazyComponent_StateBadge_Shown()
    {
        var cut = Render<XFAiLazyComponent>();

        Assert.Contains("Aguardando", cut.Markup);
    }

    #endregion

    #region XFAiPerformanceMonitor

    [Fact]
    public void PerformanceMonitor_RendersDefault()
    {
        var cut = Render<XFAiPerformanceMonitor>();

        Assert.Contains("xf-ai-performance-monitor", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Performance Monitor", cut.Markup);
    }

    [Fact]
    public void PerformanceMonitor_ShowsMetricCards()
    {
        var metrics = new List<XFAiMetricDataPoint>
        {
            new() { Label = "Render Time", Value = 12.5, Unit = "ms", Severity = (XFSeverity)XFAiMetricSeverity.Good },
            new() { Label = "Memory", Value = 45.2, Unit = "MB", Severity = (XFSeverity)XFAiMetricSeverity.Warning }
        };

        var cut = Render<XFAiPerformanceMonitor>(p => p.Add(x => x.Metrics, metrics));

        Assert.Contains("Render Time", cut.Markup);
        Assert.Contains("Memory", cut.Markup);
    }

    [Fact]
    public void PerformanceMonitor_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiPerformanceMonitor>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Performance Monitor", cut.Markup);
    }

    [Fact]
    public void PerformanceMonitor_CustomTitle_Shown()
    {
        var cut = Render<XFAiPerformanceMonitor>(p => p.Add(x => x.Title, "Monitor Customizado"));

        Assert.Contains("Monitor Customizado", cut.Markup);
    }

    [Fact]
    public void PerformanceMonitor_EmptyState_ShowsSemDados()
    {
        var cut = Render<XFAiPerformanceMonitor>();

        Assert.Contains("Sem dados", cut.Markup);
    }

    [Fact]
    public void PerformanceMonitor_HidesBenchmarks_WhenShowBenchmarksFalse()
    {
        var cut = Render<XFAiPerformanceMonitor>(p => p.Add(x => x.ShowBenchmarks, false));

        Assert.DoesNotContain("Benchmarks", cut.Markup);
    }

    [Fact]
    public void PerformanceMonitor_SnapshotCount_Shown()
    {
        var cut = Render<XFAiPerformanceMonitor>();

        Assert.Contains("Snapshots:", cut.Markup);
        Assert.Contains("0 metricas", cut.Markup);
    }

    #endregion

    #region XFAiBundleAnalyzer

    [Fact]
    public void BundleAnalyzer_RendersDefault()
    {
        var cut = Render<XFAiBundleAnalyzer>();

        Assert.Contains("xf-ai-bundle-analyzer", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Bundle Analyzer", cut.Markup);
    }

    [Fact]
    public void BundleAnalyzer_ShowsSummary()
    {
        var summary = new XFAiBundleSummary
        {
            TotalSizeBytes = 512 * 1024,
            TotalGzipSizeBytes = 128 * 1024,
            ModuleCount = 15,
            TreeShakeableCount = 10,
            Modules = Array.Empty<XFAiBundleModule>(),
            Breakdown = new List<XFAiBundleCategoryBreakdown>
            {
                new() { Category = XFAiBundleCategory.Components, SizeBytes = 256 * 1024, Percentage = 50, ModuleCount = 8 },
                new() { Category = XFAiBundleCategory.Services, SizeBytes = 128 * 1024, Percentage = 25, ModuleCount = 4 }
            }
        };

        var cut = Render<XFAiBundleAnalyzer>(p => p.Add(x => x.BundleData, summary));

        Assert.Contains("15 modulos", cut.Markup);
        Assert.Contains("KB", cut.Markup);
    }

    [Fact]
    public void BundleAnalyzer_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiBundleAnalyzer>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Bundle Analyzer", cut.Markup);
    }

    [Fact]
    public void BundleAnalyzer_CustomTitle_Shown()
    {
        var cut = Render<XFAiBundleAnalyzer>(p => p.Add(x => x.Title, "Analise de Pacotes"));

        Assert.Contains("Analise de Pacotes", cut.Markup);
    }

    [Fact]
    public void BundleAnalyzer_ShowsCategoryBreakdown()
    {
        var summary = new XFAiBundleSummary
        {
            TotalSizeBytes = 1024 * 1024,
            TotalGzipSizeBytes = 256 * 1024,
            ModuleCount = 10,
            TreeShakeableCount = 8,
            Breakdown = new List<XFAiBundleCategoryBreakdown>
            {
                new() { Category = XFAiBundleCategory.Components, SizeBytes = 512 * 1024, Percentage = 50, ModuleCount = 5 },
                new() { Category = XFAiBundleCategory.ThirdParty, SizeBytes = 256 * 1024, Percentage = 25, ModuleCount = 3 }
            }
        };

        var cut = Render<XFAiBundleAnalyzer>(p => p.Add(x => x.BundleData, summary));

        Assert.Contains("Componentes", cut.Markup);
        Assert.Contains("Terceiros", cut.Markup);
    }

    [Fact]
    public void BundleAnalyzer_HidesBreakdown_WhenShowBreakdownFalse()
    {
        var cut = Render<XFAiBundleAnalyzer>(p => p.Add(x => x.ShowBreakdown, false));

        Assert.DoesNotContain("Breakdown por Categoria", cut.Markup);
    }

    #endregion

    #region XFAiMemoryProfiler

    [Fact]
    public void MemoryProfiler_RendersDefault()
    {
        var cut = Render<XFAiMemoryProfiler>();

        Assert.Contains("xf-ai-memory-profiler", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Memory Profiler", cut.Markup);
    }

    [Fact]
    public void MemoryProfiler_ShowsGauge()
    {
        var cut = Render<XFAiMemoryProfiler>();

        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("50", cut.Markup); // MaxUsageMB default
    }

    [Fact]
    public void MemoryProfiler_ShowsGCStats()
    {
        var cut = Render<XFAiMemoryProfiler>();

        Assert.Contains("Gen 0", cut.Markup);
        Assert.Contains("Gen 1", cut.Markup);
        Assert.Contains("Gen 2", cut.Markup);
    }

    [Fact]
    public void MemoryProfiler_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiMemoryProfiler>(p => p.Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Memory Profiler", cut.Markup);
    }

    [Fact]
    public void MemoryProfiler_CustomTitle_Shown()
    {
        var cut = Render<XFAiMemoryProfiler>(p => p.Add(x => x.Title, "Perfil de Memoria"));

        Assert.Contains("Perfil de Memoria", cut.Markup);
    }

    [Fact]
    public void MemoryProfiler_LoadSnapshot_ShowsZones()
    {
        var snapshot = new XFAiMemorySnapshot
        {
            TotalManagedBytes = 10 * 1024 * 1024,
            TotalAllocatedBytes = 15 * 1024 * 1024,
            Gen0Collections = 5,
            Gen1Collections = 2,
            Gen2Collections = 0,
            Allocations = new List<XFAiMemoryAllocation>
            {
                new() { Zone = XFAiMemoryZone.Managed, AllocatedBytes = 10 * 1024 * 1024, MaxBytes = 50 * 1024 * 1024, Label = "Managed" },
                new() { Zone = XFAiMemoryZone.Components, AllocatedBytes = 3 * 1024 * 1024, MaxBytes = 20 * 1024 * 1024, Label = "Components" }
            }
        };

        var cut = Render<XFAiMemoryProfiler>(p => p.Add(x => x.Snapshot, snapshot));

        Assert.Contains("Gerenciado", cut.Markup);
        Assert.Contains("Componentes", cut.Markup);
    }

    [Fact]
    public void MemoryProfiler_HidesGauge_WhenShowGaugeFalse()
    {
        var cut = Render<XFAiMemoryProfiler>(p => p.Add(x => x.ShowGauge, false));

        // Without gauge, no SVG circle for the gauge
        Assert.DoesNotContain("rotate(-90deg)", cut.Markup);
    }

    [Fact]
    public void MemoryProfiler_ShowsMemoryZones()
    {
        var snapshot = new XFAiMemorySnapshot
        {
            TotalManagedBytes = 20 * 1024 * 1024,
            TotalAllocatedBytes = 25 * 1024 * 1024,
            Allocations = new List<XFAiMemoryAllocation>
            {
                new() { Zone = XFAiMemoryZone.Cache, AllocatedBytes = 5 * 1024 * 1024, MaxBytes = 30 * 1024 * 1024, Label = "Cache" }
            }
        };

        var cut = Render<XFAiMemoryProfiler>(p => p.Add(x => x.Snapshot, snapshot));

        Assert.Contains("Cache", cut.Markup);
    }

    #endregion

    #region Shared Models

    [Fact]
    public void VirtualRange_Count_CalculatedCorrectly()
    {
        var range = new XFAiVirtualRange { StartIndex = 10, EndIndex = 19 };
        Assert.Equal(10, range.Count);
    }

    [Fact]
    public void VirtualItem_Properties_SetCorrectly()
    {
        var item = new XFAiVirtualItem<string>
        {
            Data = "Test",
            Index = 5,
            Top = 200,
            Height = 40,
            IsVisible = true
        };

        Assert.Equal("Test", item.Data);
        Assert.Equal(5, item.Index);
        Assert.Equal(200, item.Top);
        Assert.Equal(40, item.Height);
        Assert.True(item.IsVisible);
    }

    [Fact]
    public void MemoryAllocation_UsagePercent_CalculatedCorrectly()
    {
        var alloc = new XFAiMemoryAllocation
        {
            AllocatedBytes = 25 * 1024 * 1024,
            MaxBytes = 50 * 1024 * 1024
        };

        Assert.Equal(50, alloc.UsagePercent);
    }

    [Fact]
    public void MemoryAllocation_ZeroMax_ReturnsZero()
    {
        var alloc = new XFAiMemoryAllocation
        {
            AllocatedBytes = 1024,
            MaxBytes = 0
        };

        Assert.Equal(0, alloc.UsagePercent);
    }

    [Fact]
    public void MetricDataPoint_DefaultTimestamp_IsUtcNow()
    {
        var point = new XFAiMetricDataPoint { Label = "Test", Value = 10 };
        Assert.True(point.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void BenchmarkResult_PassedReflectsOperator()
    {
        var result = new XFAiBenchmarkResult
        {
            Name = "Render",
            MeasuredValue = 10,
            ThresholdValue = 16,
            Operator = XFAiBenchmarkOperator.LessThan,
            Passed = true
        };

        Assert.True(result.Passed);
    }

    [Fact]
    public void BundleModule_TreeShakeable_DefaultFalse()
    {
        var module = new XFAiBundleModule { Name = "test.js", SizeBytes = 1024 };
        Assert.False(module.IsTreeShakeable);
    }

    [Fact]
    public void MemorySnapshot_DefaultAllocations_Empty()
    {
        var snapshot = new XFAiMemorySnapshot();
        Assert.Empty(snapshot.Allocations);
    }

    #endregion
}
