# XForge Packages

Monorepo de pacotes .NET de alta performance, simples e fluentes. Cada pacote vive em sua própria pasta
e pode ser consumido de forma independente ou em conjunto.

## Pacotes

| Pacote | Versão | Descrição | Testes |
|---|---|---|---|
| [`XForge.Mediator`](XForge.Mediator/) | 0.1.0 | Mediador in-process (requests, commands, queries, notifications, streams) com pipeline de behaviors — API mental do MediatR, `ValueTask` end-to-end. [Benchmarks](XForge.Mediator/docs/BENCHMARKS.md) | 39/39 (net8.0 + net10.0) |
| [`XForge.Mapper`](XForge.Mapper/) | 0.1.0 | Mapeador objeto-a-objeto com API 100% fluente — convenção automática, merge, polimorfismo, coleções, records | 39/39 (net8.0 + net10.0) |

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

# empacotamento
dotnet pack XForge.Mediator/src/XForge.Mediator -c Release -o XForge.Mediator/artifacts
dotnet pack XForge.Mapper/src/XForge.Mapper -c Release -o XForge.Mapper/artifacts
```

CI: [.github/workflows/ci.yml](.github/workflows/ci.yml) — build + testes + pack em cada push/PR.

## Licença

MIT.
