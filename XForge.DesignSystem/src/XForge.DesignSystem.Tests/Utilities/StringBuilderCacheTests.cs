using System.Text;
using XForge.DesignSystem.Components.Utilities.Strings;
using Xunit;
namespace XForge.DesignSystem.Tests.Utilities;
public class StringBuilderCacheTests
{
    [Fact] public void Acquire_Returns_Non_Null() { var sb = StringBuilderCache.Acquire(); Assert.NotNull(sb); StringBuilderCache.Release(sb); }
    [Fact] public void Acquire_Honors_Capacity() { var sb = StringBuilderCache.Acquire(64); Assert.True(sb.Capacity >= 64); StringBuilderCache.Release(sb); }
    [Fact] public void AcquireAfterRelease_Returns_Cached() { var sb1 = StringBuilderCache.Acquire(); StringBuilderCache.Release(sb1); var sb2 = StringBuilderCache.Acquire(); Assert.Same(sb1, sb2); StringBuilderCache.Release(sb2); }
    [Fact] public void Release_Resizes_If_Too_Large() { var sb = new StringBuilder(8192); StringBuilderCache.Release(sb); var sb2 = StringBuilderCache.Acquire(); Assert.NotSame(sb, sb2); StringBuilderCache.Release(sb2); }
    [Fact] public void AcquireAndRelease_Builds_String() { var s = StringBuilderCache.AcquireAndRelease(16, sb => sb.Append("a").Append("b").Append("c")); Assert.Equal("abc", s); }
    [Fact] public void AcquireAndRelease_Clears_Builder() { StringBuilderCache.AcquireAndRelease(16, sb => sb.Append("xyz")); var s = StringBuilderCache.AcquireAndRelease(16, sb => sb.Append("123")); Assert.Equal("123", s); }
}