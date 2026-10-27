# NBomber Performance Testing Standard

## Objetivo

Usar NBomber para validar carga, stress, throughput, latência, taxa de erro e regressão de performance de APIs e fluxos críticos.

## Por que usar

NBomber é code-first e .NET/C# friendly. Isso encaixa com XForge, KiloCode e VS Code, porque testes de carga viram código revisável, versionável e automatizável.

## Quando usar

- APIs públicas;
- endpoints críticos;
- login/autenticação;
- consultas de alto volume;
- checkout/pagamento;
- emissão fiscal;
- folha/fechamento;
- relatórios pesados;
- filas/mensageria;
- WebSockets/SignalR;
- gRPC;
- integração com banco.

## Quality Gates sugeridos

| Métrica | Regra |
|---|---|
| p95 latency | <= limite definido por endpoint |
| p99 latency | monitorar e gerar backlog se degradar |
| error rate | <= 1% |
| timeout rate | 0% |
| throughput mínimo | definido por cenário |
| regressão | não piorar > 10% vs baseline |

## Estratégia XForge

### PR
Rodar smoke performance em endpoints alterados.

### Nightly
Rodar carga maior em cenários principais.

### Release
Comparar com baseline e bloquear regressões críticas.

## Exemplo conceitual

```csharp
var scenario = Scenario.Create("get_customers", async context =>
{
    var response = await httpClient.GetAsync("/api/customers");
    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
})
.WithLoadSimulations(
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(3))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
```

## Regra

Não usar NBomber apenas para dizer que tem teste de carga. Cada cenário deve ter:

- objetivo;
- SLA;
- carga esperada;
- carga limite;
- baseline;
- limite de falha;
- relatório;
- backlog se falhar.
