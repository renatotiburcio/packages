namespace XForge.Mediator;

/// <summary>
/// Represents a void-like response for commands that produce no result.
/// A zero-allocation struct (like <c>Mediator.Unit</c>), used instead of <see cref="object"/>.
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    /// <summary>The single <see cref="Unit"/> value.</summary>
    public static readonly Unit Value = default;

    /// <summary>A completed <see cref="ValueTask{Unit}"/> that returns <see cref="Value"/>.</summary>
    public static ValueTask<Unit> CompletedTask => new(Value);

    /// <inheritdoc />
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc />
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public override string ToString() => "()";

    /// <summary>Always equal.</summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>Never different.</summary>
    public static bool operator !=(Unit left, Unit right) => false;
}
