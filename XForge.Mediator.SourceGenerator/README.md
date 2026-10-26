# XForge.Mediator.SourceGenerator

Source generator do XForge.Mediator: todo `IRequestHandler<TRequest, TResponse>` (incluindo a
variante `IRequestHandler<TRequest>` = `Unit`) implementado **na sua compilação** é descoberto em
compile-time e recebe registro no DI + dispatch por **wrapper tipado gerado** — sem reflection e
sem assembly scanning no hot path.

## Uso

```csharp
// seus handlers (nada de atributo necessário!)
public record CriarCliente(string Nome) : IRequest<int>;

public class CriarClienteHandler : IRequestHandler<CriarCliente, int>
{
    public ValueTask<int> Handle(CriarCliente request, CancellationToken ct)
        => ValueTask.FromResult(1);
}

// DI: uma linha, sem scanning
services.AddXForgeMediatorGenerated();
```

O generator emite `XForgeGeneratedMediator.g.cs` com:

```csharp
internal static class XForgeGeneratedRegistry
{
    private static readonly Dictionary<(Type Request, Type Response), Func<IXForgeHandlerWrapper>> Factories = new();
    // …cada par (request, resposta) aponta para um wrapper tipado…
}

internal sealed class CriarClienteHandlerWrapper : IXForgeHandlerWrapper
{
    public async ValueTask<object?> HandleAsync(object request, IServiceProvider sp, CancellationToken ct)
    {
        var handler = (CriarClienteHandler)sp.GetRequiredService(typeof(CriarClienteHandler));
        var behaviors = (IPipelineBehavior<CriarCliente, int>[])sp.GetServices(
            typeof(IPipelineBehavior<CriarCliente, int>));
        RequestHandlerDelegate<int> next = token => handler.Handle((CriarCliente)request, token);
        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var previous = next;
            next = token => behavior.Handle((CriarCliente)request, previous, token);
        }
        return await next(ct);
    }
}

public sealed class XForgeGeneratedSender : ISender { … }   // registry + provider, sem reflection
public static class XForgeMediatorGeneratedExtensions
{
    public static IServiceCollection AddXForgeMediatorGenerated(this IServiceCollection services) { … }
}
```

## O que está coberto (v0.1)

- Requests (`IRequestHandler<TRequest, TResponse>`) e comandos sem resposta (`IRequestHandler<TRequest>`)
- `ISender` completo: `Send<TResponse>(IRequest<TResponse>)`, `Send<TResponse>(object)` e `Send(object)`
- Pipeline behaviors do runtime (`IPipelineBehavior<,>`) encadeados na ordem do DI
- Opt-out por classe com `[XForgeMediatorIgnore]`
- `HandlerNotFoundException` do próprio runtime quando não há wrapper para o par

## O que NÃO está coberto (use o runtime)

- Streams (`CreateStream` lança `NotSupportedException` explicativo — use `AddXForgeMediator`)
- Notifications (`IPublisher`), pre/post processors, exception handlers, `MaxDepth`
- Publicação e outras features do pacote `XForge.Mediator` de runtime

## Complementaridade

O generator resolve o que conhece em compile-time; features dinâmicas seguem no motor de runtime
(`services.AddXForgeMediator(...)`). Os dois podem coexistir no mesmo app.

## Desenvolvimento

```bash
dotnet test XForge.Mediator.SourceGenerator/XForge.Mediator.SourceGenerator.sln
```

Análise REA do generator concorrente (Mediator.SourceGenerator 3.0.2): ver
[`../XForge.Mapper.SourceGenerator/docs/ANALISE-PACOTES.md`](../XForge.Mapper.SourceGenerator/docs/ANALISE-PACOTES.md).
