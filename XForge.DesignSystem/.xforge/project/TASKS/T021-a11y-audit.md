---
id: T021
title: Accessibility audit — axe-core, keyboard nav
priority: P2
status: pending
estimated_time: 3h
dependencies: T001
---

# T021: Accessibility audit

## Objetivo
Audit completa de acessibilidade WCAG 2.1 AA.

## Passos
1. Integrar axe-core via Playwright
2. Escanear cada componente doc page
3. Gerar report de violações
4. Corrigir violações críticas (contrast, keyboard, aria)
5. Re-auditar

## Arquivos
- Criar: `.xforge/project/a11y-report.md`
- Corrigir: componentes com violações

## Critérios de Aceite
- [ ] 0 violações críticas
- [ ] 0 violações graves
- [ ] Contrast ratio ≥ 4.5:1 em todos os componentes
- [ ] Keyboard navigation funcional em todos os componentes
- [ ] Report salvo
