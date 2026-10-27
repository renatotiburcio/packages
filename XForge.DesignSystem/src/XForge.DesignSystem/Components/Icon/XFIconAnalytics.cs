using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// In-memory ring buffer tracking icon pick events. Privacy: no PII, no network.
/// </summary>
public sealed class XFIconAnalytics
{
    private readonly Queue<XFIconEvent> _events = new();
    private readonly int _maxEvents;

    public XFIconAnalytics(int maxEvents = 1000) => _maxEvents = maxEvents;

    public int Count => _events.Count;

    public void Track(string iconName, string variant = "Regular", int size = 24, string? page = null)
    {
        _events.Enqueue(new XFIconEvent(iconName, variant, size, page, DateTime.UtcNow));
        while (_events.Count > _maxEvents) _events.Dequeue();
    }

    public IReadOnlyDictionary<string, int> GetTop(int n = 20)
    {
        return _events.GroupBy(e => e.IconName)
            .OrderByDescending(g => g.Count())
            .Take(n)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Dictionary<string,int> is a primitive type")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Dictionary<string,int> is a primitive type")]
    public string ExportJson() => System.Text.Json.JsonSerializer.Serialize(GetTop());
}

public sealed record XFIconEvent(string IconName, string Variant, int Size, string? Page, DateTime Timestamp);