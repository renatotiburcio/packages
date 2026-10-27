---
id: T062
title: API Generator (auto Razor pages from XML docs)
priority: P1
status: pending
estimated_time: 24h
dependencies: T021, T046
---

# T062: API Generator (auto Razor pages from XML docs)

## Objetivo
Implementar gerador automático de páginas Razor de API reference a partir de XML doc comments, integrado in-process (não CLI externo).

## Passos
1. Criar projeto `tools/XForge.Api.Generator` (Roslyn SG)
2. Implementar `ISourceGenerator` que escaneia assembly output
3. Usar `System.Reflection.MetadataLoadContext` (MIT, estável)
4. Parsear `<summary>`, `<param>`, `<returns>`, `<remarks>`, `<example>`
5. Gerar `.razor` files em `obj/Debug/net10.0/generated/ApiDocs/`
6. Cada componente gera 1 página com: Description, Parameters table, Events table, Examples
7. Integrar com Docs Compiler (T021) pipeline
8. Indexar em `docs-api-index.json` (consumido por Docs.Mcp T053)
9. Testar com 10+ componentes XForge

## Arquivos
- Criar: `tools/XForge.Api.Generator/ApiDocGenerator.cs`
- Criar: `tools/XForge.Api.Generator/XmlDocParser.cs`
- Criar: `tools/XForge.Api.Generator/RazorPageGenerator.cs`
- Modificar: `src/XForge.DesignSystem/XForge.DesignSystem.csproj` (analyzer reference)
- Criar: `tests/XForge.Api.Generator.Tests/`

## Critérios de Aceite
- [ ] Gera 1 página por componente
- [ ] Inclui parameters, events, examples, remarks
- [ ] Build incremental (regenera só o que mudou)
- [ ] Integra com T021 (Docs Compiler) e T046 (Snippets)
- [ ] Index JSON válido para MCP server (T053)
- [ ] Build 0 warnings
- [ ] Testes cobrem 10+ componentes

## Notas
- Origem: **Radzen** ⭐ (adaptado para in-process)
- Ref: `radzen-blazor/Radzen.Blazor.Api.Generator/Program.cs:1704`
- **Adaptar**: Radzen usa CLI separado (Program.cs 1704 linhas); preferimos Roslyn SG in-process
- `System.Reflection.MetadataLoadContext` 9.* (MIT)
- 24h = 16h generator + 4h tests + 4h integration