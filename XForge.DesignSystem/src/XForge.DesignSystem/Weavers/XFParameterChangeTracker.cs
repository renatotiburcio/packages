// T049: Manual change tracker (works at runtime; weaver rewrites the same API at compile time).
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Weavers;

/// <summary>
/// Tracks parameter changes on a component. Pair with <see cref="XFDirtyAttribute"/> to mark which parameters participate.
/// </summary>
public sealed class XFParameterChangeTracker
{
    private readonly object _lock = new();
    private readonly HashSet<string> _dirty = new(StringComparer.Ordinal);
    private readonly IReadOnlyCollection<string> _tracked;

    public XFParameterChangeTracker(IXFParameterDirtyWeaver weaver, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type componentType)
    {
        _tracked = weaver.GetTrackedProperties(componentType);
    }

    public event EventHandler<string>? ParameterChanged;

    public void MarkDirty(string propertyName)
    {
        if (!_tracked.Contains(propertyName)) return;
        lock (_lock)
        {
            if (_dirty.Add(propertyName)) ParameterChanged?.Invoke(this, propertyName);
        }
    }

    public bool IsDirty(string propertyName) { lock (_lock) return _dirty.Contains(propertyName); }

    public IReadOnlyCollection<string> DirtyProperties() { lock (_lock) return _dirty.ToArray(); }

    public int DirtyCount { get { lock (_lock) return _dirty.Count; } }

    public void Reset() { lock (_lock) _dirty.Clear(); }
}