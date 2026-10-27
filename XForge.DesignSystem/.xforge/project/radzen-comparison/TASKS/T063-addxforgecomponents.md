---
id: T063
title: AddXForgeComponents() extension method
priority: P0
status: pending
estimated_time: 1h
dependencies: nenhuma
---

# T063: AddXForgeComponents() extension method

## Objetivo
Criar extension method idiomático para registro de serviços XForge, seguindo o pattern `AddRadzenComponents()`.

## Passos
1. Criar `ServiceCollectionExtensions.cs` com método `AddXForgeComponents()`
2. Registrar Dialog, Notification, Tooltip, ContextMenu, Theme, AI services
3. Adicionar overloads: `AddXForgeComponents(Action<XForgeOptions>)`
4. Criar `XForgeOptions` class (Theme default, Locale default, AI provider default)
5. Documentar uso no README com exemplo Program.cs
6. Adicionar testes verificando todos os services são registrados

## Arquivos
- Criar: `src/XForge.DesignSystem/ServiceCollectionExtensions.cs`
- Criar: `src/XForge.DesignSystem/XForgeOptions.cs`
- Modificar: `README.md` (exemplo Program.cs)
- Criar: `tests/XForge.DesignSystem.Tests/ServiceCollectionExtensionsTests.cs`

## Critérios de Aceite
- [ ] `services.AddXForgeComponents()` funciona
- [ ] `services.AddXForgeComponents(opt => opt.Theme = "dark")` funciona
- [ ] Todos os services necessários registrados
- [ ] XML docs completos
- [ ] Build 0 warnings
- [ ] Testes passam

## Notas
- Origem: **Radzen** ⭐
- Ref: `radzen-blazor/Radzen.Blazor/ServiceCollectionExtensions.cs:27`
- 1h implementação direta
- Quick win #1 da Sprint 1