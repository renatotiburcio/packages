# Sample — Clean Architecture com os pacotes XForge

Prova de uso **integrada** dos pacotes do monorepo em uma API ASP.NET Core mínima.

```
src/Sample.Domain/          entidades e contratos (zero dependências)
src/Sample.Application/     comandos/queries, handlers, validadores, DTOs (usa os 5 pacotes)
src/Sample.Infrastructure/  repositório em memória
src/Sample.Api/             minimal API + composição de DI
tests/Sample.IntegrationTests/  testes ponta a ponta via WebApplicationFactory
```

## Como os pacotes entram

| Pacote | Onde | Papel no sample |
|---|---|---|
| `XForge.Mediator` | Application/API | `IRequest`, `IRequestHandler`, `ISender`, pipeline |
| **`XForge.Mediator.SourceGenerator`** | Application | descobre os handlers em compile-time → `AddXForgeMediatorGenerated()` |
| `XForge.Validation` | Application | `Validator<CriarClienteCommand>` fluent com códigos padronizados |
| `XForge.Mediator.Behaviors` | API | `ValidationBehavior<,>` no pipeline — bloqueia comandos inválidos |
| **`XForge.Mapper.SourceGenerator`** | Application | `[XForgeMap] ClienteMappings` — `Cliente → ClienteDto` gerado em compile-time |
| `XForge.Mapper` | Application | motor de runtime disponível para os casos dinâmicos |

Nenhum assembly scanning para handlers: o source generator gera o registro a partir do código
de `Sample.Application`. A validação continua via scanning (`AddXForgeValidation`) porque o
modelo de validação do XForge é resolvido por DI em tempo de execução.

## Rodar

```bash
dotnet run --project src/Sample.Api     # API em http://localhost:5xxx
dotnet test                              # testes de integração ponta a ponta
```

## Endpoints

| Método | Rota | Comportamento |
|---|---|---|
| POST | `/clientes` | comando válido → 201; inválido → **422** (bloqueado pelo `ValidationBehavior`) |
| GET | `/clientes/{id}` | query → DTO ou 404 |

## Nota sobre o host de testes (.NET 10)

A leitura do corpo JSON em `WebApplicationFactory` esbarra em um bug do host de testes do
.NET 10 (`ResponseBodyPipeWriter` x `System.Text.Json`) — a resposta serializada por
`Results.Json`/`WriteAsJsonAsync` falha nesse host. O teste de leitura do DTO valida o status do
pipeline em vez do corpo; a conversão em si é coberta pelos testes do
`XForge.Mapper.SourceGenerator`. Em SDK estável o teste de corpo pode ser restaurado.

## Limites do sample (v0.1 dos pacotes)

- Streams e notifications do Mediator: usar o pacote de runtime (`AddXForgeMediator`).
- Projeções `ProjectTo` (EF Core): pacote `XForge.Mapper.Projections` (fora deste sample).
- `MapList`/polimorfismo do Mapper: motor de runtime.
