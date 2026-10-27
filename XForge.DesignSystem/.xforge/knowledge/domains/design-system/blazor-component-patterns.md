# Blazor Component Patterns — XForge Design System

## Base Class Hierarchy

```
ComponentBase → XForgeComponentBase → XFInputComponentBase → [Component]
```

### XForgeComponentBase
- `AdditionalAttributes`: `Dictionary<string, object>?` — unmatched HTML attributes
- `Id`: auto-generated `xf-{8-char-guid}` if not set
- `Class` / `Style`: user-supplied CSS
- `Disabled`: bool
- `BaseClass`: virtual Tailwind classes
- `GetClass()` / `GetComponentClass()`: merges BaseClass + user Class

### XFInputComponentBase
- `InputRef`: ElementReference for primary input
- `FocusAsync()`: JS interop via `xf-focus.js`
- `SelectAsync()`: selects all text
- `BlurAsync()`: removes focus
- `DisposeAsync()`: cleans up JS module reference
- Triple exception handling: `InvalidOperationException`, `JSException`, `JSDisconnectedException`

## Component File Pattern

Every component uses 3 files:
```
XFButton.razor        → HTML + Razor markup
XFButton.razor.cs     → Code-behind (partial class)
XFButton.types.cs     → DTOs, enums, event args
```

## CSS Pattern (Tailwind + CSS Variables)

All CSS classes built in C# via `XFFormCss` static class:
- Size mapping: `XFSize.Sm` → `h-9 py-1.5`
- Variant mapping: `XFFormVariant.Outline` → `border-2 border-[var(--color-primary)]`
- Tone mapping: `XFSemanticColor.Primary` → `focus-within:ring-[var(--color-primary)]`
- Disabled: `disabled:opacity-50 disabled:cursor-not-allowed`
- Transitions: `transition-all duration-200 ease-out`

## Design Token Enums (15 total)

### Sizing
- `XFSize`: Sm, Md, Lg
- `XFExtendedSize`: Xs, Sm, Md, Lg, Xl
- `XFOverlaySize`: Sm, Md, Lg, Xl, Full
- `XFDensity`: Comfortable, Compact

### Visual Variants
- `XFFormVariant`: Outline, Filled, Soft, Ghost
- `XFContainerVariant`: Plain, Soft, Surface
- `XFListVariant`: Plain, Bounded, Soft
- `XFShape`: Rounded, Square, Circle, Pill, Auto

### Color/Semantic
- `XFSemanticColor`: Default, Primary, Secondary, Success, Warning, Danger, Info, Neutral
- `XFIntent`: Default, Info, Success, Warning, Danger, Primary, Secondary, Neutral
- `XFSeverity`: Info, Success, Good, Warning, Low, Medium, High, Critical

### Orientation
- `XFOrientation`: Horizontal, Vertical

## JS Interop Pattern

25 isolated ES modules in `wwwroot/js/xf-*.js`:
- **animate**, **breakpoint**, **captcha**, **chart**, **clipboard**, **color-picker**
- **dialog**, **focus**, **gesture-handler**, **lottie**, **map**, **nav-state**
- **one-time-input**, **pdf-viewer**, **popover**, **rich-text-editor**
- **signature-pad**, **speech-to-text**, **split-button**, **splitter**
- **theme**, **tooltip**

Pattern: lazy-load on first use, cache module reference, handle 3 exception types.

## Validation Pattern

```
XFValidatorBase → XFRequiredValidator, XFEmailValidator, XFLengthValidator
                → XFRangeValidator, XFRegexValidator, XFCompareValidator
                → XFCustomValidator
```

Components: `XFValidationMessage`, `XFValidationStatus`, `XFValidationSummary`

## AI Provider Pattern

```
IAiProvider → AiProviderBase → [OpenAiProvider, OllamaProvider, ...]
                            → InMemoryAiProvider
```

DI: `AddXFAi()` fluent builder, keyed registration for multi-provider.
Config: `XForgeAI` section in appsettings.json.
Security: prompt injection detection, PII masking, rate limiting, content moderation.

## Namespace

Root: `XForge.DesignSystem`
AI: `XForge.DesignSystem.AI.Components.AI.{Domain}`
Docs: `XForge.DesignSystem.Docs.Shared`
