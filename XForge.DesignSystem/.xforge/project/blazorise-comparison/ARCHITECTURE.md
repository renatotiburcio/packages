# Arquitetura — Blazorise vs XForge (consolidado 3-way)

## Stack Comparado
| Camada | XForge | MudBlazor | antd-blazor | **Blazorise** |
|--------|--------|-----------|-------------|---------------|
| Runtime | .NET 10 (WASM) | .NET 8/9/10 | .NET std2.1 + 5/6/7/8/9/10 | .NET 8/9/10 |
| CSS | Tailwind v4 | Material | Ant Design | 7 (switchable) |
| UI base | ECharts + Quill | Material Icons | Ant SVG | Bootstrap+FA/Tailwind/Ant |
| Tests | xUnit 2.9.2 + bUnit | xUnit + bUnit | xUnit + bUnit + Nunit3 | xUnit 2.9.3 + bUnit 2.7.2 + NUnit 6.2.0 |
| Validation | FluentValidation | DataAnnotations | OneOf | FluentValidation 12.1.1 (extension) |
| Source Gen | none | 1 (Roslyn) | none | 4 (Roslyn + Fody) |
| Docs | Razor manual | MudUnitTests.Viewer | Docs.Build.CLI | Docs.Compiler + Docs.Mcp |
| Licensing | MIT | MIT | MIT | Dual (commercial) |
| Pkg mgmt | direct | direct | direct | **Directory.Packages.props** ⭐ |
| Trim/AOT | no | yes (link) | no | no |

## Padrões Arquiteturais
| Padrão | Onde usar | Origem | Adotar? |
|--------|-----------|--------|---------|
| **Provider-agnostic IClassProvider/IStyleProvider** | Multi-CSS futuro | Blazorise | ✅ T044 |
| ParameterState<T> + .WithParameter/EventCallback | Auto-SetParametersAsync | MudBlazor | ✅ T001 |
| DomEventService (singleton DI) | JS interop decoupling | antd-blazor | ✅ T004 |
| Localized XML Intellisense | Multi-lang docs | antd-blazor | ✅ T005 |
| **IsEqual + Dirty notification** em setters | Reactive class/style | Blazorise | ✅ T045/T049 |
| **BaseComponent<TClasses, TStyles>** dual generic | Typed customization | Blazorise | ✅ T047 |
| **Snippets.generated.cs** (Roslyn source-gen) | Compile-time examples | Blazorise | ✅ T046 |
| **Microsoft.CodeAnalysis.CSharp scanning** | Build-time API docs | Blazorise | ✅ T021 |
| MCP server for docs (6 tools, 3 transports) | AI-friendly docs | Blazorise/antd | ✅ T053/T028 |
| CallAfterRender queue + ForwardRef | Lifecycle | antd-blazor | ✅ T017 |
| OneOf union types | Flexible params | antd-blazor | ⚠️ T018 (avaliação) |
| JSON-based locales (70+ files) | i18n storage | antd-blazor | ✅ T019 |
| CssBuilder fluent API | Class composition | MudBlazor | ✅ T010 |
| Fody Weaver auto-Dirty | IL post-process | Blazorise | ⚠️ T049 (Roslyn melhor) |
| Centralized package versions | Pkg mgmt | Blazorise | ✅ T052 |

## Componentes
| Componente | Responsabilidade | Origem | Adotar? |
|-----------|------------------|--------|---------|
| Blazorise (core) | Base classes + interfaces | Blazorise | estudo |
| Blazorise.{Bootstrap,Bootstrap5,Bulma,Material,AntDesign,FluentUI2,Tailwind} | 7 CSS providers | Blazorise | ❌ YAGNI |
| Blazorise.Analyzers | 1 Roslyn analyzer | Blazorise | estudo |
| Blazorise.Tests.bUnit | bUnit integration | Blazorise | estudo |
| 38 Extensions (DataGrid, Charts, Scheduler, etc.) | Complex components | Blazorise | select 5-10 |
| Blazorise.Docs | Razor docs site | Blazorise | estudo |
| **Blazorise.Docs.Compiler** | Generates docs-index.json + API JSON | Blazorise | ✅ T021 |
| **Blazorise.Docs.Generator** | CLI for docs generation | Blazorise | estudo |
| **Blazorise.Docs.Mcp** | MCP server, 6 tools, 3 transports | Blazorise | ✅ T053 |
| Blazorise.Docs.Server | Docs web host | Blazorise | estudo |
| 4 SourceGenerators (Generator, Generator.Features, Weavers, Weavers.Fody) | Source-gen + IL weaving | Blazorise | ⚠️ T049 parcial |

## Hierarquia de Classes (Blazorise)
```
ComponentBase
└── BaseAfterRenderComponent
    └── BaseComponent (1172 lines)
        ├── BaseElementComponent
        ├── BaseLinkComponent
        ├── BaseFocusableContainerComponent
        ├── BaseRowComponent / BaseColumnComponent / BaseGridComponent
        ├── BaseTypographyComponent
        ├── BaseInputComponent
        │   ├── BaseTextInput
        │   │   └── BaseBufferedTextInput
        │   ├── BaseCheckComponent (Checkbox, Switch, Radio)
        │   ├── BaseRadioComponent
        │   └── BaseOnScreenKeyboardInputComponent
        └── BaseComponent<TClasses, TStyles> (typed customization)
        └── BaseDraggableComponent
        └── BaseSizableFieldComponent
```

## Como cada Lib resolve "Build CSS class"
| Lib | Approach | Tradeoff |
|-----|----------|----------|
| XForge | ClassBuilder service with explicit fields | Verbose but traceable |
| MudBlazor | CssBuilder fluent | Concatenation risk |
| antd-blazor | AntDesign.targets + class | Provider-specific |
| **Blazorise** | IClassProvider DI + ClassBuilder + StyleBuilder | Most flexible (swap CSS framework) |

## Estratégia de Adoção para XForge v2.0.0
1. **Sprint 1**: Adotar base (T001, T002, T010, T019, T044, T045, T052)
2. **Sprint 2**: Adotar service layer (T004, T005, T017, T047, T049)
3. **Sprint 3-4**: Adotar docs/test (T020, T021, T022, T046, T053, T054)
4. **Sprint 5-6**: Avaliar multi-TFM, Extensions, Benchmark

## Não-objetivos v2.0.0
- Multi-CSS (Bootstrap/Tailwind/Ant switching) — só 1
- 38 Extensions — só 5-10 selecionados
- Dual-license — MIT puro
- 7 TFM — só 3 (net8/9/10)
- MAUI/Hybrid — só Blazor WASM/Server
