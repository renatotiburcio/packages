# Frontend Standards — Design Tokens, Blazor Patterns, UI/UX Rules

## Design Token Enums (15 enums)
XFSizes, XFPositions, XFVariants, XFColors, XFDirection, XFOrientation, XFAlignment, XFJustify, XFOverflow, XFFit, XFWeight, XFTextTransform, XFTextDecoration, XFCursor, XFBreakpoint

## Component Base Classes
1. XForgeComponentBase — Id (auto xf-{8char-guid}), Class, Style, Disabled, AdditionalAttributes, GetComponentClass()
2. XFInputComponentBase — extends ComponentBase + EditContext, validation, JS interop focus
3. XFCheckboxBase — three-state checkbox logic

## 3-File Pattern
Every component has: ComponentName.razor + ComponentName.razor.cs + ComponentName.types.cs

## Design Token Architecture
- All visual values driven by enum-backed parameters
- Enums map to Tailwind CSS utility classes
- No magic numbers in component markup
- Theme switching via data-theme attribute (dark/light)
- Brand color via BrandMetadataService

## Blazor-Specific Rules
- NUNCA setar [Parameter] diretamente dentro do componente
- Sempre usar CurrentValue + ValueChanged.InvokeAsync() para two-way binding
- JS interop deve ter try/catch para InvalidOperationException (prerender)
- JSDisconnectedException deve ser tratado em Blazor Server
- @onclick:preventDefault para navegacao customizada (evitar Blazor Router)

## Responsive Design
- Tailwind breakpoint system (sm, md, lg, xl, 2xl)
- Touch target minimum 44px (WCAG AAA)
- Focus indicator minimum 40% contrast ratio
- Overflow handling for navigation components (Rail, Sidebar)

## Accessibility Requirements
- aria-checked, aria-describedby, aria-invalid obrigatórios
- Keyboard navigation (Tab, Enter, Escape, Space, Arrow keys)
- support screen reader via semantic HTML + ARIA roles
- Focus trap for modals (XFDialog, XFDrawer)
- role=" switch\ for toggle components

## Shell/Layout Standards
- AppShell: Rail | Sidebar | AppBar | ContentArea | Footer
- Rail items represent apps (max ~10)
- Sidebar belongs to active app
- Same shell shared between XForge.Docs and ERP app

## TypeScript/ES Modules (25 modules)
- xf-animate.js, xf-barcode.js, xf-captcha.js, xf-chart.js, xf-color-picker.js
- xf-data-grid.js, xf-date-picker.js, xf-dialog.js, xf-drag-drop.js, xf-drawer.js
- xf-file-input.js, xf-gantt.js, xf-map.js, xf-popover.js, xf-rich-text-editor.js
- xf-sankey.js, xf-scheduler.js, xf-signature-pad.js, xf-sortable.js, xf-sparkline.js
- xf-speech-to-text.js, xf-tabs.js, xf-time-picker.js, xf-tooltip.js, xf-treeview.js

## SVG Icon Standards
- 3698 Font Awesome Pro 7.1.0 SVGs
- Nomes em kebab-case (ex: floppy-disk, nao save)
- Uso: XFSvgIcons.MarkupWithSize(\nome\, 16)
- NUNCA SVG inline nos componentes ou docs
- Tamanho padrao: 16px (h-4 w-4) para icones internos

## Package Configuration
- SDK: Microsoft.NET.Sdk.Razor
- Target: net10.0
- MIT License for Core
- Commercial License for AI
- Static Web Assets enabled
- NuGet Pack enabled
