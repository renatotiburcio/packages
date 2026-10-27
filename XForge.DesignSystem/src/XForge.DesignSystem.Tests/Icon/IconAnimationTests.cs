using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconAnimationTests
{
    [Fact]
    public void Animation_Enum_HasFiveValues()
    {
        Assert.Equal(5, System.Enum.GetValues<XFIconAnimation>().Length);
    }

    [Fact]
    public void AnimatedIcons_Loader_HasSpin()
    {
        Assert.True(AnimatedIcons.Catalog["loader"].Animation == XFIconAnimation.Spin);
    }

    [Fact]
    public void AnimatedIcons_Names_NonEmpty()
    {
        Assert.NotEmpty(AnimatedIcons.Names);
    }
}