---
id: T084
title: Lookup via DI registry + Json catalog (no AOT)
priority: P1
status: pending
estimated_time: 12h
dependencies: T078
---

# T084: Lookup via DI registry + Json catalog (no AOT)

## Objetivo
Resolver problema de reflection do FluentUI (`[RequiresUnreferencedCode]`) criando DI-based icon registry + JSON catalog. AOT-friendly zero reflection.

## Passos
1. Criar interface IXFIconRegistry { Get(name, variant, size), Search(keyword) }
2. Criar XFIconRegistry : IXFIconRegistry (DI singleton)
3. Registry mantem Dictionary<(string, XFIconVariant, XFIconSize), XFIconInfo>
4. Carregado de XFIconCatalog.json (embedded resource)
5. Search: LINQ Contains em keywords[] + Name (case-insensitive)
6. Suportar paginacao: Search(query, page=0, pageSize=20)
7. Register DI: services.AddSingleton<IXFIconRegistry, XFIconRegistry>()
8. Suportar custom icons via AddIcon(XFIconInfo custom)
9. Performance: ConcurrentDictionary + ImmutableArray para search results
10. Tests: 30+ (lookup, search, pagination, concurrent access, custom registration)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/IXFIconRegistry.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconRegistry.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconRegistryServiceCollectionExtensions.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconCatalog.json` (generated, 3698+ entries)
- Modificar: `src/XForge.DesignSystem/ServiceCollectionExtensions.cs` (register registry)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconRegistryTests.cs` (30+ tests)

## Criterios de Aceite
- [ ] Interface IXFIconRegistry bem definida
- [ ] Implementacao singleton thread-safe
- [ ] Carrega de JSON embedded resource (no reflection)
- [ ] Search por keyword + name (case-insensitive, accent-insensitive)
- [ ] Paginacao funciona (offset + limit)
- [ ] Custom icon registration via AddIcon()
- [ ] Performance: lookup O(1) hash, search O(n) com early-exit
- [ ] Memory: < 5 MB para 3698 icones
- [ ] Tests: 30+ passando, 100% cobertura
- [ ] AOT-compatible (no Reflection.Emit, no GetType())
- [ ] Build: 0 erros, 0 warnings

## Notas
- Resolve drawback principal de FluentUI.Icons (RequiresUnreferencedCode)
- JSON catalog gerado pelo AssetsGenerator (T080)
- ConcurrentDictionary para read-heavy workload
- Search com StringComparison.OrdinalIgnoreCase + Normalize(NFD) para accent
