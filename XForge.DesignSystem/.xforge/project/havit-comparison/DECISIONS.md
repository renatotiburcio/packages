# Decisoes Tecnicas - Havit.Blazor (5-way Consolidated)

## D001: ParameterState<T> (Mud)
- **Contexto**: Reduzir boilerplate de get/set em componentes Blazor (~70% codigo)
- **Opcoes**: A) ParameterState<T> Mud, B) Padrao manual
- **Escolha**: A (Mud T001)
- **Rationale**: ~70% reducao de boilerplate, [Parameter]/[CascadingParameter] automaticos
- **Tradeoffs**: Acoplamento a MudDesign System, refactoring de inputs existentes

## D002: Roslyn Analyzers (Mud + Blz merge)
- **Contexto**: Build-time enforcement de patterns Blazor
- **Opcoes**: A) MudBlazor.Analyzers (4 diag), B) Blazorise.Analyzers, C) Merge
- **Escolha**: C (T002)
- **Rationale**: Cobertura maxima, custom diagnostics para ParameterState, OnAfterRender, etc
- **Tradeoffs**: Manter build proprio, mais 24h de setup

## D003: DomEventService (antd)
- **Contexto**: Centralizar listener de DOM events do JSInterop
- **Opcoes**: A) Singleton service, B) Scoped per circuit, C) Inline por componente
- **Escolha**: B (recomendacao) - T004
- **Rationale**: Singleton global tem vazamento de estado entre sessoes, scoped eh mais seguro
- **Tradeoffs**: Pequena perda de performance vs ganho em isolamento

## D004: Localized XML (antd + Havit)
- **Contexto**: Suporte multi-idioma de intellisense e Resource
- **Opcoes**: A) 6 linguas Havit (en/de/cs/uk/zh-CN/it-IT), B) 70+ JSON antd, C) 3 (pt-BR/en/es) LATAM
- **Escolha**: C (T005) + usar _Imports.razor localized XML pattern (T074)
- **Rationale**: LATAM focus do XForge
- **Tradeoffs**: Cobre 90% mercado, ja/ko/zh/ru diferidos para v2.1

## D005: Provider-agnostic CSS (Blz)
- **Contexto**: Suportar multiplos temas (Bootstrap 5, Tailwind, etc)
- **Opcoes**: A) IClassProvider/IStyleProvider DI, B) Tailwind only v2.0.0
- **Escolha**: B (YAGNI) + fundacao T044 para v2.1+
- **Rationale**: Tailwind unico provider reduz complexidade inicial
- **Tradeoffs**: 32h T044 diferido, mas permite provider alternativo futuro

## D006: CssClassHelper pattern (Havit)
- **Contexto**: Composicao de classes CSS
- **Opcoes**: A) CssBuilder Mud (T010), B) ClassList struct Radzen (T059), C) CssClassHelper.Combine Havit
- **Escolha**: C (Havit-style) - statica, simples, return string
- **Rationale**: Zero allocation, mais simples que Rad struct
- **Tradeoffs**: Menos fluent que CssBuilder

## D007: xunit.v3 + MTP (Havit)
- **Contexto**: Runner de testes mais rapido, sem VSTest legacy
- **Opcoes**: A) xunit.v3 + Microsoft.Testing.Platform, B) xunit + VSTest padrao
- **Escolha**: A (T052)
- **Rationale**: 3-5x mais rapido em CPU, suporte Microsoft futuro, TrxReport built-in
- **Tradeoffs**: Algumas ferramentas legadas nao suportam MTP ainda

## D008: FormState cascading (Havit)
- **Contexto**: Bulk enable/disable de inputs em formulario
- **Opcoes**: A) FormState cascading parameter Havit, B) Context custom por componente, C) GroupInput wrapper
- **Escolha**: A (T067)
- **Rationale**: Cascading parameter eh o pattern Blazor idiomatico, ICascadeEnabledComponent interface
- **Tradeoffs**: Requer todos inputs implementarem ICascadeEnabledComponent

## D009: HxDialogBase<TResult> (Havit)
- **Contexto**: Modal que retorna valor ao chamador (await ShowAsync)
- **Opcoes**: A) Task<DialogResult<T>> via TaskCompletionSource (Havit), B) Two-way binding + callback, C) Event-based
- **Escolha**: A (T068)
- **Rationale**: API async/await idiomatica, esconde complexidade de callback
- **Tradeoffs**: Implementacao interna complexa (~80 linhas) mas API simples

## D010: HxMessageBoxService (Havit)
- **Contexto**: Dialogs modais Yes/No/Cancel/OK com servico
- **Opcoes**: A) IHxMessageBoxService scoped DI (Havit), B) Singleton global, C) Inline por pagina
- **Escolha**: A (T069)
- **Rationale**: DI idiomatico, isolado por circuit (Blazor Server)
- **Tradeoffs**: Requer HxMessageBoxHost renderizado 1x no MainLayout

## D011: HxChangeTracker (Havit)
- **Contexto**: Re-render escopado a INotifyPropertyChanged (evita re-render de componente pai)
- **Opcoes**: A) HxChangeTracker wrapper Havit, B) INotifyPropertyChanged em cada componente, C) StateHasChanged manual
- **Escolha**: A (T070)
- **Rationale**: Wraps child content, auto subscribe/unsubscribe em OnParametersSet/Dispose
- **Tradeoffs**: Limitado a INPC, nao funciona com arbitrary change events

## D012: HxGrid + DataProvider delegate (Havit)
- **Contexto**: Server-driven grid (paging, sorting, filtering server-side)
- **Opcoes**: A) GridDataProviderDelegate<TItem> com GridDataProviderRequest/Result, B) Items binding + manual state, C) OData automatic
- **Escolha**: A (T071)
- **Rationale**: Type-safe, async, suporta grid state (sort/page/filter) + UserState save/restore
- **Tradeoffs**: Boilerplate inicial, mas compensa com reuso de server logic

## D013: HxFilterForm + HxChipGenerator (Havit)
- **Contexto**: Form de filtros que gera chips visuais
- **Opcoes**: A) HxFilterForm : HxModelEditForm + chip generators, B) Build manual, C) Form library generica
- **Escolha**: A (T072)
- **Rationale**: Reutiliza EditForm + fluent API para chips, type-safe
- **Tradeoffs**: Requer chip generator por tipo de filtro

## D014: HxSetup.Defaults (Havit)
- **Contexto**: Settings globais de todos os componentes (label type, size, etc)
- **Opcoes**: A) HxSetup.Defaults static + GetDefaults() virtual, B) DI service, C) IOptions<T> pattern
- **Escolha**: A (T066)
- **Rationale**: Static access, virtual GetDefaults() permite descendentes sobrescrever
- **Tradeoffs**: Estado global, mas eh o pattern Havit (15 anos de maturidade)

## D015: IInputWithX interfaces (Havit)
- **Contexto**: Interface Segregation - inputs com placeholder/size/labeltype
- **Opcoes**: A) Multiplas interfaces IInputWithPlaceholder, IInputWithSize, IInputWithLabelType, B) IInputSettings unica, C) Atributo marker
- **Escolha**: A (T066)
- **Rationale**: ISP correto, baixo acoplamento, permite type checks em compilacao
- **Tradeoffs**: Mais tipos para implementar, mas padrao comprovado

## D016: Resources .resx pattern (Havit)
- **Contexto**: Localizacao de strings em componentes (Placeholders, errors)
- **Opcoes**: A) Per-component .resx files (Havit T074), B) Global Resources, C) JSON files
- **Escolha**: A (T074)
- **Rationale**: Cada componente tem seus resources, 6 linguas built-in (en/de/cs/uk/zh-CN/it-IT)
- **Tradeoffs**: Mais arquivos, mas organizado, code-gen friendly

## D017: BootstrapIconsGenerator (Havit)
- **Contexto**: Type-safe access aos 2000+ Bootstrap icons
- **Opcoes**: A) IIncrementalGenerator parseando JSON (Havit T076), B) Runtime lookup, C) string constants
- **Escolha**: A (T076)
- **Rationale**: Incremental SG rapido, type-safe HxIcon.Bootstrap.Palette, intellisense
- **Tradeoffs**: Build time +20ms, recompilacao ao mudar JSON

## D018: SmartComponents wrapping (Havit)
- **Contexto**: AI upgrade para ComboBox/PasteButton/TextArea
- **Opcoes**: A) HxSmartComboBox/PasteButton/TextArea wrapping SmartComponents MS, B) XFAi* unico namespace, C) AI inline em cada componente
- **Escolha**: A (T077)
- **Rationale**: Reutiliza SmartComponents MS oficial, Hx bootstrap styling
- **Tradeoffs**: Dependencia preview (SmartComponents 0.1.0-preview10148)

## D019: AddHxServices + TimeProvider (Havit)
- **Contexto**: Service registration padrao para todos componentes Havit
- **Opcoes**: A) AddHxServices + TimeProvider.System (Havit T073), B) AddScope manual por servico, C) AddSingleton
- **Escolha**: A (T073)
- **Rationale**: 1 chamada, 1 servico (TimeProvider) injetado em qualquer componente
- **Tradeoffs**: Adiciona Microsoft.Extensions.Hosting como dep transitiva

## D020: VSTHRD analyzer (Havit)
- **Contexto**: Threading safety enforcement em codigo async
- **Opcoes**: A) Microsoft.VisualStudio.Threading.Analyzers 17.14.15, B) Manual code review, C) Polly resilience
- **Escolha**: A (T032)
- **Rationale**: Detecta .Result/.Wait()/async-void/Try-catch-Task em build time
- **Tradeoffs**: Build time +5s, alguns falsos positivos em StartNew


## D021: ThemeService abstract (Rad)
- **Contexto**: Multi-theme com persistencia
- **Opcoes**: A) ThemeService abstract + Cookie/QueryString impls (Rad T064), B) IConfiguration binding, C) Database persisted
- **Escolha**: A (T064)
- **Rationale**: SSR-compatible, cookie/querystring swap, teste-friendly
- **Tradeoffs**: Nenhuma, alinha com pattern Radzen

## D022: AIChatService SSE (Rad)
- **Contexto**: AI chat streaming com completion iterativo
- **Opcoes**: A) IAsyncEnumerable<string> + SSE parsing (Rad T060), B) SignalR hub, C) WebSocket
- **Escolha**: A (T060)
- **Rationale**: HTTP standard, sem dep extra, suporta proxy/endpoint custom
- **Tradeoffs**: Requer handling de reconnect manual

## D023: Snippets.generated.cs (Blz)
- **Contexto**: Build-time geracao de code snippets para docs
- **Opcoes**: A) Roslyn SG parses Examples/ folder (Blz T046), B) Manual .md files, C) DocFX generators
- **Escolha**: A (T046)
- **Rationale**: Single source of truth (Examples/*.razor), build-time
- **Tradeoffs**: ~50 exemplos requerem format consistente

## D024: Docs.Mcp server (Blz + Havit)
- **Contexto**: MCP server live para AI agents consumirem docs
- **Opcoes**: A) Self-hosted MCP server (Blz T053 + Havit T065), B) Cloud-only (Rad), C) No MCP
- **Escolha**: A
- **Rationale**: Self-hosted = data privacy, custom tools, no vendor lock-in
- **Tradeoffs**: 4-6 tools para implementar (catalog, docs, samples, type)

## D025: Nao migrar (decisao explicita)
- **Contexto**: Decisoes do que NAO adotar
- **Lista de NAO**:
  - **NRT off** (Havit) - XForge USA NRT, manter
  - **Multi-TFM 7** (antd) - 3 TFMs (net8/9/10) suficiente
  - **Source-available license** (Blz) - MIT puro
  - **Cloud MCP** (Rad) - self-hosted
  - **gRPC code-first** (Havit T036) - YAGNI para Design System, nao eh backend
  - **ApplicationInsights** (Havit) - reusar OpenTelemetry T027
  - **protobuf-net.Grpc** (Havit) - YAGNI
  - **LoxSmoke.DocXml** (Havit) - reusar Microsoft.CodeAnalysis do T062
  - **EditAppTest legacy** (Havit) - deletar equivalente XForge
  - **Havit.AspNetCore.Mvc.Testing** (Havit) - usar WebApplicationFactory built-in
  - **BlazorAppTest** (Havit) - remover, manter apenas TestApp
  - **Havit.Core + Havit.AspNetCore** (Havit) - reusar Microsoft.Extensions only
  - **Havit.Storage localStorage** (Havit) - YAGNI v2.0.0

