# Roadmap — MudBlazor Comparison & Adoption

## Timeline
```
Mês 1 (Jun-Jul)     Mês 2 (Jul)         Mês 3 (Jul-Ago)     Mês 4 (Ago)
├─ Sprint 1 ────────┼─ Sprint 2 ────────┼─ Sprint 3 ────────┼─ Sprint 4-5 ─┤
│  Fundação         │  Base classes     │  Tests + Docs     │  Release    │
│  T001-T013        │  T011-T023        │  T020-T026        │  T025-T033  │
│  ~32h             │  ~22h             │  ~42h             │  ~52h       │
└───────────────────┴───────────────────┴───────────────────┴─────────────┘
```

## Fases
| Fase | Período | Entregável | Valor |
|------|---------|-----------|-------|
| Fundação | Jun-Jul 2026 | ParameterState, Analyzers, Bun, CssBuilder | Arquitetura robusta |
| Base classes | Jul 2026 | MudBaseInput/Button, ComponentBase, multi-TFM | Hierarquia sólida |
| Tests + Docs | Jul-Ago 2026 | UnitTests/Viewer pattern, Docs Compiler | Quality automation |
| Release | Ago 2026 | Snupkg, Migration, Localization | Distribuição pro |
| Docsite | Ago 2026 | Playground, Contribution docs | Developer experience |

## Comparação com MudBlazor (referência)
| Aspecto | MudBlazor 9.x | XForge atual | XForge alvo |
|---------|---------------|--------------|-------------|
| Stars | 10.4k | n/a | n/a (uso interno) |
| Components | 85+ | 152 | 152+ |
| Projects in slnx | 17 | 5 | 17+ |
| Test infra | bUnit + Viewer + Docs.Generator | xUnit+bUnit (parcial) | Full pattern |
| Analyzers | 1 lib (4 diagnostics) | 0 | 1 lib |
| Asset pipeline | Bun (TS+SCSS) | Manual | Bun |
| Multi-TFM | net8/9/10 | net10.0 only | net8/9/10 |
| Trimmable | yes (TrimMode=link) | no | yes |
| Generated docs tests | yes (Roslyn) | no (manual DocsQualityTests) | yes |
| Resources localization | yes (IsAsConstants) | no | yes |
| Release cadence | 153 releases | n/a | semver stable |

## Marcos de Sucesso
- [ ] M1: 100% componentes migrados para ParameterState<T>
- [ ] M2: Analyzers publicam como NuGet com ≥3 diagnostics
- [ ] M3: Docs Compiler gera ≥1 test por exemplo automaticamente
- [ ] M4: Package com TrimMode=link, snupkg, IsTrimmable sem warnings
- [ ] M5: Playground online (try.xforge.com) funcional
