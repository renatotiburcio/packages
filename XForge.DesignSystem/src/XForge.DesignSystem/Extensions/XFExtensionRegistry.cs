// T050: Central extension registry.
namespace XForge.DesignSystem.Extensions;

/// <summary>
/// Tracks which <see cref="IXFExtension"/> implementations are activated.
/// Thread-safe; idempotent Activate/Deactivate calls are no-ops.
/// </summary>
public sealed class XFExtensionRegistry
{
    private readonly object _lock = new();
    private readonly Dictionary<string, IXFExtension> _extensions = new();
    private readonly HashSet<string> _activeIds = new();

    public void Register(IXFExtension extension)
    {
        if (extension is null) throw new ArgumentNullException(nameof(extension));
        lock (_lock) _extensions[extension.Id] = extension;
    }

    public void Activate(string id, IServiceProvider services)
    {
        lock (_lock)
        {
            if (_activeIds.Contains(id)) return;
            if (!_extensions.TryGetValue(id, out var ext)) throw new InvalidOperationException($"Extension {id} not registered.");
            ext.Activate(services);
            _activeIds.Add(id);
        }
    }

    public void Deactivate(string id)
    {
        lock (_lock)
        {
            if (!_activeIds.Remove(id)) return;
            if (_extensions.TryGetValue(id, out var ext)) ext.Deactivate();
        }
    }

    public bool IsActive(string id) { lock (_lock) return _activeIds.Contains(id); }

    public IEnumerable<string> ActiveIds() { lock (_lock) return _activeIds.ToArray(); }

    public int RegisteredCount { get { lock (_lock) return _extensions.Count; } }

    public IReadOnlyCollection<string> RegisteredIds() { lock (_lock) return _extensions.Keys.ToArray(); }
}