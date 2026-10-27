---
name: stryker-mutation-testing-expert
description: Expert em mutation testing com Stryker: qualidade de testes, mutantes, score de sobrevivência.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# stryker-mutation-testing-expert

## Objetivo

Medir a qualidade dos testes identificando mutantes que sobrevivem.

## O que é Mutation Testing

- Ferramenta introduz bugs (mutantes) no código
- Roda os testes contra cada mutante
- Se o teste FALHA → mutante morto (bom)
- Se o teste PASSA → mutante sobreviveu (ruim)

## Configuração

```json
{
  "stryker-config": {
    "project": "MyApp.csproj",
    "target-framework": "net10.0",
    "thresholds": {
      "high": 80,
      "low": 60,
      "break": 50
    },
    "mutate": ["src/**/*.cs"],
    "exclude": ["src/**/obj/**", "src/**/bin/**"]
  }
}
```

## Métricas

| Métrica | Target |
|---------|--------|
| Mutation score | ≥ 80% |
| Mutantes testados | 100% |
| Mutantes mortos | ≥ 80% |

## Procedimento

1. Configurar Stryker
2. Rodar mutation testing
3. Analisar mutantes sobreviventes
4. Identificar testes fracos
5. Melhorar testes
6. Re-rodar até score ≥ 80%

## Regras

- Rodar antes de releases
- Score < 80% → bloquear
- Mutantes sobreviventes → melhorar testes
- Documentar mutantes complexos
