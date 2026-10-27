using System.Collections.Generic;

namespace XForge.DesignSystem.Components;

public sealed record XFComponentInfo(
    string Name,
    string Description,
    string Category,
    string[] Tags,
    string[] UsesIcons);