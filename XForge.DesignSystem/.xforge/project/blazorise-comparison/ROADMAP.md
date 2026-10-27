# Roadmap — Blazorise vs XForge (consolidado 3-way)

## Timeline
```
Mês 1        Mês 2        Mês 3        Mês 4        Mês 5        Mês 6
├─ Sprint 1 ─┼─ Sprint 2 ─┼─ Sprint 3 ─┼─ Sprint 4 ─┼─ Sprint 5 ─┼─ Sprint 6 ─┤
│ Fundação   │   Core     │  Testes     │  Docs      │  Multi-TFM │  Hardening │
│ T001-T045  │  T004-T049 │  T020-T054  │  T021-T053 │  T015-T050 │  T024-T055 │
│ 6 tasks    │  6 tasks   │  5 tasks    │  4 tasks   │  6 tasks   │  7 tasks   │
└─────────────┴────────────┴────────────┴────────────┴────────────┴────────────┘
```

## Fases
| Fase | Período | Sprints | Entregável principal |
|------|---------|---------|----------------------|
| Fundação | Mês 1 (Jun 27) | S1 | ParameterState + Provider-agnostic + Analyzers + JSON locales |
| Core | Mês 2 (Jul 11) | S2 | DomEventService + Localized + TClasses/TStyles + Fody |
| Testes | Mês 3 (Jul 25) | S3 | xUnit/bUnit + E2E + Stryker ≥85% |
| Docs | Mês 4 (Ago 8) | S4 | Docs.Compiler + Snippets + Docs.Mcp + axe-core |
| Multi-TFM | Mês 5 (Ago 22) | S5 | net8/9/10 + 6 langs + Extensions strategy |
| Hardening | Mês 6 (Set 5) | S6 | Benchmark + Virtual scrolling + Beta v2.0.0 |

## Marcos de Decisão (gates)
| Gate | Quando | Critério | Se reprovado |
|------|--------|----------|--------------|
| G1 | Fim S1 | Build clean + 0 SH errors | Estender S1 |
| G2 | Fim S2 | Sample apps rodam em net8/9/10 | Estender S2 |
| G3 | Fim S3 | Coverage ≥ 85% + 0 falhas E2E | Estender S3 |
| G4 | Fim S4 | MCP server responde 6 tools | Estender S4 |
| G5 | Fim S5 | NuGet package validado | Estender S5 |
| G6 | Fim S6 | Beta v2.0.0 published | Estender S6 |

## Comparação de Libs (decisão arquitetural)
| Aspecto | XForge atual | MudBlazor | antd-blazor | **Blazorise** |
|---------|-------------:|----------:|------------:|--------------:|
| Components core | 152 | 85+ | 70+ | **192** |
| UI providers (CSS frameworks) | 1 (Tailwind v4) | 1 (Material) | 1 (Ant) | **7** ⭐ |
| Extensions | 0 | 12 | 14 | **38** ⭐ |
| Icon packages | 1 (FontAwesome Pro) | 0 | 1 | **8** ⭐ |
| Test projects | 2 | 5 | 4 | **6** ⭐ |
| Docs projects | 1 (manual) | 1 (Mud UnitTests.Viewer) | 5 (incl. MCP) | **5** (incl. MCP) ⭐ |
| Source generators | 0 | 1 (4 diag) | 0 | **4** (Generator+Weavers) ⭐ |
| Multi-TFM | 1 (net10) | 3 (net8/9/10) | 7 (max) | **3** (net8/9/10) |
| AI components | **29** ⭐ | 0 | 0 | 0 |
| Doc languages | pt-BR | en | 6 | en |
| MCP server | no | no | yes (PR) | **YES (live!)** ⭐ |
| License | MIT | MIT | MIT | **Dual** ⚠️ |
| Analyzers | 0 | 1 (4 diag) | 0 | 1 (Blazorise.Analyzers) |
| Fody weaver | no | no | no | **YES** ⭐ |
| Provider abstraction | no | no | no | **YES (IClassProvider)** ⭐ |

## Decisão Estratégica
**Adotar do Blazorise** (P0/P1):
- T044 Provider-agnostic abstraction (IClassProvider/IStyleProvider)
- T045 ShouldAutoGenerateId em base
- T021 Docs.Compiler com Microsoft.CodeAnalysis
- T046 Snippets.generated.cs pattern
- T047 BaseComponent<TClasses,TStyles>
- T048 FluentValidation integration
- T049 Fody Weaver pattern (decidir: Fody real ou Roslyn SG?)
- T050 Strategy para 38 Extensions (escolher 5-10 para v2.0.0)
- T051 Blazorise.Licensing pattern (decidir: open core vs commercial)
- T052 Centralized package versions
- T053 Docs.Mcp server (6 tools)
- T054 Analyzers tests
- T055 Benchmark project

**NÃO adotar**:
- Multi-provider CSS (7 frameworks) → YAGNI para v2.0.0; reavaliar v3
- Dual-license → XForge é MIT puro
- 38 Extensions → cherry-pick 5-10
