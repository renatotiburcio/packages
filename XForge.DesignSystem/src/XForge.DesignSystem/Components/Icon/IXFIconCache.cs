namespace XForge.DesignSystem.Components.Icon;

public interface IXFIconCache
{
    string? GetOrAdd(string key, System.Func<string> factory);
    void Invalidate(string key);
    long Hits { get; }
    long Misses { get; }
    long Evictions { get; }
    long MemoryBytes { get; }
}