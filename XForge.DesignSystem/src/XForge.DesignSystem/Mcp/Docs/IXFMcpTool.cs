// T053: MCP tool contract.
namespace XForge.DesignSystem.Mcp.Docs;

/// <summary>Marker interface for a Docs.Mcp tool.</summary>
public interface IXFMcpTool
{
    string Name { get; }
    string Description { get; }
    string InputSchemaJson { get; }
    string Execute(string argsJson);
}

public sealed record ListComponentsArgs(string? Category = null, int Limit = 50);
public sealed record GetComponentArgs(string Name);
public sealed record SearchDocsArgs(string Query, int Limit = 10);
public sealed record ListSnippetsArgs(string? Component = null);
public sealed record GetSnippetArgs(string Component, string Title);
public sealed record ListExamplesArgs(string? Category = null);