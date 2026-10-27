---
id: T011
title: EditForm — testes de validação, state, submit flow
priority: P1
status: pending
estimated_time: 3h
dependencies: T001
---

# T011: EditForm — testes de validação, state, submit flow

## Objetivo
Testes funcionais completos para XFEditForm (formulários ERP).

## Passos
1. Ler `XFEditForm.razor.cs` para entender API e cascading context
2. Criar `XFEditFormFunctionalTests.cs` com testes de:
   - Render com model
   - Validação (required, regex, custom)
   - Submit flow (valid → submit, invalid → show errors)
   - State (pristine, dirty, submitted)
   - Disabled state
   - Integration with XFInput* components
   - FormState cascading
3. Validar: todos os testes passam

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFEditFormFunctionalTests.cs`
- Referência: `src/XForge.DesignSystem/Components/Forms/EditForm/XFEditForm.razor.cs`

## Critérios de Aceite
- [ ] 12+ testes funcionais
- [ ] Validação testada (invalid input → error shown)
- [ ] Submit testado (valid → callback invoked)
- [ ] State transitions testadas
- [ ] Integration com inputs testada
- [ ] 0 failures

## Notas
- EditForm usa `EditContext` cascading — precisa configurar `Services.AddSingleton<EditContext>()`
- Testar com model simples (ex: `Person` com Nome, Idade, Email)
