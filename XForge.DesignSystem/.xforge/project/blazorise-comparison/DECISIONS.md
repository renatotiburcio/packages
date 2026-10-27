# Decisões Técnicas — Blazorise vs XForge (consolidado 3-way)

## D001: ParameterState<T> framework
- **Contexto**: Setter boilerplate (IsEqual + Dirty em todo [Parameter])
- **Opções**: A) Manter pattern atual; B) MudBlazor ParameterState; C) Source generator
- **Escolha**: B (ParameterState<T>)
- **Rationale**: Reduz ~70% boilerplate, type-safe, usado em produção por 10.4k stars
- **Tradeoffs**: Adiciona 1 camada de abstração; requer testes de regressão
- **Origem**: MudBlazor

## D002: Roslyn Analyzers em compile-time
- **Contexto**: Validar [Parameter], [Category], naming conventions no PR
- **Opções**: A) Sem analyzer; B) MudBlazor 4-diag; C) Blazorise.Analyzers
- **Escolha**: B (estudar ambos e merge)
- **Rationale**: Previne bugs antes do runtime; economiza QA
- **Tradeoffs**: Complexidade de build, mais um pacote
- **Origem**: MudBlazor + Blazorise

## D003: DomEventService vs @onclick direto
- **Contexto**: Centralizar JS interop para DOM events
- **Opções**: A) @onclick direto; B) DomEventService singleton
- **Escolha**: B
- **Rationale**: Reuso, teste, mocking; performance melhor
- **Tradeoffs**: Mais código upfront
- **Origem**: antd-blazor

## D004: Localized XML Intellisense
- **Contexto**: Documentação em múltiplos idiomas
- **Opções**: A) 1 idioma; B) 3 (pt-BR/en/es); C) 6 (antd)
- **Escolha**: B (3 idiomas, mercado-alvo)
- **Rationale**: XForge é LATAM-focused; pt-BR + en + es cobrem 90%
- **Tradeoffs**: ja/ko/zh/ru podem ser v2.1
- **Origem**: antd-blazor

## D005: Provider-agnostic CSS abstraction
- **Contexto**: Suportar múltiplos CSS frameworks (futuro)
- **Opções**: A) Hardcoded Tailwind; B) IClassProvider/IStyleProvider DI
- **Escolha**: B
- **Rationale**: Preparar para Bootstrap/Material sem reescrever
- **Tradeoffs**: +32h upfront, mas YAGNI para v2.0.0 (só Tailwind)
- **Origem**: Blazorise
- **Nota**: Implementar mas manter 1 provider default

## D006: ShouldAutoGenerateId em base
- **Contexto**: Cada componente precisa de ID único para a11y/forms
- **Opções**: A) Cada componente gera; B) Virtual ShouldAutoGenerateId na base
- **Escolha**: B
- **Rationale**: Consistente, opt-in por override
- **Tradeoffs**: Mínimo (4h)
- **Origem**: Blazorise

## D007: CssBuilder fluent API
- **Contexto**: Construção de class string complexa
- **Opções**: A) string interpolation; B) CssBuilder
- **Escolha**: B
- **Rationale**: Type-safe, cacheable, dirty tracking
- **Tradeoffs**: Mínimo (4h)
- **Origem**: MudBlazor

## D008: Multi-TFM (net8/9/10) vs single TFM
- **Contexto**: Compatibilidade com diferentes .NET versions
- **Opções**: A) net10 only; B) net8/9/10; C) 7 TFMs (antd)
- **Escolha**: B (3 TFMs)
- **Rationale**: 95% compat, baixo overhead
- **Tradeoffs**: +16h build config
- **Origem**: MudBlazor/consenso

## D009: Docs generation strategy
- **Contexto**: Manter docs sincronizadas com API
- **Opções**: A) Manual; B) Roslyn source-gen em compile-time; C) Standalone CLI (antd)
- **Escolha**: B (Roslyn em compile-time, padrão Blazorise)
- **Rationale**: 0 configuração extra no consumer, sempre atualizado
- **Tradeoffs**: +32h
- **Origem**: Blazorise + MudBlazor.UnitTests.Docs.Generator

## D010: Docs.Mcp server (6 tools, 3 transports)
- **Contexto**: AI assistants precisam de docs programaticamente
- **Opções**: A) Sem MCP; B) MCP streamable HTTP only; C) 3 transports
- **Escolha**: C (Streamable HTTP + SSE + Stdio)
- **Rationale**: Max compatibility (Claude/Cursor/VS Code/windsurf)
- **Tradeoffs**: +32h, mas Blazorise já tem em prod (mcp.blazorise.com)
- **Origem**: Blazorise (e antd-blazor PR #4758)

## D011: Snippets.generated.cs (executable samples)
- **Contexto**: Code samples em docs devem compilar
- **Opções**: A) Markdown manual; B) .razor files compilados em test
- **Escolha**: B
- **Rationale**: Docs always correct, CI catches broken samples
- **Tradeoffs**: +24h, requer Docs.Compiler
- **Origem**: Blazorise

## D012: FluentValidation integration
- **Contexto**: Validação de forms
- **Opções**: A) DataAnnotations only; B) FluentValidation; C) Both
- **Escolha**: B (FluentValidation como provider)
- **Rationale**: Industry standard, fluent, testable
- **Tradeoffs**: +16h integration
- **Origem**: Blazorise (Blazorise.FluentValidation)

## D013: Fody Weaver vs Roslyn Source Generator
- **Contexto**: Auto-Dirty() em [Parameter] setters
- **Opções**: A) Manual; B) Fody (IL post-process); C) Roslyn SG (compile-time)
- **Escolha**: C (Roslyn SG)
- **Rationale**: Type-safe, sem IL magic, melhor debug
- **Tradeoffs**: +16h
- **Origem**: Blazorise (Fody) → Roslyn

## D014: Centralized package versions
- **Contexto**: Múltiplos projetos referenciam mesmos pacotes
- **Opções**: A) Version em cada csproj; B) Directory.Packages.props central
- **Escolha**: B
- **Rationale**: Single source of truth, 1 atualização
- **Tradeoffs**: Mínimo (2h)
- **Origem**: Blazorise (managePackageVersionsCentrally)

## D015: Licensing model
- **Contexto**: XForge v2.0.0 open source ou dual-license?
- **Opções**: A) MIT (XForge atual); B) Dual (Blazorise)
- **Escolha**: A (MIT)
- **Rationale**: Mantém comunidade, LGPL-compatible
- **Tradeoffs**: Sem revenue stream de licenças
- **Origem**: XForge

## D016: Multi-CSS provider strategy
- **Contexto**: Suportar Bootstrap/Tailwind/Ant/Material simultaneamente
- **Opções**: A) 1 framework (atual Tailwind); B) 2-3 selecionados; C) 7 (Blazorise)
- **Escolha**: A (manter 1 framework default, design preparado)
- **Rationale**: YAGNI; 95% dos users usam 1
- **Tradeoffs**: Sem vendor flexibility
- **Origem**: XForge v2.0.0

## D017: Extensions strategy (38 → ?)
- **Contexto**: Quais extensions do Blazorise importar?
- **Opções**: A) 0 (não fazer); B) 5-10; C) Todas
- **Escolha**: B (5-10 selecionados)
- **Rationale**: ROI de cada extension, evitar bloat
- **Tradeoffs**: Análise de demanda necessária
- **Origem**: Blazorise

## D018: Commercial license pattern (Blazorise.Licensing)
- **Contexto**: Adicionar telemetria de uso?
- **Opções**: A) Sem telemetria; B) Opt-in analytics
- **Escolha**: A
- **Rationale**: LGPD, privacidade
- **Tradeoffs**: Sem dados de uso
- **Origem**: XForge v2.0.0
