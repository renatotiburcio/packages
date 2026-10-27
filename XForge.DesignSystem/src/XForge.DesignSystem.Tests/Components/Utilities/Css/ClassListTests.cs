using XForge.DesignSystem.Components.Utilities.Css;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Utilities.Css;

public class ClassListTests
{
    [Fact]
    public void Default_IsEmpty()
    {
        var list = default(ClassList);
        Assert.True(list.IsEmpty);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Constructor_Null_IsEmpty()
    {
        var list = new ClassList(null);
        Assert.True(list.IsEmpty);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Constructor_Empty_IsEmpty()
    {
        var list = new ClassList("");
        Assert.True(list.IsEmpty);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Constructor_WithClass_HasCount1()
    {
        var list = new ClassList("foo");
        Assert.Equal(1, list.Count);
        Assert.Equal("foo", list.ToString());
    }

    [Fact]
    public void Add_ToEmpty_AddsClass()
    {
        var list = new ClassList().Add("foo");
        Assert.Equal(1, list.Count);
        Assert.Equal("foo", list.ToString());
    }

    [Fact]
    public void Add_Null_ReturnsSame()
    {
        var list = new ClassList("foo");
        var result = list.Add(null);
        Assert.Equal(1, result.Count);
        Assert.Equal("foo", result.ToString());
    }

    [Fact]
    public void Add_Twice_ContainsBoth()
    {
        var list = new ClassList("a").Add("b");
        Assert.Equal(2, list.Count);
        Assert.Equal("a b", list.ToString());
    }

    [Fact]
    public void Add_WithConditionTrue_AddsClass()
    {
        var list = new ClassList().Add("x", true);
        Assert.Equal(1, list.Count);
        Assert.Equal("x", list.ToString());
    }

    [Fact]
    public void Add_WithConditionFalse_DoesNotAdd()
    {
        var list = new ClassList().Add("x", false);
        Assert.True(list.IsEmpty);
    }

    [Fact]
    public void Add_TernaryConditionTrue_AddsTrueName()
    {
        var list = new ClassList().Add("yes", "no", true);
        Assert.Equal("yes", list.ToString());
    }

    [Fact]
    public void Add_TernaryConditionFalse_AddsFalseName()
    {
        var list = new ClassList().Add("yes", "no", false);
        Assert.Equal("no", list.ToString());
    }

    [Fact]
    public void Contains_ExistingClass_ReturnsTrue()
    {
        var list = new ClassList("foo");
        Assert.True(list.Contains("foo"));
    }

    [Fact]
    public void Contains_NonExistingClass_ReturnsFalse()
    {
        var list = new ClassList("foo");
        Assert.False(list.Contains("bar"));
    }

    [Fact]
    public void Contains_EmptyList_ReturnsFalse()
    {
        var list = default(ClassList);
        Assert.False(list.Contains("anything"));
    }

    [Fact]
    public void ToString_Empty_ReturnsEmpty()
    {
        var list = default(ClassList);
        Assert.Equal("", list.ToString());
    }

    [Fact]
    public void ToString_Multiple_JoinsWithSpace()
    {
        var list = new ClassList("a").Add("b").Add("c");
        Assert.Equal("a b c", list.ToString());
    }

    [Fact]
    public void ImplicitConversionToString_Works()
    {
        ClassList list = new ClassList("foo");
        string result = list;
        Assert.Equal("foo", result);
    }

    [Fact]
    public void Enumerate_ReturnsAllClasses()
    {
        var list = new ClassList("a").Add("b");
        var items = list.Enumerate().ToList();
        Assert.Equal(2, items.Count);
        Assert.Contains("a", items);
        Assert.Contains("b", items);
    }

    [Fact]
    public void IsEmpty_TrueWhenEmpty()
    {
        Assert.True(default(ClassList).IsEmpty);
        Assert.True(new ClassList("").IsEmpty);
        Assert.True(new ClassList(null).IsEmpty);
    }

    [Fact]
    public void IsEmpty_FalseWhenHasClasses()
    {
        Assert.False(new ClassList("foo").IsEmpty);
        Assert.False(new ClassList().Add("bar").IsEmpty);
    }

    [Fact]
    public void Add_Chained_Works()
    {
        var list = new ClassList().Add("a").Add("b").Add("c");
        Assert.Equal(3, list.Count);
        Assert.Equal("a b c", list.ToString());
    }
}
