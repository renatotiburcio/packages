namespace XForge.DesignSystem.Services.AI;
public class AIChatServiceOptions
{
    public string Model { get; set; } = "gpt-4o-mini";
    public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string? ApiKey { get; set; }
    public string ApiKeyHeader { get; set; } = "Authorization";
    public string ApiKeyScheme { get; set; } = "Bearer";
    public string? SystemPrompt { get; set; }
    public int MaxMessages { get; set; } = 20;
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 2048;
}