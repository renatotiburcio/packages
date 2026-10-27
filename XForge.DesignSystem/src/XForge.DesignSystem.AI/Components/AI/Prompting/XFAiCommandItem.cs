namespace XForge.DesignSystem.Components.AI.Prompting;

/// <summary>
/// Represents a single command item in the XFAiCommandPalette.
/// </summary>
public sealed record XFAiCommandItem
{
    /// <summary>Unique identifier for this command.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Display label for the command.</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>Optional description providing additional context.</summary>
    public string? Description { get; init; }

    /// <summary>Category/group for visual grouping.</summary>
    public string? Category { get; init; }

    /// <summary>SVG icon name from XFSvgIcons catalog.</summary>
    public string? Icon { get; init; }

    /// <summary>Keyboard shortcut display text (e.g., "Ctrl+K").</summary>
    public string? Shortcut { get; init; }

    /// <summary>Whether this is a slash command (triggered by "/").</summary>
    public bool IsSlashCommand { get; init; }

    /// <summary>Slash prefix for slash commands (e.g., "/relatorio").</summary>
    public string? SlashPrefix { get; init; }

    /// <summary>Whether this command is disabled.</summary>
    public bool Disabled { get; init; }
}