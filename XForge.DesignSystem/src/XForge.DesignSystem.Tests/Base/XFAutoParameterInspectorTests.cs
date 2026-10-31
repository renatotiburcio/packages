using System.Linq;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Base;
using FluentAssertions;
using Xunit;

namespace XForge.Tests.Base;

public class XFAutoParameterInspectorTests
{
    [Fact]
    public void GetParameterNames_ReturnsOnlyParameter()
    {
        var names = XFAutoParameterInspector.GetParameterNames(typeof(SampleComponent));
        names.Should().Contain("Value");
        names.Should().Contain("Disabled");
        names.Should().NotContain("Helper");
    }

    [Fact]
    public void GetParameterNames_Null_Throws()
    {
        var act = () => XFAutoParameterInspector.GetParameterNames(null!);
        act.Should().Throw<System.ArgumentNullException>();
    }

    [Fact]
    public void GetParameters_ReturnsProperties()
    {
        var props = XFAutoParameterInspector.GetParameters(typeof(SampleComponent));
        props.Should().HaveCountGreaterThan(0);
    }

    private sealed class SampleComponent : ComponentBase
    {
        [Parameter] public int Value { get; set; }
        [Parameter] public bool Disabled { get; set; }
        public string? Helper { get; set; }
    }
}