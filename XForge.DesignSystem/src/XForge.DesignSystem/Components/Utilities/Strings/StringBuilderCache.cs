using System.Text;
namespace XForge.DesignSystem.Components.Utilities.Strings;
public static class StringBuilderCache
{
    private const int MaxBuilderSize = 4096;
    [ThreadStatic] private static StringBuilder? _cached;
    public static StringBuilder Acquire(int capacity = 16)
    {
        var sb = _cached;
        if (sb is not null) { _cached = null; sb.Clear(); if (sb.Capacity < capacity) sb.Capacity = capacity; return sb; }
        return new StringBuilder(capacity);
    }
    public static void Release(StringBuilder sb)
    {
        if (sb is null) return;
        if (sb.Capacity > MaxBuilderSize) return;
        sb.Clear();
        _cached = sb;
    }
    public static string AcquireAndRelease(int capacity, System.Action<StringBuilder> build)
    {
        var sb = Acquire(capacity);
        try { build(sb); return sb.ToString(); } finally { Release(sb); }
    }
}