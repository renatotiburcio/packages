using XForge.Components.Base;
using FluentAssertions;
using Xunit;

namespace XForge.Tests.Base;

public class XFXFComponentBaseTests
{
    [Fact]
    public void DefaultTheme_IsDefault()
    {
        var c = new TestComp();
        c.PublicTheme.Should().Be("default");
    }

    [Fact]
    public void DefaultBreakpoint_IsMd()
    {
        var c = new TestComp();
        c.PublicBreakpoint.Should().Be("md");
    }

    [Fact]
    public void DefaultRTL_IsFalse()
    {
        var c = new TestComp();
        c.PublicRTL.Should().BeFalse();
    }

    [Fact]
    public void IsDarkMode_TrueWhenDark()
    {
#pragma warning disable BL0005
        var c = new TestComp { Theme = "dark" };
#pragma warning restore BL0005
        c.PublicIsDark.Should().BeTrue();
    }

    [Fact]
    public void IsDarkMode_FalseWhenLight()
    {
#pragma warning disable BL0005
        var c = new TestComp { Theme = "light" };
#pragma warning restore BL0005
        c.PublicIsDark.Should().BeFalse();
    }

    private sealed class TestComp : XFXFComponentBase
    {
        public string PublicTheme => EffectiveTheme;
        public string PublicBreakpoint => EffectiveBreakpoint;
        public bool PublicRTL => RightToLeft;
        public bool PublicIsDark => IsDarkMode;
    }
}