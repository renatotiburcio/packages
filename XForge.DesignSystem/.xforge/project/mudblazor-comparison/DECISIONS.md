# Decisões Técnicas — MudBlazor Adoption

## D001: Adotar ParameterState<T> para todos componentes
- **Contexto**: XForge atual usa fields + setters manuais; risco de re-render loop, sem change handler centralizado
- **Opções**: A) Manter padrão atual, B) Mudar para ParameterState<T> (MudBlazor style), C) Usar ReactiveUI
- **Escolha**: B (ParameterState<T>)
- **Rationale**: Padrão comprovado em 10.4k-star lib, elimina BL0005 warnings, change handlers ficam no escopo de registro
- **Tradeoffs**: Maior boilerplate no constructor; learning curve para devs acostumados com fields
- **Esforço**: 16h (T001)
- **Risco**: Médio (refactor massivo de API pública)

## D002: Criar XForge.DesignSystem.Analyzers
- **Contexto**: Não temos static analysis; erros como parâmetros não documentados só são pegos em runtime/DocsQualityTests
- **Opções**: A) Manter DocsQualityTests only, B) Analyzers Roslyn, C) StyleCop only
- **Escolha**: B (Roslyn analyzers)
- **Rationale**: Feedback em compile-time, experiencia de MudBlazor, distribuível como NuGet
- **Tradeoffs**: Custo inicial alto; precisa de TestComponents project
- **Esforço**: 24h (T002)
- **Risco**: Baixo (independente, mas requer cuidado com versionamento)

## D003: Migrar build de assets para Bun
- **Contexto**: Bundling de JS/SCSS atual é lento; MudBlazor usa Bun com great success
- **Opções**: A) Manter status quo, B) Bun, C) esbuild
- **Escolha**: B (Bun)
- **Rationale**: 10-100x mais rápido; alinhamento com MudBlazor; tooling leve via dotnet-tools.json
- **Tradeoffs**: Requer Bun runtime no PATH; lockfile bun.lock
- **Esforço**: 8h (T003)
- **Risco**: Baixo (atrás de SkipBunCompile flag)

## D004: Multi-targeting net8/9/10
- **Contexto**: XForge só roda em net10.0 (limit audience)
- **Opções**: A) Manter net10 only, B) Multi-TFM
- **Escolha**: B (multi-TFM)
- **Rationale**: Padronizar com indústria, ampliar compat (LTS net8)
- **Tradeoffs**: Build matrix testing; ajustes de API para TFM antigo
- **Esforço**: 2h (T016)
- **Risco**: Baixo

## D005: Adotar TrimMode=link + IsTrimmable
- **Contexto**: Native AOT cada vez mais importante; sem trim, warnings em produção
- **Opções**: A) Default (copy), B) TrimMode=link
- **Escolha**: B
- **Rationale**: Pronto para AOT, sem warnings
- **Tradeoffs**: Requer cuidado com reflection-heavy code
- **Esforço**: 2h (T023)
- **Risco**: Médio (deve-se rodar Native AOT validation)

## D006: MudBaseInput/Button hierarchy (vs 3-file pattern)
- **Contexto**: XForge usa XForgeComponentBase + XFInputComponentBase (3-file: razor/cs/types.cs)
- **Opções**: A) Manter 3-file, B) Migrar para MudBaseInput/MudBaseButton
- **Escolha**: A (manter 3-file) — mas adicionar CssBuilder + ParameterState
- **Rationale**: 3-file é mais clean; MudBlazor's hierarchy tem mais inheritance depth
- **Tradeoffs**: Continuamos diferentes; perdemos alguns helpers built-in
- **Esforço**: 0h (decisão de NÃO migrar)
- **Risco**: Nenhum

## D007: Usar Microsoft.Testing.Platform (não VSTest)
- **Contexto**: XForge usa Microsoft.NET.Test.Sdk (VSTest)
- **Opções**: A) Manter VSTest, B) Migrar
- **Escolha**: B
- **Rationale**: Mais rápido, melhor diagnostics, padrão emergente
- **Tradeoffs**: Migração de global.json
- **Esforço**: 8h (T022)
- **Risco**: Baixo

## D008: Docs Compiler (Roslyn generator) para tests
- **Contexto**: DocsQualityTests atual é manual; MudBlazor gera via Roslyn
- **Opções**: A) Manter manual, B) Implementar Roslyn generator
- **Escolha**: B (mas como enhancement ao que temos)
- **Rationale**: Auto-discover de Examples/, type-safe nameof
- **Tradeoffs**: Custo inicial alto (40h); complexidade
- **Esforço**: 40h (T021)
- **Risco**: Médio (Roslyn generators são complexos)

## D009: Manter naming convention XF (não migrar para Mud prefix)
- **Contexto**: MudBlazor usa Mud prefix; XForge usa XF
- **Opções**: A) Renomear para Mud, B) Renomear para outro, C) Manter XF
- **Escolha**: C (manter XF)
- **Rationale**: Brand XForge consolidado; "XF" é distintivo e curto
- **Tradeoffs**: Fica fora do padrão MudBlazor
- **Esforço**: 0h
- **Risco**: Nenhum (decisão de marca)
