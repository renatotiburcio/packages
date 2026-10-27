# Roadmap — Ant Design Blazor Adoption (consolidado MudBlazor + antd-blazor)

## Timeline
```
Mes 1 (Jun-Jul)      Mes 2 (Jul)         Mes 3 (Jul-Ago)     Mes 4 (Ago)         Mes 5-6 (Ago-Set)
+- Sprint 1 ---------+- Sprint 2 ---------+- Sprint 3 ---------+- Sprint 4 ---------+- Sprint 5-6 -----+
|  Foundation        |  Base + i18n       |  Tests + Docs      |  Release + AI     |  Hybrid + Pro    |
|  T001-T018         |  T005-T027         |  T011-T029         |  T025-T031         |  T040-T043       |
|  ~52h              |  ~46h              |  ~62h              |  ~52h              |  ~120h           |
+--------------------+--------------------+--------------------+--------------------+------------------+
```

## Fases
| Fase | Periodo | Entregavel | Valor | Origem |
|------|---------|-----------|-------|--------|
| Foundation | Jun-Jul 2026 | ParameterState, Analyzers, Bun, DomEvents, CssBuilder | Arquitetura robusta | MudBlazor + antd-blazor |
| Base + i18n | Jul 2026 | Base classes, Localized XML, Multi-TFM, CallAfterRender | i18n + UX | combinado |
| Tests + Docs | Jul-Ago 2026 | Test infra, Docs compiler, MCP server | Quality + AI | MudBlazor + antd-blazor |
| Release | Ago 2026 | Release cadence, DraftMonitor, Localization | Distribuicao pro | combinado |
| Hybrid + Pro | Ago-Set 2026 | MAUI/WPF/WinForms, Pro templates | Mercados adjacentes | antd-blazor |

## Comparacao das 3 libs (referencia)
| Aspecto | MudBlazor 9.x | antd-blazor 1.6.x | XForge atual | XForge alvo |
|---------|---------------|-------------------|--------------|-------------|
| Stars | 10.4k | 6.2k | n/a | n/a |
| Latest version | v9.5.0 (May 2026) | v1.6.1 (Apr 2026) | v1.0.0 | v2.0.0 |
| Components | 85+ | 70+ | 152 | 152+ |
| Solutions projects | 17 | 14 | 5 | 17+ |
| Test infra | 5 projects | 4 projects (TestApp/TestKit/Tests/Tests.Js) | 2 (parcial) | Full pattern |
| Analyzers | 1 lib (4 diagnostics) | 0 (uses NetAnalyzers only) | 0 | 1 lib |
| Asset pipeline | Bun (TS+SCSS) | TypeScript+rollup | Manual | Bun |
| Multi-TFM | net8/9/10 | netstandard2.1 + net5-10 (7 TFMs!) | net10.0 only | net8/9/10 |
| Trimmable | yes (TrimMode=link) | no | no | yes |
| Localized XML docs | no (single en) | YES (6 langs: en/ja/ko/ru/zh-CN/HK/TW) | no | YES (6 langs) |
| DomEventService | no (EventCallback) | YES (centralized) | no (EventCallback) | YES |
| MCP docs server | no | YES (PR #4758, Feb 2026) | no | YES |
| JSON locales | no (resx) | YES (70+ files) | no (resx) | YES (JSON) |
| OneOf union types | no | YES | no | YES |
| Hybrid (MAUI/WPF) | no | YES | no | YES (futuro) |
| Generated docs tests | yes (Roslyn) | no (CLI build tool) | no (manual) | yes (Roslyn) |
| Release cadence | frequent | weekly patch / monthly minor | n/a | weekly patch |

## Insights Exclusivos do antd-blazor (NOVOS)
1. **Localized Intellisense** (multi-language XML docs in package)
2. **DomEventService** (centralized DOM event subscription via DI)
3. **MCP server** (AI agents query docs via Model Context Protocol)
4. **OneOf** (union types para param flexiveis)
5. **DraftMonitor** (form draft state management component)
6. **CallAfterRender queue** (deferred JS interop calls)
7. **ForwardRef pattern** (expose internal DOM ref)
8. **AntDesign.targets** (build targets shipped to consumers)
9. **JSON locales** (not resx)
10. **7 TFMs** (netstandard2.1 + net5..10) - maximo coverage

## Marcos de Sucesso
- [ ] M1: 100% componentes migrados para ParameterState<T> + DomEventService
- [ ] M2: Analyzers publicam como NuGet com >=3 diagnostics
- [ ] M3: Docs em 6 idiomas (en/pt-BR/es/zh/ja/ru) com localized XML
- [ ] M4: MCP server funcional com >=10 components documentados
- [ ] M5: Package com TrimMode=link, snupkg, IsTrimmable sem warnings
- [ ] M6: DraftMonitor publicado + hybrid target working
