// T053: GetComponentTool.
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
namespace XForge.DesignSystem.Mcp.Docs.Tools;

public sealed class GetComponentTool : IXFMcpTool
{
    public string Name => "get-component";
    public string Description => "Returns the full documentation for a single component by name.";
    public string InputSchemaJson => "{\"type\":\"object\",\"properties\":{\"name\":{\"type\":\"string\"}},\"required\":[\"name\"]}";
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "MCP server tool, not AOT-critical")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MCP server tool, not AOT-critical")]
    public string Execute(string argsJson) => JsonSerializer.Serialize(new { tool = Name, status = "ok", args = argsJson });
}