---
name: benchmarkdotnet-performance-expert
description: Expert em BenchmarkDotNet, microbenchmarks, memória, regressão de algoritmos e performance de métodos críticos.
metadata:
  version: "49.0.0"
  xforge-category: "advanced-quality"
---

# benchmarkdotnet-performance-expert

## Objetivo

Medir e comparar performance de código de forma precisa e reproduzível usando BenchmarkDotNet.

## Quando Usar

- Antes de otimizar código (medir primeiro, otimizar depois)
- Comparar duas abordagens para o mesmo problema
- Detectar regressões de performance
- Validar impacto de mudanças em código crítico
- Documentar performance para decisões arquiteturais

## Configuração

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10)]
[MarkdownExporterAttribute.GitHub]
public class MyBenchmark
{
    private readonly AppDbContext _context;
    private readonly List<Product> _products;

    public MyBenchmark()
    {
        _context = new AppDbContext(/* ... */);
        _products = GenerateTestData(10000);
    }

    [Benchmark(Baseline = true)]
    public List<Product> CurrentApproach()
    {
        return _context.Products.Where(p => p.IsActive).ToList();
    }

    [Benchmark]
    public List<Product> OptimizedApproach()
    {
        return _context.Products.AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
            .ToList();
    }
}
```

## Métricas

| Métrica | Significado | Target |
|---------|-------------|--------|
| Mean | Tempo médio de execução | < baseline |
| Error | Margem de erro (95% CI) | < 5% do mean |
| StdDev | Desvio padrão (consistência) | Baixo = bom |
| Gen0 | GC collections Gen 0 | 0 |
| Gen1 | GC collections Gen 1 | 0 |
| Gen2 | GC collections Gen 2 | 0 |
| Allocated | Memória alocada por operação | Minimizar |

## Procedimento

### 1. Preparar
- Criar classe de benchmark
- Definir baseline (código atual)
- Configurar job (RuntimeMoniker, iteractions)
- Adicionar `[MemoryDiagnoser]`

### 2. Rodar
```bash
dotnet run -c Release -- --filter "*MyBenchmark*"
```

### 3. Analisar
- Comparar mean com baseline
- Verificar memory allocations
- Identificar regressões
- Verificar consistência (StdDev)

### 4. Documentar
- Salvar resultados em .xforge/operations/benchmarks/
- Comparar com versão anterior
- Registrar hardware e SO
- Documentar conclusões

### 5. Decidir
- Se mean < baseline → otimização ajudou
- Se memory < baseline → menos GC pressure
- Se StdDev alto → código instável, investigar
- Se Gen2 > 0 → memory leak ou alocação excessiva

## Regras

- Sempre rodar em Release mode (nunca Debug)
- Mínimo 3 iterações por benchmark
- Usar `[MemoryDiagnoser]` sempre
- Comparar com versão anterior (baseline)
- Documentar hardware e SO utilizados
- Baseline = código atual (não otimizado)
- Rodar em ambiente isolado (sem outros processos)
- Warmup automático antes das medições

## Exemplo de Relatório

```
| Method           | Mean     | Error    | StdDev   | Allocated |
|----------------- |---------:|---------:|---------:|----------:|
| CurrentApproach  | 12.5 ms  | 0.15 ms  | 0.12 ms  | 480 KB    |
| OptimizedApproach| 3.2 ms   | 0.05 ms  | 0.04 ms  | 32 KB     |

Melhoria: 74% mais rápido, 93% menos memória
```

## Anti-Padrões

- ❌ Rodar em Debug mode (resultados distorcidos)
- ❌ Comparar com código mock (não reflete realidade)
- ❌ Ignorar memory allocations (GC pressure)
- ❌ Rodar com outros processos consumindo CPU
- ❌ Concluir sem baseline comparativo
