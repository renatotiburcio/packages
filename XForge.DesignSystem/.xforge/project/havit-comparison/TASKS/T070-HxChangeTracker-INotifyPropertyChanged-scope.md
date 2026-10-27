---
id: T070
title: HxChangeTracker INotifyPropertyChanged scope
priority: P1
status: pending
estimated_time: 6h
dependencies: nenhuma
origin: Havit
---

# T070: HxChangeTracker INotifyPropertyChanged scope

## Objetivo
Implementar XFFxChangeTracker que escuta INotifyPropertyChanged e re-renderiza apenas child content, evitando re-render do parent. Pattern do HxChangeTracker do Havit.

## Passos
1. Criar XFFxChangeTracker : ComponentBase, IDisposable em XForge.DesignSystem/Components/Utilities/
2. Adicionar [Parameter] public RenderFragment ChildContent
3. Adicionar [Parameter, EditorRequired] public INotifyPropertyChanged Value
4. Adicionar [Inject] protected ILogger<XFFxChangeTracker> Logger
5. Adicionar private INotifyPropertyChanged _previousValueSet (detectar troca)
6. Override OnParametersSet: subscribe Value.PropertyChanged += handler, unsubscribe _previousValueSet
7. private void Value_PropertyChanged: Logger.LogDebug + StateHasChanged()
8. Override BuildRenderTree: AddContent(0, ChildContent)
9. Implementar Dispose: unsubscribe Value.PropertyChanged

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Utilities/XFFxChangeTracker.cs

## Criterios de Aceite
- [ ] XFFxChangeTracker compila, herda ComponentBase + IDisposable
- [ ] PropertyChanged handler re-renderiza child content via StateHasChanged
- [ ] OnParametersSet troca subscribe/unsubscribe corretamente
- [ ] Dispose chama unsubscribe (evita memory leak)
- [ ] Testes bUnit: troca de value re-subscribe
