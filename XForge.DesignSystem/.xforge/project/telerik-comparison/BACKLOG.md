# Backlog — Telerik UI for Blazor vs XForge Design System (7-way)

Gerado em: 2026-06-13
Fontes analisadas: 7 (MudBlazor + antd-blazor + Blazorise + Radzen + Havit + FluentUI + Telerik)
Foco: Telerik 120+ components + AI/MCP agents + Document Processing + Design tooling

## P0 — Critico (fazer primeiro)
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T102 | XForge MCP Server (10 tools) para LLMs/Agentes | 24h | nenhuma |
| T103 | ThemeBuilder Web App + CLI (CSS variables generator) | 32h | nenhuma |
| T104 | XForge Playground (REPL online, browser-based) | 24h | nenhuma |
| T105 | XForge DocumentProcessing: PDF/Word libs (read+edit) | 40h | nenhuma |
| T106 | XFFullSpreadsheet component (Excel-like + formulas) | 48h | nenhuma |

## P1 — Importante
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T107 | XForge DocumentProcessing: SpreadProcessing + Zip | 24h | T105 |
| T108 | XFPivotGrid (OLAP multi-dim analysis) | 32h | T106 |
| T109 | XFTaskBoard (Kanban drag/drop) | 16h | T010, T020 |
| T110 | XFDiagram (nodes/edges visual workflow editor) | 40h | nenhuma |
| T111 | XFGauges (4 types: Arc/Circular/Linear/Radial) | 16h | nenhuma |
| T112 | XFCharts wrapper (24+ types via ECharts 6.0) | 32h | nenhuma |
| T113 | XFMap (geolocation + markers + routes) | 32h | nenhuma |

## P2 — Desejavel
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T114 | XFQRCode generator | 6h | T078 |
| T115 | XFBarcode generator (1D + 2D) | 8h | T114 |
| T116 | XFSignature pad (handwritten signature capture) | 12h | nenhuma |
| T117 | XFDockManager (docking windows, IDE-like) | 32h | nenhuma |
| T118 | XFTileLayout + XFStackLayout + XFGridLayout | 20h | nenhuma |
| T119 | XFAnimationContainer (entrance/exit CSS animations) | 8h | T002 |

## P3 — Baixa prioridade
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T120 | XFPageTemplates (pre-designed page templates) | 16h | T001, T002 |
| T121 | XFFloatingActionButton (Material FAB pattern) | 4h | nenhuma |
| T122 | Figma Tokens Sync (push/pull design tokens) | 16h | T103 |
| T123 | XFFigma Bridge (live Figma embed in docs) | 12h | T122 |

## Resumo
- **Total: 22 tarefas** (P0: 5, P1: 7, P2: 6, P3: 4)
- **Estimativa total: ~440h** (P0: 168h, P1: 192h, P2: 86h, P3: 48h)
- **Tarefas sem dependencia**: 8 (T102-T106, T110, T111, T116, T117, T121)
- **Categorias**: AI/Agents (3), Design Tooling (3), Document Processing (4), Spreadsheet/OLAP (2), Workflow (2), Visualization (3), Data (1), Layout (3), Utility (3), Templates (1)

## Foco Especial: Telerik-Inspired Patterns

### Padroes Telerik ADOTADOS (T102-T123)
| Padrao | Origem Telerik | Beneficio XForge |
|--------|---------------|------------------|
| MCP Servers | AI Tools (1st feature) | Agent-first development |
| ThemeBuilder web | Design system tools | 4+ temas profissionais |
| REPL Playground | Telerik REPL | Marketing + dev productivity |
| Document Processing | 5 libs separadas | ERP LATAM (PDF/Word contracts) |
| Spreadsheet | Telerik Sheet | Viral, Excel-like, 48h MVP |
| PivotGrid | OLAP analysis | BI dashboards |
| TaskBoard (Kanban) | Project mgmt | Workflow visual |
| Diagram | Workflow editor | Process modeling |
| Charts/Gauges/Map | 24+ chart + 4 gauge + map | Dashboard suite |
| QR/Barcode/Signature | Utilities | Document mgmt |
| DockManager | IDE-like UI | Power user tools |
| Figma Sync | Design system sync | Designer integration |

### Padroes Telerik DEFERRED
| Padrao | Motivo |
|--------|--------|
| Reporting Integration | Baixa demanda LATAM, defer para v2.1 |
| WebMCP | Spec W3C emergente, aguardar amadurecimento |
| Cloud-only MCP | LGPD, manter self-hosted |
| Blazor Hybrid | Out of scope v2.0 (desktop) |