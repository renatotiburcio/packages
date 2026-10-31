using System.Threading.Tasks;
using XForge.DesignSystem.Components.Base;
using FluentAssertions;
using Xunit;

namespace XForge.Tests.Base;

public class XFFormForwardRefTests
{
    [Fact]
    public void ForwardRef_DefaultIsNull()
    {
        var r = new XFFormForwardRef<string>();
        r.Current.Should().BeNull();
    }

    [Fact]
    public void ForwardRef_CaptureStores()
    {
        var r = new XFFormForwardRef<string>();
        r.Capture("hello");
        r.Current.Should().Be("hello");
    }

    [Fact]
    public void ForwardRef_ReleaseClears()
    {
        var r = new XFFormForwardRef<string>();
        r.Capture("hello");
        r.Release();
        r.Current.Should().BeNull();
    }

    [Fact]
    public void ForwardRef_GenericInt()
    {
        var r = new XFFormForwardRef<int>();
        r.Capture(42);
        r.Current.Should().Be(42);
    }

    [Fact]
    public void ForwardRef_ReplaceAccepts()
    {
        var r = new XFFormForwardRef<string>();
        r.Capture("a");
        r.Capture("b");
        r.Current.Should().Be("b");
    }
}