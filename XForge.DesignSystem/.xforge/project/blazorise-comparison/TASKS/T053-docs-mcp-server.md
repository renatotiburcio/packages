---
id: T053
title: Docs.Mcp server (AI-queryable docs, 6 tools, 3 transports)
priority: P2
status: pending
estimated_time: 32h
dependencies: T021, T046
---

# T053: Docs.Mcp server (AI-queryable docs, 6 tools, 3 transports)

## Objetivo
Criar um MCP server (Model Context Protocol) que expõe a documentação XForge de forma programática para AI assistants (Claude, Cursor, GitHub Copilot). Inspirado no `Blazorise.Docs.Mcp` (live em https://mcp.blazorise.com/mcp).

## Contexto (Blazorise)
- **Origem**: `Documentation/Blazorise.Docs.Mcp/` (10 files: Program.cs, Tools/DocsTools.cs, DocsApiIndex.cs, DocsIndex.cs, README.md, etc.)
- **6 MCP tools**:
  1. `list_docs_pages(startsWith)` - lista páginas filtradas
  2. `get_docs_page(route)` - retorna página + examples + ApiRefs
  3. `get_example_code(code)` - retorna código executável de exemplo
  4. `get_component_api(typeName)` - API surface de um componente
  5. `get_docs_page_api(route)` - API de todos componentes referenciados
  6. `search_docs(query, maxResults)` - busca por route/title/description
- **3 transports**:
  - Streamable HTTP (POST/GET/DELETE `/mcp`)
  - SSE HTTP (`/mcp/sse` + `/mcp/message`)
  - Stdio (configurável, IIS disable)
- **Data sources**:
  - `docs-index.json` (gerado por Docs.Compiler a partir de Documentation/Blazorise.Docs/)
  - `docs-api-index.json` (gerado por ComponentsApiDocsGenerator a partir do source)

## Passos
1. Criar projeto `src\XForge.DesignSystem.Docs.Mcp\XForge.DesignSystem.Docs.Mcp.csproj` (net10.0, ASP.NET Core, package `ModelContextProtocol 1.2.0+`)
2. Implementar `Tools\DocsTools.cs` com 6 métodos decorados com `[McpServerTool]` e `[Description(...)]`
3. Implementar `DocsIndexProvider.cs` que carrega `docs-index.json` do output folder
4. Implementar `DocsApiIndexProvider.cs` que carrega `docs-api-index.json`
5. Implementar DTOs: `DocsPageSummary`, `DocsPageDetails`, `DocsExampleSummary`, `DocsApiComponent`
6. Configurar `Program.cs` com ASP.NET Core + MCP middleware + 3 endpoints: `/mcp`, `/mcp/sse`, `/mcp/message`
7. Adicionar `appsettings.json` com `Mcp:EnableStdio` flag
8. Criar `Test-Mcp.ps1` e `Test-Mcp-Api.ps1` (inspirado em Blazorise)
9. Publicar em IIS ou Azure App Service
10. Adicionar `.mcp\server.json` para descoberta
11. Documentar em `docs\mcp-server.md` com exemplos Claude/Cursor config

## Arquivos
- Criar: `src\XForge.DesignSystem.Docs.Mcp\XForge.DesignSystem.Docs.Mcp.csproj`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\Program.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\Tools\DocsTools.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\Tools\ComponentApiTools.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\DocsIndex.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\DocsIndexProvider.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\DocsApiIndex.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\DocsApiIndexProvider.cs`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\appsettings.json`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\Test-Mcp.ps1`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\Test-Mcp-Api.ps1`
- Criar: `src\XForge.DesignSystem.Docs.Mcp\.mcp\server.json`
- Criar: `docs\mcp-server.md`

## Critérios de Aceite
- [ ] 6 MCP tools implementados e funcionais (testáveis via Test-Mcp.ps1)
- [ ] 3 transports funcionam: Streamable HTTP, SSE, Stdio
- [ ] `list_docs_pages("/docs/components")` retorna 152+ componentes
- [ ] `get_component_api("XFButton")` retorna Parameters, Events, Methods
- [ ] `get_example_code("XFButtonExample")` retorna código .razor executável
- [ ] `search_docs("button")` retorna resultados relevantes
- [ ] `appsettings.json` permite `Mcp:EnableStdio=false` para IIS
- [ ] Documentação mostra como configurar Claude/Cursor/VS Code
- [ ] Servidor responde em < 500ms para tools simples

## Notas
- **Origem**: Blazorise (https://github.com/Megabit/Blazorise/tree/master/Documentation/Blazorise.Docs.Mcp)
- **Live reference**: https://mcp.blazorise.com/mcp (produção)
- **Package**: `ModelContextProtocol 1.2.0` (mesmo do Blazorise)
- **Inspiração antd-blazor**: PR #4758 (Fev 2026) propôs MCP; XForge implementa primeiro
- **Benefício**: AI assistants (Claude/Cursor) podem consultar XForge docs sem copy-paste
- **Cuidado**: Cachear JSON em memória; não recarregar a cada tool call
- **Futuro v2.1**: Adicionar tool `get_ai_provider_config(providerName)` para integração com LLMs
