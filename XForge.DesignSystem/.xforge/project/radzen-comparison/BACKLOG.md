# Backlog — Radzen Blazor (4-way consolidated)

Gerado em: 2026-06-13
Fontes analisadas: 6 (Repo GitHub + csproj + RadzenComponent.cs + ClassList.cs + ServiceCollectionExtensions.cs + AIChatService.cs)
Origem: https://github.com/radzenhq/radzen-blazor
Versão: 10.4.9 (MIT license)
**Consolidação 4-way**: MudBlazor (19) + antd-blazor (12) + Blazorise (16) + **Radzen (10) = 57 tasks total**

## P0 — Crítico (fazer primeiro)
| ID | Título | Estimativa | Dependências | Origem |
|----|--------|-----------:|:------------:|--------|
| T045 | ShouldAutoGenerateId + Attributes | 4h | nenhuma | Blazorise |
| T010 | CssBuilder fluent API | 4h | nenhuma | MudBlazor |
| T001 | ParameterState<T> + RegisterScope | 16h | nenhuma | MudBlazor |
| T044 | IClassProvider/IStyleProvider | 32h | T045 | Blazorise |
| T002 | Roslyn Analyzers (SH-001..N) | 24h | nenhuma | MudBlazor |
| T019 | JSON-based locales | 8h | T005 | antd-blazor |
| T056 | done | RadzenComponent base + OnBecameInvisible | 4h | T045 | **Radzen** ⭐ |
| T057 | done | Debouncer/Throttle (System.Timers) | 2h | nenhuma | **Radzen** ⭐ |
| T059 | done | ClassList struct-based fluent | 4h | nenhuma | **Radzen** ⭐ |
| T060 | done | AIChatService SSE streaming + session | 16h | T060a | **Radzen** ⭐ |
| T063 | done | AddXForgeComponents() extension | 1h | nenhuma | **Radzen** ⭐ |

## P1 — Importante
| ID | Título | Estimativa | Dependências | Origem |
|----|--------|-----------:|:------------:|--------|
| T004 | DomEventService (singleton) | 12h | nenhuma | antd-blazor |
| T005 | Localized XML Intellisense 3 langs | 24h | T019 | antd-blazor |
| T017 | CallAfterRender + ForwardRef | 8h | nenhuma | antd-blazor |
| T018 | OneOf union types | 12h | nenhuma | antd-blazor |
| T020 | bUnit + Viewer (1.36+) | 24h | nenhuma | MudBlazor |
| T021 | Docs Compiler (MSBuild) | 32h | T046 | MudBlazor |
| T022 | Stryker.NET mutation tests | 16h | T020 | MudBlazor |
| T047 | BaseComponent<TClasses,TStyles> | 16h | T044 | Blazorise |
| T048 | FluentValidation integration | 16h | nenhuma | Blazorise |
| T049 | Roslyn SG (NOT Fody) for [Parameter] | 16h | nenhuma | Blazorise |
| T050 | 38 Extensions strategy (select 5-10) | 40h | T044 | Blazorise |
| T052 | Directory.Packages.props (CPM) | 2h | nenhuma | Blazorise |
| T058 | done | StringBuilderCache ThreadStatic | 2h | T059 | done | **Radzen** ⭐ |
| T062 | done | API Generator (auto Razor pages) | 24h | T021, T046 | **Radzen** ⭐ |
| T064 | done | ThemeService abstract + 2 impls | 4h | T052 | **Radzen** ⭐ |
| T029 | IconLibrary unificada (FA + Material) | 8h | nenhuma | XForge |

## P2 — Desejável
| ID | Título | Estimativa | Dependências | Origem |
|----|--------|-----------:|:------------:|--------|
| T012 | N+1 detector + Compile-time warnings | 16h | T002 | MudBlazor |
| T053 | Docs.Mcp server (6 tools) | 32h | T021, T046 | Blazorise |
| T015 | Multi-TFM net8/9/10 | 16h | nenhuma | MudBlazor |
| T016 | Trim safety + LinkerConfig.xml | 8h | T015 | MudBlazor |
| T023 | Docs.Generator Roslyn | 24h | T021 | MudBlazor |
| T024 | Snippets integration | 16h | T023 | MudBlazor |
| T046 | Snippets.generated.cs | 24h | T021 | Blazorise |
| T054 | Analyzers test suite | 16h | T002 | Blazorise |
| T055 | Unit + E2E + Benchmark tests | 24h | T020 | Blazorise |
| T011 | Cross-platform E2E (Playwright) | 8h | T020 | MudBlazor |
| T013 | Bun for assets (optional) | 4h | nenhuma | MudBlazor |
| T061 | done | Build-time SCSS/JS via custom MSBuild | 8h | T015 | **Radzen** ⭐ |
| T065 | done | Cloud-hosted XForge MCP server | 16h | T053 | **Radzen** ⭐ |

## P3 — Baixa prioridade
| ID | Título | Estimativa | Dependências | Origem |
|----|--------|-----------:|:------------:|--------|
| T003 | Bun asset pipeline | 4h | nenhuma | MudBlazor |
| T006 | ISessionStorageService | 8h | nenhuma | XForge |
| T007 | ILocalStorageService | 8h | nenhuma | XForge |
| T008 | ThemeProvider runtime | 16h | T052 | XForge |
| T009 | LanguageService + 3 langs | 24h | T019 | XForge |
| T014 | Tailwind v4 plugins | 8h | nenhuma | MudBlazor |
| T025-T028 | 4 legado improvements | 32h | various | XForge |
| T030-T039 | 10 features secundárias | 80h | various | XForge |
| T051 | Dual-license pattern (eval) | 4h | nenhuma | Blazorise |

## Resumo
- **Total: 57 tarefas** (P0: 11, P1: 15, P2: 13, P3: 18)
- **Estimativa total**: ~1.020h
- **Origem**: MudBlazor 19 + antd-blazor 12 + Blazorise 16 + Radzen 10
- **Tarefas sem dependência**: 23 (40%)
- **Top 5 quick wins**: T045 (4h), T010 (4h), T052 (2h), T057 (2h), T063 (1h)