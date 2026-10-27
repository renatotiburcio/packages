using System.Threading.Tasks;
using XForge.Core.State;
using Xunit;
using FluentAssertions;

namespace XForge.Tests.State;

public class ParameterStateTests
{
    [Fact]
    public void Constructor_NullGetter_Throws()
    {
        var act = () => new ParameterState<int>(null!);
        act.Should().Throw<System.ArgumentNullException>();
    }

    [Fact]
    public async Task Value_BeforeInitialize_Throws()
    {
        var ps = new ParameterState<int>(() => Task.FromResult(42));
        var act = () => { var _ = ps.Value; };
        act.Should().Throw<System.InvalidOperationException>();
    }

    [Fact]
    public async Task InitializeAsync_StoresValue()
    {
        var ps = new ParameterState<int>(() => Task.FromResult(42));
        await ps.InitializeAsync();
        ps.Value.Should().Be(42);
    }

    [Fact]
    public async Task SetValueAsync_UpdatesValue()
    {
        var ps = new ParameterState<int>(() => Task.FromResult(1));
        await ps.InitializeAsync();
        await ps.SetValueAsync(99);
        ps.Value.Should().Be(99);
    }

    [Fact]
    public void Reset_ClearsState()
    {
        var ps = new ParameterState<int>(() => Task.FromResult(1));
        ps.Reset();
        var act = () => { var _ = ps.Value; };
        act.Should().Throw<System.InvalidOperationException>();
    }
}