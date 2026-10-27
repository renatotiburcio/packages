---
id: T010
title: DataGrid — testes de sorting, paging, filtering, editing
priority: P1
status: pending
estimated_time: 3h
dependencies: T001
---

# T010: DataGrid — testes de sorting, paging, filtering, editing

## Objetivo
Testes funcionais completos para XFDataGrid (componente mais crítico de ERP).

## Passos
1. Ler `XFDataGrid.razor.cs` e `XFDataGridColumn.razor.cs` para entender API
2. Criar `XFDataGridFunctionalTests.cs` com testes de:
   - Render com Items provider
   - Sorting (asc/desc, multi-column)
   - Paging (page size, navigation)
   - Filtering (string, date, numeric)
   - Editing (inline, modal)
   - Selection (single, multi)
   - Empty state, loading state
   - Custom columns (template, width, visibility)
3. Validar: todos os testes passam

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFDataGridFunctionalTests.cs`
- Referência: `src/XForge.DesignSystem/Components/DataDisplay/DataGrid/XFDataGrid.razor.cs`

## Critérios de Aceite
- [ ] 15+ testes funcionais
- [ ] Sorting testado (click header, verify order)
- [ ] Paging testado (change page, verify items)
- [ ] Filtering testado (apply filter, verify results)
- [ ] Editing testado (enter edit, save, cancel)
- [ ] Selection testado (select row, verify state)
- [ ] 0 failures

## Notas
- DataGrid usa JS interop para drag/resize — usar `JSInterop.Mode = Lovere`
- Testar com `IEnumerable<>` simples (não precisa de banco)
