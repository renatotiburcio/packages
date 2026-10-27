---
id: T068
title: HxDialogBase<TResult> (TaskCompletionSource)
priority: P0
status: pending
estimated_time: 8h
dependencies: T060
origin: Havit
---

# T068: HxDialogBase<TResult> (TaskCompletionSource)

## Objetivo
Implementar base class para dialogs modais que retornam Task<DialogResult<TResult>> via TaskCompletionSource, pattern HxDialogBase do Havit. Substitui XFModal complexo.

## Passos
1. Criar abstract XFDialogBase<TResult> : ComponentBase em XForge.DesignSystem/Modals/Base/
2. Adicionar campo protected internal XFModal _modal (referencia via @ref)
3. Adicionar private TaskCompletionSource<DialogResult<TResult>> _resultCompletion
4. Adicionar private bool _setResultOnModalClosed (evitar duplo fire)
5. Implementar public async Task<DialogResult<TResult>> ShowAsync() com await Modal.ShowAsync() + _resultCompletion.Task
6. Implementar protected internal async Task HideAsync(TResult value) com _resultCompletion.SetResult
7. Implementar protected internal async Task HideAsCancelledAsync()
8. Implementar private void HandleModalClosed() com fallback para cancelled
9. Override OnAfterRender para assert Modal != null e set _firstRendered = true
10. Criar DialogResult<TResult> record: CreateSuccessful(T), CreateCancelled()

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Modals/Base/XFDialogBase.cs
- Criar: src/XForge.DesignSystem/Components/Modals/Base/DialogResult.cs
- Modificar: src/XForge.DesignSystem/Components/Modals/XFModal.razor (expor referencia public)

## Criterios de Aceite
- [ ] XFDialogBase<TResult> compila, abstract
- [ ] ShowAsync() retorna Task<DialogResult<TResult>>
- [ ] HideAsync(T) seta result, HideAsCancelledAsync() seta cancelled
- [ ] HandleModalClosed() fallback funciona quando user clica X (escape)
- [ ] Testes com bUnit validam fluxo Show->Hide->Result
