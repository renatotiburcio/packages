using Xunit;
using XForge.Utilities.OneOf;

namespace XForge.DesignSystem.Tests.Utilities.OneOf;

public class XFFlexibleTypeTests
{
    [Fact]
    public void From_String_ReturnsStringKind()
    {
        var x = XFFlexibleType.From("hello");
        Assert.Equal(XFFlexibleTypeKind.String, x.Kind);
        Assert.Equal("hello", x.StringValue);
    }

    [Fact]
    public void From_Int_ReturnsIntKind()
    {
        var x = XFFlexibleType.From(42);
        Assert.Equal(XFFlexibleTypeKind.Int, x.Kind);
        Assert.Equal(42, x.IntValue);
    }

    [Fact]
    public void From_Bool_ReturnsBoolKind()
    {
        var x = XFFlexibleType.From(true);
        Assert.Equal(XFFlexibleTypeKind.Bool, x.Kind);
        Assert.True(x.BoolValue);
    }

    [Fact]
    public void From_Null_ReturnsNullKind()
    {
        var x = XFFlexibleType.From((object?)null);
        Assert.Equal(XFFlexibleTypeKind.Null, x.Kind);
    }

    [Fact]
    public void From_ObjectNull_ReturnsNullKind()
    {
        var x = XFFlexibleType.From((object?)null);
        Assert.Equal(XFFlexibleTypeKind.Null, x.Kind);
    }

    [Fact]
    public void From_ObjectString_ReturnsStringKind()
    {
        var x = XFFlexibleType.From((object?)"world");
        Assert.Equal(XFFlexibleTypeKind.String, x.Kind);
        Assert.Equal("world", x.StringValue);
    }

    [Fact]
    public void From_ObjectInt_ReturnsIntKind()
    {
        var x = XFFlexibleType.From((object?)99);
        Assert.Equal(XFFlexibleTypeKind.Int, x.Kind);
        Assert.Equal(99, x.IntValue);
    }

    [Fact]
    public void From_ObjectBool_ReturnsBoolKind()
    {
        var x = XFFlexibleType.From((object?)false);
        Assert.Equal(XFFlexibleTypeKind.Bool, x.Kind);
        Assert.False(x.BoolValue);
    }

    [Fact]
    public void From_UnsupportedType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => XFFlexibleType.From(3.14));
    }

    [Fact]
    public void Match_String_CallsOnString()
    {
        var x = XFFlexibleType.From("hi");
        var result = x.Match(s => s + "!", i => "int", b => "bool", () => "null");
        Assert.Equal("hi!", result);
    }

    [Fact]
    public void Match_Int_CallsOnInt()
    {
        var x = XFFlexibleType.From(7);
        var result = x.Match<string>(s => "string", i => $"{i * 2}", b => "bool", () => "null");
        Assert.Equal("14", result);
    }

    [Fact]
    public void Match_Bool_CallsOnBool()
    {
        var x = XFFlexibleType.From(true);
        var result = x.Match(s => "string", i => "int", b => b ? "yes" : "no", () => "null");
        Assert.Equal("yes", result);
    }

    [Fact]
    public void Match_Null_CallsOnNull()
    {
        var x = XFFlexibleType.From((object?)null);
        var result = x.Match(s => "string", i => "int", b => "bool", () => "null");
        Assert.Equal("null", result);
    }

    [Fact]
    public void Enum_HasFourMembers()
    {
        Assert.Equal(4, Enum.GetValues<XFFlexibleTypeKind>().Length);
        Assert.Equal(0, (int)XFFlexibleTypeKind.Null);
        Assert.Equal(1, (int)XFFlexibleTypeKind.String);
        Assert.Equal(2, (int)XFFlexibleTypeKind.Int);
        Assert.Equal(3, (int)XFFlexibleTypeKind.Bool);
    }
}
