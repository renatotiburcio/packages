---
name: benchmarkdotnet-performance-testing-expert
description: Expert em benchmarks de performance com BenchmarkDotNet: microbenchmarks, memory, comparativos.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# benchmarkdotnet-performance-testing-expert

## Objetivo

Medir e comparar performance de código de forma precisa e reproduzível.

## Configuração

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10)]
public class MyBenchmark
{
    private readonly IService _service = new MyService();

    [Benchmark(Baseline = true)]
    public void CurrentApproach()
    {
        _service.Process();
    }

    [Benchmark]
    public void NewApproach()
    {
        _service.ProcessOptimized();
    }
}
```

## Métricas

| Métrica | Significado | Target |
|---------|-------------|--------|
| Mean | Tempo médio de execução | < baseline |
| Error | Margem de erro (95% CI) | < 5% do mean |
| StdDev | Desvio padrão | Baixo = consistente |
| Gen0 | GC collections Gen 0 | 0 |
| Gen1 | GC collections Gen 1 | 0 |
| Gen2 | GC collections Gen 2 | 0 |
| Allocated | Memória alocada por op | Minimizar |

## Procedimento

### 1. Preparar
- Criar classe de benchmark
- Definir baseline (código atual)
- Configurar job (RuntimeMoniker, iteractions)
- Adicionar [MemoryDiagnoser]

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
- Registrar hardware usado
- Documentar conclusões

## Regras

- Sempre rodar em Release mode
- Mínimo 3 iterações
- Usar [MemoryDiagnoser] sempre
- Comparar com versão anterior
- Documentar hardware e SO
- Baseline = código atual (não otimizado)
