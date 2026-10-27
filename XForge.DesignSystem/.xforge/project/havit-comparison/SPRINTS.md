# Sprints — Havit.Blazor (5-way Consolidated)

## Sprint 1 (semana 1-2) — Fundação
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T063 | AddXForgeComponents() extension | 0.1 | Rad |
| T052 | xunit.v3 + MTP setup | 0.25 | Havit |
| T073 | AddHxServices + TimeProvider | 0.25 | Havit |
| T057 | Debouncer/Throttle | 0.25 | Rad |
| T045 | ShouldAutoGenerateId + Attributes | 0.5 | Blz |
| T010 | ClassBuilder fluent | 0.5 | Mud |
| T059 | ClassList struct (supersedes T010) | 0.5 | Rad |
| T056 | RadzenComponent base | 0.5 | Rad |
| T058 | StringBuilderCache | 0.25 | Rad |
| T001 | ParameterState<T> | 2 | Mud |
| T002 | Roslyn Analyzers | 3 | Mud/Blz |

## Sprint 2 (semana 3-4) — Core Base
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T066 | HxSetup + IInputWith* interfaces | 1 | Havit |
| T044 | IClassProvider/IStyleProvider | 4 | Blz |
| T004 | DomEventService | 1.5 | antd |
| T017 | CallAfterRender + ForwardRef | 1 | antd |
| T018 | OneOf union types | 1.5 | antd |
| T047 | BaseComponent<TClasses,TStyles> | 2 | Blz |
| T005 | Localized XML docs | 3 | antd |
| T049 | Fody Weaver auto-Dirty | 2 | Blz |
| T019 | JSON-based locales | 1 | antd |
| T067 | HxInputBase<T> + FormState | 1.5 | Havit |
| T070 | HxChangeTracker | 0.75 | Havit |

## Sprint 3 (semana 5-6) — Componentes
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T020 | UnitTests.Viewer | 3 | Mud |
| T022 | Stryker mutation testing | 2 | Mud |
| T012 | FluentValidation integration | 2 | Mud |
| T048 | IdGenerator DI | 0.5 | Blz |
| T069 | HxMessageBoxService + DI | 0.5 | Havit |
| T064 | ThemeService abstract | 0.5 | Rad |
| T060 | AIChatService SSE | 2 | Rad |
| T075 | HxSidebar/HxListLayout | 1 | Havit |
| T034 | Sidebar + TreeView | 1 | Mud |
| T076 | BootstrapIconsGenerator SG | 0.5 | Havit |
| T037 | Theme color resolver | 0.5 | Havit |
| T029 | CSS reset + normalize | 1 | Mud |

## Sprint 4 (semana 7-8) — Docs & MCP
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T021 | Docs compiler (CLI) | 4 | Mud |
| T046 | Snippets.generated.cs Roslyn | 3 | Blz |
| T053 | Docs.Mcp server | 2 | Blz |
| T023 | Docusaurus-style docs | 2 | Mud |
| T065 | XForge MCP server | 2 | Rad |
| T062 | API Generator | 3 | Rad |
| T074 | Resources .resx pattern | 1 | Havit |
| T011 | CSS scope generator | 1 | Mud |
| T013 | ResX→constant class | 0.5 | Mud |
| T027 | OpenTelemetry integration | 1 | antd |

## Sprint 5 (semana 9-10) — DataGrid + AI
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T015 | Multi-TFM net8/9/10 | 2 | Mud |
| T071 | HxGrid w/ DataProvider | 2 | Havit |
| T068 | HxDialogBase<TResult> | 1 | Havit |
| T072 | HxFilterForm w/ ChipGenerator | 1.5 | Havit |
| T035 | AI model registry | 1 | XForge |
| T033 | Smart Paste (AI→form) | 2 | Mud/Smart |
| T077 | HxSmart* AI components | 2 | Havit |
| T016 | CSharpier/format | 1 | Mud |

## Sprint 6 (semana 11-12) — Hardening & Release
| ID | Título | Dias | Origem |
|----|--------|------|--------|
| T024 | CDN fallback assets | 1 | Mud |
| T025 | Browser compat matrix | 1 | Mud |
| T026 | Global SuppressMessage + EditorConfig | 1 | Mud |
| T030 | AOT/trim analyzer | 2 | Mud |
| T031 | Fiddle/repl playground | 3 | Mud |
| T032 | VSTHRD analyzer | 0.5 | Havit |
| T028 | Ant.targets shipped to consumers | 2 | antd |
| T061 | Build-time SCSS/JS MSBuild | 1 | Rad |
| T036 | gRPC code-first client | 5 | Havit |

## Milestones
| Milestone | Sprint | Data estimada | Entregável |
|-----------|--------|---------------|-----------|
| Fundação | Sprint 1 | Fim semana 2 | Base classes, DI, test runner |
| Core Base | Sprint 2 | Fim semana 4 | InputBase, providers, localization |
| Componentes | Sprint 3 | Fim semana 6 | Grid, AI, layouts funcionais |
| Docs & MCP | Sprint 4 | Fim semana 8 | Docs compiler + MCP server live |
| DataGrid + AI | Sprint 5 | Fim semana 10 | HxGrid + AI providers + Smart |
| Release v2.0.0 | Sprint 6 | Fim semana 12 | Hardening + gRPC + ship |
