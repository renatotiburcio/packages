---
id: T087
title: Per-component icon registry + bundle trimmable
priority: P1
status: pending
estimated_time: 6h
dependencies: T078
---

# T087: Per-component icon registry + bundle trimmable

## Objetivo
Resolver problema de bundle huge (21034 icones) com per-component icon registry via [FluentUIComponent] attribute + Roslyn SG. Bundle 10x menor, build-time analysis.

## Passos
1. Criar attribute [FluentUIComponent] com parametro UsesIcons (XFIconInfo[])
2. Aplicar attribute nos componentes XFFluent: [FluentUIComponent(UsesIcons = [Icons.Regular.Home, ...])]
3. Criar Roslyn SG `XFIconRegistryGenerator` ([Generator] IIncrementalGenerator)
4. SG parseia assemblies em busca de [FluentUIComponent]
5. Para cada componente: gera icones usados em `XFIconCatalogIndex.g.cs`
6. Bundle final: 1 arquivo .g.cs com Dict<ComponentType, List<IconName>>
7. Trimmer custom step: drop icones nao usados (WasmPublish step)
8. Documentation: como usar [FluentUIComponent] em componentes custom
9. Tests: 10+ (SG correctness, bundle size, trimmable)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/FluentUIComponentAttribute.cs`
- Criar: `src/XForge.DesignSystem.Generators/XFIconRegistryGenerator.cs` (Roslyn SG)
- Criar: `src/XForge.DesignSystem.Generators/XForge.DesignSystem.Generators.csproj`
- Modificar: 10+ componentes XFFluent para adicionar [FluentUIComponent]
- Criar: `tests/XForge.DesignSystem.Tests/Generators/IconRegistryGeneratorTests.cs` (10+ tests)

## Criterios de Aceite
- [ ] Attribute [FluentUIComponent(UsesIcons = ...)] funcional
- [ ] Roslyn SG gera XFIconCatalogIndex.g.cs correto
- [ ] Bundle size reduzido 10x (3698 → ~200 icones media por app)
- [ ] Trimmable: WasmPublish remove icones nao usados
- [ ] Documentado: como adicionar [FluentUIComponent] em componentes custom
- [ ] Build: 0 erros, 0 warnings
- [ ] Tests: 10+ (SG, bundle, trimmable)

## Notas
- Inspirado em FluentUI bundle size issues (21034 classes)
- Tambem util para T089 ([FluentUIComponent] class discovery)
- IIncrementalGenerator para fast incremental builds
- Bundle final: 100-500 KB (vs 30 MB full)
