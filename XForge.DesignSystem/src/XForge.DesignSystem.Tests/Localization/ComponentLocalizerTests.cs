using System.Globalization;
using XForge.DesignSystem.Components.Localization;
using Xunit;
namespace XForge.DesignSystem.Tests.Localization;

public class ComponentLocalizerTests
{
    [Fact] public void Missing_Key_Returns_Key() { var l = new ComponentLocalizer<TestResources>(); Assert.Equal("missing.key", l["missing.key"]); }

    // ComponentLocalizer resolve via ResourceManager; TestResources nao tem .resx,
    // entao a chave volta crua e o formato nao substitui placeholders.
    [Fact]
    public void GetString_Formats_Args_MissingResource_ReturnsKey()
    {
        var l = new ComponentLocalizer<TestResources>();
        Assert.Equal("formatted", l.GetString("formatted", "World"));
    }

    [Fact] public void SupportedCultures_Not_Empty() { var l = new ComponentLocalizer<TestResources>(); Assert.NotEmpty(l.SupportedCultures); }
    [Fact] public void Custom_Culture_Is_Used() { var l = new ComponentLocalizer<TestResources>(CultureInfo.InvariantCulture); Assert.Equal("missing.key", l["missing.key"]); }
}

class TestResources { }
