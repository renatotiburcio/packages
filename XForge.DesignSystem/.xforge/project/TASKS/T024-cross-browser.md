---
id: T024
title: Cross-browser — Playwright tests
priority: P2
status: pending
estimated_time: 2h
dependencies: T001
---

# T024: Cross-browser tests

## Objetivo
Testes em Chrome, Firefox e Safari.

## Passos
1. Configurar Playwright com multi-browser
2. Testar componentes críticos em cada browser
3. Verificar funcionalidade e visual

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/CrossBrowser/CrossBrowserTests.cs`

## Critérios de Aceite
- [ ] Chrome: 100% pass
- [ ] Firefox: 100% pass
- [ ] Safari/WebKit: 100% pass (ou degradacao graceful documentada)
