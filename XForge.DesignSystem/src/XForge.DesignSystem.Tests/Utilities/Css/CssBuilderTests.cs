using Xunit;
using XForge.Utilities.Css;

namespace XForge.DesignSystem.Tests.Utilities.Css;

public class CssBuilderTests
{
    [Fact]
    public void Add_Null_DoesNotChange()
    {
        var builder = new CssBuilder();
        builder.Add((string?)null);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Add_Empty_DoesNotChange()
    {
        var builder = new CssBuilder();
        builder.Add(string.Empty);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Add_Whitespace_DoesNotChange()
    {
        var builder = new CssBuilder();
        builder.Add("   ");
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Add_Valid_AddsClass()
    {
        var builder = new CssBuilder();
        builder.Add("foo");
        Assert.Equal("foo", builder.ToString());
    }

    [Fact]
    public void Add_TrimsWhitespace()
    {
        var builder = new CssBuilder();
        builder.Add("  foo  ");
        Assert.Equal("foo", builder.ToString());
    }

    [Fact]
    public void Add_Multiple_JoinsWithSpace()
    {
        var builder = new CssBuilder();
        builder.Add("a").Add("b");
        Assert.Equal("a b", builder.ToString());
    }

    [Fact]
    public void Add_FactoryNull_ReturnsSame()
    {
        var builder = new CssBuilder();
        var result = builder.Add((Func<string?>)null);
        Assert.Same(builder, result);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Add_FactoryValid_CallsFactory()
    {
        var builder = new CssBuilder();
        var result = builder.Add(() => "bar");
        Assert.Same(builder, result);
        Assert.Equal("bar", builder.ToString());
    }

    [Fact]
    public void Add_WhenTrue_AddsClass()
    {
        var builder = new CssBuilder();
        builder.Add("x", true);
        Assert.Equal("x", builder.ToString());
    }

    [Fact]
    public void Add_WhenFalse_DoesNotAdd()
    {
        var builder = new CssBuilder();
        builder.Add("x", false);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Add_WhenFactoryNull_ReturnsSame()
    {
        var builder = new CssBuilder();
        var result = builder.Add("x", (Func<bool>)null);
        Assert.Same(builder, result);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Add_WhenFactoryTrue_Adds()
    {
        var builder = new CssBuilder();
        builder.Add("x", () => true);
        Assert.Equal("x", builder.ToString());
    }

    [Fact]
    public void Add_WhenFactoryFalse_DoesNotAdd()
    {
        var builder = new CssBuilder();
        builder.Add("x", () => false);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void ToString_CachesResult()
    {
        var builder = new CssBuilder();
        builder.Add("a");
        var first = builder.ToString();
        var second = builder.ToString();
        Assert.Equal(first, second);
    }

    [Fact]
    public void ToString_Empty_ReturnsEmptyString()
    {
        var builder = new CssBuilder();
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void ImplicitConversionToString_Works()
    {
        var builder = new CssBuilder();
        builder.Add("hello");
        string s = builder;
        Assert.Equal("hello", s);
    }

    [Fact]
    public void Add_Chained_Fluent()
    {
        var builder = new CssBuilder();
        var result = builder.Add("a").Add("b").Add("c");
        Assert.Same(builder, result);
        Assert.Equal("a b c", builder.ToString());
    }
}
