# Benchmarks — XForge.Mediator vs concorrentes

> **Metodologia**: BenchmarkDotNet 0.15.8, job `ShortRun` + `InProcessEmitToolchain` + `MemoryDiagnoser`.
> Handlers que completam sincronamente (mede o overhead puro de dispatch). Todos os ambientes configurados com
> `ServiceLifetime.Singleton` (a configuração recomendada/mais rápida de cada biblioteca). Diagnósticos do
> XForge desligados (default). Números **indicativos** — para publicação, rodar `--job medium` sem in-process.

- **Máquina**: AMD Ryzen 7 5700G (8C/16T), Windows 11, .NET SDK 10.0.401
- **Runtime**: .NET 8.0.21 (X64 RyuJIT x86-64-v3)
- **Data**: 2026-10-22
- **Código**: [`benchmarks/XForge.Mediator.Benchmarks/`](../benchmarks/XForge.Mediator.Benchmarks/)

## Resultados (tempo = média; alocação por operação)

### Send — request com resposta (`Send(IRequest<TResponse>)`)

| Biblioteca | Tempo | Alocação | vs MediatR (tempo) | vs MediatR (alloc) |
|---|---:|---:|---:|---:|
| **XForge.Mediator** | **88.11 ns** | **88 B** | **0.62×** | **0.24×** |
| MediatR 12.5.0 (baseline) | 141.64 ns | 360 B | 1.00× | 1.00× |
| Mediator 3.0.2 (source-gen) | 29.01 ns | 48 B | 0.21× | 0.13× |
| Arbiter.Mediation 6.13.1 | 303.34 ns | 360 B | 2.14× | 1.00× |

### Send — comando sem resposta (Unit)

| Biblioteca | Tempo | Alocação | vs MediatR (tempo) | vs MediatR (alloc) |
|---|---:|---:|---:|---:|
| **XForge.Mediator** | **75.09 ns** | **64 B** | **0.56×** | **0.24×** |
| MediatR 12.5.0 (baseline) | 133.00 ns | 264 B | 1.00× | 1.00× |
| Mediator 3.0.2 (source-gen) | 18.82 ns | 24 B | 0.14× | 0.09× |
| Arbiter.Mediation 6.13.1 | 251.07 ns | 336 B | 1.89× | 1.27× |

### Send — dispatch por runtime type (`Send(object)`)

| Biblioteca | Tempo | Alocação | vs MediatR (tempo) | vs MediatR (alloc) |
|---|---:|---:|---:|---:|
| **XForge.Mediator** | **89.65 ns** | **64 B** | **0.55×** | **0.16×** |
| MediatR 12.5.0 (baseline) | 163.74 ns | 408 B | 1.00× | 1.00× |
| Mediator 3.0.2 (source-gen) | 29.21 ns | 24 B | 0.18× | 0.06× |
| Arbiter.Mediation 6.13.1 | 272.91 ns | 240 B | 1.67× | 0.59× |

### Send — com 1 behavior open generic (pass-through)

| Biblioteca | Tempo | Alocação | vs MediatR (tempo) | vs MediatR (alloc) |
|---|---:|---:|---:|---:|
| **XForge.Mediator** | **110.30 ns** | **256 B** | **0.59×** | **0.46×** |
| MediatR 12.5.0 (baseline) | 187.18 ns | 552 B | 1.00× | 1.00× |
| Mediator 3.0.2 (source-gen) | 26.05 ns | 48 B | 0.14× | 0.09× |
| Arbiter.Mediation 6.13.1 | 367.53 ns | 520 B | 1.97× | 0.94× |

### Publish — notificação multicast (estratégia default de cada lib)

| Biblioteca | 2 handlers | 8 handlers | 2h alloc | 8h alloc |
|---|---:|---:|---:|---:|
| **XForge.Mediator** (TaskWhenAll + fast path sync) | **89.78 ns** | **205.21 ns** | **80 B** | **176 B** |
| MediatR 12.5.0 (TaskWhenAll via LINQ) | 142.89 ns | 336.16 ns | 376 B | 1096 B |
| Mediator 3.0.2 (ForeachAwait, source-gen) | 23.59 ns | 73.16 ns | 24 B | 24 B |
| Arbiter.Mediation 6.13.1 (sequencial embutido) | 302.81 ns | 343.41 ns | 496 B | 640 B |

vs MediatR: XForge **0.63×/0.61× tempo** e **0.21×/0.16× alocação** (2 e 8 handlers).

### CreateStream — streaming de 8 itens (`IAsyncEnumerable`)

| Biblioteca | Tempo | Alocação | vs MediatR (tempo) | vs MediatR (alloc) |
|---|---:|---:|---:|---:|
| **XForge.Mediator** | **4.164 µs** | **424 B** | **0.79×** | **0.47×** |
| MediatR 12.5.0 (baseline) | 5.298 µs | 904 B | 1.00× | 1.00× |
| Mediator 3.0.2 (source-gen) | 4.242 µs | 384 B | 0.80× | 0.42× |

(Arbiters não suporta streaming.)

## Leitura dos resultados

1. **XForge vence o MediatR em todos os cenários medidos** — 0.55×–0.79× do tempo e 0.16×–0.47× das alocações —
   e o Arbiter por margem maior ainda (0.29×–0.63× do tempo). As otimizações que sustentam isso:
   - processamento de exceções **integrado ao wrapper** (não é um behavior no container — fast path sem
     `GetServices`, sem array, sem closures quando não há behaviors do usuário);
   - `ValueTask` end-to-end + `Unit` readonly struct;
   - `NotificationHandlerCallback` estático cacheado por tipo (zero closures por publish);
   - `TaskWhenAllPublisher` com absorção inline de handlers síncronos (só handlers realmente pendentes
     viram `Task`);
   - diagnóstico opt-in (zero custo por padrão).
2. **Mediator 3.x (source generator) é mais rápido no dispatch puro** — categoria diferente: o pipeline é
   monomorfizado em compile-time, sem resolução de DI no caminho quente. O custo é o acoplamento ao gerador
   (handlers resolvidos estaticamente, menos flexibilidade para cenários dinâmicos/DI avançados).
   O caminho para fechar essa lacuna no XForge é o roadmap `XForge.Mediator.SourceGenerator`.
3. **Arbiter** paga telemetria sempre ativa e resoluções extras por chamada.

## Como reproduzir

```bash
dotnet run -c Release --project benchmarks/XForge.Mediator.Benchmarks -- --filter "*"
# execução completa (mais precisa):
dotnet run -c Release --project benchmarks/XForge.Mediator.Benchmarks -- --filter "*" --job medium
```
