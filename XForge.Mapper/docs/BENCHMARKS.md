# Benchmarks — XForge.Mapper vs concorrentes

> **Metodologia**: BenchmarkDotNet 0.15.8, job `ShortRun` + `MemoryDiagnoser`, toolchains `Default`
> e `InProcessEmit` (ambos reportados quando divergem de forma relevante).
> Handlers/mappers pré-aquecidos no `GlobalSetup`; configuração canônica de cada biblioteca.
> Números **indicativos** — para publicação, rodar `--job medium`.

- **Máquina**: AMD Ryzen 7 5700G (8C/16T), Windows 11, .NET SDK 10.0.401
- **Runtime**: .NET 8.0.21 (X64 RyuJIT x86-64 v3)
- **Data**: 2026-10-26 (v0.2.0 — inlining recursivo de planos)
- **Código**: [`benchmarks/XForge.Mapper.Benchmarks/`](../benchmarks/XForge.Mapper.Benchmarks/)
- **Concorrentes**: AutoMapper 12.0.1 (baseline), Mapster 10.0.13, Mapperly 4.3.1 (source generator)

## Resultados (v0.2.0)

### Mapeamento simples (6 propriedades, mesma convenção)

| Biblioteca | Tempo | Alocação | vs AutoMapper (tempo) |
|---|---:|---:|---:|
| **XForge.Mapper** | **46.9 ns** | 56 B | **0.80×** |
| AutoMapper 12.0.1 (baseline) | 58.3 ns | 56 B | 1.00× |
| Mapster 10.0.13 | 20.0 ns | 56 B | 0.34× |
| Mapperly 4.3.1 (source-gen) | 11.1 ns | 56 B | 0.19× |

### Membro customizado (expressão do usuário)

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| **XForge.Mapper** | **69.1 ns** | 128 B | **0.94×** |
| AutoMapper 12.0.1 (baseline) | 73.8 ns | 128 B | 1.00× |
| Mapster 10.0.13 | 16.7 ns | 32 B | 0.23× |
| Mapperly 4.3.1 (source-gen) | 4.7 ns | 32 B | 0.06× |

### Merge em instância existente

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| **XForge.Mapper** | **48.9 ns** | 56 B | **0.82×** |
| AutoMapper 12.0.1 (baseline) | 59.8 ns | 56 B | 1.00× |
| Mapster 10.0.13 | 25.5 ns | 56 B | 0.43× |
| Mapperly 4.3.1 (source-gen) | 9.3 ns | 56 B | 0.16× |

### Grafo complexo (Pedido + Cliente aninhado + 10 itens)

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| **XForge.Mapper** | **249.5 ns** | **776 B (0.84×)** | **1.14×** |
| AutoMapper 12.0.1 (baseline) | 219.0 ns | 928 B | 1.00× |
| Mapster 10.0.13 | 154.3 ns | 736 B | 0.70× |
| Mapperly 4.3.1 (source-gen) | 142.8 ns | 736 B | 0.65× |

Execução `InProcessEmit` (mesma rodada): XForge **0.94×** do AutoMapper (222.9 vs 238.1 ns) —
empate técnico dentro do ruído, com **menos alocação** (776 B vs 928 B).

Evolução vs v0.1.0: **830 ns → 249.5 ns (3.3× mais rápido)**; alocações 1.81× → **0.84×**
(agora abaixo do AutoMapper).

### Lista (10 objetos com nested)

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| AutoMapper 12.0.1 (baseline) | 222.1 ns | 888 B | 1.00× |
| **XForge.Mapper** | 392.4 ns | 928 B | 1.77× |
| Mapster 10.0.13 | 122.9 ns | 696 B | 0.55× |
| Mapperly 4.3.1 (source-gen) | 110.4 ns | 696 B | 0.50× |

`MapList` despacha o plano por elemento (dispatch tipado + callbacks por item) — absoluto melhorou
(465 → 392 ns), mas o teto exige o source generator (materialização da lista inteira em uma chamada).

## Leitura honesta dos resultados (v0.2.0)

1. **XForge.Mapper bate o AutoMapper em simples, customizado e merge** (0.80–0.94×) com a mesma
   alocação.
2. **Grafos complexos empatam com o AutoMapper em tempo (1.14× default / 0.94× in-process) e
   alocam MENOS (0.84×)** — consequência do inlining recursivo: objetos aninhados e coleções são
   compilados em uma única expression tree com projeções pré-compiladas e materialização
   pré-dimensionada.
3. **`MapList` (lista de nível superior) ainda tem dispatch por elemento** (1.77×) — o
   `XForge.Mapper.SourceGenerator` elimina esse custo (no roadmap).
4. **Mapperly (source generator) segue o teto absoluto** — código gerado em compile-time, zero
   runtime. Categoria diferente (como documentado no XForge.Mediator).

## Otimizações aplicadas nesta rodada (v0.2.0)

- **Inlining recursivo de planos**: aninhados viram `MemberInit`, coleções viram materializador
  pré-dimensionado com projeção de elemento — tudo em UMA expression tree por plano
  (`createAndPopulate`), com detecção de ciclos em build-time e fallback clássico com contexto.
- **Projeções de elemento pré-compiladas e injetadas como `Expression.Constant`** — lambdas citadas
  (Quote) dentro da árvore têm invocação ~3× mais lenta que delegados constantes (771 ns → 238 ns
  medidos no grafo complexo).
- **Materialização com pré-dimensionamento** via `ICollection.Count` — elimina o crescimento
  dinâmico do `ToList` sobre `Select` (~8,5 KB economizados em listas de 1.000 elementos).
- **Fast path do `Map` sem callbacks**: um único delegate cria E popula a instância.

## Como reproduzir

```bash
dotnet run -c Release --project benchmarks/XForge.Mapper.Benchmarks -- --filter "*" --job short
# execução completa (mais precisa):
dotnet run -c Release --project benchmarks/XForge.Mapper.Benchmarks -- --filter "*" --job medium
```
