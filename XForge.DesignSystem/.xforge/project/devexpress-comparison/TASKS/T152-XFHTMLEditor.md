---
id: T152
title: XFHTMLEditor
priority: P2
status: pending
estimated_time: 24h
dependencies: T144
---

# T152: XFHTMLEditor

## Objetivo
WYSIWYG HTML editor separado de RTE, com sanitization e paste cleanup.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFHTMLEditor oferece WYSIWYG HTML editor (separado de RTE que foca em rich text), toolbar com bold/italic/links/lists/tables/images/code, source view toggle, paste from Word cleanup, e HTML sanitization robusta para XSS prevention.

## Passos
1. Estudar API DevExpress HtmlEditor no docs.devexpress.com
2. Definir modelo XForge (XFHTMLEditor, HtmlToolbar, HtmlSanitizer)
3. Implementar HtmlSanitizer com whitelist de tags/attributes
4. Implementar paste cleanup (remove Word-specific markup)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
- [ ] XSS prevention: <script>, on* handlers, javascript: URLs bloqueados
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T152/XFHTMLEditor.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T152/HtmlToolbar.cs
- Criar: src/XForge.DesignSystem/Components/T152/HtmlSanitizer.cs
- Criar: src/XForge.DesignSystem/Components/T152/PasteCleanupService.cs
- Criar: src/XForge.DesignSystem.Tests/T152HTMLEditorTests.cs
- Criar: docs/components/t152-html-editor/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10
- [ ] XSS prevention validado por testes

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- HtmlSanitizer OWASP deve ser usado (MIT license)
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap