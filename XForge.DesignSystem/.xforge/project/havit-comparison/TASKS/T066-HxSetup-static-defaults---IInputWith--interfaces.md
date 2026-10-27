---
id: T066
title: HxSetup static defaults + IInputWith* interfaces
priority: P0
status: pending
estimated_time: 8h
dependencies: T045
origin: Havit
---

# T066: HxSetup static defaults + IInputWith* interfaces

## Objetivo
Adotar HxSetup.Defaults (static global) + 3 interfaces segregadas (IInputWithPlaceholder, IInputWithSize, IInputWithLabelType) do Havit para substituicao do pattern atual de settings de componentes.

## Passos
1. Criar XForgeSetup.Defaults (static LabelType, InputSize, etc) em XForge.DesignSystem.Shared.Setup
2. Criar 3 interfaces em XForge.DesignSystem.Shared/Contracts/IInputWithPlaceholder.cs, IInputWithSize.cs, IInputWithLabelType.cs
3. Adicionar metodo virtual GetDefaults() protected em XForgeComponentBase
4. Adicionar metodo abstract GetSettings() protected em XForgeComponentBase
5. Refatorar XFInput* (12+ componentes) para implementar as 3 interfaces
6. Pattern: PropriedadeXEffective = PropriedadeX ?? GetSettings()?.PropriedadeX ?? GetDefaults().PropriedadeX ?? XForgeSetup.Defaults.PropriedadeX
7. Adicionar testes para precedence (instance > settings > defaults > global)

## Arquivos
- Criar: src/XForge.DesignSystem.Shared/Setup/XForgeSetup.cs
- Criar: src/XForge.DesignSystem.Shared/Contracts/IInputWithPlaceholder.cs
- Criar: src/XForge.DesignSystem.Shared/Contracts/IInputWithSize.cs
- Criar: src/XForge.DesignSystem.Shared/Contracts/IInputWithLabelType.cs
- Modificar: src/XForge.DesignSystem/Components/Base/XForgeComponentBase.cs (adicionar GetDefaults/GetSettings virtual)
- Modificar: src/XForge.DesignSystem/Components/Forms/* (12+ componentes)

## Criterios de Aceite
- [ ] XForgeSetup.Defaults funciona como static singleton
- [ ] 3 interfaces segregadas compilar sem erros
- [ ] Pattern Effective precedence funciona corretamente
- [ ] 12+ componentes refatorados, build com 0 warnings (TreatWarningsAsErrors)
- [ ] Testes cobrindo precedence: instance > settings > defaults > global
