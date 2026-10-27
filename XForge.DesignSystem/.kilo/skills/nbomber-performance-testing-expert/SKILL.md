---
name: nbomber-performance-testing-expert
description: Expert em testes de performance com NBomber: carga, resistência, pico e soak testing.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# nbomber-performance-testing-expert

## Objetivo

Testar performance e estabilidade de APIs e serviços.

## Tipos de Teste

| Tipo | Objetivo | Duração |
|------|----------|---------|
| **Carga** | Throughput sob carga esperada | 5-10 min |
| **Resistência** | Estabilidade sob carga sustentada | 30-60 min |
| **Pico** | Comportamento sob carga repentina | 5 min |
| **Soak** | Memory leaks, degradação | 2-8h |

## Configuração

```csharp
var scenario = ScenarioBuilder
    .CreateScenario("api_load_test", steps)
    .WithLoadSimulations(
        Simulation.RampingInject(rate: 100, 
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromMinutes(5))
    );
```

## Métricas

| Métrica | Target |
|---------|--------|
| Latência p50 | < 200ms |
| Latência p95 | < 500ms |
| Latência p99 | < 1000ms |
| Throughput | > 100 req/s |
| Error rate | < 1% |

## Procedimento

1. Definir cenário de teste
2. Configurar load simulation
3. Rodar teste
4. Analisar resultados
5. Identificar gargalos
6. Otimizar e re-testar

## Regras

- Testar em ambiente isolado
- NUNCA testar em produção
- Documentar baseline
- Comparar com versão anterior
