namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Result of an AI text rewrite request.
/// </summary>
public sealed record XFAiRewriteResult
{
    /// <summary>The original text that was rewritten.</summary>
    public string OriginalText { get; init; } = string.Empty;

    /// <summary>The rewritten text.</summary>
    public string RewrittenText { get; init; } = string.Empty;

    /// <summary>The rewrite mode that was applied.</summary>
    public XFAiRewriteMode Mode { get; init; }

    /// <summary>AI confidence score between 0.0 and 1.0.</summary>
    public double Confidence { get; init; }

    /// <summary>Optional explanation of what was changed.</summary>
    public string? ChangeDescription { get; init; }
}

/// <summary>
/// Determines the rewrite strategy applied to the text.
/// </summary>
public enum XFAiRewriteMode
{
    /// <summary>Make the text more formal/professional.</summary>
    Formalize,

    /// <summary>Condense the text while keeping key information.</summary>
    Summarize,

    /// <summary>Expand the text with more detail.</summary>
    Expand,

    /// <summary>Fix grammar and spelling errors.</summary>
    FixGrammar,

    /// <summary>Adjust the tone (e.g., friendly, assertive, neutral).</summary>
    AdjustTone,

    /// <summary>Translate to another language.</summary>
    Translate
}
