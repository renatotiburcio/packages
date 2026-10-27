# Arquitetura -- Syncfusion Blazor (Phase 8)

## Stack XForge v2.0.0 (8-way)
| Camada | Tecnologia |
|--------|-----------|
| Frontend | Blazor Server + WebAssembly (.NET 10) |
| Backend | .NET 10, EF Core, Pomelo MySQL/PostgreSQL |
| UI Library | XForge v2.0.0 (8-way consolidated) |
| Themes | Fluent 2, Tailwind CSS v4, Bootstrap 5, Material 3 (4 temas) |
| AI | Microsoft.Extensions.AI + 11 providers |
| Doc Processing | PdfPig + PdfSharpCore + ClosedXML + DocumentFormat.OpenXml |
| Charts | ECharts 6.0 + Three.js (3D) |
| Tooling | Roslyn SG, XForge.AssetsGenerator, MCP Server, ThemeBuilder, Playground |

## Stack Syncfusion Original
| Camada | Syncfusion |
|--------|-----------|
| Components | 155+ Blazor (55+ Charts, 4 Gauges, 4 Office Editors) |
| Themes | Fluent 2 + Tailwind + Bootstrap 5 + Material 3 |
| AI | AI AssistView, Smart Paste/TextArea/RichTextEditor, MCP (2 tools), Code Studio, HelpBot |
| Doc | PDF Viewer, DOCX Editor, Spreadsheet Editor + .NET PDF/Word/Excel/PowerPoint libs |
| Playground | https://blazor.syncfusion.com/playground (Roslyn compile online) |
| Licensing | Commercial ($1,995/dev/year Essential Studio) + Community License (<$1M revenue, 5 devs) |
| Customers | Igniatek, McDaniel, Sistemi, Synchrony, Capita, Vialto |
| Downloads | 8.7M+ downloads, 4.5/5 (700+ G2 reviews) |

## Diagrama XForge v2.0.0 (8-way)

```
XForge v2.0.0
  +-- Core (MIT, 1 UI: Tailwind)
  |     +-- Foundation (T001-T077)
  |     +-- Icons+FluentUI (T078-T101)
  |     +-- Telerik Module (T102-T123) -- MCP, ThemeBuilder, Doc Proc, Charts, Diagrams
  |     +-- Syncfusion Module (T124-T142) -- BlockEditor, AIAssistView, MCP-AI, 3DCharts, A11y
  +-- AI (XFAI*)
  |     +-- 11 providers + Microsoft.Extensions.AI
  |     +-- Smart Components (T077, T077b, T125, T129)
  |     +-- AI AssistView (T125)
  +-- Tooling
  |     +-- MCP Server (T102, 10 tools + T126/T127 NL/codegen)
  |     +-- ThemeBuilder (T103)
  |     +-- Playground (T104)
  |     +-- Figma Sync (T122, T142)
  +-- Themes (Fluent2 + Tailwind + Bootstrap5 + Material3)
  +-- Doc Processing (T105, T107, T128 FileManager)
  +-- Charts+Visualization (T112 + T130 3D + T131 Sankey + T135 Range + T136 Bullet + T137 TreeMap)
  +-- A11y (T140 OTP, T141 Speech)
```

## Padroes
| Padrao | Onde usar |
|--------|-----------|
| MCP Server | T102 (10 tools), T126 (Agentic UI Builder), T127 (AI Coding Assistant) |
| Microsoft.Extensions.AI | T129 (wrapper), T125 (AIAssistView) |
| Notion-like Block Editor | T124 |
| 3D Charts | T130 (Three.js backend) |
| Query Builder Visual SQL | T134 |
| File Manager | T128 (browse/upload/delete/preview) |

## Componentes net-new 8-way (Syncfusion)
| Componente | Responsabilidade | Tasks |
|-----------|-----------------|-------|
| XFBlockEditor | Notion-like editing | T124 |
| XFAIAssistView | Full AI chat streaming | T125 |
| XFAIChatService | Microsoft.Extensions.AI wrapper | T129 |
| XF3DCharts | 3D visualization | T130 |
| XFSankeyDiagram | Flow visualization | T131 |
| XFRibbon | Office toolbar | T132 |
| XFStepper | Wizard nav | T133 |
| XFQueryBuilder | Visual SQL | T134 |
| XFFileManager | File browser | T128 |
| XFRangeSelector | Chart range | T135 |
| XFBulletChart | KPI chart | T136 |
| XFTreeMap | Hierarchy viz | T137 |
| XFTimeline | Vertical timeline | T138 |
| XFDashboardLayout | Dashboard grid | T139 |
| XFOTPInput | OTP input | T140 |
| XFSpeechToText | Speech recognition | T141 |
| XFFigmaNativeSync | Full Figma bidirectional | T142 |
