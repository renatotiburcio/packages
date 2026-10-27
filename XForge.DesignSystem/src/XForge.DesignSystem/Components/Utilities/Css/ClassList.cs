using System;
using System.Collections.Generic;
namespace XForge.DesignSystem.Components.Utilities.Css;
public readonly struct ClassList
{
    private readonly string[]? _classes;
    public int Count => _classes?.Length ?? 0;
    public bool IsEmpty => Count == 0;
    public ClassList(string? first)
    {
        _classes = string.IsNullOrEmpty(first) ? null : new[] { first };
    }
    public ClassList Add(string? className)
    {
        if (string.IsNullOrEmpty(className)) return this;
        if (_classes is null) return new ClassList(className);
        var arr = new string[_classes.Length + 1]; Array.Copy(_classes, arr, _classes.Length); arr[_classes.Length] = className; return new ClassList(arr);
    }
    public ClassList Add(string? className, bool condition) => condition ? Add(className) : this;
    public ClassList Add(string? trueName, string? falseName, bool condition) => Add(condition ? trueName : falseName);
    public bool Contains(string className)
    {
        if (_classes is null) return false;
        foreach (var c in _classes) if (c == className) return true; return false;
    }
    public override string ToString() => _classes is null ? string.Empty : string.Join(" ", _classes);
    public static implicit operator string(ClassList c) => c.ToString();
    public IEnumerable<string> Enumerate() => _classes ?? Array.Empty<string>();
    private ClassList(string[] arr) { _classes = arr; }
}