---
id: T001
title: Adotar ParameterState<T> framework para todos componentes XForge
priority: P0
status: pending
estimated_time: 16h
dependencies: nenhuma
---

# T001: Adotar ParameterState<T> framework

## Objetivo
Refatorar todos os componentes XForge para usar o padrao ParameterState<T> do MudBlazor, garantindo auto-properties + change handlers type-safe.

## Passos
1. Adicionar classe `ParameterState<T>` em `src/XForge.DesignSystem/Core/State/ParameterState.cs`
2. Adicionar metodo `CreateRegisterScope()` em `XForgeComponentBase`
3. Refatorar `XFInputComponentBase` para usar `_valueState`, `_disabledState`, etc.
4. Migrar 5 componentes-piloto: XFButton, XFTextInput, XFCheckbox, XFTabs, XFModal
5. Validar BL0005, sem warnings de re-render loop
6. Documentar padrao em `.xforge/knowledge/domains/design-system/parameter-state-pattern.md`

## Arquivos
- Criar: `src/XForge.DesignSystem/Core/State/ParameterState.cs`
- Modificar: `src/XForge.DesignSystem/Base/XForgeComponentBase.cs`
- Refatorar: 5 componentes-piloto

## Criterios de Aceite
- [ ] ParameterState<T> compila com testes unitarios
- [ ] 5 componentes-piloto migrados sem warnings
- [ ] Build clean (0 erros, 0 warnings)
- [ ] Docs atualizado com pattern

## Notas
- Padrao vem de MudBlazor (10.4k stars, validado em producao)
- Elimina BL0005 (set param via @ref)
- Change handlers ficam no escopo de registro
