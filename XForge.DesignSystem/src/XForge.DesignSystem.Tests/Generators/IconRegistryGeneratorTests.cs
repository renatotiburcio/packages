using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconRegistryGeneratorTests
{
    [Fact]
    public void Attribute_Stamps_UsesIcons()
    {
        var attr = new FluentUIComponentAttribute { UsesIcons = new[] { "home", "add" } };
        Assert.Equal(2, attr.UsesIcons.Length);
    }

    [Fact]
    public void BuildIndex_Returns_MapByComponent()
    {
        var input = new[] { new KeyValuePair<string, string[]>("Button", new[] { "add" }) };
        var index = XFIconRegistryGenerator.BuildIndex(input);
        Assert.Single(index);
        Assert.Contains("add", index["Button"]);
    }

    [Fact]
    public void GenerateSource_ContainsClassDeclaration()
    {
        var input = new Dictionary<string, IReadOnlyList<string>>
        {
            ["Button"] = new[] { "add" },
        };
        var src = XFIconRegistryGenerator.GenerateSource(input);
        Assert.Contains("XFIconCatalogIndex", src);
        Assert.Contains("ButtonIcons", src);
        Assert.Contains("\"add\"", src);
    }
}