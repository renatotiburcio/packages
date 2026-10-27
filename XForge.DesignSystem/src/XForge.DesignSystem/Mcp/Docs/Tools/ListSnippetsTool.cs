// T053: ListSnippetsTool.
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Mcp.Docs.Tools;

public sealed class ListSnippetsTool : IXFMcpTool
{
    public string Name => "list-snippets";
    public string Description => "Lists available code snippets, optionally filtered by component.";
    public string InputSchemaJson => "{\"type\":\"object\",\"properties\":{\"component\":{\"type\":\"string\"}}}";
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "MCP server tool, not AOT-critical")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MCP server tool, not AOT-critical")]
    public string Execute(string argsJson) => JsonSerializer.Serialize(new { tool = Name, status = "ok", args = argsJson });
}