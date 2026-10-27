# Status — Blazorise vs XForge (consolidado 3-way)

## Progresso
| Métrica | Valor |
|---------|-------|
| Análises completas | 3 (MudBlazor + antd-blazor + Blazorise) |
| Tarefas totais | **47** |
| Concluídas | 0 (análise = planejamento) |
| Em andamento | 0 |
| Bloqueadas | 0 |
| Progresso | 0% (Fase 1 /research) ✅ |
| Próxima fase | Fase 2 /document-execute |

## Última atualização
2026-06-13 (Fase 1 /research Blazorise)

## Próximas tarefas (ordem de execução Top 10 ROI)
| # | ID | Título | Prioridade | h |
|---|----|--------|------------|---|
| 1 | T044 | Provider-agnostic IClassProvider | P0 | 32h |
| 2 | T045 | ShouldAutoGenerateId + Attributes | P0 | 4h |
| 3 | T001 | ParameterState framework | P0 | 16h |
| 4 | T002 | Roslyn Analyzers | P0 | 24h |
| 5 | T021 | Docs.Compiler (CodeAnalysis) | P1 | 32h |
| 6 | T046 | Snippets.generated.cs | P1 | 24h |
| 7 | T047 | BaseComponent<TClasses,TStyles> | P1 | 16h |
| 8 | T053 | Docs.Mcp server | P2 | 32h |
| 9 | T010 | CssBuilder | P1 | 4h |
| 10 | T049 | Fody Weaver auto-Dirty | P1 | 16h |

## Bloqueios
| ID | Título | Motivo | Resolução |
|----|--------|--------|-----------|
| (nenhum) | | | |

## Comparação 3-way: 16 NET-NEW insights do Blazorise
| # | Insight | Prio | Task |
|---|---------|------|------|
| 1 | **Provider-agnostic IClassProvider/IStyleProvider** | P0 | T044 |
| 2 | **BaseComponent<TClasses,TStyles> dual generic** | P1 | T047 |
| 3 | **Microsoft.CodeAnalysis.CSharp scanning** (build-time API gen) | P1 | T021 |
| 4 | **Snippets.generated.cs** (executable code samples) | P1 | T046 |
| 5 | **Blazorise.Docs.Mcp server** (6 tools, 3 transports, live!) | P2 | T053 |
| 6 | **ShouldAutoGenerateId virtual** em base | P0 | T045 |
| 7 | **IsEqual + Dirty** notification pattern | P0 | T045 |
| 8 | **CaptureUnmatchedValues Attributes** (Dictionary<string,object>) | P0 | T045 |
| 9 | **Fody Weavers** (IL post-processing) | P1 | T049 |
| 10 | **Blazorise.FluentValidation** extension | P1 | T048 |
| 11 | **Centralized package versions** (Directory.Packages.props) | P1 | T052 |
| 12 | **Blazorise.Licensing** pattern (commercial gate) | P1 | T051 |
| 13 | **38 Extensions strategy** (DataGrid, Charts, Scheduler...) | P1 | T050 |
| 14 | **6 test projects** (Unit/Analyzers/Benchmark/E2E/BasicTestApp) | P2 | T022/T054/T055 |
| 15 | **4 Source Generators** (Roslyn + Fody) | P1 | T049 |
| 16 | **9 Demo apps** (per-provider + MAUI) | P3 | T026 |

## Comparação Detalhada 3-way
| Aspecto | XForge | MudBlazor | antd-blazor | **Blazorise** |
|---------|-------:|----------:|------------:|--------------:|
| **Components core (.razor/.cs)** | 152 | 85+ | 70+ | **192 / 285** |
| **Total .razor in repo** | ~200 | 167 | 164 | **1.958** |
| **Total .cs in repo** | ~500 | 663 | 558 | **2.209** |
| **Solution projects** | 5 | 17 | 14 | **~120 (Blazorise.slnx)** |
| **csproj files** | 5 | ~20 | 14 | **79** |
| **UI providers (CSS)** | 1 (Tailwind v4) | 1 (Material) | 1 (Ant) | **7** |
| **Extensions** | 0 | ~12 | ~14 | **38** |
| **Icon packages** | 1 | 0 | 1 | **8** |
| **Test projects** | 2 | 5 | 4 | **6** |
| **Docs projects** | 1 (manual) | 1 (Viewer) | 5 (incl MCP) | **5 (incl Mcp live!)** |
| **Source Generators** | 0 | 1 | 0 | **4** (Roslyn + Fody) |
| **Multi-TFM** | 1 | 3 | **7 (max)** | 3 |
| **AI components** | **29** ⭐ | 0 | 0 | 0 |
| **Doc languages** | pt-BR | en | 6 | 1 (en) |
| **MCP server live** | no | no | no (PR) | **YES** ⭐ |
| **Licensing** | MIT | MIT | MIT | **Dual (commercial)** |
| **Analyzers** | 0 | 1 (4 diag) | 0 | 1 (Blazorise.Analyzers) |
| **Fody weaver** | no | no | no | **YES** ⭐ |
| **Provider abstraction** | no | no | no | **YES (IClassProvider)** ⭐ |
| **GitHub stars** | n/a | 10.4k | 6.2k | 3.5k |
| **Forks** | n/a | 1.6k | 1.2k | 538 |
| **License files** | 1 (MIT) | 1 (MIT) | 1 (MIT) | 2 (Dual) |

## Adoção Final (3-way)
- **Adotar TODOS os 16 net-new do Blazorise** (P0/P1/P2)
- **Adotar 24 do MudBlazor** (ParameterState, CssBuilder, Multi-TFM, Test, Benchmark, etc.)
- **Adotar 12 do antd-blazor** (DomEventService, Localized, MCP, OneOf, CallAfterRender, etc.)
- **NÃO adotar**: Dual-license, 7 TFMs, 38 Extensions integrais, 7 CSS providers, MAUI

## Stats do Repositório Blazorise
| Métrica | Valor |
|---------|-------|
| Total files | 6.692 |
| .razor files | 1.958 |
| .cs files | 2.209 |
| .csproj | 79 |
| .md docs | 33 |
| Source/Blazorise/Components | 192 razor + 285 cs |
| Source/Extensions (38 projects) | 192 razor + 285 cs |
| Source/SourceGenerators | 4 projects |
| Source/Helpers | 2 (Analyzers, Tests.bUnit) |
| Documentation/Blazorise.Docs.* | 5 projects |
| Tests/ | 6 projects |
| Demos/ | 9 apps + Shared |
| Multi-TFM | net8.0 + net9.0 + net10.0 |
| License | Dual (source-available) |
| GitHub stars | 3.5k |
| GitHub forks | 538 |
| Latest version | 2.2.0 |

## Next Phase
**Fase 2 (/document-execute)**: Executar Top 10 ROI (T010, T045, T052, T001, T002, T044, T019, T004, T017, T047, T049) — ~200h de execução.

## Ultima atualizacao
2026-06-13T15:56 - 100% BACKLOG Blazorise