# XForge.Mapper.SourceGenerator

Source generator do XForge.Mapper: mapeamentos declarados como métodos parciais em classes
`[XForgeMap]` ganham corpo gerado **em compile-time** — mesmas convenções do motor de runtime,
com código C# puro (zero reflection, zero expression trees, zero custo de build de plano).

## Uso

```csharp
using XForge.Mapper.Generators;

[XForgeMap]
public partial class PedidoMappings
{
    public partial PedidoDto Map(Pedido source);                          // cria e popula
    public partial void Map(Pedido source, PedidoDto destination);        // merge
    public partial List<PedidoDto> Map(List<Pedido> source);              // lista
}

// consumo (aquisition normal, sem IMapper):
var mappings = new PedidoMappings();
var dto = mappings.Map(pedido);
var lista = mappings.Map(pedidos);
```

O que o generator emite (grafo com aninhado + coleção):

```csharp
public partial PedidoDto Map(Pedido source)
{
    PedidoDto destination = new PedidoDto();
    destination.Id = source.Id;
    destination.Cliente = source.Cliente == null ? null : new ClienteDto()
    {
        Nome = source.Cliente.Nome
    };
    destination.Itens = XForgeMap_Itens_0(source.Itens);

    List<ItemDto> XForgeMap_Itens_0(IEnumerable<Item> source)
    {
        if (source == null) return null;
        var list = new List<ItemDto>(source is ICollection<Item> c ? c.Count : 0);
        foreach (var item in source)
        {
            if (item is null) continue;
            list.Add(new ItemDto() { Produto = item.Produto });
        }
        return list;
    }

    return destination;
}
```

## Convenções suportadas (v0.1)

- Nomes idênticos ou case-insensitive (propriedades e campos públicos)
- Classes aninhadas com null-guard recursivo
- Coleções `List<T>`/arrays/`IEnumerable<T>` com materializador pré-dimensionado
- Records e tipos imutáveis (binding de construtor por nome; membros `init` não são reatribuídos)
- Enum → enum (cast) e enum → string (`ToString()`)
- Ciclos são detectados em build-time: o membro é ignorado com aviso (XFG005) — para ciclos,
  use o motor de runtime (`XForge.Mapper`), que resolve com cache de instâncias

## Diagnósticos

| ID | Significado |
|---|---|
| XFG001 | Classe `[XForgeMap]` não é `partial` |
| XFG002 | Membro com tipos incompatíveis (fica default) |
| XFG003 | Membro de destino sem contraparte na origem (fica default) |
| XFG004 | Assinatura de método parcial não suportada |
| XFG005 | Ciclo detectado durante a geração (membro ignorado) |

## Quando usar generator vs runtime

| Cenário | Recomendação |
|---|---|
| Mapeamentos de convenção conhecidos em compile-time | **Generator** (código puro, mais rápido) |
| Ciclos, `IgnoreIf`, `MaxDepth`, callbacks, polimorfismo | **Runtime** (`XForge.Mapper`) |
| Configuração dinâmica / multi-tenant | **Runtime** |

Os dois coexistem: o generator resolve o que consegue em compile-time; o resto segue no motor.

## Pacotes

- `XForge.Mapper.SourceGenerator` (analyzer) — depende de `XForge.Mapper` (onde vive `[XForgeMap]`)

## Desenvolvimento

```bash
dotnet test XForge.Mapper.SourceGenerator/XForge.Mapper.SourceGenerator.sln
```

Análise REA dos generators concorrentes (Mapperly 4.3.1, Mediator 3.0.2):
[`docs/ANALISE-PACOTES.md`](docs/ANALISE-PACOTES.md).
