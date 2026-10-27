# XForge Packages

Monorepo de pacotes .NET de alta performance, simples e fluentes. Cada pacote vive em sua própria pasta
e pode ser consumido de forma independente ou em conjunto.

## Pacotes

| Pacote | Versão | Descrição | Testes |
|---|---|---|---|
| [`XForge.Mediator`](XForge.Mediator/) | 0.1.0 | Mediador in-process (requests, commands, queries, notifications, streams) com pipeline de behaviors — API mental do MediatR, `ValueTask` end-to-end. [Benchmarks](XForge.Mediator/docs/BENCHMARKS.md) | 50/50 (net8.0 + net10.0) |
| [`XForge.Mediator.SourceGenerator`](XForge.Mediator.SourceGenerator/) | 0.1.0 | Source generator do Mediator: handlers da compilação ganham registro no DI + dispatch por wrapper tipado gerado, sem reflection | 6/6 (net8.0) |
| [`XForge.Mapper`](XForge.Mapper/) | 0.2.0 | Mapeador objeto-a-objeto com API 100% fluente — convenção automática, merge, polimorfismo, coleções, records; inlining recursivo de planos. [Benchmarks](XForge.Mapper/docs/BENCHMARKS.md) | 55/55 (net8.0 + net10.0) |
| [`XForge.Mapper.SourceGenerator`](XForge.Mapper.SourceGenerator/) | 0.1.0 | Source generator: métodos parciais em classes `[XForgeMap]` ganham corpo gerado em compile-time — mesmas convenções do motor, código C# puro (zero reflection) | 6/6 (net8.0) |
| [`XForge.Mapper.Projections`](XForge.Mapper.Projections/) | 0.1.0 | `ProjectTo` para `IQueryable` — projeções LINQ traduzíveis pelo EF Core | 9/9 (net8.0) |
| [`XForge.Validation`](XForge.Validation/) | 0.1.0 | Validação fluente (`Validator<T>`, `Ensure`, RuleSet, cascade) — mental model do FluentValidation, `ValueTask` async, códigos padronizados | 32/32 (net8.0 + net10.0) |
| [`XForge.Validation.TestHelper`](XForge.Validation.TestHelper/) | 0.1.0 | Assertions de teste para o Validation (`ShouldHaveValidationErrorFor`, `ShouldHaveValidationErrorWithCode`, `TestValidationResult`) — estilo FluentValidation.TestHelper | 15/15 (net8.0 + net10.0) |
| [`XForge.Mediator.Behaviors`](XForge.Mediator.Behaviors/) | 0.1.0 | Integração dos 3 pacotes: `ValidationBehavior` + `LoggingBehavior` para o pipeline do Mediator | 2/2 (net8.0) |

## Princípios

- **Simples, flexível, fácil de usar** — API mental das bibliotecas mais populares da categoria, sem armadilhas.
- **Performance primeiro** — planos/delegates compilados sob demanda e cacheados; hot path sem alocações desnecessárias.
- **Multi-arquitetura** — Clean Architecture, CQRS, Vertical Slices, DDD, modular monolith.
- **Multi-target** — `netstandard2.0` + `net8.0`.

## Desenvolvimento

```bash
# testes por pacote
dotnet test XForge.Mediator/XForge.Mediator.sln
dotnet test XForge.Mapper/XForge.Mapper.sln
dotnet test XForge.Mapper.SourceGenerator/XForge.Mapper.SourceGenerator.sln
dotnet test XForge.Mediator.SourceGenerator/XForge.Mediator.SourceGenerator.sln
dotnet test XForge.Mapper.Projections/XForge.Mapper.Projections.sln
dotnet test XForge.Validation/XForge.Validation.sln
dotnet test XForge.Validation.TestHelper/XForge.Validation.TestHelper.sln
dotnet test XForge.Mediator.Behaviors/XForge.Mediator.Behaviors.sln

# empacotamento
dotnet pack XForge.Mediator/src/XForge.Mediator -c Release -o XForge.Mediator/artifacts
dotnet pack XForge.Mapper/src/XForge.Mapper -c Release -o XForge.Mapper/artifacts
dotnet pack XForge.Mapper.SourceGenerator/src/XForge.Mapper.SourceGenerator -c Release -o XForge.Mapper.SourceGenerator/artifacts
dotnet pack XForge.Mapper.Projections/src/XForge.Mapper.Projections -c Release -o XForge.Mapper.Projections/artifacts
dotnet pack XForge.Validation/src/XForge.Validation -c Release -o XForge.Validation/artifacts
dotnet pack XForge.Mediator.Behaviors/src/XForge.Mediator.Behaviors -c Release -o XForge.Mediator.Behaviors/artifacts
```

CI: [.github/workflows/ci.yml](.github/workflows/ci.yml) — build + testes + pack em cada push/PR.

## Licença

MIT.
