# XForge DesignSystem — Complete Knowledge Base

> Source: info/ folder analysis (2026-06-13)
> Absorbed from: AGENTS.md, CLAUDE.md, DECISIONS.md, BACKLOG.md, CAPABILITY-MATRIX.md, DESIGNSYSTEM_ARCHITECTURE.md, DOCS-RAZOR-PAGE-STANDARD.md, REVIEW_GATES.md, ia01.md, ICON-INVENTORY.md, ADR-047, API-CONTRACTS.md

## 1. Project Identity

| Property | Value |
|---|---|
| **Name** | XForge Design System |
| **Version** | 1.0.0 |
| **Type** | Blazor Component Library (RCL) + AI Extensions |
| **Framework** | .NET 10.0 |
| **License** | MIT (Core) / Commercial (AI) |

## 2. Product Ecosystem (ADR-047)

### OSS (MIT)
- `XForge.DesignSystem.Shared` — Ecosystem boundaries
- `XForge.DesignSystem` — RCL with ~100 Blazor components
- `XForge.DesignSystem.Docs.Shared` — Shared docs shell
- `XForge.DesignSystem.Docs` — Blazor WASM app (Core docs)
- `XForge.DesignSystem.Playground` — Interactive playground
- `XForge.DesignSystem.SampleBasic/Intermediary/Advanced` — Sample apps

### Commercial (AI)
- `XForge.DesignSystem.AI` — RCL with ~52 AI components
- `XForge.DesignSystem.AI.Tests` — bUnit tests
- `XForge.DesignSystem.Docs.AI` — Blazor WASM app (AI docs)

### Dependency Direction
- Core does NOT reference AI
- AI packages MAY reference core/shared packages
- Shared has no product-specific dependencies

## 3. Component Architecture

### Inheritance Hierarchy
```
ComponentBase
  → XForgeComponentBase (Id, Class, Style, Disabled, AdditionalAttributes)
    → XFInputComponentBase (form integration)
      → [Specific Component]
```

### File Pattern (3-file convention)
- `{Name}.razor` — Template markup
- `{Name}.razor.cs` — Code-behind (partial class)
- `{Name}.types.cs` — Enums and type definitions

### Design Patterns (12 observed)
1. Base Class Pattern — All inherit XForgeComponentBase
2. Types File Pattern — Enums in .types.cs
3. Service Pattern — Overlays use DI services
4. Provider Pattern — XFDialogProvider, XFDrawerProvider, XFToastProvider
5. ChildContent Pattern — RenderFragment named slots
6. EditForm Integration — EditContext, ValueExpression support
7. JS Interop — 25 ES modules (xf-*.js)
8. Tailwind-First — No isolated CSS
9. data-theme — Dark/light via XFAppShell
10. data-brand — Multi-brand via XFAppShell
11. Disposable Pattern — IAsyncDisposable for JS interop
12. Event Naming — On{Action} convention

### Services (4 core)
- BrandMetadataService — Brand color/icon/title, CSS variables
- NavigationStateService — Rail navigation, localStorage persistence
- XFDrawerService — Drawer programmatic API
- XFToastService — Toast programmatic API

## 4. Golden Rules (Inviolable)

1. **SRP** — Each file, component, page has ONE responsibility
2. **Never set [Parameter] directly** — Use CurrentValue + ValueChanged.InvokeAsync()
3. **Tailwind-first** — No isolated CSS without justification
4. **WCAG AAA** — aria-checked, aria-describedby, touch 44px, focus 40%
5. **Dark/light + data-theme** — Every component supports both
6. **Docs with rendered examples** — Real, copyable, didactic
7. **Real examples with own component** — NEVER generic HTML, NEVER ellipsis
8. **Tests >= 85%** — Minimum coverage for enterprise components

## 5. Shell / Rail / Sidebar Architecture

```
AppShell
 ├── Rail          → represents apps (ERP modules)
 ├── Sidebar       → belongs to active app
 ├── AppBar
 ├── ContentArea
 └── Footer
```

### Rail Active State Pattern
- Uses CurrentBrand string comparison (not Item.IsActive property)
- Subscribes to both ActiveItemChanged and BrandChanged events
- Active style: text-primary-contrast bg-primary-contrast/15 ring-1 ring-primary-contrast/20

## 6. Review Gates

- **Architecture**: SRP, SOLID, no business rule, no duplicates
- **API**: All parameters, events, methods documented
- **UX**: Enterprise-friendly, consistent, predictable
- **Accessibility**: ARIA, keyboard, focus, screen reader, contrast
- **Performance**: Minimal re-render, virtualization, stable CSS
- **Docs**: Overview, examples, code copy, API explorer, playground
- **Tests**: Unit, rendering, interaction, accessibility, snapshot, regression

## 7. Anti-Patterns Catalog (9 entries)

1. Accent in component name (non-ASCII)
2. Non-existent enum value referenced
3. Double-quote escaping in RenderFragment
4. overflow-y-auto + anchor nav (breaks layout)
5. Missing preventDefault on anchor clicks
6. Non-existent icon name
7. Missing accents in PT-BR text
8. Missing T() helper for bilingual text
9. Generic HTML instead of XForge component

## 8. Pipelines

### Component Pipeline
```
Memory Retrieve → Current Inventory → Reference Analysis → Capability Matrix →
API Contract → UX/UI Contract → Accessibility Contract → Performance Contract →
Implementation Plan → Docs Plan → Test Plan → Review → Memory Update
```

### Docs Pipeline
```
Memory Retrieve → Docs Architecture Check → Page Contract → Live Preview →
Code Renderer → API Explorer → Playground → Accessibility Section →
Performance Section → Examples → Review → Memory Update
```

### Gap Analysis Pipeline
```
/designsystem-analyze-current → /reference-analyze-all → /designsystem-gap-analysis →
/designsystem-backlog → /designsystem-sprint-plan → /component-roadmap →
/designsystem-changelog-update → /designsystem-memory-update → /review-designsystem
```
