# Arquitetura — 3 Libs Comparadas (MudBlazor, antd-blazor, XForge)

## Stack antd-blazor
| Camada | Tecnologia |
|--------|-----------|
| Frontend | Blazor (Server/WASM/Hybrid) |
| Backend | n/a (lib) |
| Build | MSBuild + TS+rollup |
| Package | NuGet (MIT) |
| Test | bUnit + custom TestKit |
| Analyzers | NetAnalyzers only (Microsoft) |
| Docs | Blazor site + AntDesign.Docs.Build CLI |
| Multi-TFM | netstandard2.1, net5,6,7,8,9,10 (7 TFMs!) |
| i18n | 70+ JSON locales (6 com Intellisense) |
| MCP | YES (PR #4758, Feb 2026) |
| Hybrid | MAUI/WPF/WinForms |

## Comparacao hierarquia de classes
| Lib | Root | Forms | Inputs | Containers | Dom |
|-----|------|-------|--------|-----------|-----|
| **MudBlazor** | MudComponentBase | MudFormComponent | MudBaseInput/MudInput | MudBaseItemsControl | (inline) |
| **antd-blazor** | AntComponentBase | AntInputComponentBase | AntInputBoolComponentBase | AntContainerComponentBase | AntDomComponentBase (with DomEventService) |
| **XForge** | XForgeComponentBase | XFInputComponentBase | (3-file pattern) | (manual) | (inline) |

## Diagram: DomEventService (antd-blazor innovation)
```
[Component (AntDomComponentBase)]
        |
        | @inject IDomEventListener DomEventListener
        |
        v
[DomEventService] (singleton, registered in DI)
        |
        | subscribe('click', element, callback)
        | subscribe('resize', window, callback)
        |
        v
[IJSRuntime] -> JS module
```
Diferenca fundamental: antd-blazor centraliza eventos DOM via service,
em vez de inline JS interop em cada componente.

## Patterns Arquiteturais Comparados
| Pattern | MudBlazor | antd-blazor | XForge alvo |
|---------|-----------|-------------|-------------|
| Param state mgmt | ParameterState<T> | plain auto-prop + lifecycle | ParameterState<T> (T001) |
| DOM events | inline EventCallback | DomEventService (DI) | DomEventService (T004) |
| CSS composition | CssBuilder | CssSizeLength + manual | CssBuilder (T010) |
| Class hierarchy | MudBaseInput (deep) | AntInputComponentBase (shallow) | XFInputComponentBase (shallow) |
| Theme | CSS vars + design tokens | AntDesign token system | CSS vars (T014) |
| Localization | resx (.NET) | JSON (70+ files) | JSON (T019) |
| Intellisense | single (en) | 6 langs (en/ja/ko/ru/zh) | 6 langs (T005) |
| Analyzers | Roslyn (4) | NetAnalyzers (Microsoft) | Roslyn (T002) |
| Asset build | Bun (TS+SCSS) | TypeScript+rollup | Bun (T003) |
| Generated docs tests | Roslyn (Yes) | CLI build tool (not at compile-time) | Roslyn (T021) |
| MCP server | no | YES (PR #4758) | YES (T028) |
| Form drafts | no | DraftMonitor component | (T030) |
| Multi-TFM | 3 (net8/9/10) | 7 (netstd2.1 + net5-10) | 3 (net8/9/10) |
| Trimmable | yes | no | yes |
| OneOf | no | yes | yes (T018) |
| CallAfterRender | no | yes (queue) | yes (T017) |
| ForwardRef | no | yes (RefBack) | yes (T017) |
| Hybrid (MAUI/WPF) | no | yes | no (futuro, T040) |

## Componentes / Cobertura comparada
| Componente | MudBlazor | antd-blazor | XForge |
|------------|-----------|------------|--------|
| Button (basic) | MudButton | AntButton | XFButton |
| ButtonGroup | MudButtonGroup | AntButtonGroup | XFButtonGroup |
| FAB | MudButtonFAB | (no FAB, usa shape=circle) | n/a |
| TextField | MudTextField | AntInput | TextInput |
| NumericField | MudNumericField | AntInputNumber | NumericInput |
| DatePicker | MudDatePicker | AntDatePicker | DateInput |
| TimePicker | MudTimePicker | AntTimePicker | TimeInput |
| DateRangePicker | MudDateRangePicker | AntRangePicker | n/a (gap) |
| Select | MudSelect | AntSelect | XFSelect |
| TreeSelect | MudTreeView | AntTreeSelect | n/a (gap) |
| Cascader | n/a | AntCascader | n/a (gap) |
| AutoComplete | MudAutocomplete | AntAutoComplete | AutoComplete |
| Table | MudTable | AntTable | XFTable |
| DataGrid | MudDataGrid | AntTable (rich) | XFDataGrid |
| TreeView | MudTreeView | AntTree | TreeView |
| Form | MudForm | AntForm | Form |
| Modal | MudDialog | AntModal | XFModal |
| Drawer | MudDrawer | AntDrawer | Drawer |
| Notification | no (use Snackbar) | AntNotification | Toast |
| Message | no (use Snackbar) | AntMessage | n/a (gap) |
| Steps | MudStepper | AntSteps | Steps |
| Tabs | MudTabs | AntTabs | Tabs |
| Menu | MudMenu | AntMenu | Menu |
| Tree | MudTreeView | AntTree | TreeView |
| Charts | MudCharts (sub-lib) | n/a | n/a (gap) |
| Tag | no | AntTag | Tag |
| Timeline | MudTimeline | AntTimeline | Timeline |
| Statistic | n/a | AntStatistic | n/a (gap) |
| Descriptions | n/a | AntDescriptions | n/a (gap) |
| Comment | n/a | AntComment | n/a (gap) |
| Result | n/a | AntResult | n/a (gap) |
| Affix | n/a | AntAffix | n/a (gap) |
| Watermark | n/a | AntWatermark | n/a (gap) |
| Splitter | MudSplitPanel | AntSplitter | Splitter |
| Segmented | n/a | AntSegmented | n/a (gap) |
| Tour | n/a | AntTour | n/a (gap) |
| ConfigProvider | ThemeProvider | AntConfigProvider | n/a (gap) |

## Analyzers
| Source | ID | Diagnostic | XForge equivalent |
|--------|----|-----------|-------------------|
| MudBlazor | Mud0001 | Unknown parameter on component | XF0001 (T002) |
| MudBlazor | Mud0002 | Invalid attribute casing (LowerCase pattern) | XF0002 (T002) |
| MudBlazor | Mud0003 | Missing [Category] on public Parameter | XF0003 (T002) |
| MudBlazor | Mud0004 | Missing XML doc on public Parameter | XF0004 (T002) |
| antd-blazor | (uses NetAnalyzers Microsoft.CodeAnalysis.NetAnalyzers) | CA2007 etc | (already enabled) |

## Build / Package
| Setting | MudBlazor | antd-blazor | XForge atual | XForge alvo |
|---------|-----------|-------------|--------------|-------------|
| TargetFrameworks | net8;net9;net10 | netstd2.1;net5;net6;net7;net8;net9;net10 | net10 | net8;net9;net10 |
| Nullable | enable | (default) | enable | enable |
| IsTrimmable | true | false | false | true |
| TrimMode | link | (n/a) | (n/a) | link |
| GenerateDocumentationFile | true | true | false | true |
| DocumentationFile | ./obj | ./LocalizedIntellisenseFiles/$(AssemblyName).xml | (n/a) | ./obj + localized |
| IncludeSymbols | true | true | false | true |
| SymbolPackageFormat | snupkg | snupkg | (none) | snupkg |
| EmbedUntrackedSources | true | true | false | true |
| Build targets shipped | no | YES (AntDesign.targets) | no | YES (T027) |
| InternalsVisibleTo | 7 assemblies | 2 (Tests, Docs.Build.CLI) | 2 (Tests) | 5+ |
| Analyzers shipped | yes (2 .dll) | no (only NetAnalyzers) | no | yes (T002) |
| LanguageResource | resx (AsConstants) | JSON (70+ files) | n/a | JSON (T019) |
| Multi-lang intellisense | no | YES (6 langs) | no | YES (T005) |
| MCP server | no | YES | no | YES (T028) |
