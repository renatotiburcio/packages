# Backlog — Havit.Blazor (5-way Consolidated)

Gerado em: 2026-06-13
Fontes analisadas: 5 (MudBlazor + antd-blazor + Blazorise + Radzen + Havit)
Origem: https://github.com/havit/Havit.Blazor (v4.24.3, MIT)
Tarefas Havit-only (NEW): 12 (T066-T077)
Tarefas consolidadas totais: 69

## P0 — Crítico
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T001 | ParameterState<T> MudBlazor | 16h | Mud | nenhuma |
| T002 | Roslyn Analyzers (merge Mud+Blazorise) | 24h | Mud/Blz | T001 |
| T010 | ClassBuilder fluent | 4h | Mud/T059 ⭐ | nenhuma |
| T044 | IClassProvider/IStyleProvider (Blz base) | 32h | Blz | nenhuma |
| T045 | ShouldAutoGenerateId + Attributes | 4h | Blz | T044 |
| T047 | BaseComponent<TClasses,TStyles> | 16h | Blz | T044 |
| T052 | xunit.v3 + MTP (Havit pattern) | 2h | **Havit ⭐** | nenhuma |
| T056 | RadzenComponent base + OnBecameInvisible | 4h | Rad | nenhuma |
| T057 | Debouncer/Throttle | 2h | Rad | nenhuma |
| T059 | ClassList struct (supersedes T010) | 4h | Rad ⭐ | nenhuma |
| T060 | AIChatService SSE streaming | 16h | Rad | nenhuma |
| T063 | AddXForgeComponents() extension | 1h | Rad ⭐ | nenhuma |
| T066 | done | HxSetup static defaults + IInputWith* interfaces | 8h | **Havit ⭐** | T045 |
| T067 | done | HxInputBase<T> : InputBase<TValue> w/ FormState cascade | 12h | **Havit ⭐** | T066 | done |
| T068 | done | HxDialogBase<TResult> (TaskCompletionSource) | 8h | **Havit ⭐** | T060 |
| T069 | done | HxMessageBoxService + AddHxMessageBoxHost DI | 4h | **Havit ⭐** | T063 |

## P1 — Importante
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T004 | DomEventService (antd) | 12h | antd | T063 |
| T005 | Localized XML docs pt-BR/en/es | 24h | antd | T002 |
| T017 | CallAfterRender + ForwardRef | 8h | antd | T063 |
| T018 | OneOf union types | 12h | antd | nenhuma |
| T019 | JSON-based locales 70+ | 8h | antd | T005 |
| T046 | Snippets.generated.cs Roslyn | 24h | Blz | T002 |
| T048 | IdGenerator DI | 4h | Blz | T045 |
| T049 | Fody Weaver auto-Dirty | 16h | Blz | T002 |
| T058 | StringBuilderCache | 2h | Rad | nenhuma |
| T064 | ThemeService abstract | 4h | Rad | nenhuma |
| T070 | done | HxChangeTracker (INotifyPropertyChanged scope) | 6h | **Havit ⭐** | nenhuma |
| T071 | done | HxGrid w/ GridDataProviderDelegate<T> | 16h | **Havit ⭐** | T067 | done |
| T072 | done | HxFilterForm w/ HxChipGenerator pipeline | 12h | **Havit ⭐** | T067 | done |
| T073 | done | AddHxServices extension + TimeProvider | 2h | **Havit ⭐** | T063 |
| T074 | done | Resources .resx (per-component) pattern | 8h | **Havit ⭐** | T005 |
| T075 | done | HxSidebar/HxListLayout (layout shells) | 8h | **Havit ⭐** | T067 | done |

## P2 — Desejável
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T011 | CSS scope generator (Mud) | 8h | Mud | nenhuma |
| T013 | ResX→constant class | 4h | Mud | T005 |
| T020 | UnitTests.Viewer | 24h | Mud | T052 |
| T029 | CSS reset + normalize | 8h | Mud | nenhuma |
| T053 | Docs.Mcp server | 16h | Blz | T046 |
| T061 | Build-time SCSS/JS MSBuild | 8h | Rad | nenhuma |
| T062 | API Generator Razor pages | 24h | Rad | T002 |
| T065 | XForge MCP server (self-hosted) | 16h | Rad | T053 |
| T076 | done | BootstrapIconsGenerator SG (IIncremental) | 4h | **Havit ⭐** | nenhuma |
| T077 | done | HxSmart* AI components (SmartComponents wrapping) | 16h | **Havit ⭐** | T060 |

## P3 — Baixa prioridade
| ID | Título | Estimativa | Origem | Dependências |
|----|--------|-----------|--------|--------------|
| T012 | FluentValidation integration | 16h | Mud | nenhuma |
| T015 | Multi-TFM net8/9/10 | 16h | Mud | nenhuma |
| T016 | CSharpier/format integration | 8h | Mud | nenhuma |
| T021 | Docs compiler (standalone CLI) | 32h | Mud | T046 |
| T022 | Stryker mutation testing | 16h | Mud | T052 |
| T023 | Docusaurus-style docs | 16h | Mud | T053 |
| T024 | CDN fallback assets | 8h | Mud | nenhuma |
| T025 | Browser compat matrix | 8h | Mud | nenhuma |
| T026 | Global SuppressMessage + EditorConfig | 8h | Mud | nenhuma |
| T027 | OpenTelemetry integration | 8h | antd | nenhuma |
| T028 | Ant.targets shipped to consumers | 16h | antd | nenhuma |
| T030 | AOT/trim analyzer | 16h | Mud | T002 |
| T031 | Fiddle/repl playground | 24h | Mud | T053 |
| T032 | VSTHRD analyzer (Visual Studio Threading) | 4h | **Havit ⭐** | T002 |
| T033 | Smart Paste (AI→structured form) | 16h | Mud/Smart | T077 | done |
| T034 | Sidebar + TreeView layouts | 8h | Mud | T075 | done |
| T035 | AI model registry (11 providers) | 8h | XForge | T060 |
| T036 | gRPC code-first client | 40h | Havit | nenhuma |
| T037 | Theme color resolver (auto-light/dark) | 4h | Havit | T064 |

## Resumo
- Total: 69 tarefas (P0: 16, P1: 16, P2: 9, P3: 18)
- Tarefas Havit-only: 12 (T066-T077)
- Estimativa total: ~720h
- Tarefas sem dependência: 19
- 5-way priority matrix: Mud=19, antd=12, Blz=16, Rad=10, Havit=12
