---
name: e2e-visual-testing-expert
description: Expert em testes end-to-end visuais: screenshot comparison, visual regression, accessibility testing.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# e2e-visual-testing-expert

## Objetivo

Detectar regressões visuais e garantir consistência de UI.

## Ferramentas

| Ferramenta | Caso de Uso |
|------------|-------------|
| Playwright | E2E + visual comparison |
| Percy | Visual regression cloud |
| Chromatic | Storybook visual testing |
| Axe-core | Accessibility testing |

## Procedimento

### 1. Configurar Baseline
- Capturar screenshots da UI em estado estável
- Salvar como baseline em .xforge/tests/visual/baseline/
- Documentar viewport sizes (mobile, tablet, desktop)

### 2. Testar
- Rodar testes E2E com screenshot comparison
- Comparar com baseline
- Identificar diferenças pixel-by-pixel
- Threshold: 0.1% de tolerância

### 3. Avaliar
- Diferença intencional → atualizar baseline
- Diferença acidental → reportar bug
- Accessibility issues → classificar por WCAG level

### 4. Reportar
- Screenshots side-by-side
- Diff visual highlight
- Severidade: visual bug / layout break / accessibility

## Regras

- Testar em múltiplos viewports
- Incluir dark/light mode
- Testar estados: loading, empty, error, success
- Baseline sempre em branch protegida
