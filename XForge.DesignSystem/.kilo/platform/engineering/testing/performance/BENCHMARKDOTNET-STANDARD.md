# BenchmarkDotNet Standard

## Objetivo

Usar BenchmarkDotNet para medir microperformance de métodos críticos de forma confiável e reproduzível.

## Quando usar

- cálculo financeiro;
- cálculo fiscal;
- serialização/parsing;
- mapeamentos pesados;
- validações em massa;
- geração de relatórios;
- algoritmos usados em loops;
- pipelines de importação/exportação.

## Quando não usar

- fluxo completo de API;
- performance de tela;
- carga concorrente;
- dependência externa instável.

Para isso, usar NBomber, Playwright ou Testcontainers.

## Quality Gate

Benchmarks não devem bloquear todo PR por padrão.

Recomendado:

- rodar em benchmark job separado;
- comparar com baseline;
- falhar apenas quando regressão ultrapassar limite definido;
- para código crítico, abrir backlog P0/P1.

## Exemplo

```csharp
[MemoryDiagnoser]
public class CustomerParserBenchmark
{
    [Benchmark]
    public Customer Parse()
    {
        return CustomerParser.Parse(input);
    }
}
```
