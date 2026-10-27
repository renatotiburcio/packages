---
id: T023
title: Theme system — contrast, dark mode validation
priority: P2
status: pending
estimated_time: 2h
dependencies: T001
---

# T023: Theme system validation

## Objetivo
Validar sistema de temas (light/dark/brand) e contrast ratios.

## Passos
1. Validar que todos os componentes funcionam em light e dark mode
2. Calcular contrast ratios dos tokens
3. Verificar que tokens CSS estão consistentes
4. Corrigir inconsistências

## Arquivos
- Criar: `.xforge/project/theme-audit.md`
- Corrigir: tokens inconsistentes

## Critérios de Aceite
- [ ] Todos os componentes testados em light + dark
- [ ] Contrast ratio ≥ 4.5:1 em light mode
- [ ] Contrast ratio ≥ 4.5:1 em dark mode
- [ ] Tokens consistentes
