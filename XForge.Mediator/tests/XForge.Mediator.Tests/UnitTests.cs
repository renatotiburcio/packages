namespace XForge.Mediator.Tests;

public class UnitTests
{
    [Fact]
    public void Unit_values_are_always_equal()
    {
        Assert.Equal(default(Unit), Unit.Value);
        Assert.True(default(Unit) == Unit.Value);
        Assert.False(default(Unit) != Unit.Value);
        Assert.True(default(Unit).Equals(Unit.Value));
        Assert.Equal(default(Unit).GetHashCode(), Unit.Value.GetHashCode());
    }

    [Fact]
    public void Unit_compares_equal_to_any_unit()
    {
        Assert.Equal(0, default(Unit).CompareTo(Unit.Value));
        Assert.Equal("()", default(Unit).ToString());
    }

    [Fact]
    public async Task Unit_completed_task_returns_value()
    {
        var unit = await Unit.CompletedTask;
        Assert.Equal(Unit.Value, unit);
    }

    [Fact]
    public void Unit_is_a_value_type_with_zero_size_implication()
    {
        Assert.True(typeof(Unit).IsValueType);
        Assert.True(typeof(IEquatable<Unit>).IsAssignableFrom(typeof(Unit)));
        Assert.True(typeof(IComparable<Unit>).IsAssignableFrom(typeof(Unit)));
    }

    [Fact]
    public void Handler_not_found_exception_builds_clear_message()
    {
        var exception = new HandlerNotFoundException(typeof(Ping), typeof(Pong));

        Assert.Equal(typeof(Ping), exception.RequestType);
        Assert.Equal(typeof(Pong), exception.ResponseType);
        Assert.Contains(typeof(Ping).Name, exception.Message);
        Assert.Contains(typeof(Pong).Name, exception.Message);
        Assert.Contains("IRequestHandler", exception.Message);
    }
}
