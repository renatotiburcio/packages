// T055: Benchmarks for class building.
using BenchmarkDotNet.Attributes;

namespace XForge.DesignSystem.Benchmarks;

[MemoryDiagnoser]
public class XFClassBuilderBenchmarks
{
    [Params(8, 32, 128)] public int N { get; set; }

    [Benchmark(Baseline = true, Description = "String concat")]
    public string Concat()
    {
        var s = string.Empty;
        for (var i = 0; i < N; i++) s += "c" + i;
        return s;
    }

    [Benchmark(Description = "StringBuilder")]
    public string StringBuilder()
    {
        var sb = new System.Text.StringBuilder(N * 4);
        for (var i = 0; i < N; i++) sb.Append("c").Append(i);
        return sb.ToString();
    }

    [Benchmark(Description = "StringBuilderCache")]
    public string StringBuilderCache()
    {
        return XForge.DesignSystem.Components.Utilities.Strings.StringBuilderCache.AcquireAndRelease(N * 4, sb =>
        {
            for (var i = 0; i < N; i++) sb.Append("c").Append(i);
        });
    }
}