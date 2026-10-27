# Arquitetura — XForge DesignSystem

## Stack
| Camada | Tecnologia |
|--------|-----------|
| Framework | Blazor (.NET 10) |
| UI | Tailwind CSS v4 + CSS Variables (tokens) |
| Render | Server + WebAssembly + Hybrid |
| State | ComponentBase + CascadingValue |
| Mediator | XForge.MediatR (não MediatR oficial) |
| Mapping | AutoMapper (não Mapster) |
| Validation | FluentValidation |
| PDF | QuestPDF |
| Tests | bUnit + xUnit + Moq |
| Docs | Blazor Server (documentação interativa) |

## Diagrama

```
[App Blazor] → [XForge.DesignSystem Components]
                    │
                    ├── [Base] XForgeComponentBase (class, style, tokens)
                    ├── [Primitives] Typography, Icon, Badge, Skeleton
                    ├── [Actions] Button, FAB, SplitButton
                    ├── [Forms] EditForm, Select, Input*, DatePicker, Upload
                    ├── [DataDisplay] DataGrid, Table, TreeView, Chart, List
                    ├── [Layout] Grid, Stack, Shell, Sidebar, Rail
                    ├── [Navigation] Menu, Tabs, Breadcrumb, Steps
                    ├── [Overlays] Dialog, Drawer, Popover, Tooltip
                    ├── [Feedback] Toast, Alert, Progress, Chat
                    ├── [Utilities] Animate, DragDrop, BreakpointObserver
                    ├── [AI] Chat, Vision, Prompt, Voice, NFeAnalyzer
                    └── [CrossPlatform] DeviceBridge, GestureHandler, OfflineStorage
```

## Padrões
| Padrão | Onde usar | Arquivo referência |
|--------|----------|-------------------|
| Smart Enum | Cores, tamanhos, variantes | `XFButtonColor`, `XFExtendedSize` |
| Template Method | `GetComponentClass()` em todos os componentes | `XForgeComponentBase` |
| Cascading Value | EditForm context, Theme provider | `XFEditForm.razor` |
| RenderFragment | Composição de conteúdo | Todos os componentes |
| CSS Variables | Temas light/dark/brand | `tokens.css` |
| BOM-safe encoding | Todos os arquivos `.cs` | `UTF8Encoding($true)` |

## Estrutura de Pastas
```
src/
├── XForge.DesignSystem/          ← Componentes (155 .razor.cs)
│   ├── Components/
│   │   ├── Actions/              ← Button, FAB, SplitButton
│   │   ├── Forms/                ← EditForm, Select, Inputs, Upload
│   │   ├── DataDisplay/          ← DataGrid, Table, Chart, TreeView
│   │   ├── Layout/               ← Grid, Stack, Shell, Sidebar
│   │   ├── Navigation/           ← Menu, Tabs, Breadcrumb, Steps
│   │   ├── Overlays/             ← Dialog, Drawer, Popover, Tooltip
│   │   ├── Feedback/             ← Toast, Alert, Progress, Chat
│   │   ├── Utilities/            ← Animate, DragDrop, Breakpoint
│   │   ├── AI/                   ← Chat, Vision, Prompt, Voice
│   │   └── CrossPlatform/        ← DeviceBridge, GestureHandler
│   └── Core/                     ← Base classes, tokens, primitives
├── XForge.DesignSystem.Docs/      ← Documentação interativa (223 pages)
├── XForge.DesignSystem.Docs.AI/   ← AI component docs (69 pages)
├── XForge.DesignSystem.Tests/     ← Testes (347 files, ~6883 methods)
└── XForge.Docs.MCP/              ← MCP tools (6 servers)
```

## Componentes por Categoria
| Categoria | Quantidade | Complexidade |
|-----------|:----------:|:------------:|
| Actions | 3 | ⭐⭐ |
| Forms | 35+ | ⭐⭐⭐⭐⭐ |
| DataDisplay | 25+ | ⭐⭐⭐⭐ |
| Layout | 15+ | ⭐⭐⭐ |
| Navigation | 10+ | ⭐⭐⭐ |
| Overlays | 8+ | ⭐⭐⭐⭐ |
| Feedback | 10+ | ⭐⭐⭐ |
| Utilities | 10+ | ⭐⭐ |
| AI | 20+ | ⭐⭐⭐⭐ |
| CrossPlatform | 5 | ⭐⭐⭐ |
| Primitives | 15+ | ⭐⭐ |
