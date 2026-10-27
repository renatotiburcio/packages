// T053: ListExamplesTool.
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Mcp.Docs.Tools;

public sealed class ListExamplesTool : IXFMcpTool
{
    public string Name => "list-examples";
    public string Description => "Lists runnable example projects, optionally filtered by category.";
    public string InputSchemaJson => "{\"type\":\"object\",\"properties\":{\"category\":{\"type\":\"string\"}}}";
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "MCP server tool, not AOT-critical")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MCP server tool, not AOT-critical")]
    public string Execute(string argsJson) => JsonSerializer.Serialize(new { tool = Name, status = "ok", args = argsJson });
}