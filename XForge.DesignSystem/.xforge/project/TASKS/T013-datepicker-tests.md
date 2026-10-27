---
id: T013
title: DatePicker/TimeInput — formato BR, cultura, validação
priority: P1
status: pending
estimated_time: 2h
dependencies: T001
---

# T013: DatePicker/TimeInput — formato BR, cultura, validação

## Objetivo
Testes para componentes de data/hora com formato brasileiro.

## Passos
1. Testar DatePicker: dd/MM/yyyy, min/max, disabled dates
2. Testar TimeInput: HH:mm, 24h, increments
3. Testar DateInput: raw date parsing, validation
4. Validar cultura pt-BR

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFDatePickerFunctionalTests.cs`
- Criar: `src/XForge.DesignSystem.Tests/Components/XFTimeInputFunctionalTests.cs`

## Critérios de Aceite
- [ ] DatePicker: 6+ testes
- [ ] TimeInput: 5+ testes
- [ ] Formato BR testado (dd/MM/yyyy)
- [ ] Validação de data inválida testada
- [ ] 0 failures
