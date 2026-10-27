using Xunit;
using XForge.DesignSystem.Providers;

namespace XForge.DesignSystem.Tests.Providers;

public sealed class XFTailwindClassProviderTests
{
    private readonly XFTailwindClassProvider _provider = new();

    [Fact]
    public void ProviderName_ReturnsTailwind()
    {
        Assert.Equal("Tailwind", _provider.ProviderName);
    }

    [Fact]
    public void Button_Default_ReturnsBlueFilled()
    {
        var result = _provider.Button(outline: false);
        Assert.Contains("bg-", result);
        Assert.Contains("text-white", result);
        Assert.Contains("rounded", result);
    }

    [Fact]
    public void Button_Outline_ReturnsBorderClass()
    {
        var result = _provider.Button(outline: true);
        Assert.Contains("border", result);
        Assert.Contains("text-", result);
        Assert.Contains("hover:bg-", result);
    }

    [Fact]
    public void Button_Sm_Size()
    {
        var result = _provider.Button(outline: false, size: "sm");
        Assert.Contains("px-2 py-1 text-sm", result);
    }

    [Fact]
    public void Button_Lg_Size()
    {
        var result = _provider.Button(outline: false, size: "lg");
        Assert.Contains("px-4 py-2 text-lg", result);
    }

    [Fact]
    public void Button_CustomColor()
    {
        var result = _provider.Button(outline: false, color: "red");
        Assert.Contains("bg-red-600", result);
    }

    [Fact]
    public void Button_OutlineCustomColor()
    {
        var result = _provider.Button(outline: true, color: "red");
        Assert.Contains("border-red", result);
        Assert.Contains("text-red", result);
    }

    [Fact]
    public void Input_Default()
    {
        var result = _provider.Input();
        Assert.Contains("border-gray-300", result);
        Assert.Contains("focus:ring-2", result);
    }

    [Fact]
    public void Input_Disabled()
    {
        var result = _provider.Input(disabled: true);
        Assert.Contains("opacity-50 cursor-not-allowed", result);
    }

    [Fact]
    public void Input_CustomColor()
    {
        var result = _provider.Input(color: "green");
        Assert.Contains("focus:ring-green-500", result);
    }

    [Fact]
    public void Card_ReturnsExactString()
    {
        Assert.Equal("bg-white rounded-lg shadow p-4", _provider.Card());
    }

    [Fact]
    public void Modal_ReturnsExactString()
    {
        Assert.Equal("fixed inset-0 z-50 flex items-center justify-center bg-black/50", _provider.Modal());
    }

    [Fact]
    public void Table_ReturnsExactString()
    {
        Assert.Equal("min-w-full divide-y divide-gray-200", _provider.Table());
    }

    [Fact]
    public void Alert_Default()
    {
        var result = _provider.Alert();
        Assert.Contains("bg-blue-50", result);
        Assert.Contains("text-blue-900", result);
    }

    [Fact]
    public void Alert_CustomColor()
    {
        var result = _provider.Alert(color: "red");
        Assert.Contains("bg-red-50", result);
        Assert.Contains("text-red-900", result);
    }

    [Fact]
    public void Badge_Default()
    {
        var result = _provider.Badge();
        Assert.Contains("bg-gray-100", result);
    }

    [Fact]
    public void Badge_CustomColor()
    {
        var result = _provider.Badge(color: "green");
        Assert.Contains("bg-green-100", result);
        Assert.Contains("text-green-800", result);
    }
}
