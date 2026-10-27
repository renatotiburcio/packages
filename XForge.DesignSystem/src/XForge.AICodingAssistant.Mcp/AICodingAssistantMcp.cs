using ModelContextProtocol.Server;

namespace XForge.AICodingAssistant.Mcp;

/// <summary>T127: XForge AICodingAssistant MCP -- code gen/refactor. Syncfusion. P0. 24h. Dep T102+T126.</summary>
[McpServerToolType]
public class AICodingAssistantMcp
{
    [McpServerTool(Name = "generate_code", Description = "Generate XForge code from description")]
    public Task<string> GenerateCodeAsync(string description, string language = "csharp") { return Task.FromResult($"// {description}"); }
    [McpServerTool(Name = "refactor_code", Description = "Refactor existing code per XForge patterns")]
    public Task<string> RefactorCodeAsync(string code, string goal) { return Task.FromResult(code); }
    [McpServerTool(Name = "explain_pattern", Description = "Explain an XForge pattern")]
    public Task<string> ExplainPatternAsync(string pattern) { return Task.FromResult($"XForge pattern: {pattern}"); }
}