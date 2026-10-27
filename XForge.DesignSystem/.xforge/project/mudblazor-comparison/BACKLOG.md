# Backlog — MudBlazor Comparison & Adoption

Gerado em: 2026-06-13
Fontes analisadas: 6 (GitHub homepage, repo clonado, MudBlazor.csproj, AGENTS.md, ROADMAP.md, src structure)

## P0 — Crítico (bloqueia produção)
| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T001 | [x] Adotar ParameterState<T> framework para todos componentes XForge | 16h | nenhuma |
| T002 | [x] Criar projeto XForge.DesignSystem.Analyzers (Roslyn) | 24h | nenhuma |
| T003 | [x] Migrar para Bun asset pipeline (TS + SCSS) | 8h | nenhuma |

## P1 — Importante (qualidade DX)
| ID | Título | Estimativa | Dependências |
| T010 | [x] Implementar CssBuilder utility (substituir concat de strings) | 4h | nenhuma |
| T011 | [x] Adotar padrão MudBaseInput / MudBaseButton (hierarquia) | 12h | T001 |
| T012 | [x] Auto-properties em todos [Parameter] (regra AGENTS.md) | 6h | T001 |
| T013 | [x] XML docs + [Category(...)] em todo public Parameter | 8h | T012 |
| T014 | [x] MudComponentBase com Theme/RTL/Breakpoint cascading | 8h | nenhuma |
| T015 | [x] Renomear pastas: MudBlazor (XForge), MudIcon (XFIcon), etc | 4h | nenhuma |
| T016 | [x] Multi-targeting net8/9/10 (IsPackable com TFM matrix) | 2h | nenhuma |

## P2 — Desejável (maturidade)
| ID | Título | Estimativa | Dependências |
| T020 | [x] XForge.DesignSystem.UnitTests + UnitTests.Viewer pattern | 16h | T001 |
| T021 | [x] Docs Compiler (Roslyn generator) para auto-gerar tests | 40h | T001,T020 |
| T022 | [x] Microsoft.Testing.Platform migration | 8h | nenhuma |
| T023 | [x] IsTrimmable=true + TrimMode=link no csproj | 2h | T016 |
| T024 | [x] Simbolos de debug (snupkg) + EmbedUntrackedSources | 2h | nenhuma |
| T025 | [x] Publish 153-style release cadence (releases/runner) | 4h | T020 |
| T026 | [x] XForge.DesignSystem.Benchmarks (BenchmarkDotNet) | 8h | T020 |

## P3 — Baixa prioridade (nice-to-have)
| ID | Título | Estimativa | Dependências |
| T030 | [x] Localization (ResourceManager + IsAsConstants) | 8h | T001 |
| T031 | [x] Migration guide automático em breaking changes | 8h | T025 |
| T032 | [x] Contribution/Migration docs page na docsite | 4h | T030 |
| T033 | [x] Interactive Playground (try.mudblazor.com clone) | 24h | T020,T022 |

## Resumo
- Total: 22 tarefas (P0: 3, P1: 7, P2: 7, P3: 5)
- Estimativa total: 218h (~5.5 semanas a 40h/sem)
- Tarefas sem dependência: 9 (T001, T002, T003, T010, T014, T015, T016, T022, T024)
