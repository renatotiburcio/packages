using System.Linq;
using XForge.Localization.Json;
using FluentAssertions;
using Xunit;

namespace XForge.Tests.Localization;

public class XFJsonLocaleProviderTests
{
    [Fact]
    public void DefaultCulture_IsEn()
    {
        var p = new XFJsonLocaleProvider();
        p.CurrentCulture.Should().Be("en");
    }

    [Fact]
    public void DefaultLocaleCount_IsZero()
    {
        var p = new XFJsonLocaleProvider();
        p.LoadedLocaleCount.Should().Be(0);
    }

    [Fact]
    public void RegisterLocale_AddsToCount()
    {
        var p = new XFJsonLocaleProvider();
        p.RegisterLocale("en", new System.Collections.Generic.Dictionary<string, string> { { "x", "y" } });
        p.LoadedLocaleCount.Should().Be(1);
    }

    [Fact]
    public void Get_AfterRegister_ReturnsValue()
    {
        var p = new XFJsonLocaleProvider();
        p.RegisterLocale("en", new System.Collections.Generic.Dictionary<string, string> { { "x", "y" } });
        p.Get("x").Should().Be("y");
    }

    [Fact]
    public void Get_FallsBackToEn()
    {
        var p = new XFJsonLocaleProvider();
        p.RegisterLocale("en", new System.Collections.Generic.Dictionary<string, string> { { "x", "y" } });
        p.CurrentCulture = "pt-BR";
        p.Get("x").Should().Be("y");
    }

    [Fact]
    public void Get_UnknownKey_ReturnsNull()
    {
        var p = new XFJsonLocaleProvider();
        p.Get("nope").Should().BeNull();
    }

    [Fact]
    public void RegisterLocaleFromJson_Parses()
    {
        var p = new XFJsonLocaleProvider();
        p.RegisterLocaleFromJson("en", "{\"a\":\"b\"}");
        p.Get("a").Should().Be("b");
    }

    [Fact]
    public void AvailableCultures_ReturnsLoaded()
    {
        var p = new XFJsonLocaleProvider();
        p.RegisterLocale("en", new System.Collections.Generic.Dictionary<string, string>());
        p.RegisterLocale("pt-BR", new System.Collections.Generic.Dictionary<string, string>());
        p.AvailableCultures.Should().HaveCount(2);
    }
}