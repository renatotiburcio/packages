# Changelog — XForge.Mediator

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/).

## [0.1.0] - 2026-10-22

### Adicionado
- Contratos: `IRequest<T>`, `IRequest`, `ICommand`, `IQuery`, `INotification`, `IStreamRequest<T>`
  (marcadores de conveniência `ICommand`/`IQuery` derivam de `IRequest` — o dispatch só depende de `IRequest`).
- Handlers: `IRequestHandler<TReq,TRes>`, `IRequestHandler<TReq>` (Unit), `IStreamRequestHandler<TReq,TRes>`, `INotificationHandler<T>`.
- Dispatch: `Send` (genérico, runtime-typed e com resposta explícita), `Publish` (multicast), `CreateStream` — `ValueTask` end-to-end.
- Pipeline: `IPipelineBehavior`, `IStreamPipelineBehavior`, pré/pós processadores, exception actions/handlers com estado
  — pré/pós auto-registrados apenas quando existem; processamento de exceções integrado ao wrapper (custo zero sem exceção).
- Publicação: `INotificationPublisher` com `TaskWhenAllPublisher` (fast path síncrono) e `ForeachAwaitPublisher`.
- Registro: `AddXForgeMediator()` de 1 linha ou configuração rica (scan por assemblies, `TypeEvaluator`, `Lifetime`,
  open generics, `[XForgeMediatorIgnore]`).
- Diagnóstico opt-in (`ActivitySource` + `Meter`).
- AOT: caminho genérico AOT-safe; overloads dinâmicos anotados (`RequiresDynamicCode`/`RequiresUnreferencedCode`).
- Multi-target: `netstandard2.0` + `net8.0`.
- Benchmarks (BenchmarkDotNet): 0.55–0.79× do tempo e 0.16–0.47× das alocações do MediatR 12.5 em todos os cenários.
- 39 testes + benchmarks + documentação de análise REA de 7 bibliotecas da categoria.
