# Arquitetura -- DevExpress Blazor (Phase 9 / 9-way)

Gerado em: 2026-06-13
Source: https://www.devexpress.com/blazor/
Consolidation: 9-way

## Stack DevExpress (reference)
| Camada | Tecnologia |
|--------|-----------|
| Frontend | Blazor Server + WebAssembly (native C#) |
| Backend | .NET 8 / .NET 9 / .NET 10 |
| Banco | Any (SQL, JSON, Excel, EF Core, MongoDB, Object) |
| ORM | Entity Framework Core / XPO (DevExpress ORM) |
| Framework UI | DevExpress Blazor v25.2.x (70+ components) |
| Themes | 4 built-in Bootstrap (Blazing Berry, Blazing Dark, Office White, Purple) + 20+ Bootswatch |
| Size Modes | Small, Medium, Large |
| AI | AI Chat, HTML Editor with AI, Rich Text Editor with AI, Semantic Search |
| MCP Server | FREE Documentation MCP Server (300,000+ help topics) |
| IDE | VS 2026 / VS 2022 / VS Code / JetBrains Rider |
| Browser | Edge, Firefox, Chrome (Android), Safari (iOS) |
| Accessibility | Section 508 + WCAG claims |

## Diagrama
```
[Blazor Server] + [Blazor WASM]
       |
       v
[DevExpress Blazor v25.2.x]
       |
       v
[70+ Native Components: Grid, Pivot, Chart, Scheduler, RTE, Editors, Tabs, Form Layout, Accordion, TreeView, ...]
       |
       v
[Reporting: Designer + Viewer + BI Dashboard]
       |
       v
[XAF Security API: User/Role/Permission/Membership]
       |
       v
[Data Sources: SQL, JSON, Excel, EF Core, MongoDB, Object]
```

## Padroes ADOTADOS de DevExpress
| Padrao | Onde usar | Task |
|--------|----------|------|
| FormLayout Items (LayoutGroup/LayoutTabPages/TabPage) | Forms complexas enterprise | T143 |
| RichTextEditor DOCX/RTF/TXT | Documentos business | T144 |
| PivotTable (cross-tab) | Excel-like reports | T145 |
| TreeList (Tree + Grid hybrid) | Master-detail hierarchical | T146 |
| FilterBuilder (criteria expressions) | Data filtering UI | T147 |
| XAF Security API patterns | RBAC server-side | T156 |
| SearchBox (suggestions) | Input search | T148 |
| ColorPalette grid | Color picker | T149 |
| LayoutBreakpoint | Responsive design | T150 |
| FileInput (preview) | File upload | T158 |
| Splitter | Resizable panels | T161 |
| Drawer | Off-canvas nav | T162 |
| Tabs | Container | T163 |
| Carousel | Image slideshow | T151 |
| HTMLEditor | WYSIWYG HTML | T152 |
| Ribbon extended | Office toolbar | T153 |
| ReportViewer | End-user report view | T154 |
| BI Dashboard (v2.1) | BI integration | T155 |
| ReportDesigner (v2.1) | Ad-hoc report design | T157 |

## Componentes DevExpress Catalogo (70+)
### AI Integration (4)
- AI Chat, HTML Editor with AI, Rich Text Editor with AI, Semantic Search
### Data Management (4)
- Grid, TreeList, Pivot Table, Filter Builder
### Data Visualization (14)
- Area, Bar, Bar Gauge, Bubble, Candlestick, Donut, Line, Map, Pie, Polar/Radar, Range Selector, Sankey, Sparkline, Stock Chart
### Data Editors (17)
- Calendar, Checkbox, Color Palette, ComboBox, Date Edit, Date Range Picker, Dropdown Box, List Box, Masked Input, Memo, Radio, Radio Group, Search Box, Spin/Numeric Edit, Tag Box, Text Box, Time Edit
### Layout (8)
- Carousel, Drawer, Form Layout, Grid Layout, Layout Breakpoint, Splitter, Stack Layout, Tabs
### Navigation (6)
- Accordion, Context Menu, Menu, Ribbon, Toolbar, TreeView
### Dialogs & Windows (6)
- Dropdown, Flyout, Message Box, Popup, Toast, Window
### Utility Components (5)
- Button, Loading Panel, Pager, Progress Bar, Wait Indicator
### Buttons (3)
- Button, Button Group, DropDown Button, Split Button
### Analytics & Reporting (3)
- Report Viewer, Report Designer, BI Dashboard
### File Viewers & Editors (3)
- HTML Editor, PDF Viewer, Rich Text Editor
### File Management (2)
- File Input, Upload
### Scheduling (1)
- Scheduler (Day/Week/Work Week/Month/Timeline views)

## Top Clientes DevExpress (mercado)
- Fortune 500 enterprise
- Financial services
- Healthcare
- Manufacturing
- Government (Section 508)
- Mais de 1.5M developers usam DevExpress globalmente

## Awards & Recognition
- Visual Studio Magazine 2025 Readers Choice Awards (multi-categoria)
- ComponentSource Top 100 Publisher
- Visual Studio Marketplace high ratings

## Pricing (reference, nao adotado)
| Tier | Preco/ano/dev | Inclui |
|------|---------------|--------|
| ASP.NET & Blazor | $1,099.99 | ASP.NET + Blazor + DevExtreme |
| DXperience | $1,699.99 | + WinForms + WPF + VCL + Reporting |
| Universal | $2,299.99 | + .NET MAUI + XAF + Dashboard + Office File API |
| Renewal (~50% off) | $549.99 - $1,149.99 | Continua updates + support 12 meses |

## Licensa
- **Modelo**: Per-developer subscription, 12 meses, inclui updates + support
- **Renewal**: Opcional, ~50% preco novo
- **60-day money-back guarantee**
- **11+ licenses**: Contact sales direto
- **Redistribution**: Termos separados (docs.devexpress.com/Blazor/401758/)

## Integracao com XForge v2.0.0
| Aspecto | Como DevExpress informa | Como XForge v2.0.0 implementa |
|---------|-------------------------|-------------------------------|
| Form Engine | DxFormLayout (items, groups, tabs) | T143 XFFormLayout (24h) |
| RTE | DxRichEdit (DOCX/RTF/TXT) | T144 XFRichTextEditor (40h) |
| Pivot | DxPivotGrid (multi-dim analysis) | T108 PivotGrid (32h) + T145 PivotTable (32h) |
| Tree + Grid | DxTreeList (hybrid) | T146 XFTreeList (24h) |
| Filter | DxFilterBuilder (criteria) | T147 XFFilterBuilder (24h) |
| Security | XAF Security API | T156 XFXAFSecurityAPI (24h) |
| Reporting | DxReport + BI Dashboard | T105 (PDF/Word) + T154 Viewer (16h) + T155 BI v2.1 |
| MCP | Free MCP Server | T102 self-hosted (LGPD) |

## 9-way Cross-Reference
- XForge 152 core + 29 AI = 181 components (MIT, 0 USD/yr)
- DevExpress 70+ components ($1,099.99 - $2,299.99/yr per dev)
- Net-new from DevExpress: 19 tasks (~360h) = 11.8% do roadmap 9-way
- Maior unico: T144 RTE DOCX + T156 XAF Security (resolve CRIT-01)