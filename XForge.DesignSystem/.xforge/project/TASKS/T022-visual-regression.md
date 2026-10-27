---
id: T022
title: Visual regression — screenshot tests
priority: P2
status: pending
estimated_time: 3h
dependencies: T001
---

# T022: Visual regression tests

## Objetivo
Testes de regressão visual para componentes críticos.

## Passos
1. Configurar Playwright + screenshot comparison
2. Gerar screenshots baseline para 20+ componentes
3. Configurar CI para comparar em cada PR
4. Tolerância: 0.1% pixel diff

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Visual/ComponentVisualTests.cs`
- Criar: `tests/baseline/` (screenshots)

## Critérios de Aceite
- [ ] Screenshots baseline para 20+ componentes
- [ ] Comparação automática em CI
- [ ] 0 regressões detectadas
