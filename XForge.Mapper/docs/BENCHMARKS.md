# Benchmarks — XForge.Mapper vs concorrentes

> **Metodologia**: BenchmarkDotNet 0.15.8, job `ShortRun` + `InProcessEmitToolchain` + `MemoryDiagnoser`.
> Handlers/mappers pré-aquecidos no `GlobalSetup`; configuração canônica de cada biblioteca.
> Números **indicativos** — para publicação, rodar `--job medium` sem in-process.

- **Máquina**: AMD Ryzen 7 5700G (8C/16T), Windows 11, .NET SDK 10.0.401
- **Runtime**: .NET 8.0.21 (X64 RyuJIT x86-64 v3)
- **Data**: 2026-10-24
- **Código**: [`benchmarks/XForge.Mapper.Benchmarks/`](../benchmarks/XForge.Mapper.Benchmarks/)
- **Concorrentes**: AutoMapper 12.0.1 (baseline), Mapster 10.0.13, Mapperly 4.3.1 (source generator)

## Resultados

### Mapeamento simples (6 propriedades, mesma convenção)

| Biblioteca | Tempo | Alocação | vs AutoMapper (tempo) |
|---|---:|---:|---:|
| **XForge.Mapper** | **60.5 ns** | 56 B | **0.89×** |
| AutoMapper 12.0.1 (baseline) | 67.8 ns | 56 B | 1.00× |
| Mapster 10.0.13 | 21.6 ns | 56 B | 0.32× |
| Mapperly 4.3.1 (source-gen) | 10.9 ns | 56 B | 0.16× |

### Membro customizado (expressão do usuário)

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| **XForge.Mapper** | **84.1 ns** | 128 B | **0.92×** |
| AutoMapper 12.0.1 (baseline) | 91.1 ns | 128 B | 1.00× |
| Mapster 10.0.13 | 16.9 ns | 32 B | 0.19× |
| Mapperly 4.3.1 (source-gen) | 5.6 ns | 32 B | 0.06× |

### Merge em instância existente

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| **XForge.Mapper** | **60.8 ns** | 56 B | **0.91×** |
| AutoMapper 12.0.1 (baseline) | 66.7 ns | 56 B | 1.00× |
| Mapster 10.0.13 | 28.9 ns | 56 B | 0.43× |
| Mapperly 4.3.1 (source-gen) | 12.3 ns | 56 B | 0.19× |

### Grafo complexo (Pedido + Cliente aninhado + 10 itens)

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| AutoMapper 12.0.1 (baseline) | 290.5 ns | 928 B | 1.00× |
| **XForge.Mapper** | 830.2 ns | 1680 B | 2.86× |
| Mapster 10.0.13 | 163.1 ns | 736 B | 0.56× |
| Mapperly 4.3.1 (source-gen) | 149.9 ns | 736 B | 0.52× |

### Lista (10 objetos com nested)

| Biblioteca | Tempo | Alocação | vs AutoMapper |
|---|---:|---:|---:|
| AutoMapper 12.0.1 (baseline) | 293.3 ns | 888 B | 1.00× |
| **XForge.Mapper** | 464.6 ns | 928 B | 1.59× |
| Mapster 10.0.13 | 153.7 ns | 696 B | 0.52× |
| Mapperly 4.3.1 (source-gen) | 134.5 ns | 696 B | 0.46× |

## Leitura honesta dos resultados

1. **XForge.Mapper é mais rápido que o AutoMapper em mapeamentos simples, customizados e merge**
   (0.89–0.92×) com a mesma alocação — e isso com planos construídos **sem `Expression.Compile` de
   árvore inteira** (build de plano mais rápido e AOT-friendlier).
2. **Grafos complexos (aninhados + listas grandes) ainda ficam entre AutoMapper e Mapster/Mapperly**
   (1.6–2.9× do AutoMapper). A causa é estrutural: cada elemento da lista despacha para o plano do tipo
   via dicionário, enquanto Mapster/Mapperly **inlinam o plano do elemento na expressão do pai**.
   Fechar essa lacuna é o próximo passo do motor (inlining recursivo de planos) e está no roadmap,
   junto com o `XForge.Mapper.SourceGenerator` para AOT total.
3. **Mapperly (source generator) é o teto absoluto** — código gerado em compile-time, zero runtime.
   Comparação de categoria diferente (como documentado no XForge.Mediator).

## Otimizações já aplicadas nesta rodada

- Fast action: TODAS as atribuições context-free compiladas em **um único delegate** por plano
  (antes: dispatch por membro via interface).
- Conversores aninhados com **plano cacheado no closure** (sem lookup de dicionário por elemento).
- Plano cacheado por par de tipos em `ConcurrentDictionary` na configuração.

## Como reproduzir

```bash
dotnet run -c Release --project benchmarks/XForge.Mapper.Benchmarks -- --filter "*"
# execução completa (mais precisa):
dotnet run -c Release --project benchmarks/XForge.Mapper.Benchmarks -- --filter "*" --job medium
```
