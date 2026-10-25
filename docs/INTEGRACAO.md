# Integração dos pacotes XForge

Os três pacotes do workspace são **independentes** — cada um pode ser usado sozinho — mas foram
projetados para se encaixarem em aplicações CQRS/Clean Architecture:

```
Controller/Endpoint
    │
    ▼
XForge.Mediator (Send)                     XForge.Mapper (MapTo/ProjectTo)
    │  ValidationBehavior                        │
    ▼                                            ▼
Handler ──usa──▶ XForge.Mapper (MapTo)     XForge.Validation (valida DTOs)
    │
    ▼
Domain / Infra
```

## Uso individual

| Pacote | Sozinho |
|---|---|
| `XForge.Mediator` | Envie requests/commands/queries/notifications com pipeline de behaviors |
| `XForge.Mapper` | Mapeie objetos com `MapTo`/`Map` fluente — sem Mediator, sem validação |
| `XForge.Validation` | Valide qualquer objeto com `Validator<T>` ou proteja argumentos com `Ensure` |

## Uso integrado (receita completa)

```csharp
var services = new ServiceCollection();

// 1) Mediator com scan de handlers
services.AddXForgeMediator(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CriarClienteHandler>());

// 2) Behaviors (Validation + Logging) no pipeline
services.AddXForgeBehaviors();

// 3) Validadores no DI (o ValidationBehavior os resolve automaticamente)
services.AddXForgeValidation(typeof(CriarClienteValidator).Assembly);

// 4) Mapper (configuração fluente)
services.AddXForgeMapper(cfg =>
    cfg.CreateMap<Pedido, PedidoDto>());

var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();
var mapper   = provider.GetRequiredService<IMapper>();

// Fluxo completo: request → validação no pipeline → handler → mapping
var dto = await mediator.Send(new CriarPedido("produto", 2));
```

### Exemplo de handler integrado

```csharp
public class CriarPedidoHandler : IRequestHandler<CriarPedido, PedidoDto>
{
    private readonly IMapper _mapper;

    public CriarPedidoHandler(IMapper mapper) => _mapper = mapper;

    public async ValueTask<PedidoDto> Handle(CriarPedido request, CancellationToken ct)
    {
        var pedido = Pedido.Criar(request.Produto, request.Quantidade);
        var dto = _mapper.Map<PedidoDto>(pedido);
        return await Task.FromResult(dto);
    }
}
```

### Projeções para leitura (EF Core)

```csharp
var pedidosDto = context.Pedidos
    .Where(p => p.ClienteId == clienteId)
    .ProjectTo<PedidoDto>(mapper.Configuration)
    .ToListAsync();
```

## Matriz de dependências

| Pacote | Depende de |
|---|---|
| XForge.Mediator | Microsoft.Extensions.DependencyInjection.Abstractions |
| XForge.Mapper | Microsoft.Extensions.DependencyInjection.Abstractions |
| XForge.Validation | Microsoft.Extensions.DependencyInjection.Abstractions |
| XForge.Mapper.Projections | XForge.Mapper |
| XForge.Mediator.Behaviors | XForge.Mediator + XForge.Validation |

Nenhum pacote depende de outro no núcleo — as integrações (`Projections`, `Behaviors`) são pacotes
opcionais que fazem a ponte.
