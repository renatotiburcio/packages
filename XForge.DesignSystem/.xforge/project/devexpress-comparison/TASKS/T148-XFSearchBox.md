---
id: T148
title: XFSearchBox
priority: P1
status: pending
estimated_time: 8h
dependencies: T001
---

# T148: XFSearchBox

## Objetivo
Input com suggestions dropdown, debounce e recent searches.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFSearchBox oferece input com suggestions dropdown async, debounce 300ms, navegacao por teclado (Arrow Up/Down/Enter/Esc), recent searches persistidos em localStorage, e data provider plugavel (async/Task<IEnumerable<T>>).

## Passos
1. Estudar API DevExpress Search no docs.devexpress.com
2. Definir modelo XForge (XFSearchBox, SearchSuggestion, SearchProvider)
3. Implementar debounce com CancellationToken
4. Persistir recent searches em localStorage (Blazored.LocalStorage opcional)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T148/XFSearchBox.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T148/SearchSuggestion.cs
- Criar: src/XForge.DesignSystem/Components/T148/ISearchProvider.cs
- Criar: src/XForge.DesignSystem/Components/T148/RecentSearchesService.cs
- Criar: src/XForge.DesignSystem.Tests/T148SearchBoxTests.cs
- Criar: docs/components/t148-search-box/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA (ARIA combobox)
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap