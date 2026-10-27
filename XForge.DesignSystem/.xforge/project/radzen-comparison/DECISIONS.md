# Decisões Técnicas — Radzen Blazor (4-way consolidated)

## D001: ParameterState<T> (MudBlazor)
- **Contexto**: Reduzir boilerplate de `[Parameter]+OnParameterSet+SetParametersAsync`
- **Opções**: A (custom) | B (ParameterState<T> MudBlazor) | C (Roslyn SG)
- **Escolha**: **B (ParameterState<T>)**
- **Rationale**: ~70% boilerplate reduction, battle-tested em 9.5k stars
- **Tradeoff**: Lock-in ao pattern MudBlazor, mas é só API
- **Origem**: MudBlazor

## D002: Analyzers Strategy (MudBlazor + Blazorise)
- **Contexto**: Validar uso de componentes em compile-time
- **Opções**: A (4 MudBlazor) | B (Blazorise.Analyzers 3) | C (merge both)
- **Escolha**: **C (merge)**
- **Rationale**: MudBlazor tem 4, Blazorise tem 3 — complementa
- **Tradeoff**: 16h de implementação (T002)
- **Origem**: MudBlazor + Blazorise

## D003: DomEventService (antd-blazor)
- **Contexto**: Centralizar event handling no DOM
- **Opções**: A (per-component handlers) | B (singleton service) | C (event delegation)
- **Escolha**: **B (singleton service)**
- **Rationale**: Reuso, performance, testability
- **Tradeoff**: Estado global pode vazar; usar scoped por circuit
- **Origem**: antd-blazor

## D004: Localized XML Intellisense (antd-blazor)
- **Contexto**: IntelliSense em múltiplos idiomas
- **Opções**: A (en only) | B (6 langs) | C (3 langs LATAM)
- **Escolha**: **C (3 langs: pt-BR, en, es)**
- **Rationale**: LATAM-first, ja/ko/zh/ru deferred
- **Tradeoff**: 24h setup; langs extras em v2.1
- **Origem**: antd-blazor

## D005: Provider-agnostic CSS (Blazorise)
- **Contexto**: Suportar múltiplos temas (Tailwind, Bootstrap, etc)
- **Opções**: A (Tailwind only) | B (IClassProvider/IStyleProvider) | C (CSS-in-JS)
- **Escolha**: **B (foundation) + A (YAGNI 7 providers)**
- **Rationale**: Preparado para futuro, sem over-engineering
- **Tradeoff**: 32h upfront (T044), 7 providers fica para v2.1
- **Origem**: Blazorise

## D006: ShouldAutoGenerateId + Attributes (Blazorise)
- **Contexto**: ID único para todos componentes (a11y, JS)
- **Opções**: A (manual) | B (ShouldAutoGenerateId) | C (Roslyn SG)
- **Escolha**: **B (ShouldAutoGenerateId virtual)**
- **Rationale**: Foundation, 4h implementation
- **Tradeoff**: Componentes customizados devem implementar override
- **Origem**: Blazorise

## D007: bUnit version 1.36+ (Radzen ⭐)
- **Contexto**: Testes de componentes Blazor
- **Opções**: A (bUnit 1.34) | B (bUnit.web 1.36) | C (playwright component testing)
- **Escolha**: **B (bUnit.web 1.36+)**
- **Rationale**: Radzen usa; melhorias de API e net10 support
- **Tradeoff**: Breaking changes de 1.34→1.36 (avaliar)
- **Origem**: **Radzen**

## D008: AddRadzenComponents() Pattern (Radzen ⭐)
- **Contexto**: Como o usuário registra serviços
- **Opções**: A (manual) | B (AddRadzenComponents extension) | C (auto via reflection)
- **Escolha**: **B (AddXForgeComponents() extension)**
- **Rationale**: Idiomático .NET, type-safe, discoverable
- **Tradeoff**: 1h setup (T063)
- **Origem**: **Radzen**

## D009: Debouncer com System.Timers (Radzen ⭐)
- **Contexto**: Debounce search/filter input
- **Opções**: A (CancellationTokenSource) | B (System.Timers.Timer) | C (Channel<T>)
- **Escolha**: **B (System.Timers.Timer)**
- **Rationale**: Simpler API, debounce + throttle built-in
- **Tradeoff**: Não-cancelable uma vez iniciado; OK para UI
- **Origem**: **Radzen**

## D010: OnBecameInvisible Pattern (Radzen ⭐)
- **Contexto**: Componentes com Visible=false (Dialog, Drawer, Modal)
- **Opções**: A (Dispose) | B (OnBecameInvisible cleanup) | C (keep JS alive)
- **Escolha**: **B (cleanup sem dispose)**
- **Rationale**: Componente toggleable sem re-mount = melhor perf
- **Tradeoff**: JS handlers precisam ser reattached on Visible=true
- **Origem**: **Radzen**

## D011: ClassList struct-based (Radzen ⭐)
- **Contexto**: Fluent CSS class builder
- **Opções**: A (StringBuilder) | B (struct ClassList) | C (CssBuilder MudBlazor)
- **Escolha**: **B (struct ClassList) + A (StringBuilderCache)**
- **Rationale**: Zero-alloc no hot path, fluent API ergonômica
- **Tradeoff**: 4h implementação (T059), 2h StringBuilderCache (T058)
- **Origem**: **Radzen**

## D012: AIChatService SSE streaming (Radzen ⭐)
- **Contexto**: Streaming de respostas de IA
- **Opções**: A (HttpClient + SSE manual) | B (IAsyncEnumerable + primary ctor) | C (SignalR)
- **Escolha**: **B (IAsyncEnumerable + primary ctor)**
- **Rationale**: Idiomatic C# 12, EnumeratorCancellation, session memory
- **Tradeoff**: Lock-based session dict (não é multi-server)
- **Origem**: **Radzen**

## D013: ThemeService abstract + 2 impls (Radzen ⭐)
- **Contexto**: Persistência do tema escolhido
- **Opções**: A (cookie only) | B (abstract + 2 impls) | C (localStorage)
- **Escolha**: **B (abstract ThemeService)**
- **Rationale**: Strategy pattern, testável, pluggable storage
- **Tradeoff**: 4h implementação (T064)
- **Origem**: **Radzen**

## D014: MSBuild via Radzen.MSBuild (Radzen ⭐)
- **Contexto**: Build-time SCSS compilation + JS minification
- **Opções**: A (manual) | B (Radzen.MSBuild 0.0.6) | C (custom Target in csproj)
- **Escolha**: **C (custom Target inline)**
- **Rationale**: Não usar 0.0.6 (pre-alpha); preferir Microsoft.Web.LibraryManager + custom Target
- **Tradeoff**: Mais código, controle total
- **Origem**: **Radzen (adaptado)**

## D015: API Generator (Radzen ⭐)
- **Contexto**: Auto-gerar páginas de API reference
- **Opções**: A (manual) | B (Radzen.Api.Generator CLI) | C (Roslyn SG in-process)
- **Escolha**: **C (Roslyn SG in-process)**
- **Rationale**: Integrado ao build, sem processo externo
- **Tradeoff**: 24h implementação (T062), mais complexo
- **Origem**: **Radzen (adaptado)**

## D016: Cloud-hosted MCP (Radzen ⭐)
- **Contexto**: MCP server para AI assistants
- **Opções**: A (no MCP) | B (self-hosted) | C (cloud-hosted)
- **Escolha**: **B (self-hosted)**
- **Rationale**: Custo, LGPD, controle
- **Tradeoff**: 16h implementação (T065)
- **Origem**: **Radzen (adaptado)**

## D017: 4 TFMs (Radzen ⭐)
- **Contexto**: Suporte a múltiplas versões .NET
- **Opções**: A (net10 only) | B (net7/8/9/10) | C (net8/9/10)
- **Escolha**: **C (net8/9/10)**
- **Rationale**: net7 é EOL; manter 3 LTS
- **Tradeoff**: Mais testes por TFM
- **Origem**: **Radzen (filtrado)**

## D018: PersistentComponentState para Theme (Radzen ⭐)
- **Contexto**: SSR-friendly theme persistence
- **Opções**: A (cookie only) | B (PersistentComponentState + cookie) | C (localStorage only)
- **Escolha**: **B (both)**
- **Rationale**: SSR-safe + cross-request
- **Tradeoff**: Complexidade extra
- **Origem**: **Radzen**

## D019: Internal Visibility for Tests (Radzen ⭐)
- **Contexto**: Testar membros internal
- **Opções**: A (public) | B (InternalsVisibleTo) | C (Reflection)
- **Escolha**: **B (InternalsVisibleTo)**
- **Rationale**: Encapsulamento + testabilidade
- **Tradeoff**: Coupling entre lib e test project
- **Origem**: **Radzen**

## D020: Não usar `Radzen Blazor Pro` (Radzen ⭐)
- **Contexto**: Premium features (Studio, MCP, themes)
- **Opções**: A (pagar Pro) | B (free only) | C (selective inspiration)
- **Escolha**: **C (selective inspiration, MIT puro)**
- **Rationale**: Manter open source 100% MIT
- **Tradeoff**: Sem Studio, sem Pro MCP (mas fazemos nosso)
- **Origem**: **Radzen (decisão consciente)**