# Checklist — Advanced Quality Testing

## Performance / NBomber

- [ ] Cenário tem objetivo claro.
- [ ] SLA definido.
- [ ] p95/p99 definidos.
- [ ] Error rate limite definido.
- [ ] Throughput esperado definido.
- [ ] Baseline salvo.
- [ ] Regressão máxima definida.
- [ ] Relatório gerado.

## BenchmarkDotNet

- [ ] Método crítico identificado.
- [ ] Benchmark isolado.
- [ ] MemoryDiagnoser quando útil.
- [ ] Baseline definido.
- [ ] Regressão avaliada.

## Testcontainers

- [ ] Dependência real em container.
- [ ] Dados isolados.
- [ ] Migrations executadas.
- [ ] Cleanup garantido.
- [ ] Sem banco compartilhado.

## Architecture Tests

- [ ] Camadas protegidas.
- [ ] Dependências proibidas testadas.
- [ ] Endpoints não acessam DbContext direto.
- [ ] XForge.MediatR obrigatório.
- [ ] AutoMapper obrigatório.

## Acessibilidade

- [ ] Teclado.
- [ ] Contraste.
- [ ] Labels.
- [ ] Foco.
- [ ] ARIA/roles.
- [ ] Fluxos críticos.
