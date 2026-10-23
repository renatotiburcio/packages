# Análise REA — Consolidação dos pacotes mediadores

> Análise estática (shipped-artifact) via MCP **REA** (`inspect_managed_artifact` + `inspect_managed_members`)
> dos pacotes NuGet reais baixados de `api.nuget.org` (`.nupkg` → `lib/**.dll`).
> Evidências: metadados ECMA-335 completos (tipos, campos, métodos, assinaturas, literais `#US`, caches de implementação).
> Estático não carrega/executa o assembly; comportamento de runtime é inferência rotulada.

## 1. Pacotes analisados

| Pacote | Versão | DLL analisada | TFM | Tipos | Métodos | Evidência (ev_) |
|---|---|---|---|---|---:|---:|
| MediatR (LuckyPenny/jbogard) | 12.5.0 (última Apache) | `lib/netstandard2.0/MediatR.dll` | netstandard2.0 | 99 | 411 | `4cd59858…1eb` |
| MediatR | 14.2.0 (comercial) | `lib/net8.0/MediatR.dll` | net8.0 | 108 | 469 | `48611139…5dc` |
| Mediator (martinothamar) | 3.0.2 | `lib/net8.0/Mediator.dll` (Abstractions) | net8.0 | 59 | 109 | `f178682e…6ce` |
| Mediator.Net (mayuanyang) | 5.0.0 | `lib/net8.0/Mediator.Net.dll` | net8.0 | 89 | 272 | `fdd452fd…deb` |
| MassTransit (Abstractions) | 9.2.4 | `lib/net8.0/MassTransit.Abstractions.dll` | net8.0 | 1398 | 6099 | `b99cc579…c1d` |
| FluentMediator (ivanpaulovich) | 0.4.7 | `lib/netstandard1.1/FluentMediator.dll` | netstandard1.1 | 76 | 176 | `af127150…039` |
| FluentMediator.DI | 0.4.7 | `lib/netstandard1.1/…DI.dll` | netstandard1.1 | 8 | 20 | `cfd14614…298` |
| Arbiter.Mediation (loresoft) | 6.13.1 | `lib/net8.0/Arbiter.Mediation.dll` | net8.0 | 38 | 77 | `90bfc62b…24d` |

Obs.: o ID NuGet `Mediator` puro pertence a outro projeto (Rui Carvalho, “framework html em tempo real”) —
o pacote do martinothamar é `Mediator.Abstractions` (+ `Mediator.SourceGenerator`).

## 2. Superfície observada por biblioteca

### 2.1 MediatR 12.5.0 — a referência de simplicidade (namespace `MediatR`)

**Contratos** (evidência: tabela TypeDef + assinaturas de métodos):
- `IMediator : ISender, IPublisher`
- `ISender`: `Send<TResponse>(IRequest<TResponse>, ct) → Task<TResponse>`; `Send(object, ct) → Task<object>`; `Send<TResponse>(object, ct)`; `CreateStream<TResponse>(IStreamRequest<TResponse>, ct) → IAsyncEnumerable<TResponse>`; `CreateStream(object, ct)`
- `IPublisher`: `Publish<TNotification>(TNotification, ct) → Task`; `Publish(object, ct) → Task`
- Mensagens: `IRequest`, `IRequest<TResponse>`, `IStreamRequest<TResponse>`, `INotification`
- Handlers: `IRequestHandler<TRequest,TResponse>` (`Handle → Task<TRes>`), `IRequestHandler<TRequest>` (→ `Task`), `IStreamRequestHandler<TReq,TRes>`, `INotificationHandler<T>`
- Delegates: `RequestHandlerDelegate<TResponse> = Func<ct, Task<TRes>>`, `StreamHandlerDelegate<TResponse>`
- Behaviors: `IPipelineBehavior<TRequest,TResponse>.Handle(TReq, next, ct)`, `IStreamPipelineBehavior<TReq,TRes>`
- Pipeline avançado (`MediatR.Pipeline`): `IRequestPreProcessor<T>`, `IRequestPostProcessor<TReq,TRes>`,
  `IRequestExceptionAction<TReq,TEx>`, `IRequestExceptionHandler<TReq,TRes,TEx>` + `RequestExceptionHandlerState<TRes>`,
  com 4 behaviors concretos (pre/post/exception-action/exception-handler)
- Publicação: `INotificationPublisher.Publish(IEnumerable<NotificationHandlerExecutor>, TNotification, ct)`;
  estratégias `ForeachAwaitPublisher` (sequencial) e `TaskWhenAllPublisher` (paralelo);
  `NotificationHandlerExecutor` = `record(object HandlerInstance, Func<object, ct, Task> HandlerCallback)`

**Implementação** (evidência: campos de `MediatR.Mediator`):
- `_serviceProvider`, `_publisher` + 3 caches estáticos:
  `_requestHandlers : ConcurrentDictionary<Type, RequestHandlerBase>`,
  `_notificationHandlers : ConcurrentDictionary<Type, NotificationHandlerWrapper>`,
  `_streamRequestHandlers : ConcurrentDictionary<Type, StreamRequestHandlerBase>`
- Wrappers genéricos singleton (`RequestHandlerWrapperImpl<TReq,TRes>`, `NotificationHandlerWrapperImpl<T>`,
  `StreamRequestHandlerWrapperImpl<TReq,TRes>`) constróem a cadeia de behaviors por chamada e são cacheados por tipo.

**Registro** (evidência: `MediatRServiceConfiguration` — 19 propriedades + métodos):
`AddMediatR(cfg => …)` com `Lifetime`, `TypeEvaluator`, `MediatorImplementationType`, `NotificationPublisher(Type)`,
`AssembliesToRegister`, `AddBehavior/AddOpenBehavior`, `AddStreamBehavior/AddOpenStreamBehavior`,
`AddRequestPreProcessor/AddOpenRequestPreProcessor`, `AddRequestPostProcessor/AddOpenRequestPostProcessor`,
`AutoRegisterRequestProcessors`, `RegisterGenericHandlers`, cotas de escaneamento (`MaxGenericTypeParameters`,
`MaxTypesClosing`, `MaxGenericTypeRegistrations`, `RegistrationTimeout`) e `RequestExceptionActionProcessorStrategy`.

### 2.2 MediatR 14.2.0 — evolução comercial
Mesma API do 12.5 (evidência: diff de tipos), acrescida de `MediatR.Licensing.*` (`License`, `LicenseValidator`,
`Edition`, `ProductType`, `BuildInfo`) e renomeação `ServiceCollectionExtensions` → `MediatRServiceCollectionExtensions`.
**Conclusão:** a API estável de referência é a do 12.5; o 13+ muda licença, não recursos.

### 2.3 Mediator 3.0.2 (martinothamar) — máximo desempenho via source generator
- Mensagens com semântica explícita: `IRequest/ICommand/IQuery/INotification` + marcadores `IBaseRequest/IBaseCommand/IBaseQuery`
  (para open generics) e variantes de stream `IStreamRequest/IStreamCommand/IStreamQuery`.
- `IMediator : ISender, IPublisher` — **tudo `ValueTask`** (3 overloads de `Send` + 3 de `CreateStream`).
- `Unit` = `readonly struct` com `Value` (`ref`) e `Unit.ValueTask` pronta — zero alocação.
- Behaviors: `IPipelineBehavior<TMsg,TRes>`, `IStreamPipelineBehavior`, classes base `MessagePreProcessor<TMsg,TRes>`,
  `MessagePostProcessor<TMsg,TRes>` e tratamento de exceção `MessageExceptionHandler<TMsg,TRes,TEx>`
  (`Handle` + `Handled` + `ExceptionHandlingResult<T>`), `MessageHandlerDelegate<TMsg,TRes>`.
- Exceções: `InvalidMessageException`, `MissingMessageHandlerException`.
- `INotificationPublisher` + `ForeachAwait`/`TaskWhenAll` (mesma estrutura do MediatR).
- O dispatch é gerado em código (switch por tipo) — sem reflection no caminho quente; o runtime mantém apenas contratos.

### 2.4 Mediator.Net 5.0.0 (mayuanyang) — pipes por tipo de mensagem
- Contratos separados por papel: `ICommand`, `IEvent`, `IRequest`+`IResponse` (+ `IStreamRequest`).
- Handlers: `ICommandHandler<T>`/`ICommandHandler<T,TRes>`/`IEventHandler<T>`/`IRequestHandler<T,TRes>`/`IStreamCommandHandler<,>`/`IStreamRequestHandler<,>`.
- `IMediator`: `SendAsync` (3 overloads), `PublishAsync` (2), `RequestAsync` (2), `CreateStream` (2) — `Task` (não `ValueTask`).
- **Pipelines distintos** por fluxo: `ConfigureGlobalReceivePipe/CommandReceivePipe/EventReceivePipe/RequestPipe/PublishPipe`
  com `IPipeSpecification<T>` e configuradores — middleware granular por tipo de mensagem.
- `IDependencyScope` (abstração própria de DI com `BeginScope`), `MediatorBuilder.RegisterHandlers` (scan),
  contexto de recebimento (`IReceiveContext<T>`, `IPublishContext<T>`), `MessageHandlerRegistry`/`MessageBinding`,
  exceções `NoHandlerFoundException`/`MoreThanOneHandlerException`.

### 2.5 MassTransit 9.2.4 (Abstractions) — mensageria distribuída, não in-process
1.398 tipos/31 namespaces (evidência agregada por namespace). Núcleo relevante ao padrão mediator:
- `IBus`, `IPublishEndpoint`, `ISendEndpointProvider`, `IRequestClient<T>` (request/response sobre transport com timeout),
  `RequestHandle`, `Response<T>`, `RequestTimeout`, `RequestFaultException`.
- `IConsumer<T>`, `IConsumerFactory`, `ConsumerDefinition`, sagas (`ISaga`, state machines), `IFilter<TContext>`/`Pipe`
  (middleware com observers), routing slips (`Courier`), `Fault<T>`, JobService, scheduling, message data, topology.
- Namespace `MassTransit.Mediator` — o próprio mediator in-memory (`AddMediator`), confirmando que o “mediator” é um subconjunto.
**Uso na consolidação:** inspiration para observers/filters e semântica de timeout/fault — mas transport, saga, outbox e
routing slips ficam **de fora** por escopo (o XForge.Mediator é in-process).

### 2.6 FluentMediator 0.4.7 — pipelines explícitos fluentes
- `IMediator`/`ISyncMediator`/`IAsyncMediator`/`ICancellableMediator`.
- Registro fluente por mensagem: `IPipelineAsyncBuilder<TRequest>.Call<THandler>((h, r) => …)` e
  `.Return<TResponse, THandler>((h, r) => …)` → `Direct<TRequest, THandler, TResponse>` (lambda encadeada;
  evidência: campos `_methods : List<Func<Direct…>>`, `RequestType`, `Name`).
- Múltiplos handlers por mensagem (pipeline “long-running”), pipelines nomeados (`INamedPipeline`),
  hook de fallback `PipelineNotFoundEventArgs`, estratégias sync/async/cancellable em namespaces separados,
  `IPipelineBehavior<TRequest>` (1 genérico apenas).
**Conclusão:** flexível, mas desacoplado de convenções (sem scan automático; cada pipeline é manual) — bom para
roteamento custom, ruim como padrão único.

### 2.7 Arbiter.Mediation 6.13.1 (loresoft) — MediatR-like moderno e enxuto
- 38 tipos apenas: `IMediator` (`Send<TReq,TRes>`/`Send<TRes>(object)`/`Send(object)`/`Publish<T>` — `ValueTask`,
  confirmado pela compilação dos benchmarks de comparação), `IRequest`/`IRequest<TRes>` (marcadores separados,
  `IRequest` **não** herda de `IRequest<Unit>`), `INotification`, `INotificationHandler<T>`, `IPipelineBehavior<TReq,TRes>`,
  `IRequestHandler<TReq,TRes>`, `RequestHandlerDelegate<T>`, `Unit` (record struct).
- Implementação: `_handlerCache : ConcurrentDictionary<Type, Func<IServiceProvider, object?>>` + `_serviceProvider`.
- **Observabilidade nativa** (evidência: `MediatorTelemetry`, 17 campos): `ActivitySource("Arbiter.Mediation")` +
  `Meter` com `mediator.send.count`, `mediator.publish.count`, `mediator.send.duration`, `mediator.publish.duration`
  e tags (`request_type`, `response_type`, `handler`, `behavior`, `operation`); contadores/durações também
  medidos via `Interlocked` (`_sendCounter`, `_sendDuration`).
- **AOT**: anotações `RequiresDynamicCode`/`RequiresUnreferencedCode` nos overloads dinâmicos (`Send(object)`)
  (evidência: campos estáticos `DynamicSendRequiresDynamicCode/UnreferencedCode`).
- `AddMediator(services, cfg?)` — registro de uma linha.

## 3. Matriz de recursos consolidada

| Recurso | MediatR | Mediator | Mediator.Net | MassTransit | FluentMediator | Arbiter | **XForge.Mediator** |
|---|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
| Request/response (`Send`) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Comando sem resposta (`Unit`) | ✅ | ✅ (struct) | ✅ | ✅ | ✅ | ✅ (struct) | ✅ (struct) |
| Notificações multicast (`Publish`) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Streaming `IAsyncEnumerable` | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Pipeline behaviors (request + stream) | ✅ | ✅ | ✅ (pipes por tipo) | ✅ (filters) | ✅ (1 gen.) | ✅ | ✅ |
| Pré/pós processadores auto-registrados | ✅ | ✅ (bases) | via pipes | filters | ❌ | ❌ | ✅ (auto quando existem) |
| Tratamento de exceção no pipeline | ✅ (actions+handlers+estado) | ✅ | pipes | faults | ❌ | ❌ | ✅ (actions+handlers+estado) |
| Estratégia de publicação configurável | ✅ (seq/paralelo) | ✅ | sequencial | transport | sequencial | sequencial | ✅ (seq/paralelo/custom) |
| Dispatch por runtime type (`object`) | ✅ | ✅ | ✅ | ✅ | ❌ | ✅ | ✅ |
| Marcadores CQRS (`ICommand`/`IQuery`) | ❌ | ✅ | ✅ (ICommand/IEvent) | ❌ | ❌ | ❌ | ✅ (conveniência) |
| DI agnóstica no núcleo (IServiceProvider) | ✅ (ServiceFactory) | ❌ (gerada p/ MS.DI) | ✅ (IDependencyScope) | ✅ | ✅ | MS.DI | ✅ (IServiceProvider) |
| Extensão MS.DI com scan + opções | ✅ (rica) | ✅ | ✅ | ✅ | ✅ | ✅ (1 linha) | ✅ (rica + 1 linha) |
| Sem reflection no caminho quente | ❌ | ✅ (source-gen) | ❌ | ❌ | ❌ (lambdas registradas) | ❌ | ✅* (delegates em cache; *sem source-gen na v1) |
| Telemetria (ActivitySource/Meter) | ❌ (v14 licença) | ❌ | ❌ | ✅ | ❌ | ✅ | ✅ (opt-out) |
| AOT annotations (caminho dinâmico) | ❌ | ✅ (gerado) | ❌ | ✅ | ❌ | ✅ | ✅ |
| ValueTask end-to-end | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ | ✅ |

## 4. Decisões de design do XForge.Mediator

1. **API mental do MediatR 12.5** (simplicidade comprovada), com **`ValueTask` end-to-end** como o Mediator 3.x e
   o Arbiter 6.x (menos alocação; regra: await único por chamada — documentado).
2. **`Unit` readonly struct** para comandos sem resposta (zero alocação; paridade Mediator/Arbiter).
3. **Marcadores de conveniência** `ICommand`/`IQuery`/`ICommand<TRes>` derivando de `IRequest` — semântica CQRS
   opcional sem fragmentar o dispatch (apenas `IRequest`/`IStreamRequest`/`INotification` são contratos de dispatch).
4. **Pipeline completo do MediatR**: behaviors (request/stream) + pré/pós processadores + exception actions/handlers
   com estado (`IRequestExceptionHandlerState<TResponse>`). Pré/pós processadores são **auto-registrados e os
   behaviors built-in só entram no pipeline quando existem** (fast path sem overhead quando não usados);
   o processamento de exceções é **integrado ao dispatch wrapper** (innermost, custo zero sem exceção, funciona
   com handlers vindos do scan ou manuais). Simplificação deliberada: uma única estratégia de exceção
   (actions → handlers), sem o enum `RequestExceptionActionProcessorStrategy` do MediatR.
5. **Motor**: caches estáticos por tipo com wrappers genéricos singleton (padrão comprovado do MediatR) +
   **fast path**: quando não há behaviors para o tipo, o handler é invocado diretamente (sem montar cadeia);
   `Publish` com fast paths para 0/1 handler; `Task.WhenAll` só com N>1.
6. **Dispatch dinâmico (`object`)** com reflexão **cacheada** (1× por tipo), anotado
   `[RequiresDynamicCode]`/`[RequiresUnreferencedCode]` (paridade Arbiter); caminho genérico é AOT-safe.
7. **Registro**: `AddXForgeMediator()` de uma linha (Arbiter) **ou** configuração rica (MediatR):
   scan por assemblies, `TypeEvaluator`, `Lifetime`, `AddBehavior/AddOpenBehavior(+Stream)`,
   `AddOpenRequestPreProcessor/PostProcessor`, publisher configurável, `RegisterGenericHandlers`.
   `IMediator/ISender/IPublisher` seguem `cfg.Lifetime` (default `Transient`, igual ao MediatR — resolvidos pelo
   escopo, o que preserva handlers scoped; `Singleton` é opt-in para máxima performance), handlers/behaviors
   `Transient` por padrão (configurável).
8. **Núcleo agnóstico**: o motor depende apenas de `IServiceProvider` (qualquer container pode instanciar
   `XForge.Mediator.Mediator` manualmente); a integração MS.DI é um anexo opcional de conveniência.
9. **Observabilidade opt-in** (inspirada no Arbiter): `ActivitySource` + `Meter` (`xforge.mediator.send/publish.*`)
   ativáveis via opções; **desligada por padrão** (custo zero no dispatch).
10. **Multi-target**: `netstandard2.0` (máxima compatibilidade, incl. .NET Framework via BCL packages) + `net8.0`.

### Não entram (escopo)
Transport/mensageria distribuída, sagas, outbox, scheduling, routing slips (MassTransit — pertencem a um pacote
futuro de integração, ex. `XForge.Mediator.MassTransit`), source generator (roadmap: `XForge.Mediator.SourceGenerator`),
serialization, multi-tenancy.

## 5. Evidências REA

- Origem dos artefatos: `.nupkg` de `api.nuget.org` extraídos para
  `C:\Users\renat\AppData\Local\Temp\opencode\rea-nuget\ext` (paths nas evidências).
- IDs de evidência completos (membros): ver tabela §1. Ex. MediatR 12.5.0:
  `ev_4cd5985853fc598bd4f7dd62e67cb2e8560f07e031b1ddd4b8d440778dbed1eb`;
  Arbiter.Mediation: `ev_90bfc62b34d2efe0fcdf97a36c948e979a79b04a332f78e11937f57d0c7dc24d`
  (artefato sha256 `eb45fa705c2bc69cce366a4b750a53cdc3e2e2b49ab3493a182f56bbd0de8333`, MVID `3fd14b1f-c0a6-482d-9843-4da7aa54c4bb`).
- Limitações registradas pelo REA: inspeção estática não carrega/executa o assembly; tokens são build-local
  (sha256 + MVID); assinaturas genéricas retornam operandos codificados (`class:N`/`var:N`).
