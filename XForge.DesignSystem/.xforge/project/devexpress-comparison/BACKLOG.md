# Backlog -- DevExpress Blazor (9-way Consolidation)

Gerado em: 2026-06-13
Fontes analisadas: 9 (MudBlazor + antd + Blazorise + Radzen + Havit + FluentUI + Telerik + Syncfusion + DevExpress)
Source: https://www.devexpress.com/blazor/
Consolidation: 9-way

## P0 -- Critico (fazer primeiro)
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T143 | XFFormLayout Engine (mature form with templates/columns/tabs) | 24h | T001 |
| T144 | XFRichTextEditor DOCX/RTF/TXT (mature word processing) | 40h | T002 |
| T145 | XFPivotTable OLAP (Excel-like cross-tab) | 32h | T108 |
| T146 | XFTreeList hybrid (Tree + Grid combined) | 24h | T010 |
| T147 | XFFilterBuilder visual (criteria builder) | 24h | T134 |
| T156 | XFXAFSecurityAPI (role-based Security API) | 24h | T001 |

## P1 -- Importante
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T148 | XFSearchBox (search editor with suggestions) | 8h | T001 |
| T149 | XFColorPalette (color picker grid) | 12h | T001 |
| T150 | XFLayoutBreakpoint (responsive breakpoints) | 8h | T002 |
| T158 | XFFileInput (file selection with preview) | 8h | T001 |
| T161 | XFSplitter (resizable split panels) | 8h | T001 |
| T162 | XFDrawer (off-canvas navigation drawer) | 12h | T001 |
| T163 | XFTabs (basic tabbed container) | 8h | T001 |

## P2 -- Desejavel
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T151 | XFCarousel (slideshow/image carousel) | 8h | T001 |
| T152 | XFHTMLEditor (WYSIWYG HTML editor) | 24h | T144 |
| T153 | XFRibbonDE (extend T132 with DevExpress features) | 12h | T132 |
| T154 | XFReportViewer (end-user report viewer) | 16h | T105 |

## P3 -- Baixa prioridade
| ID | Titulo | Estimativa | Dependencias |
|----|--------|-----------|--------------|
| T155 | XFBIDashboard (BI Dashboard integration) | 32h | T154,T112 |
| T157 | XFReportDesigner (full ad-hoc Report Designer) | 40h | T105 |

## Resumo
- Total: 19 tarefas (P0: 6, P1: 7, P2: 4, P3: 2)
- Estimativa total: ~360h
- Tarefas sem dependencia: 0 (todas dependem de T001/T002 ou tasks ja existentes)
- Origem: 9 fontes (novas 19 tarefas deste source: DevExpress)
- 9-way total consolidado: 161 tarefas (142 de Phase 8 + 19 deste Phase 9)
- 9-way horas totais: ~2392h (~10 meses / 14 sprints)