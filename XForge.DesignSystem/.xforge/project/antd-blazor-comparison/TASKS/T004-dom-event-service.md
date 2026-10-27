---
id: T004
title: Adotar DomEventService pattern (centralized DOM events)
priority: P0
status: pending
estimated_time: 12h
dependencies: nenhuma
---

# T004: Adotar DomEventService pattern

## Objetivo
Centralizar subscription de DOM events em um service DI-injected, eliminando boilerplate de JS interop em cada componente XForge.

## Contexto
antd-blazor introduziu `IDomEventListener` + `DomEventService` (singleton, registered in DI). Componentes injetam o service e chamam `Subscribe("click", element, callback)` em vez de inline JS interop.

## Passos
1. Criar `src/XForge.DesignSystem/Core/Dom/IDomEventListener.cs` (interface)
2. Criar `src/XForge.DesignSystem/Core/Dom/DomEventService.cs` (implementacao singleton)
3. Criar `src/XForge.DesignSystem/Core/Dom/DomEventSubscription.cs` (handle disposable)
4. Criar `src/XForge.DesignSystem/Core/Base/XFDomComponentBase.cs` (helper class)
5. Criar `src/XForge.DesignSystem/Interop/DomEventInterop.ts` (JS module)
6. Registrar `DomEventService` como singleton em `ServiceCollectionExtensions`
7. Migrar 5 componentes-piloto: Modal (click outside), Dropdown (click anywhere), Popover (escape), Tabs (keyboard), Tooltip (hover)
8. Documentar pattern em `.xforge/knowledge/domains/design-system/dom-event-service.md`

## Arquivos
- Criar: `src/XForge.DesignSystem/Core/Dom/IDomEventListener.cs`
- Criar: `src/XForge.DesignSystem/Core/Dom/DomEventService.cs`
- Criar: `src/XForge.DesignSystem/Core/Dom/DomEventSubscription.cs`
- Criar: `src/XForge.DesignSystem/Core/Base/XFDomComponentBase.cs`
- Criar: `src/XForge.DesignSystem/Interop/DomEventInterop.ts`
- Modificar: 5 componentes-piloto
- Modificar: `XForge.DesignSystem.csproj` (incluir novo TS file no Bun pipeline)

## Criterios de Aceite
- [ ] IDomEventListener compila com testes unitarios
- [ ] 5 componentes-piloto migrados sem warnings
- [ ] Build clean (0 erros, 0 warnings)
- [ ] Performance nao regride (benchmark comparativo)
- [ ] Docs pattern publicado

## Notas
- Padrao vem de antd-blazor (DomEventService central)
- Resolve o problema de "JS interop repetido em 50+ componentes"
- Permite mocking em tests (injetar fake IDomEventListener)
- Service eh opcional: componentes podem usar EventCallback direto
