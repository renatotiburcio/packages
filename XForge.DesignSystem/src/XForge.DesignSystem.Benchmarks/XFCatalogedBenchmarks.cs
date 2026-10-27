// T055: Blazorise.Benchmark project (BenchmarkDotNet).
// P2, 16h, depends T025 (BenchmarkDotNet suite).
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace XForge.DesignSystem.Benchmarks;

/// <summary>Marker summary of every benchmark shipped in the XForge.Benchmark project.</summary>
public static class XFCatalogedBenchmarks
{
    public const int Count = 3;
    public static readonly string[] Suite = { "ClassBuilder", "ParameterState", "StringBuilderCache" };
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class XFBenchmarkSuite
{
    public static void Run() => BenchmarkRunner.Run<XFBenchmarkSuite>();
}