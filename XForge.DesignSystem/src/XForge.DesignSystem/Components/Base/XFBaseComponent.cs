// T045: BaseComponent with ShouldAutoGenerateId + CaptureUnmatchedValues (Blazorise pattern)
// P0, 4h, no deps
namespace XForge.DesignSystem.Components.Base;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Base class for XForge components supporting ShouldAutoGenerateId and CaptureUnmatchedValues.
/// Inspired by Blazorise BaseComponent pattern.
/// </summary>
public abstract class XFBaseComponent : ComponentBase
{
    /// <summary>Whether to auto-generate an Id when none is provided. Default true.</summary>
    [Parameter] public bool ShouldAutoGenerateId { get; set; } = true;

    /// <summary>Explicit id; null when <see cref="ShouldAutoGenerateId"/> is true.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>CSS classes (Tailwind utility list).</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Inline styles.</summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>Captures any additional HTML attributes splatted onto the root element.</summary>
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Resolves the effective id: explicit Id, or xf-{8-char-guid} when ShouldAutoGenerateId.</summary>
    protected string EffectiveId => Id ?? (ShouldAutoGenerateId ? $"xf-{Guid.NewGuid().ToString("N")[..8]}" : string.Empty);
}