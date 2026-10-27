namespace XForge.DesignSystem.Components.AI.Forms;

public sealed class XFAiFormInsight
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string? NaturalLanguageQuery { get; set; }
    public string AiSummary { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public IReadOnlyList<XFAiAutofillSuggestion> AutofillSuggestions { get; set; } = Array.Empty<XFAiAutofillSuggestion>();
    public IReadOnlyList<XFAiValidationFix> ValidationFixes { get; set; } = Array.Empty<XFAiValidationFix>();
    public IReadOnlyList<XFAiFormInconsistency> Inconsistencies { get; set; } = Array.Empty<XFAiFormInconsistency>();
}

public sealed class XFAiAutofillSuggestion
{
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public string SuggestedValue { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

public sealed class XFAiValidationFix
{
    public string FieldName { get; set; } = string.Empty;
    public string CurrentValue { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
    public string ValidationRule { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

public sealed class XFAiFormInconsistency
{
    public string Field1 { get; set; } = string.Empty;
    public string Field2 { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "medium";
    public string? Suggestion { get; set; }
}
