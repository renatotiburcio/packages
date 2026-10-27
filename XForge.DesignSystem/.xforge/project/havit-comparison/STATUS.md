# Status - Havit.Blazor (5-way Consolidated)

## Progresso
| Metrica | Valor |
|---------|-------|
| Tarefas totais | 69 |
| Concluidas | 12 |
| Em andamento | 0 |
| Pendentes | 69 |
| Progresso | 0% |
| Origens analisadas | 5 (Mud+antd+Blz+Rad+Havit) |
| Tarefas net-new (Havit) | 12 (T066-T077) |

## Ultima atualizacao
2026-06-13

## Proximas tarefas (Sprint 1 - quick wins)
| ID | Titulo | Prioridade | Origem | Estimativa |
|----|--------|-----------|--------|-----------|
| T063 | AddXForgeComponents() extension | P0 | Rad | 1h |
| T052 | xunit.v3 + MTP setup | P0 | Havit | 2h |
| T073 | AddHxServices + TimeProvider | P0 | Havit | 2h |
| T057 | Debouncer/Throttle | P0 | Rad | 2h |
| T045 | ShouldAutoGenerateId + Attributes | P0 | Blz | 4h |
| T010 | ClassBuilder fluent | P0 | Mud | 4h |
| T059 | ClassList struct (supersedes T010) | P0 | Rad | 4h |
| T056 | RadzenComponent base | P0 | Rad | 4h |
| T058 | StringBuilderCache | P1 | Rad | 2h |
| T001 | ParameterState<T> | P0 | Mud | 16h |
| T002 | Roslyn Analyzers | P0 | Mud/Blz | 24h |
| T066 | HxSetup + IInputWith* | P0 | Havit | 8h |
| T067 | HxInputBase<T> + FormState | P0 | Havit | 12h |
| T068 | HxDialogBase<TResult> | P0 | Havit | 8h |
| T069 | HxMessageBoxService | P0 | Havit | 4h |
| T044 | IClassProvider/IStyleProvider | P0 | Blz | 32h |
| T047 | BaseComponent<TClasses,TStyles> | P0 | Blz | 16h |

## Bloqueios
| ID | Titulo | Motivo |
|----|--------|--------|
| (nenhum) | | |

## 5-Way Source Distribution
| Origem | Tasks | Percent | Net-new Patterns |
|--------|------:|--------:|------------------|
| MudBlazor | 19 | 27.5% | ParameterState, Analyzers, Bun, IsTrimmable |
| antd-blazor | 12 | 17.4% | DomEventService, Localized XML, CallAfterRender, OneOf |
| Blazorise | 16 | 23.2% | Provider abstraction, Fody Weaver, MCP server, Snippets |
| Radzen | 10 | 14.5% | RadzenComponent base, ClassList, AIChatService SSE, AddRadzenComponents |
| **Havit** | **12** | **17.4%** | **HxInputBase, FormState, HxDialogBase, HxGrid, HxChangeTracker, HxSmart AI, BootstrapIcons SG** |
| **TOTAL** | **69** | 100% | |

## Key Numbers (Havit-only)
| Metrica | Valor |
|---------|-------|
| Bootstrap lib files | 550 (75 razor + 322 cs + 84 resx + 18 js + 7 css) |
| Public Hx components | 62 razor |
| Total Hx files (incl nongeneric, base) | 219 |
| Resources files | 84 (14 components x ~6 langs) |
| JavaScript modules | 18 |
| Smart components (AI) | 3 (ComboBox, PasteButton, TextArea) |
| MCP tools | 4 (Catalog, Docs, Samples, Type) |
| Source generators | 1 (BootstrapIconsGenerator, IIncremental) |
| Multi-TFM | 3 (net8/9/10) |
| License | MIT |
| NuGet version | 4.24.3 |
| Total test files | 66 (xunit.v3 + MTP) |
| Grpc packages | 9 (Client, Server, Core, etc) |
| Storage packages | 2 (Storage, E2ETests) |
| AppInsights packages | 5 (main, TestApp, E2ETests, Tests, etc) |
| Total project count | 28 (incl docs, tests, demos) |
| Build warnings | 0 (TreatWarningsAsErrors) |
| Central package mgmt | yes (Directory.Packages.props) |
| Editor config | tab size 4, no NRT, _underscore private fields |
| TimeProvider DI | yes (AddHxServices) |
| Bootstrap CSS | 5.3.8 bundled |
| Form pattern | FormState cascading + HxInputBase<T> : InputBase<TValue> |
| Modal return type | Task<DialogResult<TResult>> |
| Auto Id | yes (per pattern) |
| Setup static class | HxSetup.Defaults |
| Interface segregation | IInputWithPlaceholder, IInputWithSize, IInputWithLabelType |
| Debouncer | yes (CancellationToken-based) |
| Color mode | Auto/Light/Dark enum |
| Bootstrap flavor | HavitDefault/PlainBootstrap |
| Renderer per language | Microsoft.Extensions.Localization + resx |

