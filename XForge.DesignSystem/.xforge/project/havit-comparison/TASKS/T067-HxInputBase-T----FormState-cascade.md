---
id: T067
title: HxInputBase<T> + FormState cascade
priority: P0
status: pending
estimated_time: 12h
dependencies: T066
origin: Havit
---

# T067: HxInputBase<T> + FormState cascade

## Objetivo
Adotar pattern HxInputBase<TValue> : InputBase<TValue> com ICascadeEnabledComponent e cascading FormState parameter do Havit para bulk enable/disable de forms.

## Passos
1. Criar XFInputBase<TValue> : InputBase<TValue>, ICascadeEnabledComponent, IFormValueComponent em XForge.DesignSystem/Forms/Base/
2. Adicionar [CascadingParameter] protected FormState FormState { get; set; }
3. Implementar ICascadeEnabledComponent.FormState (interface segregation)
4. Criar XFInputBaseWithInputGroups : XFInputBase<TValue> (para inputs com icones)
5. Criar XFFormState (cascading value provider) - container component
6. Refatorar 12+ inputs (Text, Number, Date, Select, MultiSelect, Checkbox, Radio, Switch, etc) para herdar de XFInputBase<T>
7. Pattern: EnabledEffective = Enabled ?? FormState.Enabled
8. Adicionar testes para cascadign FormState propagation

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Forms/Base/XFInputBase.cs
- Criar: src/XForge.DesignSystem/Components/Forms/Base/XFInputBaseWithInputGroups.cs
- Criar: src/XForge.DesignSystem/Components/Forms/Base/XFFormState.cs
- Criar: src/XForge.DesignSystem/Components/Forms/Base/IFormValueComponent.cs
- Criar: src/XForge.DesignSystem/Components/Forms/Base/ICascadeEnabledComponent.cs
- Modificar: src/XForge.DesignSystem/Components/Forms/* (12+ componentes)

## Criterios de Aceite
- [ ] XFInputBase<T> compila, herda de InputBase<TValue> com EditContext
- [ ] XFFormState cascadinha funciona (wrap multi-inputs com disable bulk)
- [ ] Todos 12+ inputs refatorados herdam de XFInputBase<T>
- [ ] Build 0 warnings, 100% testes passando
- [ ] Padrao FormState functional testado com bUnit
