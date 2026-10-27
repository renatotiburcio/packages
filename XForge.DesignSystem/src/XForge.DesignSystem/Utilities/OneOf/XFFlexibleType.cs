using System;

namespace XForge.Utilities.OneOf;

/// <summary>
/// Tagged union for flexible parameter types.
/// Inspired by antd-blazor's OneOf use to allow multiple input types per parameter.
/// </summary>
public readonly struct XFFlexibleType
{
    public XFFlexibleTypeKind Kind { get; }
    public string? StringValue { get; }
    public int IntValue { get; }
    public bool BoolValue { get; }

    private XFFlexibleType(XFFlexibleTypeKind kind, string? s, int i, bool b)
    {
        Kind = kind; StringValue = s; IntValue = i; BoolValue = b;
    }

    public static XFFlexibleType From(string? value) => new(XFFlexibleTypeKind.String, value, 0, false);
    public static XFFlexibleType From(int value) => new(XFFlexibleTypeKind.Int, null, value, false);
    public static XFFlexibleType From(bool value) => new(XFFlexibleTypeKind.Bool, null, 0, value);
    public static XFFlexibleType From(object? value) => value switch
    {
        null => new XFFlexibleType(XFFlexibleTypeKind.Null, null, 0, false),
        string s => From(s),
        int i => From(i),
        bool b => From(b),
        _ => throw new ArgumentException($"Unsupported type: {value.GetType()}"),
    };

    public T Match<T>(Func<string?, T> onString, Func<int, T> onInt, Func<bool, T> onBool, Func<T> onNull) => Kind switch
    {
        XFFlexibleTypeKind.String => onString(StringValue),
        XFFlexibleTypeKind.Int => onInt(IntValue),
        XFFlexibleTypeKind.Bool => onBool(BoolValue),
        _ => onNull(),
    };
}

public enum XFFlexibleTypeKind { Null, String, Int, Bool }