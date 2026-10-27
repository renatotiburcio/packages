---
id: T102
title: XForge MCP Server (10 tools) para LLMs/Agentes
priority: P0
status: pending
estimated_time: 24h
dependencies: nenhuma
---

# T102: XForge MCP Server (10 tools)

## Objetivo
Implementar MCP Server self-hosted com 10 tools para permitir que LLMs/Agentes (Claude, GPT, Gemini) gerem codigo XForge com contexto completo.

## Passos
1. Criar projeto `XForge.DesignSystem.Mcp` (.NET 10, netstandard2.1)
2. Adicionar `ModelContextProtocol` 0.4.x NuGet (mesmo pattern Havit T065)
3. Implementar 10 tools como `[McpServerTool]` (Destructive=false, Idempotent=true, ReadOnly=true):
   - `get_component_catalog` (lista 181 componentes)
   - `get_component_docs` (param: componentName)
   - `get_component_samples` (param: componentName, scenario)
   - `get_type_doc` (param: typeName)
   - `get_icon_catalog` (param: variant?, size?, keyword?)
   - `get_emoji_catalog` (param: group?, keyword?)
   - `get_theme_tokens` (param: themeName)
   - `get_design_token` (param: tokenName)
   - `generate_snippet` (param: componentName, props)
   - `validate_accessibility` (param: htmlSnippet)
4. HTTP transport com `Stateless=true` (Havit pattern)
5. OpenTelemetry instrumentation (`McpToolActivitySource`)
6. Endpoint `/health` para probes
7. Registrar em `XForgeSetup.Mcp` extension method
8. Docker container self-hosted + docker-compose

## Arquivos
- Criar: `src/XForge.DesignSystem.Mcp/Program.cs`
- Criar: `src/XForge.DesignSystem.Mcp/Tools/` (10 arquivos)
- Criar: `src/XForge.DesignSystem.Mcp/XForge.DesignSystem.Mcp.csproj`
- Criar: `docker/xforge-mcp.dockerfile`
- Modificar: `.kilo/commands/research.md` (registrar MCP)

## Criterios de Aceitacao
- [ ] MCP server roda local via `dotnet run --project XForge.DesignSystem.Mcp`
- [ ] 10 tools acessiveis via Claude Desktop (`claude_desktop_config.json`)
- [ ] Cada tool retorna JSON estruturado com schema validado
- [ ] OpenTelemetry traces exportados para OTLP endpoint
- [ ] Docker image <100MB
- [ ] Documentacao em `docs/mcp/getting-started.md`
- [ ] Testes em `XForge.DesignSystem.Mcp.Tests` (10 tests, 1 por tool)
- [ ] LGPD compliant (zero dados enviados a cloud)

## Notas
- Inspirado em Havit.Documentation.Mcp (4 tools) e Telerik MCP Servers
- Self-hosted e OBRIGATORIO (LGPD Art. 46) — rejeitar cloud-only
- Latencia target: <500ms por tool
- Latencia target: <500ms por tool