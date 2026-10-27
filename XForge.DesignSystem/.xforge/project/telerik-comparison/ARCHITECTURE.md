# Arquitetura — Telerik vs XForge (7-way consolidation)

## Stack (XForge + Telerik-influenced)
| Camada | Tecnologia | Inspiracao |
|--------|-----------|------------|
| Frontend | .NET 10 + Blazor WASM + Tailwind v4 | Telerik (.NET 9/10) |
| Backend | .NET 10 Minimal API + XForge.MediatR | Telerik (Blazor) |
| Banco | PostgreSQL (Pomelo) | Telerik (agnostic) |
| AI | 11 providers + MCP servers | Telerik (AI tools) |
| Document Processing | QuestPDF (gerar) + Telerik-style libs (ler/editar) | Telerik (T105) |
| Reporting | Telerik Reporting (deferred) | Telerik |
| Themes | 4+ temas profissionais (Telerik approach) | Telerik (T103) |
| Design Tooling | ThemeBuilder web + Figma sync | Telerik (T103, T122) |
| Infra | Docker + self-hosted (cloud-agnostic) | Telerik |

## Telerik 120+ Components — Mapeamento para XForge

### AI/Agents (4 Telerik)
- AI Prompt, Inline AI Prompt, PromptBox, SmartPasteButton, Chat
- XForge ja tem T060 (AIChatService) + T077 (HxSmart*)
- **ACAO**: Adotar Telerik SmartPasteButton (clipboard→form auto-fill) como T077b

### Data Management (8 Telerik)
- DataGrid, Filter, ListView, Pager, TreeList (XForge ja tem)
- **PivotGrid** → T108 (NOVO)
- **Spreadsheet** → T106 (NOVO)
- **TaskBoard** → T109 (NOVO)

### File Upload (5 Telerik)
- DropZone, File Manager, FileSelect, PDF Viewer, Upload
- XForge ja tem T027 basico → upgrade

### Diagrams and Maps (2 Telerik)
- **Diagram** → T110 (NOVO)
- **Map** → T113 (NOVO)

### Data Visualization (24+ Telerik)
- **Charts wrapper** → T112 (XForge ja usa ECharts 6.0)
- **QR Code** → T114
- **Barcode** → T115

### Editors (24 Telerik)
- 18+ ja cobertos em XForge
- **Novos**: DropDownTree, MultiColumnComboBox, Signature → T116

### Scheduling (3 Telerik)
- Calendar, Gantt, Scheduler (XForge ja tem todos)

### Layout (15 Telerik)
- 9+ ja cobertos
- **Novos**: AnimationContainer, DockManager, GridLayout, StackLayout, TileLayout → T117, T118, T119

### Gauges (4 Telerik)
- **Gauges** → T111 (NOVO)

### Interactivity (15 Telerik)
- 12+ ja cobertos
- **Novos**: ChunkProgressBar, ValidationTooltip → upgrade

### Document Processing (5 Telerik) — NOVA CATEGORIA
- PDFProcessing, SpreadProcessing, SpreadStreamProcessing, WordsProcessing, ZipLibrary
- **T105, T107** (net-new)

### Productivity (Telerik)
- VS Code extension (XForge ja tem T046)
- **Telerik REPL** → T104 (NOVO)

### Reporting (Telerik)
- **Reporting Integration** → DEFERRED (baixa demanda LATAM)

## Diagrama
```
[XForge.Playground (T104)]
       | live preview
       v
[XForge MCP Server (T102)] <-- [LLM Agents (Claude, GPT, Gemini)]
       | tools: catalog, samples, types, snippets, theme
       v
[XForge.DesignSystem.Engine]
   |- Icons (T078) + Emoji (T079)
   |- Charts (T112) + Gauges (T111) + Diagram (T110)
   |- Doc Processing (T105, T107) + Spreadsheet (T106)
   |- Layout (T117-T119) + Templates (T120)

[ThemeBuilder CLI (T103)] <--> [Figma (T122, T123)]
       | CSS variables
       v
[XForge 4+ Themes]
```

## Padroes (7-way)
| Pattern | Origem | Adotado em |
|---------|--------|------------|
| MCP Server | Telerik + Havit | T102 |
| ThemeBuilder | Telerik | T103 |
| REPL/Playground | Telerik | T104 |
| Document Processing | Telerik | T105, T107 |
| PivotGrid | Telerik | T108 |
| AI Components | Telerik + Havit | T060, T077 |
| Color Tokens | FluentUI | T083 |
| Figma Sync | Telerik | T122, T123 |
| AI Prompt/SmartPaste | Telerik | T077b (proposed) |