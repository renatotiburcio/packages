# XForge.Mediator

> Mediador in-process **simples, flexível e rápido** para .NET — requests, commands, queries, notifications e streams,
> com pipeline de behaviors, pré/pós processadores, tratamento de exceções e estratégias de publicação.
>
> Projetado a partir da análise REA (decompilação/inspeção estática) de 7 bibliotecas da categoria:
> [MediatR](https://github.com/jbogard/MediatR) (12.5.0 e 14.2.0), [Mediator](https://github.com/martinothamar/Mediator) (3.0.2),
> [Mediator.Net](https://github.com/mayuanyang/Mediator.Net) (5.0.0), [MassTransit](https://github.com/MassTransit/MassTransit) (9.2.4),
> [FluentMediator](https://github.com/ivanpaulovich/FluentMediator) (0.4.7) e [Arbiter.Mediation](https://github.com/loresoft/Arbiter) (6.13.1).
> Veja [`docs/ANALISE-PACOTES.md`](docs/ANALISE-PACOTES.md) para a consolidação completa com evidências.

## Por quê?

- **API mental do MediatR** — fácil de aprender, fácil de migrar.
- **`ValueTask` end-to-end** — combinado com wrappers cacheados e exception processing integrado ao wrapper:
  **0.55–0.79× do tempo e 0.16–0.47× das alocações do MediatR 12.5** em todos os cenários medidos
  (veja [`docs/BENCHMARKS.md`](docs/BENCHMARKS.md)).
- **Pipeline completo** — behaviors (request/stream), pré/pós processadores e exception actions/handlers,
  auto-registrados **apenas quando existem** (fast path sem overhead quando não usados).
- **Zero dependência de reflection no estado estacionário** — wrappers por tipo construídos 1× e cacheados;
  dispatch dinâmico (`object`) anotado para Native AOT.
- **Agnóstico de arquitetura** — Clean Architecture, CQRS, Vertical Slices, DDD, modular monolith: contratos
  por papel (`IRequest`, `ICommand`, `IQuery`, `INotification`, `IStreamRequest`) sem acoplar o dispatch a nenhum deles.
- **Observabilidade opt-in** — `ActivitySource` + `Meter` (`XForge.Mediator`) ativáveis com uma linha
  (`cfg.EnableDiagnostics = true`); **zero custo por padrão**.

## Instalação

```bash
dotnet add package XForge.Mediator
```

Multi-target: `netstandard2.0` (incl. .NET Framework via BCL packages) e `net8.0`.

## Início rápido

```csharp
using XForge.Mediator;

// 1) Mensagens
public record Ping(string Message) : IRequest<string>;
public record SalvarCliente(Guid Id) : ICommand;              // sem resposta (Unit)
public record ObterCliente(Guid Id) : IQuery<ClienteDto>;      // com resposta
public record ClienteSalvo(Guid Id) : INotification;

// 2) Handlers
public class PingHandler : IRequestHandler<Ping, string>
{
    public ValueTask<string> Handle(Ping request, CancellationToken ct)
        => ValueTask.FromResult(request.Message + " pong");
}

public class SalvarClienteHandler : IRequestHandler<SalvarCliente>   // retorna ValueTask<Unit>
{
    public ValueTask<Unit> Handle(SalvarCliente request, CancellationToken ct)
    {
        // ...persistência...
        return Unit.CompletedTask;
    }
}

public class ClienteSalvoHandler1 : INotificationHandler<ClienteSalvo> { /* ... */ }
public class ClienteSalvoHandler2 : INotificationHandler<ClienteSalvo> { /* ... */ }

// 3) Registro
services.AddXForgeMediator(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Ping>();
    cfg.AddOpenBehavior(typeof(ValidacaoBehavior<,>));   // pipeline behaviors
});

// 4) Uso
public class ClientesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Salvar(SalvarCliente cmd)
    {
        await mediator.Send(cmd);                                  // comando
        await mediator.Publish(new ClienteSalvo(cmd.Id));          // notificação multicast
        var cliente = await mediator.Send(new ObterCliente(cmd.Id)); // query
        return Ok(cliente);
    }
}
```

## Recursos

### Send — request/response

```csharp
var pong   = await mediator.Send(new Ping("olá"));       // genérico (AOT-safe)
var boxed  = await mediator.Send((object)request);       // runtime type (resposta descoberta)
var pong2  = await mediator.Send<string>((object)r);     // runtime type + resposta explícita
```

### Publish — notificações multicast

Handlers registrados recebem a notificação. Estratégias configuráveis:

- `TaskWhenAllPublisher` (default) — paralelo, com fast path para 0/1 handler;
- `ForeachAwaitPublisher` — sequencial;
- custom: implemente `INotificationPublisher`.

```csharp
services.AddXForgeMediator(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Pinged>();
    cfg.NotificationPublisher = new ForeachAwaitPublisher(); // ou cfg.NotificationPublisherType = typeof(...)
});
```

Publicar sem handlers é no-op (semântica MediatR).

### CreateStream — streaming

```csharp
public record BuscarNumeros(int Count) : IStreamRequest<int>;

public class BuscarNumerosHandler : IStreamRequestHandler<BuscarNumeros, int>
{
    public async IAsyncEnumerable<int> Handle(BuscarNumeros r, [EnumeratorCancellation] CancellationToken ct)
    {
        for (var i = 0; i < r.Count; i++) { await Task.Yield(); yield return i; }
    }
}

await foreach (var n in mediator.CreateStream(new BuscarNumeros(3), ct)) { ... }
```

### Pipeline behaviors

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // antes
        var response = await next(ct).ConfigureAwait(false);
        // depois
        return response;
    }
}

// registro (ordem de registro = ordem de execução, do mais externo ao mais interno)
cfg.AddBehavior(typeof(LoggingBehavior<,>));     // fechado ou aberto
cfg.AddOpenBehavior<LoggingBehavior<,>>();       // helper genérico
cfg.AddStreamBehavior(typeof(MeuStreamBehavior<,>)); // para pipelines de streaming
```

### Pré/pós processadores (auto-registrados)

```csharp
public class AuditoriaPreProcessor : IRequestPreProcessor<SalvarCliente>
{
    public ValueTask Process(SalvarCliente request, CancellationToken ct) { /* ... */ return ValueTask.CompletedTask; }
}

public class AuditoriaPostProcessor : IRequestPostProcessor<SalvarCliente, Unit>
{
    public ValueTask Process(SalvarCliente request, Unit response, CancellationToken ct) { /* ... */ }
}
```

Ordem do pipeline: **seus behaviors → pós-processors → pré-processors → [processamento de exceções integrado ao wrapper] → handler**.

### Tratamento de exceções

```csharp
public class RecoverHandler : IRequestExceptionHandler<SalvarCliente, Unit, DbUpdateException>
{
    public ValueTask Handle(SalvarCliente request, DbUpdateException ex,
        IRequestExceptionHandlerState<Unit> state, CancellationToken ct)
    {
        state.SetHandled(Unit.Value);   // recupera: a pipeline devolve a resposta
        return ValueTask.CompletedTask;
    }
}

public class LogAction : IRequestExceptionAction<SalvarCliente, DbUpdateException>
{
    public ValueTask Process(SalvarCliente request, DbUpdateException ex, CancellationToken ct) { /* log */ }
}
```

Ações rodam antes dos handlers; a hierarquia da exceção é percorrida da mais derivada para a base.
Sem nenhum handler que recupere, a exceção original é relançada (stack preservado).
O processamento de exceções é **integrado ao dispatch wrapper** (não é um behavior registrado no container):
funciona com handlers de exceção vindos do scan ou registrados manualmente, e tem **custo zero quando não há exceção**.

## Referência de configuração

```csharp
services.AddXForgeMediator(cfg =>
{
    // Escaneamento
    cfg.RegisterServicesFromAssemblyContaining<Ping>();       // 1 linha
    cfg.RegisterServicesFromAssemblies(typeof(Ping).Assembly, typeof(Outro).Assembly);
    cfg.TypeEvaluator = t => !t.Name.EndsWith("Internal");    // filtro de tipos
    cfg.RegisterGenericHandlers = true;                       // handlers open generic (default: false)

    // Lifetimes
    cfg.Lifetime = ServiceLifetime.Transient;                 // handlers, behaviors, processors E o mediator
                                                              // (Transient = seguro p/ handlers scoped, default MediatR)
                                                              // Singleton = máxima performance quando os handlers forem singleton-safe

    // Pipeline
    cfg.AddBehavior(typeof(LoggingBehavior<,>));              // request behaviors
    cfg.AddStreamBehavior(typeof(MetricStreamBehavior<,>));   // stream behaviors
    cfg.AddRequestPreProcessor(typeof(MeuPreProcessor));      // + auto-registra o behavior built-in
    cfg.AddRequestPostProcessor(typeof(MeuPostProcessor));
    cfg.AutoRegisterRequestProcessors = true;                 // default

    // Publicação
    cfg.NotificationPublisher = new ForeachAwaitPublisher();  // ou NotificationPublisherType = typeof(...)

    // Outros
    cfg.EnableDiagnostics = true;                             // ActivitySource/Meter (default: false — opt-in)
});

// ou simplesmente:
services.AddXForgeMediator();
```

Excluir tipos do scan (fixtures, duplicatas, registrado manualmente):

```csharp
[XForgeMediatorIgnore]
public class MeuHandler : IRequestHandler<Ping, string> { /* não é registrado pelo scan */ }
```

## Performance

Resultados medidos (BenchmarkDotNet, .NET 8, detalhes em [`docs/BENCHMARKS.md`](docs/BENCHMARKS.md)):

| Cenário | XForge | MediatR 12.5 | Arbiter 6.13 | Mediator 3.x (source-gen) |
|---|---:|---:|---:|---:|
| Send request | **88 ns / 88 B** | 142 ns / 360 B | 303 ns / 360 B | 29 ns / 48 B |
| Send comando (Unit) | **75 ns / 64 B** | 133 ns / 264 B | 251 ns / 336 B | 19 ns / 24 B |
| Send(object) runtime dispatch | **90 ns / 64 B** | 164 ns / 408 B | 273 ns / 240 B | 29 ns / 24 B |
| Send com behavior | **110 ns / 256 B** | 187 ns / 552 B | 368 ns / 520 B | 26 ns / 48 B |
| Publish (2 / 8 handlers) | **90 / 205 ns** | 143 / 336 ns | 303 / 343 ns | 24 / 73 ns |
| Stream (8 itens) | **4.16 µs / 424 B** | 5.30 µs / 904 B | — | 4.24 µs / 384 B |

Otimizações que sustentam os números:

| Decisão | Benefício |
|---|---|
| `ValueTask<T>` em todo o dispatch | Sem alocação de `Task` no caminho síncrono |
| Exception processing **integrado ao wrapper** (não é behavior no container) | Fast path sem `GetServices`, sem array, sem closures quando não há behaviors do usuário |
| Wrappers genéricos cacheados por tipo (`ConcurrentDictionary` estático) | Lookup de dicionário no estado estacionário; reflection só no 1º dispatch |
| Fast path sem behaviors | Handler invocado direto (sem montar cadeia) quando não há behaviors para o tipo |
| `Unit` = `readonly struct` | Comandos sem resposta sem boxing |
| `NotificationHandlerCallback` estático cacheado por tipo | Zero closures por publish |
| `TaskWhenAllPublisher` com absorção inline de handlers síncronos | Só handlers realmente pendentes viram `Task` |
| Diagnósticos opt-in | Zero custo por padrão |

> O Mediator 3.x (source generator) permanece mais rápido no dispatch puro — categoria diferente (pipeline
> monomorfizado em compile-time). O caminho para fechar essa lacuna é o roadmap `XForge.Mediator.SourceGenerator`.

## Native AOT / Trimming

- Os overloads genéricos (`Send<TResponse>(IRequest<TResponse>)`, `CreateStream<TResponse>(IStreamRequest<TResponse>)`,
  `Publish<TNotification>(TNotification)`) são AOT-safe.
- Os overloads dinâmicos (`Send(object)`, `Send<TResponse>(object)`, `CreateStream(object)`, `Publish(object)`)
  são anotados com `[RequiresDynamicCode]`/`[RequiresUnreferencedCode]` (paridade Arbiter).

## Migrando do MediatR

| MediatR | XForge.Mediator | Observação |
|---|---|---|
| `Task<TResponse> Handle(...)` | `ValueTask<TResponse> Handle(...)` | await único; conversão trivial |
| `IRequest` → `Task Handle` (covariante, .NET 5+) | `ValueTask<Unit> Handle` retornando `Unit.CompletedTask` | `ValueTask` é struct — sem covariância; marcador `IRequestHandler<T>` continua existindo |
| `IRequest`, `IStreamRequest`, `INotification` | idênticos | dispatch pelos mesmos contratos |
| — | `ICommand`, `IQuery` (novos) | marcadores de conveniência CQRS (herdam de `IRequest`) |
| `Send/Publish/CreateStream` | mesmos nomes/semântica | agora com `ValueTask` |
| `RequestExceptionActionProcessorStrategy` | único fluxo (actions → handlers) | simplificação deliberada |
| Licença comercial (13+) | MIT | — |

## Estrutura do pacote

```
XForge.Mediator/
├── src/XForge.Mediator/            # núcleo: contratos, motor, pipeline, publicação, DI, diagnóstico
│   ├── Contracts/                  # IRequest, ICommand, IQuery, INotification, handlers, behaviors
│   ├── Pipeline/                   # pré/pós processors + exception processor (built-in behaviors)
│   ├── Internal/                   # wrappers cacheados, registrar, helpers (não público)
│   ├── Mediator.cs                 # implementação de IMediator/ISender/IPublisher
│   ├── NotificationPublishers.cs   # TaskWhenAll (default) e ForeachAwait
│   └── XForgeMediatorServiceCollectionExtensions.cs  # AddXForgeMediator
├── tests/XForge.Mediator.Tests/    # 50 testes (net8.0 + net10.0)
├── docs/ANALISE-PACOTES.md         # consolidação REA das 7 bibliotecas analisadas
└── README.md
```

## Licença

MIT.
