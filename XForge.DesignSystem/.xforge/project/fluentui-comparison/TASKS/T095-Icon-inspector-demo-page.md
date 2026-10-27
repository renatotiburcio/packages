---
id: T095
title: Icon inspector demo page (com search)
priority: P2
status: pending
estimated_time: 4h
dependencies: T085
---

# T095: Icon inspector demo page (com search)

## Objetivo
Pagina demo completa de Icon inspector: search, variant/size switchers, color picker, skintone picker, copy-as-SVG/JSON, favorites.

## Passos
1. Criar `examples/Demo/Pages/IconInspectorDemo.razor`
2. Top: search bar (debounce 250ms, multi-keyword)
3. Left: filters (variant, size, category, color)
4. Center: grid de icones (paginated 24/page)
5. Right: selected icon details (SVG, Name, Variant, Size, Path d, Source URL)
6. Actions: Copy SVG, Copy path d, Copy as class ref, Add to favorites
7. Favorites: localStorage, persiste entre sessoes
8. Theme toggle (light/dark)
9. Responsive: stack vertical em mobile
10. A11y: skip link, focus management, aria-live search results

## Arquivos
- Criar: `examples/Demo/Pages/IconInspectorDemo.razor`
- Criar: `examples/Demo/Pages/IconInspectorDemo.razor.cs`
- Modificar: `examples/Demo/NavMenu.razor` (add link)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconInspectorTests.cs` (8+ tests)

## Criterios de Aceite
- [ ] Search bar funciona (multi-keyword, debounce)
- [ ] Filters aplicam em tempo real
- [ ] Grid paginado (24/page)
- [ ] Selected details (SVG + path + metadata)
- [ ] Copy actions funcionam
- [ ] Favorites persistem em localStorage
- [ ] Theme toggle (light/dark)
- [ ] Responsive (mobile-friendly)
- [ ] A11y compliant
- [ ] Tests: 8+ (search, filter, copy, favorites)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Integra T078-T094 (todos os icon/emoji components)
- Serve como showcase do icon system
- Pode ser usado em design handoff (export SVG/JSON)
