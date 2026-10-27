using XForge.Components.Base;
using FluentAssertions;
using Xunit;

namespace XForge.Tests.Base;

public class XFCategoryAttributeTests
{
    [Fact]
    public void Ctor_StoresCategory()
    {
        var attr = new XFCategoryAttribute(XFCategories.Layout);
        attr.Category.Should().Be("Layout");
    }

    [Fact]
    public void Ctor_NullCategory_Throws()
    {
        var act = () => new XFCategoryAttribute(null!);
        act.Should().Throw<System.ArgumentNullException>();
    }

    [Fact]
    public void SubCategory_DefaultNull()
    {
        var attr = new XFCategoryAttribute(XFCategories.Layout);
        attr.SubCategory.Should().BeNull();
    }

    [Fact]
    public void SubCategory_Settable()
    {
        var attr = new XFCategoryAttribute(XFCategories.Layout) { SubCategory = "Grid" };
        attr.SubCategory.Should().Be("Grid");
    }

    [Fact]
    public void XFCategories_Constants()
    {
        XFCategories.Layout.Should().NotBeNullOrEmpty();
        XFCategories.Inputs.Should().NotBeNullOrEmpty();
        XFCategories.AI.Should().NotBeNullOrEmpty();
        XFCategories.Enterprise.Should().NotBeNullOrEmpty();
    }
}