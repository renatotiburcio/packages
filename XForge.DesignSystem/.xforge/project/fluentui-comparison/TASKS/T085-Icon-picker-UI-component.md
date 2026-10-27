---
id: T085
title: Icon picker UI component (XFSelector com search)
priority: P1
status: pending
estimated_time: 8h
dependencies: T078, T020
---

# T085: Icon picker UI component (XFSelector com search)

## Objetivo
Criar componente Blazor `<XFIconPicker />` que permite buscar e selecionar icones. UX similar a FluentUI docs picker, com search, variant switch e size selector.

## Passos
1. Criar `Components/Icon/XFIconPicker.razor` (markup com XFSelector + grid)
2. Criar `Components/Icon/XFIconPicker.razor.cs` (codigo)
3. Parametros: Value (XFIconInfo), Variant, Size, OnValueChanged (EventCallback)
4. Search bar (input com debounce 250ms)
5. Grid de icones (24 por pagina, 6 colunas desktop / 3 mobile)
6. Variant switcher (toggle Regular/Filled/Light/Color)
7. Size switcher (16/20/24/32/48)
8. Selected state visual (border highlight)
9. Keyboard navigation (arrow keys, enter, escape)
10. Accessibility: aria-selected, role=grid, focus management
11. Suportar drag-and-drop para canvas (bonus)
12. Lazy load: paginacao de 24 icones (infinite scroll)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconPicker.razor`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconPicker.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconPicker.razor.types.cs`
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor` (use new component)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconPickerTests.cs` (15+ tests)
- Criar: `examples/Demo/Pages/IconPickerDemo.razor` (showcase)

## Criterios de Aceite
- [ ] Search bar com debounce (250ms)
- [ ] Grid de icones (responsive 3-6 colunas)
- [ ] Variant + Size switchers
- [ ] Paginacao (24/pagina, infinite scroll)
- [ ] Keyboard navigation (arrow + enter + escape)
- [ ] A11y: aria-selected, role=grid, focus management
- [ ] OnValueChanged event
- [ ] Two-way binding (@bind-Value)
- [ ] Tests: 15+ (search, paginacao, keyboard, a11y, event)
- [ ] Demo page funcional
- [ ] Build: 0 erros, 0 warnings

## Notas
- Baseado em FluentUI Blazor docs icon picker
- Usar XFSelector (T020) como wrapper, mas com grid customizado
- Lazy load via ScrollViewer (infinite scroll)
- Debouncer (T057) para search input
