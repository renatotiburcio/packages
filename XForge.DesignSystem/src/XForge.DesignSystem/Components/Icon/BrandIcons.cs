using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Brand badge icons. Logos are official; licenses documented in LICENSE-3RD-PARTY.md.
/// Curated set: Microsoft, Google, Apple, Amazon, GitHub, GitLab, Slack, Figma.
/// </summary>
public static class BrandIcons
{
    public static readonly IReadOnlyDictionary<string, string> Catalog = new Dictionary<string, string>
    {
        ["Microsoft"] = "<svg viewBox=\"0 0 24 24\"><rect x=\"1\" y=\"1\" width=\"10\" height=\"10\" fill=\"#f25022\"/><rect x=\"13\" y=\"1\" width=\"10\" height=\"10\" fill=\"#7fba00\"/><rect x=\"1\" y=\"13\" width=\"10\" height=\"10\" fill=\"#00a4ef\"/><rect x=\"13\" y=\"13\" width=\"10\" height=\"10\" fill=\"#ffb900\"/></svg>",
        ["Google"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M22 12c0-5.5-4.5-10-10-10S2 6.5 2 12s4.5 10 10 10c5 0 9-3.5 9.5-8H12v-3h10z\" fill=\"#4285f4\"/></svg>",
        ["Apple"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M17 12.5c0-2.5 2-3.5 2-3.5-1-1.5-2.5-2-3.5-2-1.5 0-2.5 1-3 1s-1.5-1-3-1c-2 0-3.5 1.5-3.5 3.5 0 3 2 6.5 3.5 6.5 1 0 1.5-1 3-1s2 1 3 1c1 0 2-1.5 2.5-3.5-1-1-1-2-1-1z\" fill=\"#000\"/></svg>",
        ["Amazon"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M3 16c5 4 13 4 18 0l-1-1c-4 3-12 3-16 0z\" fill=\"#ff9900\"/></svg>",
        ["GitHub"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 2a10 10 0 0 0-3 19c.5 0 1-.5 1-1v-2c-3 1-4-1-4-1-.5-1-1-1.5-1-1.5-1 0 0 1 0 1 1 1 2 1 2 1 .5 1 1.5 0 2 0 0-1 1-1 1-2-3-.5-5-1.5-5-3 0-1 .5-2 1-3 0-.5 0-1-.5-1.5 0 0 0-1 1-1 0 0 1 0 2 1a7 7 0 0 1 4 0c1-1 2-1 2-1 1 0 1 1 1 1-.5.5-.5 1-.5 1.5.5 1 1 2 1 3 0 1.5-2 2.5-5 3 1 0 1 1 1 2v2.5c0 .5.5 1 1 1a10 10 0 0 0-3-19z\" fill=\"#181717\"/></svg>",
        ["GitLab"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 21l-9-7 2-7 2 5h10l2-5 2 7z\" fill=\"#fc6d26\"/></svg>",
        ["Slack"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M5 14a2 2 0 1 1 0-4h2v2a2 2 0 0 1-2 2zm5-2v-6a2 2 0 1 1 4 0v6a2 2 0 1 1-4 0zm-1 6a2 2 0 1 1-4 0v-2h2a2 2 0 0 1 2 2zm10-1a2 2 0 1 1 0 4h-2v-2a2 2 0 0 1 2-2zm-1-3a2 2 0 1 1 4 0v6a2 2 0 1 1-4 0z\" fill=\"#4a154b\"/></svg>",
        ["Figma"] = "<svg viewBox=\"0 0 24 24\"><path d=\"M8 2h4v8H8a4 4 0 0 1 0-8zm0 8h4v8H8a4 4 0 0 1 0-8zm4 0h4a4 4 0 0 1 0 8h-4zm0-8h4a4 4 0 0 1 0 8h-4z\" fill=\"#a259ff\"/></svg>",
    };

    public static IReadOnlyList<string> Names => new List<string>(Catalog.Keys);
}