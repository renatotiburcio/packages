# Arquitetura — Radzen Blazor (4-way consolidated)

## Stack
| Camada | Tecnologia | Origem |
|--------|-----------|--------|
| Runtime | .NET 10 (LTS) | 4-way |
| UI Framework | Blazor Server + WebAssembly + Auto + MAUI Hybrid | Radzen ⭐ |
| CSS | 5 temas SCSS (material/standard/default/humanistic/software) | Radzen ⭐ |
| AI Integration | 4 components (AIChat, Chat, Markdown, SpeechToText) | Radzen ⭐ |
| Tests | xUnit 2.4.1 + bunit.web 1.36.0 + coverlet + Stryker | Radzen + MudBlazor |
| Build | MSBuild + Radzen.MSBuild + Directory.Build.props | Radzen + MudBlazor |
| Docs | Roslyn SG + Docs Compiler + API Generator + MCP server | 3-way |
| Theming | CSS Variables + PersistentComponentState (SSR) | Radzen ⭐ |

## Diagrama de Camadas (4-way pattern)
```
[App.razor: RadzenTheme] → [ThemeService] → [CookieThemeService | QueryStringThemeService]
       ↓
[Layout/MainLayout.razor] → [RadzenBody + RadzenChat + RadzenAIChat]
       ↓
[RadzenComponent (base)] ← [ClassList + StringBuilderCache + Debouncer]
       ↓
[FormComponent<T>] → [FormComponentWithAutoComplete<T>]
       ↓
[RadzenDataGrid<T> | RadzenScheduler | RadzenChart | RadzenGantt] (110+ components)
       ↓
[ServiceCollection.AddRadzenComponents()] → [DI Scoped: Dialog | Notification | Tooltip | ContextMenu | Theme | AIChat]
       ↓
[wwwroot: Radzen.Blazor.min.js (TerserMinify)] + [themes/*.css (SassCompile)]
```

## Hierarquia de Classes
| Nível | Classe | Responsabilidade | Origem |
|------|--------|------------------|--------|
| L0 | `ComponentBase` | Built-in Blazor | — |
| L1 | `RadzenComponent` | Attributes, Element, MouseEnter/Leave, ContextMenu, Culture, Debouncer, JS cleanup | **Radzen** ⭐ |
| L2 | `FormComponent<T>` | Value, EditContext, Validation | **Radzen** |
| L3 | `FormComponentWithAutoComplete<T>` | AutoComplete + AriaAutoComplete | **Radzen** |
| L4 | `RadzenTextBox`, `RadzenPassword`, `RadzenNumeric` | 110+ concrete components | **Radzen** |
| L4 | `RadzenDataGridColumn<TItem>` | Composite (cascading Grid + Parent) | **Radzen** ⭐ |

## Padrões Arquiteturais (4-way consolidated)
| Padrão | Onde usar | Origem |
|--------|----------|--------|
| **CaptureUnmatchedValues** Attributes | Pass-through HTML attrs | Radzen + Blazorise |
| **OnBecameInvisible** cleanup | Toggleable components (Dialog, Drawer) | **Radzen** ⭐ |
| **Debouncer/Throttle** System.Timers | TextBox, Slider, Numeric | **Radzen** ⭐ |
| **ClassList** struct-based | CSS class composition | **Radzen** ⭐ |
| **StringBuilderCache** ThreadStatic | Perf (rendering path) | **Radzen** ⭐ |
| **Primary ctor + IAsyncEnumerable** | AI streaming services | **Radzen** ⭐ |
| **AddRadzenComponents()** extension | DI registration idiom | **Radzen** ⭐ |
| **PersistentComponentState** | SSR theme persistence | **Radzen** ⭐ |
| **ParameterState<T>** | Reactive state | MudBlazor |
| **Roslyn Analyzers** | Build-time validation | MudBlazor + Blazorise |
| **ClassBuilder/StyleBuilder** | CSS composition | Blazorise |
| **DomEventService** singleton | Event delegation | antd-blazor |
| **OneOf** union types | Multi-modal params | antd-blazor |
| **Localized XML Intellisense** | Build-time i18n | antd-blazor |
| **Docs.Mcp** server | AI-friendly docs | Blazorise + Radzen |

## Componentes-Chave (4-way consolidation)
| Componente | Responsabilidade | Origem única |
|-----------|-----------------|--------------|
| RadzenComponent | Base com OnBecameInvisible, MouseEnter, Culture | Radzen ⭐ |
| FormComponent<T> | EditContext, validation, value binding | Radzen |
| ClassList | CSS class fluent builder | Radzen ⭐ |
| StringBuilderCache | StringBuilder pooling | Radzen (.NET MIT) ⭐ |
| Debouncer | Debounce/throttle actions | Radzen ⭐ |
| AIChatService | SSE streaming + session memory | Radzen ⭐ |
| AIChatServiceOptions | IOptions pattern (C# 12) | Radzen ⭐ |
| ConversationSession | In-memory multi-session | Radzen ⭐ |
| IAIChatService | Provider-agnostic AI | Radzen ⭐ |
| RadzenTheme | CSS injection + persistence | Radzen ⭐ |
| ThemeService (abstract) | Theme storage abstraction | Radzen ⭐ |
| CookieThemeService | Cookie-based persistence | Radzen ⭐ |
| QueryStringThemeService | URL-based persistence | Radzen ⭐ |

## Build & Test Pipeline
```
git commit
  → pre-commit (dotnet format + SH rules)
  → Restore (Directory.Packages.props CPM)
  → Build (SassCompile → MoveCss → MinifyJs → Compile)
  → Radzen.MSBuild (SCSS + JS minification)
  → Test (xUnit + bunit 1.36 + coverlet 1.3)
  → Stryker (mutation tests, weekly)
  → Docs Compiler (Roslyn → API .razor pages)
  → Api.Generator (XML docs → API pages)
  → Docs MCP server (Docker)
  → Pack (snupkg with portable PDBs)
```

## Theming Architecture (Radzen pattern)
| Theme | Variants | WCAG |
|-------|---------|------|
| material | base, dark, wcag, dark-wcag | ✓ |
| standard | base, dark, wcag, dark-wcag | ✓ |
| default | base, dark, wcag, dark-wcag | ✓ |
| humanistic | base, dark, wcag, dark-wcag | ✓ |
| software | base, dark, wcag, dark-wcag | ✓ |
| **Total** | **20 free SCSS files** | ✓ |

Premium (Radzen Blazor Pro): Material 3, Fluent — NÃO copiar (manter Tailwind v4).

## Cross-cutting Patterns
- **Lazy component registration** via `AddRadzenComponents()` extension
- **Scoped services** for Dialog/Notification/Tooltip/ContextMenu/Theme/AIChat
- **Singleton ConvSession** with thread-safe dictionary
- **Static JsonSerializerOptions** cached (avoid alloc per request)
- **Debounce 500ms** default for search inputs
- **IReadOnlyDictionary<string,object>** for Attributes (immutable)
- **InternalsVisibleTo("XForge.DesignSystem.Tests")** for test access

## Performance Considerations (from Radzen)
- **StringBuilderCache MaxBuilderSize=360** — trade-off coverage vs alloc
- **Debouncer Task.Run** — avoid sync-over-async
- **Cached JsonSerializerOptions** static — avoid per-call alloc
- **ConcurrentDictionary OR lock-protected dict** for sessions
- **Lazy DotNetObjectReference** — defer creation until first access
- **JSRuntime.InvokeVoid (sync)** when no return needed