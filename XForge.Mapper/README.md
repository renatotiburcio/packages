# XForge.Mapper

> Mapeador objeto-a-objeto **simples, flexível e rápido** para .NET — com **API 100% fluente** (configuração e uso),
> mapeamento por convenção, customizações por membro, merge em instâncias existentes, polimorfismo, coleções,
> enums, records/imutáveis e validação de configuração.
>
> Projetado a partir da análise REA (decompilação/inspeção estática) de 4 bibliotecas da categoria:
> [AutoMapper](https://github.com/LuckyPennySoftware/AutoMapper) (12.0.1 e 16.2.0), [Mapster](https://github.com/MapsterMapper/Mapster) (10.0.13),
> [Mapperly](https://github.com/riok/mapperly) (4.3.1) e [EggMapper](https://github.com/eggspot/EggMapper) (1.17.19).
> Veja [`docs/ANALISE-PACOTES.md`](docs/ANALISE-PACOTES.md) para a consolidação completa com evidências.

## Por quê?

- **Fluente de ponta a ponta** — configuração E uso em estilo encadeado, com seletores de membro em uma lambda só
  (`.Map(d => d.Nome, s => s.First + " " + s.Last)`) — sem o `ForMember(d => d.X, o => o.MapFrom(...))` aninhado.
- **Plano compilado por par de tipos** — delegates tipados por membro construídos uma única vez (sem
  `Expression.Compile` de árvore inteira); execução sem boxing e sem alocação de contexto quando o grafo é simples.
- **Convenção primeiro** — membros com nomes equivalentes (case-insensitive) mapeiam sozinhos, inclusive objetos
  aninhados e coleções, sem `CreateMap` explícito quando não houver customização.
- **Merge nativo** — `Map(source, existing)` atualiza uma instância existente (padrão PATCH de APIs).
- **Ciclos e profundidade** — cache de instâncias por grafo (referências circulares terminam corretamente) e `MaxDepth`.
- **Validação explícita** — `Validate()` + `RequireDestinationMemberSource` com erros claros.
- **DI integrada** — `AddXForgeMapper` registra `IMapper` singleton; ou use o mapper estático default.

## Instalação

```bash
dotnet add package XForge.Mapper
```

Multi-target: `netstandard2.0` (incl. .NET Framework) e `net8.0`.

## Início rápido

```csharp
using XForge.Mapper;

// 1) Configuração fluente
public sealed class AppMapperConfiguration : MapperConfiguration
{
    public AppMapperConfiguration() : base(cfg =>
    {
        cfg.CreateMap<Pessoa, PessoaDto>()
            .Map(d => d.NomeCompleto, s => $"{s.Nome} {s.Sobrenome}".Trim())   // expressão custom
            .Map(d => d.Cidade, s => s.Endereco!.Cidade)                        // caminho aninhado
            .Map(d => d.Tipo, s => s.Tipo.ToString())                           // conversão custom
            .Ignore(d => d.Id)
            .IgnoreIf(d => d.Apelido, s => s.EsconderApelido)
            .BeforeMap((s, d) => /* ... */)
            .AfterMap((s, d) => /* ... */);
    })
    {
    }
}

// 2) Uso fluente
var dto = pessoa.MapTo<PessoaDto>();                  // mapper estático default (convenção + nested)
var dto2 = pessoa.MapTo<PessoaDto>(mapper);           // com IMapper injetado
mapper.Map(pessoa, dtoExistente);                     // merge/patch em instância existente
var lista = pessoas.MapListTo<PessoaDto>();           // coleções

// 3) DI
services.AddXForgeMapper(cfg =>
{
    cfg.CreateMap<Pessoa, PessoaDto>().TwoWays();
});
public class ClientesController(IMapper mapper) : ControllerBase
{
    [HttpGet]
    public IActionResult Get(int id) => Ok(_pessoas.First(p => p.Id == id).MapTo<PessoaDto>(mapper));
}
```

## Recursos

### Convenção automática
Membros com o mesmo nome (case-insensitive), propriedades e campos, mapeiam sem configuração —
inclusive **objetos aninhados** (planos sintetizados por convenção) e **coleções** (`List`, arrays,
`HashSet`, `IEnumerable`, dicionários com conversão de chave/valor).

### Mapa fluente por membro
```csharp
cfg.CreateMap<Pedido, PedidoDto>()
    .Map(d => d.TotalFormatado, s => s.Total.ToString("C"))
    .Ignore(d => d.InternalId)
    .IgnoreIf(d => d.Cupom, s => !s.TemCupom)
    .ConstructUsing(s => new PedidoDto(s.Id))
    .Include<PedidoExpress, PedidoDto>()     // polimorfismo
    .IncludeBase<PedidoBase, PedidoDtoBase>()
    .MaxDepth(5)
    .TwoWays();                              // registra o reverso
```

### Tratamento de exceções no pipeline
```csharp
public class RecoverHandler : IRequestExceptionHandler<SalvarCliente, Unit, DbUpdateException> { ... }
```
Ações rodam antes dos handlers; a hierarquia da exceção é percorrida da mais derivada para a base.
Sem nenhum handler que recupere, a exceção original é relançada (stack preservado).

### Runtime dispatch por tipo (`object`)
Suportado e anotado `[RequiresDynamicCode]`/`[RequiresUnreferencedCode]` (paridade Arbiter); caminho genérico é AOT-safe.

## Matriz de recursos vs concorrentes

| Recurso | AutoMapper | Mapster | Mapperly | EggMapper | **XForge.Mapper** |
|---|:-:|:-:|:-:|:-:|:-:|
| Fluent config | ✅ | ✅ | ❌ (atributos) | ✅ | ✅ (uma lambda por membro) |
| Convenção automática + nested | ✅ | ✅ | ✅ | ✅ | ✅ |
| Merge em instância existente | ✅ | ✅ | ❌ | ✅ (Patch) | ✅ |
| Polimorfismo (Include/derived) | ✅ | ✅ | ✅ | ✅ | ✅ |
| Ciclos + MaxDepth | ✅ | ✅ | ✅ | ✅ | ✅ |
| Enum ↔ string/number | ✅ | ✅ | ✅ | ✅ | ✅ |
| Records/imutáveis (ctor binding) | ✅ | ✅ | ✅ | ✅ | ✅ |
| Validação de configuração | ✅ | ✅ | compile-time | ✅ | ✅ |
| Projeção LINQ (ProjectTo) | ✅ | ✅ | ❌ | ✅ | 🗺️ roadmap |
| Zero reflection em runtime | ❌ | ❌ | ✅ | ❌ | 🗺️ roadmap (source generator) |

## Performance

Resultados medidos (BenchmarkDotNet, .NET 8 — detalhes e metodologia em [`docs/BENCHMARKS.md`](docs/BENCHMARKS.md)):

| Cenário | XForge.Mapper | AutoMapper 12 | Mapster 10 | Mapperly 4.3 (source-gen) |
|---|---:|---:|---:|---:|
| Simples (6 propriedades) | **46.9 ns** | 58.3 ns | 20.0 ns | 11.1 ns |
| Membro customizado | **69.1 ns** | 73.8 ns | 16.7 ns | 4.7 ns |
| Merge em instância existente | **48.9 ns** | 59.8 ns | 25.5 ns | 9.3 ns |
| Grafo complexo (nested + 10 itens) | 249.5 ns / **776 B** | 219.0 ns / 928 B | 154 ns | 143 ns |
| Lista (10 itens) | 392 ns | **222 ns** | 123 ns | 110 ns |

Leitura honesta: **batemos o AutoMapper em simples/custom/merge (0.80–0.94×)** e, no grafo complexo,
empatamos em tempo (1.14× default, 0.94× in-process) **alocando menos (0.84×)** — o inlining
recursivo compila aninhados e coleções em uma única expression tree. `MapList` (lista de nível
superior) ainda tem dispatch por elemento (1.77×); o `XForge.Mapper.SourceGenerator` (roadmap) fecha
esse custo e persegue o teto do Mapperly.

## Como funciona (performance)

1. **Inlining recursivo de planos** — objetos aninhados (MemberInit) e coleções (materializador
   pré-dimensionado + projeção de elemento pré-compilada) são compilados em **uma única expression
   tree** por plano (`createAndPopulate`): uma invocação por mapa, técnica AutoMapper/Mapster.
2. **Ciclos detectados em build-time** — o grafo de tipos é percorrido na construção; ciclos caem
   para o caminho clássico com `MappingContext` (cache de instâncias, `MaxDepth`).
3. **Plano por par de tipos** — construído sob demanda (ou no `Validate()`) e cacheado em
   `ConcurrentDictionary` na configuração.
4. **Projeções como constantes** — a projeção de elemento de cada coleção é compilada uma vez e
   injetada como `Expression.Constant` (lambdas citadas têm invocação ~3× mais lenta).
5. **Contexto sob demanda** — `MappingContext` só é criado quando o plano declara necessidade
   (ciclos, `MaxDepth`, callbacks com mapper) — zero alocação em grafos simples.

## Roadmap

- Atributos de mapeamento (`[XForgeMap]`, `[XForgeIgnore]`).
- `XForge.Mapper.SourceGenerator` — dispatch gerado em compile-time (AOT total; inclui materialização
  de listas em uma única chamada).
- Naming strategies (PascalCase/snake_case/camelCase), flattening/unflattening.

## Estrutura

```
XForge.Mapper/
├── src/XForge.Mapper/
│   ├── Contracts (IMapper, expressões fluent, exceções)
│   ├── Configuration (MapperConfiguration, MappingExpression, DSL)
│   ├── Internal (planos com inlining recursivo, bindings, conversores, contexto — não público)
│   ├── XForgeMapper.cs (implementação de IMapper)
│   ├── MappingExtensions.cs (MapTo/MapOver/MapListTo)
│   └── XForgeMapperServiceCollectionExtensions.cs (AddXForgeMapper)
├── tests/XForge.Mapper.Tests/     # 52 testes (net8.0 + net10.0)
├── benchmarks/                    # BenchmarkDotNet vs AutoMapper/Mapster/Mapperly
└── docs/ANALISE-PACOTES.md        # consolidação REA das 4 bibliotecas analisadas
```

## Licença

MIT.
