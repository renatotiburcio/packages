---
id: T016
title: TreeView — expand/collapse, binding, virtualização
priority: P1
status: pending
estimated_time: 2h
dependencies: T001
---

# T016: TreeView — expand/collapse, binding, virtualização

## Objetivo
Testes para XFTreeView e XFVirtualList.

## Passos
1. Testar TreeView: expand node, collapse, select, bind hierarchical data
2. Testar VirtualList: render visible items, scroll, dynamic load
3. Validar com dados hierárquicos simulados

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFTreeViewFunctionalTests.cs`
- Criar: `src/XForge.DesignSystem.Tests/Components/XFVirtualListFunctionalTests.cs`

## Critérios de Aceite
- [ ] TreeView: 8+ testes
- [ ] VirtualList: 5+ testes
- [ ] Expand/collapse testado
- [ ] Selection testado
- [ ] 0 failures
