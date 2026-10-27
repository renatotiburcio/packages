# XForge DesignSystem — Documentation Standards

## `*Docs.razor` Page Standard
Canonical reference: `XForge.Docs/Components/Pages/EditFormDocs.razor`

## Required Sections (17)
1. Overview
2. Problem solved
3. When to use / When not to use
4. Anatomy
5. Variants
6. States (default, hover, focus, active, disabled, loading, error, success, empty)
7. API (parameters, events, methods)
8. Accessibility (ARIA, labels, focus, screen reader)
9. Keyboard support
10. Responsiveness
11. Performance
12. Enterprise examples
13. Anti-patterns
14. Playground
15. Design Tokens
16. Related Components
17. Changelog

## Page Architecture
```razor
<main id="main-content" class="mx-auto grid max-w-6xl gap-6 px-6 py-10">
    <!-- Header + Hero card with badges, value proposition, rendered preview, quick facts -->
    <!-- Problem solved | Quick contract | When to use / When not to use | Anatomy -->
    <!-- Rendered examples (min 6, ideal 8-11) -->
    <!-- API table (XFTable) | Behavior | Types and contracts -->
    <!-- Accessibility | Keyboard | Performance -->
    <!-- Anti-patterns | Related components | Changelog -->
</main>
```

## Component Usage Rule
- Prefer XForge components over raw HTML
- Use XFCard, XFGrid, XFStack, XFTable, XFCode, XFBadge, XFButton, XFSvgIcons.MarkupWithSize
- Raw HTML only when no XForge equivalent exists
- Raw HTML tables NOT allowed — use XFTable
- Native form controls NOT allowed when XForge equivalent exists

## Example Pattern
Every example must have:
- Real rendered preview
- Complete copyable code via XFCode with Copyable="true"
- Preview and code in separate sections with border-t
- Code WITHOUT ellipsis (...) — production-ready

## Code Snippet Rule
Never use @@ in code snippet strings. Use helper variable:
```csharp
private static readonly string At = ((char)64).ToString();
private string ExampleCode => $@"<XFCheckbox {At}bind-Value=""myVar"" />";
```

## API Contract Rule
Before writing API section, inspect: .razor, .razor.cs, *.types.cs, event args classes, item/model classes, cascading contexts, public methods, tests

## Keyboard Section Rule
Mandatory for interactive components. Place after Accessibility, before Performance. Use XFTable with key + behavior columns.

## Accessibility Rule
Must be concrete: semantic element/ARIA role, label/description requirements, focus behavior, error announcement, keyboard behavior, disabled/read-only behavior, screen-reader relevant states.

## Performance Rule
Must be specific: render strategy, state management, event frequency, JS interop presence/absence, large-list guidance, layout shift and stable dimensions.

## Icons
- 3,692 icons from Font Awesome Pro 7.1.0
- All icons via XFSvgIcons.MarkupWithSize("nome", 16) — NEVER inline SVG
- Icon names in kebab-case
- All icon names must exist in catalog (icon_names.txt)

## Bilingual Support
- All text via @T("PT-BR", "EN-US") helper
- PT-BR as primary language
- ASCII-only in repository files (use validacao not validação)

## Validation Before Completion
```powershell
rg -n "[^\x00-\x7F]" <page>                              # Check non-ASCII
rg -n "<table|<thead|<tbody|<tr|<td|<th" <page>          # Check raw HTML tables
rg -n "<input|<select|<textarea|<button" <page>           # Check native form controls
dotnet build XForge.Docs/XForge.Docs.csproj -v:minimal    # Build check
```

## Brand Icons (DocsNavigation)
| Brand | Icon |
|---|---|
| components | layer-group |
| display | table |
| feedback | circle-info |
| layout | sliders |
| forms | rectangle-list |
| tokens | palette |
| playground | gamepad |
| home | house |
