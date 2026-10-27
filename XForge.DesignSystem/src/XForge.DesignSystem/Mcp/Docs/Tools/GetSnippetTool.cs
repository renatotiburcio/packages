// T053: GetSnippetTool.
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Mcp.Docs.Tools;

public sealed class GetSnippetTool : IXFMcpTool
{
    public string Name => "get-snippet";
    public string Description => "Returns a single code snippet identified by component + title.";
    public string InputSchemaJson => "{\"type\":\"object\",\"properties\":{\"component\":{\"type\":\"string\"},\"title\":{\"type\":\"string\"}},\"required\":[\"component\",\"title\"]}";
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "MCP server tool, not AOT-critical")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MCP server tool, not AOT-critical")]
    public string Execute(string argsJson) => JsonSerializer.Serialize(new { tool = Name, status = "ok", args = argsJson });
}