# Roadmap — Radzen Blazor (4-way consolidated, 6 meses)

## Timeline

```
Mês 1 (Jun-Jul)    Mês 2 (Jul-Ago)    Mês 3 (Ago-Set)    Mês 4 (Set-Out)    Mês 5 (Out-Nov)    Mês 6 (Nov-Dez)
├─ Sprint 1 ──────┼─ Sprint 2 ───────┼─ Sprint 3 ──────┼─ Sprint 4 ──────┼─ Sprint 5 ──────┼─ Sprint 6 ──────┤
│  Fundação P0    │   Core Arch      │  Testes + QA    │  Docs + MCP     │   Multi-TFM     │  Hardening     │
│  45h (10 tasks) │   90h (10 tasks) │  90h (7 tasks)  │  110h (7 tasks) │   40h (4 tasks) │  50h (5 tasks) │
└─────────────────┴──────────────────┴─────────────────┴────────────────┴────────────────┴────────────────┘
                       ▲                   ▲                ▲                 ▲                  ▲
                  Foundation OK        Tests Green    Docs Live + MCP   net8/9/10 OK     v2.0.0 RELEASE
```

## Fases
| Fase | Período | Entregável | Origem contribuição |
|------|---------|-----------|---------------------|
| Fundação | Mês 1 | T001, T002, T010, T019, T045, T052 + 4 Radzen | MudBlazor + Blazorise + Radzen |
| Core | Mês 2 | T004, T017, T018, T044, T047, T005, T049 + 3 Radzen | MudBlazor + antd-blazor + Blazorise + Radzen |
| Testes | Mês 3 | T020, T022, T029, T012, T048, T054, T055 | MudBlazor + Blazorise + XForge |
| Docs | Mês 4 | T021, T046, T062, T053, T023, T024, T065 | MudBlazor + Blazorise + Radzen |
| Multi-TFM | Mês 5 | T015, T016, T011, T013 | MudBlazor |
| Release | Mês 6 | T050, T061, T051, T009, T008 | Blazorise + Radzen + XForge |

## Marcos de Decisão
| Marco | Quando | Decisão |
|-------|--------|---------|
| D-90 | Sprint 1 fim | Validar fundação antes de avançar |
| D-60 | Sprint 3 fim | Coverage ≥ 70%? Avançar para docs |
| D-30 | Sprint 5 fim | MCP server live? Validar premium features |
| D-7 | Sprint 6 fim | Release candidate + smoke tests |
| D-0 | 2026-09-05 | Release v2.0.0 |

## Riscos por Sprint
| Sprint | Risco | Mitigação |
|--------|-------|-----------|
| S1 | Analyzers quebram build | SH-001..N como warnings, não errors |
| S2 | IClassProvider over-engineering | YAGNI — implementar T044 só para Tailwind provider |
| S3 | Coverage < 70% | Adiar T022 (Stryker) para v2.1 |
| S4 | Docs.Mcp hosting cost | Avaliar self-hosted (MCP stdio) vs cloud |
| S5 | Trim warnings | LinkerConfig.xml desde T015 (não esperar T016) |
| S6 | 38 Extensions scope creep | Limitar a 5-10 prioritários (DataGrid, Charts, Scheduler, Gantt, AI) |

## Critérios de Saída por Sprint
| Sprint | Critério |
|--------|----------|
| S1 | 100% P0 done, build clean, 0 warnings |
| S2 | Architecture consistente, exemplos funcionais |
| S3 | Coverage ≥ 70%, mutation score ≥ 50% |
| S4 | Docs 100% published, MCP server live |
| S5 | Multi-TFM build OK, trim OK |
| S6 | Release notes + 0 P0 bugs + 0 security issues |