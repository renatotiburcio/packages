---
name: accessibility-automation-expert
description: Expert em automação de testes de acessibilidade: axe-core, pa11y, Lighthouse CI.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# accessibility-automation-expert

## Objetivo

Automatizar testes de acessibilidade no pipeline de CI/CD.

## Ferramentas

### Axe-core
```javascript
// Playwright + Axe
const results = await page.evaluate(async () => {
    return await axe.run();
});
expect(results.violations).toHaveLength(0);
```

### Pa11y
```bash
pa11y --standard WCAG2AA http://localhost:3000
```

### Lighthouse CI
```yaml
# .github/workflows/lighthouse.yml
- uses: treosh/lighthouse-ci-action@v10
  with:
    urls: |
      http://localhost:3000
    configPath: ./lighthouserc.json
```

## Regras de CI/CD

| Critério | Threshold | Ação |
|----------|-----------|------|
| Violations A | 0 | Bloquear PR |
| Violations AA | 0 | Bloquear PR |
| Score Lighthouse | ≥ 90 | Warning se < 90 |
| Critical issues | 0 | Bloquear merge |

## Procedimento

1. Configurar axe-core no CI
2. Configurar pa11y para smoke tests
3. Configurar Lighthouse CI
4. Definir thresholds
5. Bloquear PRs com violações
6. Gerar relatório de tendência

## Regras

- Acessibilidade é obrigatória, não opcional
- Testar em toda build
- Baseline de score para detectar regressões
- Documentar exceções aceitas
