---
id: T071
title: HxGrid w DataProvider delegate
priority: P1
status: pending
estimated_time: 16h
dependencies: T067
origin: Havit
---

# T071: HxGrid w DataProvider delegate

## Objetivo
Implementar XFGrid<TItem> server-driven com GridDataProviderDelegate<TItem>, suportando paging, sorting, filtering server-side + UserState save/restore. Pattern do HxGrid Havit.

## Passos
1. Criar XFGrid<TItem> : ComponentBase, IAsyncDisposable com [CascadingTypeParameter(nameof(TItem))]
2. Adicionar [Parameter, EditorRequired] public GridDataProviderDelegate<TItem> DataProvider
3. Adicionar [Parameter, EditorRequired] public RenderFragment Columns
4. Adicionar [Parameter] public bool SelectionEnabled = true
5. Adicionar [Parameter] public bool MultiSelectionEnabled = false
6. Criar public delegate Task<GridDataProviderResult<TItem>> GridDataProviderDelegate<TItem>(GridDataProviderRequest request)
7. Criar classes GridDataProviderRequest (StartIndex, Count, Sorting, Filters)
8. Criar classes GridDataProviderResult<TItem> (Data, TotalCount)
9. Criar SortingItem class (SortString, SortDirection)
10. Criar XFGridColumn<TItem> : ComponentBase + XFGridColumnBase
11. Implementar ColumnsRegistration cascading value (registrar colunas para render)
12. Implementar LoadDataAsync, RefreshAsync, SelectionChanged handlers
13. Suportar Templates: LoadingDataTemplate, EmptyDataTemplate, PaginationTemplate, LoadMoreTemplate

## Arquivos
- Criar: src/XForge.DesignSystem/Components/DataGrid/XFGrid.razor
- Criar: src/XForge.DesignSystem/Components/DataGrid/XFGrid.razor.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/XFGridColumn.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/XFGridColumnBase.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/GridDataProviderDelegate.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/GridDataProviderRequest.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/GridDataProviderResult.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/SortingItem.cs
- Criar: src/XForge.DesignSystem/Components/DataGrid/GridUserState.cs

## Criterios de Aceite
- [ ] XFGrid<TItem> compila com [CascadingTypeParameter]
- [ ] DataProvider async delegate invocado em paging/sorting/filtering
- [ ] Columns registration via cascading funciona
- [ ] Selection (single + multi) funcional
- [ ] Testes bUnit: mock DataProvider valida request/result
