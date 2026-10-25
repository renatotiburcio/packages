# XForge.Mediator.Behaviors

> Behaviors de pipeline para o [XForge.Mediator](../XForge.Mediator/) — validação integrada com o
> [XForge.Validation](../XForge.Validation/) e logging com duração.

## Instalação

```bash
dotnet add package XForge.Mediator.Behaviors
```

Depende de `XForge.Mediator` e `XForge.Validation`.

## Uso integrado (os 3 pacotes)

```csharp
var services = new ServiceCollection();

services.AddXForgeMediator(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CriarClienteHandler>());

services.AddXForgeBehaviors();                                  // Validation + Logging behaviors
services.AddXForgeValidation(typeof(CriarClienteValidator).Assembly); // IValidator<> do DI

var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();

await mediator.Send(new CriarCliente("Renato", "renato@x.com"));    // flui pelo pipeline
await mediator.Send(new CriarCliente(string.Empty, "x"));            // lança ValidationException (2 erros)
```

## Behaviors incluídos

| Behavior | Descrição |
|---|---|
| `ValidationBehavior<TRequest, TResponse>` | Executa todos os `IValidator<TRequest>` do DI antes do handler; lança `ValidationException` agregando as falhas |
| `LoggingBehavior<TRequest, TResponse>` | Log de duração em stdout (demo — substitua pelo seu logger) |

Ordem do pipeline (registro = execução, do mais externo ao mais interno):
**Logging → Validation → Handler**.
