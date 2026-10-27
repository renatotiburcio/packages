---
id: T099
title: Brand badge icons (8 marcas comuns)
priority: P3
status: pending
estimated_time: 4h
dependencies: T078
---

# T099: Brand badge icons (8 marcas comuns)

## Objetivo
Curadoria de 8 brand badge icons (Microsoft, Google, Apple, Amazon, GitHub, GitLab, Slack, Figma) com logos oficiais SVG. Usado em login SSO e integrations.

## Passos
1. Curar 8 brand logos (SVG oficiais, licensing OK):
   - Microsoft (CC BY 4.0)
   - Google (CC BY 4.0)
   - Apple (logo guidelines)
   - Amazon (logo guidelines)
   - GitHub (logos.github.io)
   - GitLab (CC BY-SA 4.0)
   - Slack (logo guidelines)
   - Figma (Figma brand kit)
2. Criar XFIconCatalog entries em Brand subcategory
3. Criar <XFBadgeIcon Brand="Microsoft" Size="32" /> (helper component)
4. Licenses: documentar origem em LICENSE-3RD-PARTY
5. Multi-color support (logos sao colored)
6. Tests: 8+ (cada brand renderiza correto, license atribuido)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/BrandIcons.cs` (8 brand SVG paths)
- Criar: `src/XForge.DesignSystem/Components/Icon/XFBadgeIcon.razor`
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFBadgeIcon.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFBadgeIcon.razor.types.cs`
- Modificar: `LICENSE-3RD-PARTY.md` (add brand attributions)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/BadgeIconTests.cs` (8+ tests)

## Criterios de Aceite
- [ ] 8 brand icons curados (Microsoft, Google, Apple, Amazon, GitHub, GitLab, Slack, Figma)
- [ ] SVG paths oficiais
- [ ] Licenses documentados
- [ ] XFBadgeIcon component com Brand, Size
- [ ] Multi-color support
- [ ] Tests: 8+ (cada brand + license)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Brand logos tem licensing especifico
- Usar apenas logos oficiais (nao recriar)
- Para uso comercial: verificar guidelines de cada marca
- LICENSE-3RD-PARTY essencial para compliance
