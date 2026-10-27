using XForge.DesignSystem.Components;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class ComponentDiscoveryTests
{
    [Fact]
    public void GetByName_Finds_Exact()
    {
        var d = new XFComponentDiscovery(new[]
        {
            new XFComponentInfo("Button", "Clickable", "Forms", new[] { "btn" }, new[] { "add" }),
        });
        Assert.NotNull(d.GetByName("Button"));
        Assert.Equal("Clickable", d.GetByName("Button")!.Description);
    }

    [Fact]
    public void GetByCategory_Filters()
    {
        var d = new XFComponentDiscovery(new[]
        {
            new XFComponentInfo("Button", "", "Forms", System.Array.Empty<string>(), System.Array.Empty<string>()),
            new XFComponentInfo("Card", "", "Layout", System.Array.Empty<string>(), System.Array.Empty<string>()),
        });
        Assert.Single(d.GetByCategory("Forms"));
    }

    [Fact]
    public void GetAll_Returns_All()
    {
        var d = new XFComponentDiscovery(new[]
        {
            new XFComponentInfo("A", "", "x", System.Array.Empty<string>(), System.Array.Empty<string>()),
            new XFComponentInfo("B", "", "y", System.Array.Empty<string>(), System.Array.Empty<string>()),
        });
        Assert.Equal(2, System.Linq.Enumerable.Count(d.GetAll()));
    }
}