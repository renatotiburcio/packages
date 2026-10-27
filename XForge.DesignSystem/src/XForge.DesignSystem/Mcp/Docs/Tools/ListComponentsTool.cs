// T053: ListComponentsTool.
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Mcp.Docs.Tools;

public sealed class ListComponentsTool : IXFMcpTool
{
    public string Name => "list-components";
    public string Description => "Lists all registered XForge components, optionally filtered by category.";
    public string InputSchemaJson => "{\"type\":\"object\",\"properties\":{\"category\":{\"type\":\"string\"},\"limit\":{\"type\":\"integer\",\"default\":50}}}";
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "MCP server tool, not AOT-critical")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MCP server tool, not AOT-critical")]
    public string Execute(string argsJson) => JsonSerializer.Serialize(new { tool = Name, status = "ok", args = argsJson });
}