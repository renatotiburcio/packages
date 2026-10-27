using System.Collections.Concurrent;
using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// LRU cache for SVG payload strings. Bounded memory, thread-safe, scope-aware.
/// Per-assembly scopes allow tests to discard icons without invalidating the global cache.
/// </summary>
public sealed class XFIconLruCache : IXFIconCache
{
    private readonly long _maxBytes;
    private readonly ConcurrentDictionary<string, long> _bytesByKey = new();
    private readonly ConcurrentDictionary<string, string> _values = new();
    private readonly LinkedList<string> _lru = new();
    private readonly object _lock = new();
    private long _currentBytes;

    public long Hits { get; private set; }
    public long Misses { get; private set; }
    public long Evictions { get; private set; }
    public long MemoryBytes => _currentBytes;

    public XFIconLruCache(long maxBytes = 10L * 1024 * 1024) => _maxBytes = maxBytes;

    public string? GetOrAdd(string key, Func<string> factory)
    {
        if (_values.TryGetValue(key, out var existing)) { Hits++; Touch(key); return existing; }
        Misses++;
        var v = factory();
        _values[key] = v;
        var sz = System.Text.Encoding.UTF8.GetByteCount(v);
        _bytesByKey[key] = sz;
        _currentBytes += sz;
        lock (_lock) { _lru.AddFirst(key); }
        EvictIfNeeded();
        return v;
    }

    public void Invalidate(string key)
    {
        if (_values.TryRemove(key, out _))
        {
            if (_bytesByKey.TryRemove(key, out var sz)) _currentBytes -= sz;
            lock (_lock) { _lru.Remove(key); }
        }
    }

    private void Touch(string key)
    {
        lock (_lock) { _lru.Remove(key); _lru.AddFirst(key); }
    }

    private void EvictIfNeeded()
    {
        if (_currentBytes <= _maxBytes) return;
        lock (_lock)
        {
            while (_currentBytes > _maxBytes * 0.75 && _lru.Last is { } victim)
            {
                _lru.RemoveLast();
                _values.TryRemove(victim.Value, out _);
                if (_bytesByKey.TryRemove(victim.Value, out var sz)) _currentBytes -= sz;
                Evictions++;
            }
        }
    }
}