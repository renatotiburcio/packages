---
id: T147
title: XFFilterBuilder-visual
priority: P0
status: pending
estimated_time: 24h
dependencies: T134
---

# T147: XFFilterBuilder-visual

## Objetivo
Construtor visual de criteria expressions type-safe, sem SQL injection.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFFilterBuilder permite construcao visual de criterios com operadores logicos (And/Or/Not), conditions (=, !=, <, >, BETWEEN, IN, LIKE, IS NULL), totalmente type-safe em C# (sem string SQL), com save/load em JSON para persistencia.

## Passos
1. Estudar API DevExpress FilterControl no docs.devexpress.com
2. Definir modelo XForge (FilterCriteria, FilterCondition, FilterGroup, FilterOperator)
3. Implementar ExpressionBuilder para LINQ Expression trees
4. Implementar persistencia JSON
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T147/XFFilterBuilder.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T147/FilterCriteria.cs
- Criar: src/XForge.DesignSystem/Components/T147/FilterCondition.cs
- Criar: src/XForge.DesignSystem/Components/T147/FilterGroup.cs
- Criar: src/XForge.DesignSystem/Components/T147/FilterOperator.cs
- Criar: src/XForge.DesignSystem/Components/T147/Serialization/FilterJsonSerializer.cs
- Criar: src/XForge.DesignSystem.Tests/T147FilterBuilderTests.cs
- Criar: docs/components/t147-filter-builder/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Type-safe via Expression trees - SEM SQL injection
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap