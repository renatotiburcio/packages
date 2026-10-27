---
id: T057
title: Debouncer/Throttle com System.Timers
priority: P0
status: pending
estimated_time: 2h
dependencies: nenhuma
---

# T057: Debouncer/Throttle com System.Timers

## Objetivo
Adicionar utility `Debouncer` para debounce/throttle de ações async em TextBox, Slider, Numeric, Search inputs.

## Passos
1. Criar `Utilities/Debouncer.cs` (internal class, IDisposable)
2. Implementar `Debounce(int interval, Func<Task> action)` com System.Timers.Timer
3. Implementar `Throttle(int interval, Func<Task> action)`
4. Wrap com try-catch para TaskCanceledException
5. Adicionar método `Debounce()` em `XForgeComponentBase` que delega
6. Aplicar em TextBox onChange (500ms default)

## Arquivos
- Criar: `src/XForge.DesignSystem/Utilities/Debouncer.cs`
- Modificar: `src/XForge.DesignSystem/Components/Base/XForgeComponentBase.cs`
- Modificar: `src/XForge.DesignSystem/Components/Forms/XFTextBox.razor.cs`

## Critérios de Aceite
- [ ] Debounce 500ms default em TextBox
- [ ] Cancelamento limpo via Dispose
- [ ] Throttle respeita intervalo mínimo
- [ ] TaskCanceledException tratado silenciosamente
- [ ] Build 0 warnings
- [ ] Testes unitários para Debouncer (mock Timer)

## Notas
- Origem: **Radzen** ⭐
- Ref: `radzen-blazor/Radzen.Blazor/Debouncer.cs` (79 lines)
- 2h implementação direta
- Use `System.Timers.Timer` (não `System.Threading.Timer`) para UI thread affinity