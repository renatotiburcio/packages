using System.Text;
using XForge.DesignSystem.Components.Utilities.Strings;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Utilities.Strings;

public sealed class StringBuilderCacheTests
{
    [Fact]
    public void Acquire_DefaultCapacity_ReturnsNonNull()
    {
        var sb = StringBuilderCache.Acquire();
        Assert.NotNull(sb);
        StringBuilderCache.Release(sb);
    }

    [Fact]
    public void Acquire_SpecificCapacity_ReturnsWithCapacity()
    {
        var sb = StringBuilderCache.Acquire(256);
        Assert.True(sb.Capacity >= 256);
        StringBuilderCache.Release(sb);
    }

    [Fact]
    public void Acquire_ClearedContent()
    {
        var sb = StringBuilderCache.Acquire();
        sb.Append("dirty");
        StringBuilderCache.Release(sb);
        var sb2 = StringBuilderCache.Acquire();
        Assert.Equal(0, sb2.Length);
        StringBuilderCache.Release(sb2);
    }

    [Fact]
    public void Acquire_ExpandsCapacityIfNeeded()
    {
        var sb = StringBuilderCache.Acquire(100);
        Assert.True(sb.Capacity >= 100);
        StringBuilderCache.Release(sb);
    }

    [Fact]
    public void Release_Null_DoesNotThrow()
    {
        StringBuilderCache.Release(null);
    }

    [Fact]
    public void Release_LargeBuilder_DoesNotCache()
    {
        var sb = StringBuilderCache.Acquire(5000);
        Assert.True(sb.Capacity >= 4096);
        StringBuilderCache.Release(sb);
        var sb2 = StringBuilderCache.Acquire();
        Assert.NotSame(sb, sb2);
        StringBuilderCache.Release(sb2);
    }

    [Fact]
    public void AcquireAndRelease_ReturnsBuiltString()
    {
        var result = StringBuilderCache.AcquireAndRelease(16, sb => sb.Append("hello"));
        Assert.Equal("hello", result);
    }

    [Fact]
    public void AcquireAndRelease_ReleasesBuilder()
    {
        var result1 = StringBuilderCache.AcquireAndRelease(16, sb => sb.Append("first"));
        var result2 = StringBuilderCache.AcquireAndRelease(16, sb => sb.Append("second"));
        Assert.Equal("first", result1);
        Assert.Equal("second", result2);
    }

    [Fact]
    public void AcquireAndRelease_AfterLoop_Releases()
    {
        for (var i = 0; i < 100; i++)
        {
            var n = i;
            var result = StringBuilderCache.AcquireAndRelease(16, sb => sb.Append(n));
            Assert.Equal(n.ToString(), result);
        }
    }

    [Fact]
    public void AcquireAndRelease_EmptyBuild_ReturnsEmptyString()
    {
        var result = StringBuilderCache.AcquireAndRelease(16, _ => { });
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Acquire_WasReleased_ReusesInstance()
    {
        var sb = StringBuilderCache.Acquire();
        Assert.NotNull(sb);
        StringBuilderCache.Release(sb);
        var sb2 = StringBuilderCache.Acquire();
        Assert.NotNull(sb2);
        StringBuilderCache.Release(sb2);
    }
}
