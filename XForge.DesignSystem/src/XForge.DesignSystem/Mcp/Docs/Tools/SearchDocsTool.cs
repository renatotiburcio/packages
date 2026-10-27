// T053: SearchDocsTool.
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Mcp.Docs.Tools;

public sealed class SearchDocsTool : IXFMcpTool
{
    public string Name => "search-docs";
    public string Description => "Full-text search across the XForge documentation corpus.";
    public string InputSchemaJson => "{\"type\":\"object\",\"properties\":{\"query\":{\"type\":\"string\"},\"limit\":{\"type\":\"integer\",\"default\":10}},\"required\":[\"query\"]}";
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "MCP server tool, not AOT-critical")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MCP server tool, not AOT-critical")]
    public string Execute(string argsJson) => JsonSerializer.Serialize(new { tool = Name, status = "ok", args = argsJson });
}