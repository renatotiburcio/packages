namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Configuration options for AI providers, bindable from appsettings.json.
/// </summary>
public sealed class XFAiOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "XForgeAI";

    /// <summary>The default provider to use (e.g., "openai", "ollama", "openrouter").</summary>
    public string DefaultProvider { get; set; } = "inmemory";

    /// <summary>Default model for all providers (can be overridden per provider).</summary>
    public string? DefaultModel { get; set; }

    /// <summary>Default temperature for AI requests.</summary>
    public float? DefaultTemperature { get; set; }

    /// <summary>Default max tokens for AI requests.</summary>
    public int? DefaultMaxTokens { get; set; }

    /// <summary>Whether to enable audit logging.</summary>
    public bool EnableAudit { get; set; } = true;

    /// <summary>Whether to enable memory service.</summary>
    public bool EnableMemory { get; set; } = true;

    /// <summary>Provider-specific configurations.</summary>
    public Dictionary<string, XFAiProviderConfig> Providers { get; set; } = new();

    /// <summary>Web search configuration.</summary>
    public XFAiWebSearchConfig? WebSearch { get; set; }

    /// <summary>Security configuration.</summary>
    public XFAiSecurityConfig Security { get; set; } = new();

    /// <summary>Rate limiting configuration.</summary>
    public XFAiRateLimitConfig RateLimit { get; set; } = new();
}

/// <summary>
/// Configuration for a specific AI provider.
/// </summary>
public sealed class XFAiProviderConfig
{
    /// <summary>Provider type: "openai", "ollama", "gemini", "anthropic", "azure-openai", "openrouter", "deepseek", "local-llm".</summary>
    public required string Type { get; set; }

    /// <summary>API key (use environment variables or secret manager in production).</summary>
    public string? ApiKey { get; set; }

    /// <summary>Base URL override for the provider.</summary>
    public string? BaseUrl { get; set; }

    /// <summary>Default model for this provider.</summary>
    public string? Model { get; set; }

    /// <summary>Azure OpenAI resource name (for azure-openai type).</summary>
    public string? ResourceName { get; set; }

    /// <summary>Azure OpenAI deployment name (for azure-openai type).</summary>
    public string? DeploymentName { get; set; }

    /// <summary>Azure OpenAI API version (for azure-openai type).</summary>
    public string? ApiVersion { get; set; }

    /// <summary>OpenRouter HTTP Referer (for openrouter type).</summary>
    public string? HttpReferer { get; set; }

    /// <summary>OpenRouter App Title (for openrouter type).</summary>
    public string? AppTitle { get; set; }

    /// <summary>Whether this provider is enabled.</summary>
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Configuration for web search capabilities.
/// </summary>
public sealed class XFAiWebSearchConfig
{
    /// <summary>Search provider: "bing", "google", "brave", "serpapi".</summary>
    public string Provider { get; set; } = "bing";

    /// <summary>Search API key.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Custom search engine ID (for Google).</summary>
    public string? SearchEngineId { get; set; }

    /// <summary>Maximum results per search.</summary>
    public int MaxResults { get; set; } = 10;

    /// <summary>Whether to enable safe search.</summary>
    public bool SafeSearch { get; set; } = true;

    /// <summary>Default language for search results.</summary>
    public string Language { get; set; } = "pt-BR";
}

/// <summary>
/// Security configuration for AI operations.
/// </summary>
public sealed class XFAiSecurityConfig
{
    /// <summary>Whether to enable prompt injection detection.</summary>
    public bool EnablePromptInjectionDetection { get; set; } = true;

    /// <summary>Whether to enable PII masking in prompts.</summary>
    public bool EnablePiiMasking { get; set; }

    /// <summary>Whether to enable content moderation.</summary>
    public bool EnableContentModeration { get; set; }

    /// <summary>Maximum prompt length in characters.</summary>
    public int MaxPromptLength { get; set; } = 100_000;

    /// <summary>Blocked patterns (regex) for prompt injection detection.</summary>
    public List<string> BlockedPatterns { get; set; } = new()
    {
        @"ignore\s+(all\s+)?previous\s+instructions",
        @"you\s+are\s+now\s+a",
        @"system\s*:\s*you\s+are",
        @"forget\s+(all\s+)?(your|previous)\s+(instructions|rules)",
        @"override\s+(your|system)\s+(instructions|rules|safety)"
    };
}

/// <summary>
/// Rate limiting configuration.
/// </summary>
public sealed class XFAiRateLimitConfig
{
    /// <summary>Maximum requests per minute.</summary>
    public int MaxRequestsPerMinute { get; set; } = 60;

    /// <summary>Maximum tokens per minute.</summary>
    public int MaxTokensPerMinute { get; set; } = 100_000;

    /// <summary>Maximum concurrent requests.</summary>
    public int MaxConcurrentRequests { get; set; } = 5;

    /// <summary>Whether rate limiting is enabled.</summary>
    public bool Enabled { get; set; } = true;
}
