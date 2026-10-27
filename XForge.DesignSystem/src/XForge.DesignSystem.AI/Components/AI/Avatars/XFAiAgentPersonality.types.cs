namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// Tone of voice for an AI agent personality.
/// </summary>
public enum XFAiAgentTone
{
    /// <summary>Professional and formal.</summary>
    Formal,
    /// <summary>Friendly and conversational.</summary>
    Friendly,
    /// <summary>Concise and direct.</summary>
    Concise,
    /// <summary>Detailed and thorough.</summary>
    Detailed,
    /// <summary>Empathetic and supportive.</summary>
    Empathetic,
    /// <summary>Custom tone defined by system prompt.</summary>
    Custom
}

/// <summary>
/// Expertise domain for an AI agent personality.
/// </summary>
public enum XFAiAgentExpertise
{
    /// <summary>General-purpose assistant.</summary>
    General,
    /// <summary>Finance and accounting.</summary>
    Finance,
    /// <summary>Fiscal and tax compliance.</summary>
    Fiscal,
    /// <summary>Stock and inventory management.</summary>
    Stock,
    /// <summary>HR and payroll.</summary>
    HR,
    /// <summary>Legal and compliance.</summary>
    Legal,
    /// <summary>IT and development.</summary>
    IT,
    /// <summary>Sales and CRM.</summary>
    Sales,
    /// <summary>Custom expertise domain.</summary>
    Custom
}

/// <summary>
/// Defines the personality configuration for an AI agent.
/// </summary>
public sealed record XFAiAgentPersonalityConfig
{
    /// <summary>Display name of the agent.</summary>
    public required string Name { get; init; }

    /// <summary>Short description or tagline.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Tone of voice.</summary>
    public XFAiAgentTone Tone { get; init; } = XFAiAgentTone.Friendly;

    /// <summary>Expertise domain.</summary>
    public XFAiAgentExpertise Expertise { get; init; } = XFAiAgentExpertise.General;

    /// <summary>Custom system prompt (overrides tone/expertise defaults).</summary>
    public string? SystemPrompt { get; init; }

    /// <summary>Avatar icon SVG name (from icon catalog).</summary>
    public string? IconName { get; init; }

    /// <summary>Avatar color (Tailwind color class, e.g., "bg-violet-500").</summary>
    public string? AvatarColor { get; init; }

    /// <summary>Avatar text color (Tailwind class, e.g., "text-white").</summary>
    public string? AvatarTextColor { get; init; }

    /// <summary>Temperature setting for AI requests (0.0–2.0).</summary>
    public double Temperature { get; init; } = 0.7;

    /// <summary>Greeting message shown on first interaction.</summary>
    public string? Greeting { get; init; }

    /// <summary>Suggested prompts for the user.</summary>
    public IReadOnlyList<string>? SuggestedPrompts { get; init; }

    /// <summary>Whether the agent can use tools.</summary>
    public bool AllowToolCalling { get; init; }

    /// <summary>Maximum context tokens for this agent.</summary>
    public int? MaxContextTokens { get; init; }
}
