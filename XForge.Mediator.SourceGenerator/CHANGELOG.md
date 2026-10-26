# Changelog — XForge.Mediator.SourceGenerator

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/).

## [0.1.0] - 2026-10-26

### Adicionado
- Source generator (IIncrementalGenerator) que descobre `IRequestHandler<TRequest, TResponse>`
  (incluindo `IRequestHandler<TRequest>` = `Unit`) na compilação do usuário — sem atributo obrigatório.
- Registry de dispatch gerado (`XForgeGeneratedRegistry`) com um wrapper tipado por par
  (request, resposta) montando a cadeia de `IPipelineBehavior<,>` do DI na ordem de registro.
- `XForgeGeneratedSender : ISender` completo (`Send<TResponse>(IRequest<TResponse>)`,
  `Send<TResponse>(object)` e `Send(object)`), com `HandlerNotFoundException` do próprio runtime.
- Extensão `AddXForgeMediatorGenerated(this IServiceCollection)` — registro dos handlers
  (transient) e do sender, sem assembly scanning.
- Opt-out por classe via `[XForgeMediatorIgnore]` (atributo do pacote de runtime).
- 6 testes com harness Roslyn real: compila o código gerado, carrega o assembly e valida o
  dispatch, a variante `Unit`, o opt-out e o encadeamento de behaviors em runtime.

### Limitações (v0.1) — use o pacote de runtime nestes casos
- Streams (`CreateStream` lança `NotSupportedException` explicativo).
- Notifications, pre/post processors, exception handlers e demais features do runtime
  (`services.AddXForgeMediator(...)`).
