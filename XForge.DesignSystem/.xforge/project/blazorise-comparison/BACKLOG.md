# Backlog — Blazorise vs XForge (consolidado 3-way)

Gerado em: 2026-06-13
Fontes analisadas: 6 (repo clonado 6.692 files, GitHub, csproj, BaseComponent.cs, Docs.Mcp, Docs.Compiler, Docs.Mcp README)
Consolidado com: MudBlazor (24 tasks) + antd-blazor (11 tasks) = base 35 tasks
+ tasks exclusivas do Blazorise: 12 (T044–T055)
**Total: 47 tarefas** (P0:6, P1:14, P2:15, P3:12)

## P0 — Crítico (fazer primeiro)
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T001 | ParameterState framework (auto-SetParametersAsync) | 16h | MudBlazor | nenhuma |
| T002 | Roslyn Analyzers (4 diagnostics) | 24h | MudBlazor | nenhuma |
| T004 | DomEventService (singleton DI) | 12h | antd-blazor | nenhuma |
| T005 | Localized XML docs (pt-BR/en/es) | 24h | antd-blazor | T001 |
| T044 | done | **Provider-agnostic abstraction (IClassProvider/IStyleProvider)** | 32h | Blazorise | nenhuma |
| T045 | done | **ShouldAutoGenerateId em base + Attributes CaptureUnmatchedValues** | 4h | Blazorise | nenhuma |

## P1 — Importante
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T010 | CssBuilder fluent API | 4h | MudBlazor | nenhuma |
| T015 | Multi-TFM net8/9/10 | 16h | MudBlazor | T001 |
| T017 | CallAfterRender + ForwardRef | 8h | antd-blazor | nenhuma |
| T018 | OneOf union types | 12h | antd-blazor | T001 |
| T019 | JSON-based locales (replacing resx) | 8h | antd-blazor | nenhuma |
| T020 | Test infrastructure (xUnit + bUnit Viewer) | 24h | MudBlazor | nenhuma |
| T021 | Docs Compiler (Microsoft.CodeAnalysis) | 32h | Blazorise/Mud | T002 |
| T046 | done | **Snippets.generated.cs (executable code samples)** | 24h | Blazorise | T021 |
| T047 | done | **BaseComponent<TClasses,TStyles> (typed customization)** | 16h | Blazorise | T044 |
| T048 | done | **FluentValidation integration (Blazorise.FluentValidation)** | 16h | Blazorise | nenhuma |
| T049 | done | **Fody Weaver for [Parameter] auto-Dirty()** | 16h | Blazorise | T045 |
| T050 | done | **38 Extensions strategy (DataGrid/Charts/Scheduler pick)** | 8h | Blazorise | nenhuma |
| T051 | done | **Blazorise.Licensing pattern (commercial gate)** | 8h | Blazorise | nenhuma |
| T052 | done | **Centralized package versions (Directory.Packages.props)** | 2h | Blazorise | nenhuma |

## P2 — Desejável
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T003 | Bun asset pipeline | 16h | MudBlazor | nenhuma |
| T011 | TrimMode=link + IsTrimmable | 8h | MudBlazor | T015 |
| T012 | Coverlet + Stryker mutation | 16h | MudBlazor | T020 |
| T013 | EmbedUntrackedSources + snupkg | 4h | MudBlazor | nenhuma |
| T014 | Resx performance (IsAsConstants=true) | 4h | MudBlazor | T019 |
| T016 | Localization 6 langs (ja/ko/zh/ru) | 24h | antd-blazor | T005 |
| T022 | E2E tests (Playwright + NUnit) | 32h | Blazorise | T020 |
| T024 | Component virtual scrolling | 24h | MudBlazor | T001 |
| T025 | BenchmarkDotNet suite | 16h | Blazorise | T020 |
| T053 | done | **Docs.Mcp server (AI-queryable docs, 6 tools)** | 32h | Blazorise | T021, T046 |
| T054 | done | **Blazorise.Analyzers tests** | 16h | Blazorise | T002 |
| T055 | done | **Blazorise.Benchmark project** | 16h | Blazorise | T025 |
| T027 | AntDesign.targets (shipped MSBuild) | 8h | antd-blazor | T015 |
| T028 | MCP server for docs (fallback) | 32h | antd-blazor | T021 |
| T029 | TestKit separation | 8h | antd-blazor | T020 |

## P3 — Baixa prioridade
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T006 | Bun runtime install | 4h | MudBlazor | T003 |
| T007 | MudForm deep validation | 16h | MudBlazor | T002 |
| T008 | Theming system v2 | 24h | MudBlazor | T044 |
| T009 | Analyzers (FluentValidation alignment) | 16h | MudBlazor | T002 |
| T023 | Accessibility audit (axe-core) | 16h | MudBlazor | T020 |
| T026 | MAUI / Hybrid targets | 32h | antd-blazor | T015 |
| T030 | DraftMonitor component | 8h | antd-blazor | nenhuma |
| T031 | IconTemplate composable slot | 8h | antd-blazor | nenhuma |
| T032 | BulkOperations extension | 16h | Blazorise | nenhuma |
| T033 | Charts (ECharts/Chart.js) extension | 24h | Blazorise | nenhuma |
| T034 | DataGrid advanced (filter/sort/group) | 24h | Blazorise | T032 |
| T035 | Scheduler (calendar) extension | 32h | Blazorise | nenhuma |

## Resumo
- Total: **47 tarefas** (P0:6, P1:14, P2:15, P3:12)
- Estimativa total: **~750h** (~94 dias úteis)
- Tarefas sem dependência: **19** (40%)
- Origem: MudBlazor 19, antd-blazor 12, Blazorise 16

## Top 10 ROI (ordem de execução)
| # | Tarefa | h | Por quê |
|---|--------|---|---------|
| 1 | T044 Provider-agnostic | 32h | Fundação para multi-CSS no futuro |
| 2 | T045 ShouldAutoGenerateId + Attributes | 4h | Fundação de identidade |
| 3 | T001 ParameterState | 16h | Fundação de estado |
| 4 | T002 Analyzers | 24h | Previne bugs em PR |
| 5 | T021 Docs.Compiler | 32h | Foundation para T046/T053 |
| 6 | T046 Snippets.generated.cs | 24h | Exemplos sempre compilam |
| 7 | T047 BaseComponent<TClasses,TStyles> | 16h | Customização tipada |
| 8 | T053 Docs.Mcp server | 32h | AI-assist friendly docs |
| 9 | T010 CssBuilder | 4h | Quick win |
| 10 | T049 Fody Weaver | 16h | Remove boilerplate |
