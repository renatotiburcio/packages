using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconLruCacheTests
{
    [Fact]
    public void GetOrAdd_Caches_FirstHitMiss()
    {
        var cache = new XFIconLruCache();
        var v1 = cache.GetOrAdd("k", () => "v1");
        var v2 = cache.GetOrAdd("k", () => "v2");
        Assert.Equal("v1", v1);
        Assert.Equal("v1", v2);
        Assert.Equal(1, cache.Hits);
        Assert.Equal(1, cache.Misses);
    }

    [Fact]
    public void Invalidate_RemovesEntry()
    {
        var cache = new XFIconLruCache();
        cache.GetOrAdd("k", () => "v");
        cache.Invalidate("k");
        var v = cache.GetOrAdd("k", () => "new");
        Assert.Equal("new", v);
    }

    [Fact]
    public void Eviction_Bounds_Memory()
    {
        var cache = new XFIconLruCache(maxBytes: 100);
        for (int i = 0; i < 20; i++)
            cache.GetOrAdd($"k{i}", () => new string('x', 50));
        Assert.True(cache.Evictions > 0);
    }
}