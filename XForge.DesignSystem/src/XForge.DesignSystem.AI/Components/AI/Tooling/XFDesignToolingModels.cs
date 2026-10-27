namespace XForge.DesignSystem.Components.AI.Tooling;

// ── Enums ──────────────────────────────────────────────────────────────────

/// <summary>Theme editor color channel.</summary>
public enum XFThemeColorChannel
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Neutral,
    Background,
    Surface,
    Text
}

/// <summary>Design token category.</summary>
public enum XFDesignTokenCategory
{
    Color,
    Typography,
    Spacing,
    Border,
    Shadow,
    Motion,
    Breakpoint,
    ZIndex,
    Opacity,
    Other
}

/// <summary>Design token export format.</summary>
public enum XFDesignTokenFormat
{
    CSS,
    SCSS,
    JSON,
    TypeScript
}

/// <summary>Figma sync status.</summary>
public enum XFFigmaSyncStatus
{
    Disconnected,
    Connected,
    Syncing,
    Synced,
    Error,
    Conflict
}

/// <summary>Figma sync direction.</summary>
public enum XFFigmaSyncDirection
{
    Import,
    Export,
    Bidirectional
}

/// <summary>Component generator output type.</summary>
public enum XFGeneratorOutputType
{
    Component,
    Service,
    Page,
    Layout
}

/// <summary>Storybook component status.</summary>
public enum XFStorybookComponentStatus
{
    Stable,
    Beta,
    Deprecated,
    Experimental
}

// ── Records ────────────────────────────────────────────────────────────────

/// <summary>A theme color definition.</summary>
public record XFThemeColor
{
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public XFThemeColorChannel Channel { get; init; }
    public string CssVariable { get; init; } = "";
    public string? DarkValue { get; init; }
}

/// <summary>A theme typography definition.</summary>
public record XFThemeTypography
{
    public string Name { get; init; } = "";
    public string FontFamily { get; init; } = "";
    public string FontSize { get; init; } = "";
    public string FontWeight { get; init; } = "";
    public string LineHeight { get; init; } = "";
    public string LetterSpacing { get; init; } = "";
    public string CssVariable { get; init; } = "";
}

/// <summary>A theme spacing definition.</summary>
public record XFThemeSpacing
{
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public string CssVariable { get; init; } = "";
}

/// <summary>A theme preset configuration.</summary>
public record XFThemePreset
{
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public IReadOnlyList<XFThemeColor> Colors { get; init; } = Array.Empty<XFThemeColor>();
    public IReadOnlyList<XFThemeTypography> Typography { get; init; } = Array.Empty<XFThemeTypography>();
    public IReadOnlyList<XFThemeSpacing> Spacing { get; init; } = Array.Empty<XFThemeSpacing>();
}

/// <summary>A design token entry.</summary>
public record XFDesignToken
{
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public XFDesignTokenCategory Category { get; init; }
    public string CssVariable { get; init; } = "";
    public string? Description { get; init; }
    public string? Group { get; init; }
    public string? DarkValue { get; init; }
}

/// <summary>A Figma token entry.</summary>
public record XFFigmaToken
{
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public string FigmaStyleId { get; init; } = "";
    public XFDesignTokenCategory Category { get; init; }
    public DateTime LastSynced { get; init; }
    public bool HasConflict { get; init; }
}

/// <summary>Figma sync result.</summary>
public record XFFigmaSyncResult
{
    public XFFigmaSyncStatus Status { get; init; }
    public int TokensImported { get; init; }
    public int TokensExported { get; init; }
    public int Conflicts { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime SyncedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Component generator configuration.</summary>
public record XFGeneratorConfig
{
    public string ComponentName { get; init; } = "";
    public string Namespace { get; init; } = "XForge.DesignSystem.Components";
    public XFGeneratorOutputType OutputType { get; init; } = XFGeneratorOutputType.Component;
    public bool IncludeTests { get; init; } = true;
    public bool IncludeDocs { get; init; } = true;
    public bool ImplementAsyncDisposable { get; init; }
    public bool ImplementDisposable { get; init; }
    public bool TailwindFirst { get; init; } = true;
    public bool SupportDarkLight { get; init; } = true;
    public bool SupportDataTheme { get; init; } = true;
    public string? BaseClass { get; init; }
    public IReadOnlyList<string> Parameters { get; init; } = Array.Empty<string>();
}

/// <summary>Component generator output result.</summary>
public record XFGeneratorOutput
{
    public string FileName { get; init; } = "";
    public string Content { get; init; } = "";
    public XFGeneratorOutputType OutputType { get; init; }
}

/// <summary>A Storybook component entry.</summary>
public record XFStorybookComponent
{
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";
    public string Description { get; init; } = "";
    public XFStorybookComponentStatus Status { get; init; } = XFStorybookComponentStatus.Stable;
    public string DocsRoute { get; init; } = "";
    public int ParameterCount { get; init; }
    public int TestCount { get; init; }
}

/// <summary>A Storybook component control (prop editor).</summary>
public record XFStorybookControl
{
    public string ParameterName { get; init; } = "";
    public string Type { get; init; } = "";
    public string? DefaultValue { get; init; }
    public string? Description { get; init; }
    public bool IsEnum { get; init; }
    public IReadOnlyList<string>? EnumValues { get; init; }
}
