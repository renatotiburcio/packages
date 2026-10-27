---
id: T093
title: FluentUI → Tailwind class mapping tokens
priority: P2
status: pending
estimated_time: 4h
dependencies: T083
---

# T093: FluentUI → Tailwind class mapping tokens

## Objetivo
Mapear tokens do FluentUI Color/Size para Tailwind utility classes. Usado para consistencia visual entre design system e Tailwind apps.

## Passos
1. Criar `DesignTokens/FluentUIToTailwindMap.cs` (static dictionary)
2. Mapear XFIconColor → Tailwind classes:
   - Accent → text-blue-600 / dark:text-blue-400
   - Success → text-green-600
   - Warning → text-yellow-600
   - Error → text-red-600
   - Important → text-purple-600
3. Mapear XFIconSize → Tailwind classes:
   - Size16 → w-4 h-4
   - Size20 → w-5 h-5
   - Size24 → w-6 h-6
   - Size32 → w-8 h-8
4. Integrar em XFFluentIcon.razor: auto-aplica classes Tailwind baseado em Color/Size
5. Tests: 10+ (mappings corretos, integration)

## Arquivos
- Criar: `src/XForge.DesignSystem/DesignTokens/FluentUIToTailwindMap.cs`
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Icon/FluentUIToTailwindTests.cs` (10+ tests)

## Criterios de Aceite
- [ ] 6 colors mapeados
- [ ] 4 sizes mapeados
- [ ] Auto-aplicacao em XFFluentIcon
- [ ] Tests: 10+ (mappings, integration)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Tailwind v4: tokens via CSS custom properties (ja no T001)
- Bridge entre FluentUI (3rd party convention) e Tailwind (utility-first)
- Opcional: utility helper XFColor.ToTailwindClass()
