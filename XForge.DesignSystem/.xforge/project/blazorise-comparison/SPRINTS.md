# Sprints — Blazorise vs XForge (consolidado 3-way)

## Sprint 1 (semana 1-2) — Fundação
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T045 | ShouldAutoGenerateId + Attributes | 0.5 | Blazorise |
| T010 | CssBuilder | 0.5 | MudBlazor |
| T052 | Directory.Packages.props | 0.25 | Blazorise |
| T001 | ParameterState framework | 2 | MudBlazor |
| T044 | Provider-agnostic IClassProvider | 4 | Blazorise |
| T002 | Roslyn Analyzers | 3 | MudBlazor |
| T019 | JSON-based locales | 1 | antd-blazor |

## Sprint 2 (semana 3-4) — Core
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T004 | DomEventService | 1.5 | antd-blazor |
| T017 | CallAfterRender + ForwardRef | 1 | antd-blazor |
| T018 | OneOf union types | 1.5 | antd-blazor |
| T047 | BaseComponent<TClasses,TStyles> | 2 | Blazorise |
| T005 | Localized XML docs (pt-BR/en/es) | 3 | antd-blazor |
| T049 | Fody Weaver auto-Dirty | 2 | Blazorise |

## Sprint 3 (semana 5-6) — Testes
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T020 | Test infrastructure xUnit + bUnit | 3 | MudBlazor |
| T054 | Blazorise.Analyzers tests | 2 | Blazorise |
| T029 | TestKit separation | 1 | antd-blazor |
| T012 | Coverlet + Stryker | 2 | MudBlazor |
| T022 | E2E Playwright + NUnit | 4 | Blazorise |

## Sprint 4 (semana 7-8) — Docs
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T021 | Docs.Compiler (CodeAnalysis) | 4 | Blazorise/Mud |
| T046 | Snippets.generated.cs | 3 | Blazorise |
| T053 | Docs.Mcp server | 4 | Blazorise |
| T023 | axe-core a11y audit | 2 | MudBlazor |

## Sprint 5 (semana 9-10) — Multi-TFM
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T015 | Multi-TFM net8/9/10 | 2 | MudBlazor |
| T011 | TrimMode=link | 1 | MudBlazor |
| T013 | EmbedUntrackedSources + snupkg | 0.5 | MudBlazor |
| T016 | Localization 6 langs | 3 | antd-blazor |
| T048 | FluentValidation integration | 2 | Blazorise |
| T050 | 38 Extensions strategy | 1 | Blazorise |

## Sprint 6 (semana 11-12) — Hardening
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T025 | BenchmarkDotNet | 2 | Blazorise |
| T055 | Blazorise.Benchmark project | 2 | Blazorise |
| T014 | Resx perf (IsAsConstants) | 0.5 | MudBlazor |
| T027 | AntDesign.targets | 1 | antd-blazor |
| T028 | MCP server fallback | 4 | antd-blazor |
| T051 | Blazorise.Licensing pattern | 1 | Blazorise |
| T024 | Virtual scrolling | 3 | MudBlazor |

## Milestones
| Milestone | Sprint | Data estimada | Entregável |
|-----------|--------|---------------|-----------|
| Fundação completa | Sprint 1 | 2026-06-27 | ParameterState + Provider-agnostic + Analyzers |
| Core API estabilizado | Sprint 2 | 2026-07-11 | DomEventService + Localized + TClasses/TStyles |
| Cobertura ≥ 85% | Sprint 3 | 2026-07-25 | xUnit/bUnit + E2E + Stryker |
| Docs AI-friendly | Sprint 4 | 2026-08-08 | Docs.Compiler + Snippets + Docs.Mcp |
| Multi-TFM publicado | Sprint 5 | 2026-08-22 | net8/9/10 + 6 langs + 38 Ext strategy |
| Beta release v2.0.0 | Sprint 6 | 2026-09-05 | All P0+P1 done, benchmarks green |
