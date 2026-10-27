---
id: T089
title: [FluentUIComponent] attribute → class discovery
priority: P1
status: pending
estimated_time: 4h
dependencies: T080
---

# T089: [FluentUIComponent] attribute → class discovery

## Objetivo
Descobrir automaticamente todos os componentes Blazor anotados com [FluentUIComponent] para docs auto-gerada e icon registry.

## Passos
1. Criar attribute [FluentUIComponent(Name, Description, Category, UsesIcons)]
2. Aplicar nos 152 componentes core
3. Criar Roslyn SG `XFComponentDiscoveryGenerator`
4. SG gera `XFFluentUIComponentIndex.g.cs` com metadata
5. API runtime: `IEnumerable<XFComponentInfo> GetAllComponents()` (DI singleton)
6. Suportar search: GetByName, GetByCategory, GetByTag
7. UI helper: <XFComponentGrid Filter="..." /> (grid de componentes para docs)
8. Tests: 15+ (SG, discovery, search, UI grid)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/FluentUIComponentAttribute.cs`
- Criar: `src/XForge.DesignSystem/Components/IXFComponentDiscovery.cs`
- Criar: `src/XForge.DesignSystem/Components/XFComponentDiscovery.cs`
- Criar: `src/XForge.DesignSystem/Components/XFComponentInfo.cs`
- Criar: `src/XForge.DesignSystem.Generators/XFComponentDiscoveryGenerator.cs` (Roslyn SG)
- Modificar: 10+ componentes core (apply attribute)
- Criar: `src/XForge.DesignSystem/Components/XFComponentGrid.razor` (UI helper)
- Criar: `tests/XForge.DesignSystem.Tests/ComponentDiscoveryTests.cs` (15+ tests)

## Criterios de Aceite
- [ ] Attribute [FluentUIComponent] com 5 propriedades
- [ ] SG gera index.g.cs correto
- [ ] Runtime discovery via DI singleton
- [ ] Search por name/category/tag funciona
- [ ] UI grid renderiza componentes com search/filter
- [ ] Aplicado a 152+ componentes core
- [ ] Tests: 15+ (SG, discovery, search, UI)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Reusar padrao de [FluentUIComponent] de T087
- Auto-geracao de docs: link docs page via SG
- Performance: pre-computed index, no reflection runtime
