using System.Collections.Generic;
using System.Text;

namespace XForge.Utilities.Css;

/// <summary>
/// Fluent CSS class builder. MudBlazor-inspired.
/// </summary>
public sealed class CssBuilder
{
    private readonly StringBuilder _buffer = new();
    private readonly List<string> _classes = new();
    private bool _dirty = true;

    public CssBuilder Add(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) _classes.Add(value.Trim());
        _dirty = true;
        return this;
    }

    public CssBuilder Add(Func<string?>? valueFactory)
    {
        if (valueFactory is null) return this;
        return Add(valueFactory());
    }

    public CssBuilder Add(string? value, bool when)
    {
        if (when && !string.IsNullOrWhiteSpace(value)) _classes.Add(value.Trim());
        _dirty = true;
        return this;
    }

    public CssBuilder Add(string? value, Func<bool>? whenFactory)
    {
        return whenFactory is null ? this : Add(value, whenFactory());
    }

    public override string ToString()
    {
        if (_dirty)
        {
            _buffer.Clear();
            for (int i = 0; i < _classes.Count; i++)
            {
                if (i > 0) _buffer.Append(' ');
                _buffer.Append(_classes[i]);
            }
            _dirty = false;
        }
        return _buffer.ToString();
    }

    public static implicit operator string(CssBuilder b) => b.ToString();
}