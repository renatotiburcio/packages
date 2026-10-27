---
description: Executar testes: unit, integration, e2e, mutation, performance.
agent: code
---

# /testar

## Categoria

validação

## Objetivo

Executar e gerenciar testes do projeto.

## Sub-comandos

| Sub-comando | Tipo |
|-------------|------|
| `/testar unit` | Testes unitários |
| `/testar integration` | Testes de integração |
| `/testar e2e` | Testes end-to-end |
| `/testar mutation` | Mutation testing (Stryker) |
| `/testar performance` | Benchmarks (BenchmarkDotNet) |
| `/testar tudo` | Todos os tipos |

## Procedimento

### Unit Tests
```powershell
dotnet test --filter "Category=Unit" --verbosity normal
```
- Arrange-Act-Assert pattern
- Um teste por comportamento
- Sem dependências externas
- Coverage mínimo: 85%

### Integration Tests
```powershell
dotnet test --filter "Category=Integration" --verbosity normal
```
- Testar com banco real (TestContainers)
- Testar com APIs reais (WireMock)
- Cleanup após cada teste

### E2E Tests
- Playwright ou Selenium
- Fluxos críticos do usuário
- Login, CRUD, checkout, etc.

### Mutation Testing
```powershell
dotnet stryker
```
- Mutant score ≥ 80%
- Identificar testes fracos

### Performance
```powershell
dotnet run --project benchmarks
```
- Baseline vs atual
- Latência, throughput, memória

## Saída

```json
{
  "unit": {"passed": 145, "failed": 0, "coverage": 87},
  "integration": {"passed": 23, "failed": 0},
  "mutation": {"score": 82},
  "performance": {"baseline": "2.1ms", "current": "1.9ms"},
  "ready": true
}
```

## Exemplos

```
/testar
/testar unit do módulo de pagamentos
/testar tudo e reportar coverage
/testar mutation do projeto
```
