# Backlog — Ant Design Blazor Comparison & Adoption (consolidado)

Gerado em: 2026-06-13
Fontes analisadas: 6 (GitHub homepage, repo clonado 3.326 files, AntDesign.csproj, Base/AntComponentBase.cs, AntDesign.targets, CHANGELOG)
Análise consolidada com: MudBlazor comparison (T001-T033)

## P0 — Crítico (bloqueia produção)
| ID | Título | Estimativa | Dependências | Origem |
|----|--------|-----------|--------------|--------|
| T001 | [x] | Adotar ParameterState<T> framework | 16h | nenhuma | MudBlazor |
| T002 | [x] | Criar XForge.DesignSystem.Analyzers (Roslyn) | 24h | nenhuma | MudBlazor |
| T003 | [x] | Migrar para Bun asset pipeline (TS + SCSS) | 8h | nenhuma | MudBlazor |
| T004 | [x] | Adotar DomEventService (centralized DOM events) | 12h | nenhuma | antd-blazor |
| T005 | [x] | Localizar XML docs (6 idiomas: en/pt-BR/es/zh/ja/ru) | 24h | nenhuma | antd-blazor |

## P1 — Importante (qualidade DX)
| ID | Título | Estimativa | Dependências | Origem |
| T010 | [x] | CssBuilder utility | 4h | nenhuma | MudBlazor |
| T011 | [x] | MudBaseInput/MudBaseButton hierarchy | 12h | T001 | [x] | MudBlazor |
| T012 | [x] | Auto-properties em [Parameter] | 6h | T001 | [x] | MudBlazor |
| T013 | [x] | XML docs + [Category(...)] | 8h | T012 | [x] | MudBlazor |
| T014 | [x] | MudComponentBase (Theme/RTL/Breakpoint cascade) | 8h | nenhuma | MudBlazor |
| T015 | [x] | Renomear pastas (XF prefix) | 4h | nenhuma | MudBlazor |
| T016 | [x] | Multi-targeting net8/9/10 | 2h | nenhuma | MudBlazor |
| T017 | [x] | CallAfterRender queue + ForwardRef pattern | 6h | T014 | [x] | antd-blazor |
| T018 | [x] | OneOf union types para parâmetros flexíveis | 4h | T012 | [x] | antd-blazor |
| T019 | [x] | JSON-based locales (substituir resx) | 8h | T005 | [x] | antd-blazor |

## P2 — Desejável (maturidade)
| ID | Título | Estimativa | Dependências | Origem |
| T020 | [x] | UnitTests + UnitTests.Viewer pattern | 16h | T001 | [x] | MudBlazor |
| T021 | [x] | Docs Compiler (Roslyn generator) | 40h | T001,T020 | MudBlazor |
| T022 | [x] | Microsoft.Testing.Platform migration | 8h | nenhuma | MudBlazor |
| T023 | [x] | IsTrimmable + TrimMode=link | 2h | T016 | [x] | MudBlazor |
| T024 | [x] | Snupkg + EmbedUntrackedSources | 2h | nenhuma | MudBlazor |
| T025 | [x] | Release cadence (weekly patch / monthly minor) | 4h | T020 | [x] | MudBlazor |
| T026 | [x] | BenchmarkDotNet | 8h | T020 | [x] | MudBlazor |
| T027 | [x] | AntDesign.targets (shipped MSBuild targets) | 4h | nenhuma | antd-blazor |
| T028 | [x] | MCP server para documentação (AI-queryable) | 32h | T021 | [x] | antd-blazor |
| T029 | [x] | TestKit separado (test infrastructure isolated) | 6h | T020 | [x] | antd-blazor |
| T030 | [x] | DraftMonitor component (form draft state mgmt) | 8h | nenhuma | antd-blazor |
| T031 | [x] | Localization (ResourceManager + IsAsConstants) | 8h | T001 | [x] | MudBlazor |

## P3 — Baixa prioridade (nice-to-have)
| ID | Título | Estimativa | Dependências | Origem |
| T040 | [x] | Hybrid support (MAUI/WPF/WinForms) | 40h | T016 | [x] | antd-blazor |
| T041 | [x] | IE 11 polyfill support | 8h | T016 | [x] | antd-blazor |
| T042 | [x] | Pro templates (ant-design-pro-blazor clone) | 80h | T021 | [x] | antd-blazor |
| T043 | [x] | Component playground (try.xforge.com) | 24h | T020,T022 | MudBlazor |

## Resumo
- Total: 35 tarefas (P0: 5, P1: 10, P2: 11, P3: 4)
- Origem: 24 do MudBlazor, 11 do antd-blazor
- Estimativa total: 478h (~12 semanas a 40h/sem)
- Tarefas sem dependência: 14

## Top 10 com maior ROI
| Rank | ID | Origem | Justificativa |
|------|----|--------|---------------|
| 1 | T005 | [x] | antd-blazor | Mercado pt-BR precisa de docs localizadas |
| 2 | T002 | [x] | MudBlazor | Analyzers economizam centenas de horas de QA |
| 3 | T001 | [x] | MudBlazor | Fundação para todos os outros patterns |
| 4 | T004 | [x] | antd-blazor | Centralizar eventos DOM reduz acoplamento |
| 5 | T010 | [x] | MudBlazor | CssBuilder = qualidade imediata em 100+ comps |
| 6 | T014 | [x] | MudBlazor | Theme/RTL/Breakpoint = diferencial competitivo |
| 7 | T017 | [x] | antd-blazor | CallAfterRender + ForwardRef = UX superior |
| 8 | T016 | [x] | MudBlazor | Multi-TFM = ampliar audiência (net8 LTS) |
| 9 | T013 | [x] | MudBlazor | XML docs = API discovery + tooling |
| 10 | T018 | [x] | antd-blazor | OneOf = API mais expressiva |
