---
id: T092
title: Auto-classify icons por keyword (Category inferred)
priority: P2
status: pending
estimated_time: 4h
dependencies: T078
---

# T092: Auto-classify icons por keyword (Category inferred)

## Objetivo
Auto-classificar 3698 Font Awesome icons em 20+ categories (Communication, Media, Travel, etc.) baseado em keywords. Permite filtros visuais no picker.

## Passos
1. Criar enum XFIconCategory (Communication, Media, Travel, Money, Tools, etc., 20+)
2. Criar mapping table: keywords[] → XFIconCategory
3. Implementar classificador: XFIconClassifier.Classify(XFIconInfo) → XFIconCategory
4. Algoritmo: match keywords[] com category rules, score, best match
5. Pre-computar 3698 classificacoes em build time (XFIconCategories.g.cs)
6. API: IXFIconClassifier (DI singleton)
7. UI: <XFIconPicker FilterCategory="Communication" /> no demo
8. Tests: 15+ (classificacao, edge cases, conflicts, performance)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconCategory.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/IXFIconClassifier.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconClassifier.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconCategoryRules.cs` (mapping data)
- Criar: `src/XForge.DesignSystem.Generators/XFIconCategoryGenerator.cs` (pre-compute)
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFIconPicker.razor` (add category filter)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconClassifierTests.cs` (15+ tests)

## Criterios de Aceite
- [ ] 20+ categories definidas
- [ ] 3698 icones classificados (pre-compute)
- [ ] Algoritmo deterministico + rapido
- [ ] UI: filtro por category no picker
- [ ] Tests: 15+ (cobertura 80%+ dos icones)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Inspirado em Font Awesome categories (https://fontawesome.com/categories)
- Pode usar ML/embedding classifier (out of scope v2.0.0)
- Para v2.0.0: rule-based + manual override
