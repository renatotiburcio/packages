---
id: T146
title: XFTreeList-hybrid
priority: P0
status: pending
estimated_time: 24h
dependencies: T010
---

# T146: XFTreeList-hybrid

## Objetivo
Tree + Grid combinado, dados hierarquicos com colunas editaveis e master-detail.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFTreeList combina arvore e grid em um unico componente: cada no tem varias colunas, ordenacao/filtragem/paginacao em qualquer coluna, virtual scrolling, selecao de nos (single/multi), e master-detail com templates customizados.

## Passos
1. Estudar API DevExpress TreeList no docs.devexpress.com
2. Definir modelo XForge (XFTreeList, XFTreeNode, XFTreeColumn)
3. Implementar virtual scrolling hierarquico
4. Renderizar colunas editaveis via templates
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T146/XFTreeList.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T146/XFTreeNode.cs
- Criar: src/XForge.DesignSystem/Components/T146/XFTreeColumn.cs
- Criar: src/XForge.DesignSystem/Components/T146/VirtualScrolling/VirtualTreeScrolling.cs
- Criar: src/XForge.DesignSystem.Tests/T146TreeListTests.cs
- Criar: docs/components/t146-tree-list/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap