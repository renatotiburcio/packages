# Roadmap — Havit.Blazor (5-way Consolidated)

## Timeline
```
Mês 1 (Jun)        Mês 2 (Jul)        Mês 3 (Ago)        Mês 4 (Set)        Mês 5 (Out)        Mês 6 (Nov)
├─ Sprint 1 ─────┼─ Sprint 2 ─────┼─ Sprint 3 ─────┼─ Sprint 4 ─────┼─ Sprint 5 ─────┼─ Sprint 6 ─────┤
│  Fundação      │  Core Base      │  Componentes   │  Docs & MCP   │  DataGrid+AI   │  Hardening    │
│  T001-T063     │  T004-T070      │  T020-T076     │  T021-T074    │  T015-T077     │  T024-T061     │
│  45h           │  90h            │  90h           │  110h         │  40h           │  50h           │
└────────────────┴────────────────┴────────────────┴────────────────┴────────────────┴────────────────┘
```

## Fases
| Fase | Período | Entregável | Tasks |
|------|---------|-----------|-------|
| Fundação | Jun/26 (S1) | Base classes, DI, xunit.v3/MTP, ParameterState, Roslyn analyzers | T001, T002, T010, T045, T052, T056, T057, T058, T059, T063, T073 |
| Core Base | Jul/26 (S2) | InputBase, providers, localization, FormState cascade, ChangeTracker | T004, T005, T017, T018, T019, T044, T047, T049, T066, T067, T070 |
| Componentes | Ago/26 (S3) | Grid, AI, layouts, theming, AIChatService | T012, T020, T022, T029, T034, T037, T048, T060, T064, T069, T075, T076 |
| Docs & MCP | Set/26 (S4) | Docs compiler, Roslyn SG, MCP server live, OpenTelemetry | T011, T013, T021, T023, T027, T046, T053, T062, T065, T074 |
| DataGrid+AI | Out/26 (S5) | HxGrid, HxDialogBase, HxFilterForm, AI providers, Smart components | T015, T016, T033, T035, T068, T071, T072, T077 |
| Hardening | Nov/26 (S6) | AOT, CDN, EditorConfig, gRPC code-first, ship v2.0.0 | T024, T025, T026, T028, T030, T031, T032, T036, T061 |

## Cumulative Metrics
| Sprint | Tasks Done | Hours Done | Cumulative Coverage |
|--------|-----------:|-----------:|--------------------:|
| S1 | 11 | 45 | 6.25% |
| S2 | 22 | 135 | 18.75% |
| S3 | 34 | 225 | 31.25% |
| S4 | 44 | 335 | 46.53% |
| S5 | 52 | 375 | 52.08% |
| S6 | 61 | 425 | 59.03% |

## Release Strategy
- **v2.0.0-alpha**: After S3 (Ago/26) — internal dogfooding
- **v2.0.0-beta**: After S4 (Set/26) — public preview, MCP live
- **v2.0.0-rc**: After S5 (Out/26) — freeze API, perf tuning
- **v2.0.0 GA**: After S6 (Nov/26) — production ready

## 5-Way Source Distribution
| Origem | Tasks | % | 1st Task | Last Task |
|--------|------:|--:|----------|-----------|
| MudBlazor | 19 | 27.5% | T001 | T031 |
| antd-blazor | 12 | 17.4% | T004 | T028 |
| Blazorise | 16 | 23.2% | T044 | T053 |
| Radzen | 10 | 14.5% | T056 | T065 |
| **Havit ⭐ NEW** | **12** | **17.4%** | **T066** | **T077** |
| **TOTAL** | **69** | 100% | | |
