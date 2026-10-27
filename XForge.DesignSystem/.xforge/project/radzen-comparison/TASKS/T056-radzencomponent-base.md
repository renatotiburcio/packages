---
id: T056
title: RadzenComponent base com OnBecameInvisible cleanup
priority: P0
status: pending
estimated_time: 4h
dependencies: T045
---

# T056: RadzenComponent base com OnBecameInvisible cleanup

## Objetivo
Estender `XForgeComponentBase` com `OnBecameInvisible()` cleanup pattern para componentes toggleable (Dialog, Drawer, Modal).

## Passos
1. Adicionar `MouseEnter`, `MouseLeave`, `ContextMenu` event callbacks na base
2. Implementar `OnBecameInvisible()` virtual que cleanup JS handlers sem dispose
3. Adicionar `Culture` + `CascadingParameter DefaultCulture` para i18n
4. Adicionar `GetCssClass()` que combina `GetComponentCssClass()` + attr `class`
5. Adicionar `GetId()` que usa attr `id` ou `UniqueID` (base64 10-char GUID)
6. Adicionar `Debouncer` interno (T057) + `CurrentStyle` (parse "k:v;k:v")
7. Implementar `OnBecameInvisible()` em Dialog/Drawer/Modal

## Arquivos
- Modificar: `src/XForge.DesignSystem/Components/Base/XForgeComponentBase.cs`
- Modificar: `src/XForge.DesignSystem/Components/Overlay/XFDialog.razor.cs`
- Modificar: `src/XForge.DesignSystem/Components/Overlay/XFDrawer.razor.cs`

## Critérios de Aceite
- [ ] Dialog toggleable sem re-mount preserva state
- [ ] JS handlers cleanup ao Visible=false
- [ ] Re-attach JS handlers ao Visible=true
- [ ] Culture cascading funciona
- [ ] MouseEnter/Leave funcionam com TooltipService
- [ ] ContextMenu funciona com ContextMenuService
- [ ] Build 0 warnings
- [ ] Testes cobrem OnBecameInvisible

## Notas
- Origem: **Radzen** ⭐
- Ref: `radzen-blazor/Radzen.Blazor/RadzenComponent.cs:407`
- Pattern killer: re-render friendly sem dispose
- Complementa T045 (ShouldAutoGenerateId)