# Arquitetura — MudBlazor (analisada) vs XForge (atual)

## Stack MudBlazor
| Camada | Tecnologia |
|--------|-----------|
| Frontend | Blazor (Server/WASM) |
| Backend | n/a (lib) |
| Build | MSBuild + Bun (TS+SCSS) |
| Package | NuGet (MIT) |
| Test | bUnit + Microsoft.Testing.Platform |
| Analyzers | Roslyn (custom) |
| Docs | Custom Blazor site + Roslyn generator |
| Multi-TFM | net8.0, net9.0, net10.0 |

## Stack XForge.DesignSystem (atual)
| Camada | Tecnologia |
|--------|-----------|
| Frontend | Blazor WebAssembly (RCL) |
| Backend | n/a (lib) |
| Build | MSBuild puro |
| Package | NuGet (MIT) v1.0.0 |
| Test | xUnit 2.9.2 + bUnit 2.3.4 (parcial) |
| Analyzers | nenhum |
| Docs | Razor pages estáticas (*Docs.razor) |
| TFM | net10.0 only |

## Hierarquia de Classes MudBlazor
```
ComponentBase (Microsoft)
  └─ MudComponentBase
       ├─ MudBaseItemsControl<T>
       │    └─ MudBaseBindableItemsControl<T>
       ├─ MudBaseButton
       ├─ MudBaseInput<T> / MudBaseInputControl<T,U>
       │    └─ MudBooleanInput
       ├─ MudFormComponent<T,U>
       │    └─ MudBaseInput<T>
       │         └─ MudInput<T> / MudInputControl<T,U>
       └─ MudBaseSelectItem / ...
```

## Diagrama (simplificado)
```
[Pages/Components/<X>Page.razor] ──references──> [Examples/<X>Example.razor] (in docs)
                                                    │
                                                    ▼
       [Library: MudX.razor / .razor.cs] ──extends──> [MudComponentBase / MudBaseInput / MudBaseButton]
              │                                                │
              │                                                ▼
              │                                       [ParameterState<T> registry]
              │                                                │
              │                                                ▼
              └─[CssBuilder, CSS variables, design tokens, Theme/RTL cascade]──┘
                                                          │
                                                          ▼
                                                   [TS Interop via TScripts/]
```

## Padrões Arquiteturais Chave (a adotar)
| Padrão | Onde | Benefício |
|--------|------|-----------|
| **ParameterState<T>** | Todos [Parameter] | Lifecycle previsível, evita BL0005, change handlers |
| **CreateRegisterScope()** | Constructor | Param registration em batch |
| **CssBuilder** | Class composition | Concatenação type-safe |
| **CSS variables + design tokens** | Themes | Theming + dark mode sem reescrever |
| **Auto-properties only** | [Parameter] | Garante BL semantics corretas |
| **Category(...) attribute** | [Parameter] | API introspection (ApiTypeTable gera docs) |
| **GenerateDocumentationFile** | csproj | XML docs obrigatórios (analyzer falha) |
| **IsTrimmable + TrimMode=link** | csproj | Native AOT ready |
| **EmbedUntrackedSources + snupkg** | csproj | Debug symbols em produção |
| **InternalsVisibleTo** | csproj | Mockability sem expor public API |
| **Bun for assets** | TScripts + Styles | Build 10-100x mais rápido que MSBuild tasks |

## Componentes / Comparação
| Componente | MudBlazor | XForge | Notas |
|------------|-----------|--------|-------|
| Button | MudButton + MudButtonGroup + MudButtonFAB | XFButton + XFButtonGroup | XF sem FAB |
| Input | MudTextField/Mask/NumericField/Date/Time | TextInput/InputMask/Date/Time | Granular similar |
| Select | MudSelect<T> (single API) | XFSelect (similar) | Parity OK |
| Table | MudTable + MudDataGrid | XFTable + XFDataGrid | DataGrid mais rico |
| Dialog | MudDialog (provider) | XFModal + XFMessageBox | Mud = provider pattern melhor |
| Tabs | MudTabs | XFTabs | Parity OK |
| Navigation | AppBar/NavMenu/Drawer/Breadcrumbs/Pagination/Stepper | Similar | Parity OK |
| Charts | MudCharts (sub-lib) | nenhum | Gap a fechar |
| DataGrid | MudDataGrid (rich) | XFDataGrid | Parity parcial |
| AI | nenhum | 29 AI components | Vantagem XForge |
| Theme/RTL/Breakpoint | ThemeProvider+RTLProvider+BreakpointProvider | manual | Gap a fechar |

## Analyzers (diagnostics)
| ID MudBlazor | Descrição | XForge equivalente |
|--------------|-----------|---------------------|
| Mud0001 | Unknown parameter on component | TBD (T002) |
| Mud0002 | Invalid attribute casing (LowerCase pattern) | TBD |
| Mud0003 | Missing [Category] on public Parameter | TBD |
| Mud0004 | Missing XML doc on public Parameter | TBD |

## Build / Package
| Setting | MudBlazor | XForge atual | XForge alvo |
|---------|-----------|--------------|-------------|
| TargetFramework | net8.0;net9.0;net10.0 | net10.0 | net8.0;net9.0;net10.0 |
| Nullable | enable | enable | enable |
| IsTrimmable | true | (default false) | true |
| TrimMode | link | (default copy) | link |
| EnableTrimAnalyzer | true | (default false) | true |
| GenerateDocumentationFile | true | (default false) | true |
| IncludeSymbols | true | false | true |
| SymbolPackageFormat | snupkg | (none) | snupkg |
| EmbedUntrackedSources | true | false | true |
| PublishRepositoryUrl | true | false | true |
| InternalsVisibleTo | 7 assemblies | 2 (Tests) | 7+ (Tests, Viewer, Analyzers, Benchmarks, Docs, DocsGen) |
| Analyzer shipped | yes (2 .dll) | no | yes |
| LanguageResource.resx | AsConstants=true | n/a | yes (T030) |
