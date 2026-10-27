---
id: T081
title: XFFluentIcon.razor<TIcon> generic component (variant+size)
priority: P0
status: pending
estimated_time: 8h
dependencies: T078
---

# T081: XFFluentIcon.razor<TIcon> generic component (variant+size)

## Objetivo
Criar componente Blazor generico `<XFFluentIcon Value="..." />` inspirado em FluentIcon<Icon>. Suporta variant, size, color customization e eventos.

## Passos
1. Criar `Components/Icon/XFFluentIcon.razor` (markup)
2. Criar `Components/Icon/XFFluentIcon.razor.cs` (codigo, partial)
3. Criar `XFFluentIcon.razor.types.cs` (parameter docs)
4. Parametros: Value (TIcon), Variant, Size, Color, CustomColor, Width, OnClick, Focusable
5. Validar: CustomColor so funciona se Color = Custom
6. Renderizar SVG inline com viewBox correto
7. Suportar Focusable (a11y): tabindex=0, role=button
8. Suportar keyboard: Enter/Space → OnClick
9. Suportar slot: <XFFluentIcon>@Body</XFFluentIcon>
10. Aplicar XFCssBuilder para class/style fluent (T059)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor.types.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Icon/FluentIconTests.cs` (10+ tests)

## Criterios de Aceite
- [ ] Renderiza SVG inline (nao data URI, nao font)
- [ ] Suporta 4 variants (Regular/Filled/Light/Color)
- [ ] Suporta 8 sizes (10/12/16/20/24/28/32/48) + Custom
- [ ] Color enum: Accent/Lightweight/Success/Warning/Error/Important/Custom
- [ ] CustomColor so aceita se Color = Custom
- [ ] OnClick + a11y (tabindex, role, keyboard)
- [ ] CSS variables: var(--xf-icon-fill-rest), var(--xf-icon-fill-hover)
- [ ] Documentado em XML comments (17 secoes DOCS-RAZOR-PAGE-STANDARD)
- [ ] Tests: 10+ (render, color, size, onClick, focusable, a11y)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Baseado em Microsoft.FluentUI.AspNetCore.Components.FluentIcon
- Reusar XFCssBuilder (T059 - ja criado)
- Nao precisa JS interop (renderiza tudo Razor-side)
- MarkupString com SVG inline (mesmo padrao FluentUI)
