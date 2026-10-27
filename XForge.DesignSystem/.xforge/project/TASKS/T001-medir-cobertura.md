---
id: T001
title: Medir cobertura real de testes
priority: P0
status: pending
estimated_time: 2h
dependencies: nenhuma
---

# T001: Medir cobertura real de testes

## Objetivo
Obter métrica real de cobertura de código (linhas, branches, métodos) para baseline preciso.

## Passos
1. Executar `dotnet test --collect:"XPlat Code Coverage"` no XForge.DesignSystem.Tests
2. Gerar report com `reportgenerator`
3. Extrair: overall coverage, main lib coverage, branch coverage
4. Salvar baseline em `.xforge/project/coverage-baseline.json`
5. Identificar componentes com < 50% cobertura

## Arquivos
- Criar: `.xforge/project/coverage-baseline.json`
- Criar: `.xforge/project/coverage-report.md`

## Critérios de Aceite
- [ ] Cobertura overall medida e documentada
- [ ] Cobertura por projeto medida
- [ ] Componentes com < 50% cobertura identificados
- [ ] Baseline salvo para comparação futura

## Notas
- Baseline anterior: 10.1% overall, 13.9% main lib, 1.7% branch
- Usar coverlet + reportgenerator
- Pode demorar 10+ minutos (6883 tests)
