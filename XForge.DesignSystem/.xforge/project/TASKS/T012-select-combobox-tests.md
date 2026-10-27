---
id: T012
title: Select/Combobox — binding, search, cascading
priority: P1
status: pending
estimated_time: 2h
dependencies: T001
---

# T012: Select/Combobox — binding, search, cascading

## Objetivo
Testes funcionais para XFSelect e XFCombobox.

## Passos
1. Ler componentes para entender diferenças
2. Testar Select: binding, selection change, disabled options
3. Testar Combobox: search/filter, selection, custom template
4. Validar: todos passam

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFSelectFunctionalTests.cs`
- Criar: `src/XForge.DesignSystem.Tests/Components/XFComboboxFunctionalTests.cs`

## Critérios de Aceite
- [ ] Select: 8+ testes
- [ ] Combobox: 8+ testes
- [ ] Binding testado (select → value updated)
- [ ] Search testado (type → filter)
- [ ] 0 failures
