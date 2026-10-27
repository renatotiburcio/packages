namespace XForge.DesignSystem.Components.AI.Analysis;

/// <summary>
/// Represents an AI-generated SQL query with validation, security analysis, and metadata.
/// </summary>
public sealed class XFAiSqlQuery
{
    /// <summary>Unique identifier for this query generation.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>The original natural language query from the user.</summary>
    public string NaturalLanguageQuery { get; set; } = string.Empty;

    /// <summary>The AI-generated SQL statement.</summary>
    public string GeneratedSql { get; set; } = string.Empty;

    /// <summary>AI explanation of what the SQL does.</summary>
    public string? Explanation { get; set; }

    /// <summary>AI optimization suggestions for the query.</summary>
    public string? Optimization { get; set; }

    /// <summary>Security and correctness validation result.</summary>
    public XFAiSqlValidationResult ValidationResult { get; set; } = new();

    /// <summary>Whether the generated SQL is read-only (SELECT only).</summary>
    public bool IsReadOnly { get; set; } = true;

    /// <summary>Whether this query requires human approval before execution.</summary>
    public bool RequiresApproval { get; set; }

    /// <summary>AI confidence score between 0.0 and 1.0.</summary>
    public double Confidence { get; set; }

    /// <summary>List of database tables referenced in the SQL.</summary>
    public IReadOnlyList<string> TouchedTables { get; set; } = Array.Empty<string>();

    /// <summary>Optional parameterized query values.</summary>
    public Dictionary<string, string>? Parameters { get; set; }
}

/// <summary>
/// Validation result for SQL security, correctness, and policy compliance.
/// </summary>
public sealed class XFAiSqlValidationResult
{
    /// <summary>Whether the SQL passed all validation checks.</summary>
    public bool IsValid { get; set; } = true;

    /// <summary>Whether the SQL contains only read operations.</summary>
    public bool IsReadOnlyQuery { get; set; } = true;

    /// <summary>Whether the SQL contains potential injection patterns.</summary>
    public bool HasInjectionRisk { get; set; }

    /// <summary>Whether the SQL contains a blocked operation.</summary>
    public bool HasBlockedOperation { get; set; }

    /// <summary>Error message if validation failed.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Non-fatal warnings about the generated SQL.</summary>
    public IReadOnlyList<string> Warnings { get; set; } = Array.Empty<string>();
}
