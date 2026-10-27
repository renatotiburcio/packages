---
id: T064
title: ThemeService abstract + Cookie + QueryString impls
priority: P1
status: pending
estimated_time: 4h
dependencies: T052
---

# T064: ThemeService abstract + Cookie + QueryString impls

## Objetivo
Implementar `IThemeService` abstract com 2 implementações (Cookie, QueryString) para persistência de tema SSR-friendly.

## Passos
1. Criar `IThemeService` interface (Theme, Wcag, Href, WcagHref, ThemeChanged event)
2. Criar `ThemeService` abstract base
3. Criar `CookieThemeService` que persiste em cookie
4. Criar `QueryStringThemeService` que persiste em URL ?theme=material
5. Criar `XForgeTheme.razor` que injeta theme no `<head>`
6. Adicionar `PersistentComponentState` integration para SSR
7. Adicionar `ThemeChanged` event com StateHasChanged
8. Registrar 2 impls no AddXForgeComponents (T063) com factory

## Arquivos
- Criar: `src/XForge.DesignSystem/Services/Theme/IThemeService.cs`
- Criar: `src/XForge.DesignSystem/Services/Theme/ThemeService.cs` (abstract)
- Criar: `src/XForge.DesignSystem/Services/Theme/CookieThemeService.cs`
- Criar: `src/XForge.DesignSystem/Services/Theme/QueryStringThemeService.cs`
- Criar: `src/XForge.DesignSystem/Components/Theming/XForgeTheme.razor` + `.razor.cs`
- Modificar: `src/XForge.DesignSystem/ServiceCollectionExtensions.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Theme/ThemeServiceTests.cs`

## Critérios de Aceite
- [ ] `IThemeService` permite trocar impl via DI
- [ ] Cookie persiste entre requests
- [ ] QueryString permite deep-link com theme
- [ ] SSR-friendly via PersistentComponentState
- [ ] ThemeChanged event dispara re-render
- [ ] Build 0 warnings
- [ ] Testes cobrem 2 impls

## Notas
- Origem: **Radzen** ⭐
- Ref: `radzen-blazor/Radzen.Blazor/RadzenTheme.razor.cs:141` + `ThemeService.cs` + `CookieThemeService.cs` + `QueryStringThemeService.cs`
- Strategy pattern: trocar impl sem mudar consumidores
- 4h = 2h interface + 1h cookie + 1h querystring