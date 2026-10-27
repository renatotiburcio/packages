---
id: T069
title: HxMessageBoxService + AddHxMessageBoxHost DI
priority: P0
status: pending
estimated_time: 4h
dependencies: T063
origin: Havit
---

# T069: HxMessageBoxService + AddHxMessageBoxHost DI

## Objetivo
Implementar servico IHXMessageBoxService para dialogs Yes/No/Cancel/OK via DI, com MessageBoxServiceCollectionExtensions.AddHxMessageBoxHost().

## Passos
1. Criar interface IXFMessageBoxService em XForge.DesignSystem/Modals/Services/
2. Criar XFMessageBoxService : IXFMessageBoxService (scoped DI)
3. Criar XFMessageBoxHost.razor + .razor.cs (renderiza dialog stack)
4. Criar XFMessageBoxRequest class (title, message, buttons enum)
5. Criar static class MessageBoxServiceCollectionExtensions com AddXForgeMessageBoxHost(this IServiceCollection)
6. Registrar IXFMessageBoxService como Scoped (per-circuit isolation)
7. Criar MessageBoxButtons enum: OK, OKCancel, YesNo, YesNoCancel
8. Criar MessageBoxResult enum: OK, Cancel, Yes, No

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Modals/Services/IXFMessageBoxService.cs
- Criar: src/XForge.DesignSystem/Components/Modals/Services/XFMessageBoxService.cs
- Criar: src/XForge.DesignSystem/Components/Modals/XFMessageBoxHost.razor
- Criar: src/XForge.DesignSystem/Components/Modals/XFMessageBoxHost.razor.cs
- Criar: src/XForge.DesignSystem.Shared/Extensions/MessageBoxServiceCollectionExtensions.cs
- Criar: src/XForge.DesignSystem/Components/Modals/MessageBoxRequest.cs
- Criar: src/XForge.DesignSystem/Components/Modals/MessageBoxButtons.cs
- Criar: src/XForge.DesignSystem/Components/Modals/MessageBoxResult.cs

## Criterios de Aceite
- [ ] IXFMessageBoxService compila, scoped DI
- [ ] XFMessageBoxHost renderiza sem erros
- [ ] AddXForgeMessageBoxHost() registra servico corretamente
- [ ] Testes bUnit validam ShowMessageBoxAsync com cada button combination
- [ ] Build 0 warnings
