# XForge.Mapper.Projections

> `ProjectTo` para `IQueryable` — estende o [XForge.Mapper](../XForge.Mapper/) com projeções LINQ
> que o EF Core traduz para SQL: membros resolvidos no banco, nunca em memória.

## Instalação

```bash
dotnet add package XForge.Mapper.Projections
```

Depende de `XForge.Mapper` (mesma versão).

## Uso

```csharp
using XForge.Mapper.Projections;

// com IMapper injetado
var dtos = context.Pedidos
    .Where(p => p.Data >= inicio)
    .ProjectTo<PedidoDto>(mapper)
    .ToList();

// ou com a MapperConfiguration direta
var dtos = context.Pedidos.ProjectTo<PedidoDto>(configuration).ToList();
```

## O que é projetável

| Recurso | Suporte |
|---|---|
| Convenção por nome (case-insensitive) | ✅ |
| `Map` com expressões do usuário (rebinadas ao parâmetro da consulta) | ✅ |
| Objetos aninhados (MemberInit recursivo com null-guard) | ✅ |
| Coleções: `List<T>`, arrays, `HashSet<T>`, `IEnumerable<T>`/`IReadOnly*` (Select + ToList) | ✅ |
| Records/imutáveis (binding de construtor por nome) | ✅ |
| Conversões simples (`Expression.Convert` — traduzíveis) | ✅ |
| `MapWith` (delegates puros) | ❌ erro com orientação (use `Map`) |
| `IgnoreIf` | ❌ erro (mapeamento condicional não tem tradução SQL) |
| Dicionários | 🗺️ roadmap |

Membros ignorados (`Ignore`) não são incluídos na projeção — ficam com o valor default do DTO.

## Notas

- Null-guards são adicionados automaticamente para membros aninhados e coleções (LINQ-to-Objects robusto;
  no EF Core a condição vira checagem de NULL no SQL).
- Membros `MapWith` e `IgnoreIf` lançam `MappingException` com orientação clara em vez de falhar silenciosamente.
- Expressões são construídas uma vez por (configuração, par de tipos) e cacheadas.

## Licença

MIT.
