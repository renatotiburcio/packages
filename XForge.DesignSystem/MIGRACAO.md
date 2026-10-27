# Migração — XForge.DesignSystem

Migração do design system que vivia em `temp/xforge-design-system` (fora do controle de
versionamento do monorepo) para o monorepo `renatotiburcio/packages`.

## Análise do que foi migrado

Biblioteca de componentes **Blazor/Razor** (`Microsoft.NET.Sdk.Razor`, net10.0) madura e
extensa, com 15 projetos em 3 camadas:

| Camada | Projetos | Papel |
|---|---|---|
| **Componentes** | `XForge.DesignSystem` (núcleo, 297 `.razor`), `.Shared` | componentes, temas e tokens |
| **Docs** | `.Docs`, `.Docs.AI`, `.Docs.Shared` | site de documentação (Blazor) |
| **Ferramentas** | `.Analyzers`, `.Generators.AssetsGenerator`, `.Generators.DocsCompiler`, `.Docs.MCP` | analisador Roslyn, geradores de assets/docs e servidor MCP de documentação |
| **Qualidade** | `.Tests` (+ `DocsQualityTests`), `.AI.Tests`, `.Benchmarks`, `.E2E` | testes, benchmarks e E2E |

O núcleo é compilado com `IsTrimmable`, símbolos empacotados (`snupkg`) e documentação XML —
compatível com os padrões de publicação do monorepo.

## O que foi preservado

- Estrutura completa de `src/`, `tests/`, `docs/`, `scripts/`, `.github/`, `.kilo/`,
  `.mimocode/`, `.xforge/`
- `Directory.Packages.props` (central package management), `NuGet.config`,
  `tsconfig.json`, `package.json`/`BUN-PIPELINE.md` (pipeline de assets), `docker-compose.yml`
- Os arquivos de convenção do repo original (`AGENTS.md`, `.githooks/`, `.gitattributes`,
  `.kilocodeignore`, `kilo.jsonc`)

## O que NÃO foi copiado

`bin/`, `obj/`, `.vs/`, `tmp/` e artefatos de build — reconstruídos no primeiro build.

## Verificação

- `dotnet build src/XForge.DesignSystem` — **passa** (31 avisos pré-existentes; o projeto
  original deliberadamente usa `TreatWarningsAsErrors=false`)

## Notas

- O repo original tinha `.slnx` (formato XML do SDK 10); o CI do monorepo referencia os
  projetos pelos paths.
- Testes completos (`.Tests`, `.E2E`, `.AI.Tests`) exigem o runtime de ASP.NET/Blazor e o
  pipeline de assets (`BUN-PIPELINE.md`); rodar localmente antes de publicar.
