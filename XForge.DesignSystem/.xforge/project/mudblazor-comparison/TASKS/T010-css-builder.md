---
id: T010
title: Implementar CssBuilder utility (substituir concat de strings)
priority: P1
status: pending
estimated_time: 4h
dependencies: nenhuma
---

# T010: Implementar CssBuilder utility

## Objetivo
Substituir concats manuais de CSS classes em componentes XForge por uma API fluente type-safe, inspirada no MudBlazor CssBuilder.

## Passos
1. Criar classe `CssBuilder` em `src/XForge.DesignSystem/Utilities/CssBuilder.cs`
2. API: `new CssBuilder("base")`, `.AddClass("x", cond)`, `.AddClass(extra)`, `.Build()`
3. API paralela `StyleBuilder` para inline styles
4. Migrar 10 componentes-piloto para usar CssBuilder
5. Adicionar testes unitarios para CssBuilder

## Arquivos
- Criar: `src/XForge.DesignSystem/Utilities/CssBuilder.cs`
- Criar: `src/XForge.DesignSystem/Utilities/StyleBuilder.cs`
- Criar: `src/XForge.DesignSystem.Tests/Utilities/CssBuilderTests.cs`
- Modificar: 10 componentes-piloto

## Criterios de Aceite
- [ ] CssBuilder compila com testes
- [ ] Build output identico ao atual (sem regressao visual)
- [ ] 10 componentes-piloto migrados
- [ ] Code review aprova pattern

## Notas
- MudBlazor usa CssBuilder em 100% dos componentes
- Sem regex ou string.Split em hot path
