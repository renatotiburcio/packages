# Arquitetura - Havit.Blazor (5-way Consolidated)

## Stack
| Camada | Tecnologia | Origem |
|--------|-----------|--------|
| Frontend | Blazor Server + WebAssembly (.NET 10) | Tudo |
| Bootstrap | 5.3.8 (default) + Plain flavor | Havit |
| Testes | xunit.v3 + Microsoft.Testing.Platform | **Havit** |
| Testes UI | bunit.web 1.40.0 | All |
| Localizacao | .resx (en/de/cs/uk/zh-CN/it-IT) | **Havit** |
| Mocking | Moq 4.20.72 | antd/Mud |
| Code Analysis | Microsoft.VisualStudio.Threading.Analyzers 17.14.15 | **Havit** |
| Source Gen | Microsoft.CodeAnalysis 5.3.0 | **Havit** |
| MCP | ModelContextProtocol.AspNetCore 1.4.0 | Blz/Rad |
| AI Inference | SmartComponents.AspNetCore 0.1.0-preview | **Havit** |
| Build | TreatWarningsAsErrors=true | All |
| Multi-TFM | net10.0 + net9.0 + net8.0 | **Havit/Mud/Rad** |

## Diagrama de Camadas
```
+--------------------------------------------------------------+
| User Code (XForge Design System components)                  |
+--------------------------------------------------------------+
| XForge.DesignSystem         | XForge.DesignSystem.AI         |
| - XF* (UI)                  | - XFAi* (AI)                   |
| - Tailwind v4 (unica CSS)   | - 11 providers                 |
+-----------------------------+--------------------------------+
| XForge.DesignSystem.Shared                                    |
| - BaseComponent<TClasses,TStyles> (T047)                     |
| - IClassProvider/IStyleProvider (T044)                       |
| - ParameterState<T> (T001)                                   |
| - AddXForgeComponents() (T063)                               |
+--------------------------------------------------------------+
| XForge.Internals (separate package)                          |
| - CssClassHelper, Debouncer, StringBuilderCache              |
| - BootstrapIconsGenerator (T076)                             |
| - Docs Compiler + MCP server                                 |
+--------------------------------------------------------------+
```

## Base Class Hierarchy (5-way)
```
ComponentBase
+-- XForgeComponentBase (XForge)              [Id gen, theme, attrs]
    +-- RadzenComponent (T056)                [OnBecameInvisible, Mouse]
    |   +-- FormComponent<T>
    |       +-- FormComponentWithAutoComplete<T>
    +-- BaseComponent (Blazorise T047)        [dual generic, providers]
    |   +-- BaseElementComponent / BaseInputComponent / etc
    +-- MudComponentBase (Mud T001)           [ParameterState<T>]
    |   +-- MudBaseInput<T>
    |       +-- MudFormComponent<T,U>
    +-- AntComponentBase (antd)               [CallAfterRender, RefBack]
    +-- HxInputBase<T> (Havit T067)           [FormState cascade, Settings]
        +-- HxInputBaseWithInputGroups
            +-- HxInputText / HxInputDate / HxSelect / etc
```

## Padroes Arquiteturais
| Padrao | Origem | Onde usar |
|--------|--------|-----------|
| ParameterState<T> (auto two-way binding) | Mud T001 | Todos os inputs |
| Roslyn Analyzers (4+ diagnostics) | Mud+Blz T002 | Build-time enforcement |
| CssClassHelper.Combine | **Havit** | Class composition |
| Debouncer async w/ CancellationToken | Havit+Rad T057 | Search/autocomplete |
| IClassProvider/IStyleProvider DI | Blz T044 | Provider abstraction |
| ShouldAutoGenerateId + Attributes | Blz T045 | Auto Id |
| HxChangeTracker (INPC scope) | **Havit T070** | Re-render scope control |
| HxDialogBase<TResult> (TaskCompletionSource) | **Havit T068** | Modal returns |
| HxMessageBoxService (DI singleton) | **Havit T069** | Message box |
| FormState cascading | **Havit T067** | Bulk enable/disable |
| HxSetup.Defaults (static global settings) | **Havit T066** | App-wide defaults |
| IInputWithLabelType/IInputWithSize/IInputWithPlaceholder | **Havit T066** | Interface segregation |
| HxGrid w/ DataProvider delegate | **Havit T071** | Server-driven grid |
| HxFilterForm + HxChipGenerator pipeline | **Havit T072** | Filter as chips |
| Resources .resx per component (en/de/cs/uk/zh-CN/it-IT) | **Havit T074** | Localization |
| BootstrapIconsGenerator (IIncremental) | **Havit T076** | Build-time icon enum |
| SmartComponents wrapping (ComboBox/Paste/TextArea) | **Havit T077** | AI upgrade |
| AddHxServices + TimeProvider | **Havit T073** | DI bootstrap |
| VSTHRD analyzer | **Havit T032** | Threading safety |
| ThemeService abstract | Rad T064 | Multi-theme |
| AIChatService SSE | Rad T060 | AI streaming |
| Docs.Mcp server | Blz T053 + Havit T065 | AI docs access |

## Componentes (5-way consolidated)
| Componente | Origem | Funcao |
|-----------|--------|--------|
| XFButton/Button/Submit | Mud+antd+Havit | Bootstrap button variants |
| XFInput* family | Mud+Havit | Text/Number/Date/Percent/Range/DateRange |
| XFSelect/XFMultiSelect | antd+Havit | Single/multi dropdown |
| XFAutosuggest/XFSearchBox | **Havit** | Typeahead w/ templated items |
| XFCheckbox/XFRadio/XFCheckboxList/XFRadioButtonList | Mud+Havit | Boolean/choice inputs |
| XFTag/XFInputTags | **Havit** | Multi-tag input |
| XFGrid | Mud+**Havit** | Server-driven data grid |
| XFPager | Mud+Havit | Pagination |
| XFModal/XFDialogBase | Mud+**Havit** | Modal w/ TaskCompletionSource return |
| XFDrawer/XFOffcanvas | antd+Havit | Side drawer |
| XFMessageBox | **Havit** | Yes/No/Cancel dialogs |
| XFMessenger (toast) | **Havit** | Toast service |
| XFTabs/XFTabPanel | Havit | Tab container |
| XFTreeView | Havit | Hierarchical data |
| XFAiOrchestration | XForge | AI multi-provider routing |
| XFAiChat/XFChat | Rad+**Havit** | AI chat UI |
| XFSidebar | **Havit** | App-level nav sidebar |
| XFListLayout | **Havit** | Page list w/ filter in offcanvas |


## Stack Comparison vs Other 4 Libraries
| Caracteristica | MudBlazor | antd-blazor | Blazorise | Radzen | **Havit** | XForge (target) |
|---------------|-----------|-------------|-----------|--------|--------|------|
| CSS framework | Material | Ant Design | 7 providers | Themes | Bootstrap 5.3.8 | Tailwind v4 |
| Components | 85+ | 70+ | 192 | 110+ | 62+74=136 | 181 |
| Source generators | 4 (Analyzers) | none | 4 (incl Fody) | 1 (API) | **1 (Icons)** | 0 (planned T076) |
| MCP server | no | no | YES live | cloud only | **YES local** | planned T053/T065 |
| AI features | 0 | 0 | 0 | 4 chat | **3 Smart** | 29 |
| Localization | 1 lang | JSON 70+ | resource | resx | **resx 6 langs** | pt-BR/en/es (T005) |
| Form pattern | ParameterState | Form/EditContext | DataAnnotations | FormComponent | **FormState cascade** | T067 |
| Test runner | MTP+xunit.v3 | bunit+standard | MTP+xunit.v3 | bunit+MSTest | **xunit.v3+MTP** | T052 |
| TFM count | 3 | 7 | 3 | 4 | **3** | 3 (T015) |
| Bootstrap Icons | no | no | no | no | **2k+ via SG** | T076 |
| Modal async return | Task | Task | Task | Task | **Task<DialogResult<T>>** | T068 |
| TimeProvider DI | no | no | no | no | **yes** | T073 |

## Project Structure
```
Havit.Blazor/
+-- Havit.Blazor.Components.Web.Bootstrap/    <- Main lib (75 razor + 322 cs)
|   +-- Accordions/ Badges/ Buttons/ Calendar/ Cards/...
|   +-- Resources/                              <- .resx files (84 files, 6 langs)
|   +-- wwwroot/                                <- bootstrap.min.css + 18 JS
|   +-- HxSetup.cs                              <- Static defaults
|   +-- GlobalSettings.cs, ColorMode.cs, BootstrapFlavor.cs
+-- Havit.Blazor.Components.Web/               <- Core (38 cs)
|   +-- CssClassHelper.cs, Debouncer.cs, HxChangeTracker.cs, HxDynamicElement.cs
|   +-- HxRedirectTo.cs, ServiceCollectionExtensions.cs (AddHxServices)
+-- Havit.Blazor.Components.Web.Bootstrap.Smart/  <- AI components (3 cs)
|   +-- HxSmartComboBox.cs, HxSmartPasteButton.cs, HxSmartTextArea.cs
+-- Havit.Blazor.Components.Web.Bootstrap.InternalSourceGenerators/  <- Icon SG
+-- Havit.Blazor.Documentation/                 <- Docs site (WASM)
+-- Havit.Blazor.Documentation.Mcp/             <- MCP server (4 tools)
+-- Havit.Blazor.Components.Web.Bootstrap.Tests/  <- xunit.v3 (66 files)
+-- Havit.Blazor.Grpc.*/  + ApplicationInsights/ + Storage/
+-- Directory.Build.props, Directory.Packages.props, .editorconfig, AGENTS.md
```

## Conventions & EditorConfig (Havit)
- **Indentation**: Tab, size 4
- **var**: prefer implicit unless type non-obvious
- **private fields**: _underscoreName
- **Sort system usings first**
- **Accessibility modifiers required** (non-interface)
- **Braces always required** (warning level)
- **No Nullable Reference Types** (XForge DOES use NRT, adapt pattern)
- **Spell check** + custom exclusion.dic

## Build System
- global.json: SDK 10.0.300, MTP test runner
- Directory.Build.props: VersionPrefix 2.0.0, EnforceCodeStyleInBuild, TreatWarningsAsErrors
- Directory.Packages.props: ManagePackageVersionsCentrally=true, CentralPackageTransitivePinningEnabled=true, TFM-conditional versions
- Per-project: CopyCustomContent target copies .xml to Documentation/XmlDoc
- Custom MSBuild target AttachCssAsScoped for lib CSS bundling

