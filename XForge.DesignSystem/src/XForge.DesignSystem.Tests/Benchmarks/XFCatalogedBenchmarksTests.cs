// T055: Lightweight tests for benchmark catalog (does not run BenchmarkDotNet).
using Xunit;
using XForge.DesignSystem.Benchmarks;

namespace XForge.DesignSystem.Tests.Benchmarks;

public class XFCatalogedBenchmarksTests
{
    [Fact] public void Count_Is3() => Assert.Equal(3, XFCatalogedBenchmarks.Count);

    [Fact] public void Suite_ContainsStringBuilderCache() => Assert.Contains("StringBuilderCache", XFCatalogedBenchmarks.Suite);
}