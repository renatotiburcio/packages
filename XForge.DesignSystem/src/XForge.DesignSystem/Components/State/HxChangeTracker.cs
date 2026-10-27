using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace XForge.DesignSystem.Components.State;
public class HxChangeTracker : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly Dictionary<string, object?> _original = new();
    private readonly Dictionary<string, object?> _current = new();
    public bool IsDirty => !OriginalsEqual();
    public int ChangeCount => _current.Count;
    public IReadOnlyDictionary<string, object?> Changes => _current;
    public void Snapshot(string name, object? value) { _original[name] = value; _current[name] = value; }
    public void Update(string name, object? value) { if (!_current.TryGetValue(name, out var prev) || !Equals(prev, value)) { _current[name] = value; Raise(name); } }
    public void AcceptChanges() { _original.Clear(); foreach (var kv in _current) _original[kv.Key] = kv.Value; }
    public void RejectChanges() { _current.Clear(); foreach (var kv in _original) _current[kv.Key] = kv.Value; }
    public void Reset() { _original.Clear(); _current.Clear(); }
    private bool OriginalsEqual() { foreach (var kv in _current) if (!_original.TryGetValue(kv.Key, out var orig) || !Equals(orig, kv.Value)) return false; return true; }
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}