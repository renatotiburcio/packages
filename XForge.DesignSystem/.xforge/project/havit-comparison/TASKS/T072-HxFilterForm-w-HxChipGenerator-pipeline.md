---
id: T072
title: HxFilterForm w HxChipGenerator pipeline
priority: P1
status: pending
estimated_time: 12h
dependencies: T067
origin: Havit
---

# T072: HxFilterForm w HxChipGenerator pipeline

## Objetivo
Implementar XFFilterForm : XFModelEditForm com chip generators que convertem filtros em chips visuais. Pattern HxFilterForm Havit.

## Passos
1. Criar XFFilterForm : ComponentBase (nao herda ModelEditForm direto, usa EditForm internamente)
2. Adicionar [Parameter] public object Model (filter model)
3. Adicionar [Parameter] public EventCallback OnFilter
4. Adicionar [Parameter] public EventCallback OnClear
5. Criar interface IXFChipGenerator com Task<RenderFragment> GenerateChipAsync(FilterContext ctx)
6. Criar XFChipGenerator (abstract base) com template parameter
7. Criar XFChipList.razor + .razor.cs (renderiza chips com remove button)
8. Implementar pipeline: EditForm submit -> OnFilter callback -> chip list updated
9. Adicionar XFFilterChipRemoveActionBuilder (helper para actions)
10. Testes: filter 3 props -> 3 chips rendered, remove chip -> state updated

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Filter/XFFilterForm.cs
- Criar: src/XForge.DesignSystem/Components/Filter/IXFChipGenerator.cs
- Criar: src/XForge.DesignSystem/Components/Filter/XFChipGenerator.cs
- Criar: src/XForge.DesignSystem/Components/Filter/XFChipList.razor
- Criar: src/XForge.DesignSystem/Components/Filter/XFChipList.razor.cs
- Criar: src/XForge.DesignSystem/Components/Filter/ChipItem.cs

## Criterios de Aceite
- [ ] XFFilterForm compila, herda ComponentBase
- [ ] Chip generator pipeline renderiza chips baseado em filter state
- [ ] Remove chip dispara EventCallback OnFilter
- [ ] Testes bUnit: 3 props -> 3 chips, remove -> 2 chips
- [ ] Pattern extensivel para novos chip generators
