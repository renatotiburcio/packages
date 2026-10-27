---
id: T073
title: AddHxServices extension + TimeProvider
priority: P1
status: pending
estimated_time: 2h
dependencies: T063
origin: Havit
---

# T073: AddHxServices extension + TimeProvider

## Objetivo
Implementar AddXForgeServices() extension method que registra Localization + TimeProvider + servicos base. Pattern AddHxServices Havit.

## Passos
1. Criar static class XForgeServiceCollectionExtensions em XForge.DesignSystem.Shared/Extensions/
2. Criar public static IServiceCollection AddXForgeServices(this IServiceCollection services)
3. Chamar services.AddLocalization()
4. Chamar services.TryAddSingleton(TimeProvider.System)
5. Adicionar servicos XForgeScoped (multi-instance production)
6. Documentar uso em README: services.AddXForgeServices() antes de AddRazorPages/AddServerSideBlazor

## Arquivos
- Criar: src/XForge.DesignSystem.Shared/Extensions/XForgeServiceCollectionExtensions.cs
- Modificar: docs/getting-started.md (documentar AddXForgeServices)

## Criterios de Aceite
- [ ] AddXForgeServices() compila, retorna IServiceCollection (fluent)
- [ ] Localization e TimeProvider registrados
- [ ] Documentacao atualizada
- [ ] Build 0 warnings
