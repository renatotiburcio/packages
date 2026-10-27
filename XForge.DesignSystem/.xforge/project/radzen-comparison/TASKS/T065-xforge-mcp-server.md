---
id: T065
title: XForge MCP server (self-hosted)
priority: P2
status: pending
estimated_time: 16h
dependencies: T053
---

# T065: XForge MCP server (self-hosted)

## Objetivo
Implementar MCP server self-hosted para AI assistants (Cursor, Claude Code, Copilot) com acesso a docs/APIs/templates XForge.

## Passos
1. Criar projeto `tools/XForge.Mcp.Server` (ASP.NET Core minimal API)
2. Adicionar pacote `ModelContextProtocol` 1.2.0+ (MIT)
3. Implementar 6 tools (paralelos a T053):
   - `list_components()`: retorna 110+ componentes XForge
   - `get_component(name)`: retorna API + examples
   - `get_example_code(component, scenario)`: retorna snippet
   - `get_ai_provider(name)`: retorna config OpenAI/Ollama/etc
   - `get_theme(name)`: retorna theme config
   - `get_docs(topic)`: retorna docs geradas (T021, T046, T062)
4. Implementar 3 transports: Streamable HTTP (`/mcp`), SSE (`/mcp/sse`), Stdio
5. Criar Dockerfile multi-stage
6. Documentar uso com Claude Code / Cursor / Copilot
7. Publicar em container registry (GHCR)
8. Testes E2E com MCP client

## Arquivos
- Criar: `tools/XForge.Mcp.Server/Program.cs`
- Criar: `tools/XForge.Mcp.Server/Tools/ComponentTools.cs`
- Criar: `tools/XForge.Mcp.Server/Tools/AIProviderTools.cs`
- Criar: `tools/XForge.Mcp.Server/Tools/ThemeTools.cs`
- Criar: `tools/XForge.Mcp.Server/Tools/DocsTools.cs`
- Criar: `tools/XForge.Mcp.Server/Dockerfile`
- Criar: `docs/mcp-server.md`
- Criar: `tests/XForge.Mcp.Server.Tests/`

## Critérios de Aceite
- [ ] 6 tools funcionais
- [ ] 3 transports (Streamable HTTP, SSE, Stdio)
- [ ] Dockerfile build OK
- [ ] Claude Code pode consumir via stdio
- [ ] Docs completos
- [ ] Build 0 warnings
- [ ] Testes E2E com MCP client

## Notas
- Origem: **Radzen** ⭐ (adaptado de Pro cloud → self-hosted)
- Radzen Pro tem MCP cloud; XForge faz self-hosted
- Ref: `blazorise-comparison/TASKS/T053-docs-mcp-server.md` (paralelo)
- ModelContextProtocol 1.2.0 (MIT)
- 16h = 8h tools + 4h transports + 4h Docker + tests