---
id: T044
title: Provider-agnostic abstraction (IClassProvider/IStyleProvider)
priority: P0
status: pending
estimated_time: 32h
dependencies: nenhuma
---

# T044: Provider-agnostic abstraction (IClassProvider/IStyleProvider)

## Objetivo
Adotar o padrão de abstração de CSS do Blazorise: `IClassProvider` + `IStyleProvider` injetáveis via DI, permitindo que o mesmo componente XForge renderize com Tailwind v4 (atual) ou outro framework no futuro sem mudanças no código do componente.

## Contexto (Blazorise)
- **Origem**: `Source/Blazorise/Base/BaseComponent.cs:564-569` injeta `IClassProvider` e `IStyleProvider`
- **Padrão**: 7 implementações (Bootstrap, Bootstrap5, Bulma, Material, AntDesign, FluentUI2, Tailwind)
- **Contrato**: `ClassProvider.Button(Color)` → `"btn btn-primary"`, `StyleProvider.Margin(...)` → `"margin: 1rem"`
- **Tradeoff**: +32h upfront, mas YAGNI para v2.0.0 (só 1 provider); fundação para v2.1+

## Passos
1. Criar `Abstractions\IClassProvider.cs` com métodos para cada classe (Button, Input, Card, Alert, Badge, etc.)
2. Criar `Abstractions\IStyleProvider.cs` com fluent styles (Sizing, Spacing, Color)
3. Criar `Abstractions\IClassDefinitions.cs` (DTO para passar para provider)
4. Criar `Providers\TailwindClassProvider.cs` com implementações Tailwind v4
5. Criar `Providers\TailwindStyleProvider.cs` com utility classes
6. Modificar `XForgeComponentBase` para injetar `IClassProvider` e `IStyleProvider` via `[Inject]`
7. Refatorar `XFButton`, `XFInput`, `XFAlert`, `XFCard` para chamar `ClassProvider.X()` em vez de string hardcoded
8. Registrar no DI: `services.AddXForgeDesignSystem().UseTailwindProvider()`
9. Criar testes bUnit verificando que `ClassProvider` é chamado corretamente
10. Documentar em `docs/providers.md` como adicionar novo provider

## Arquivos
- Criar: `src\XForge.DesignSystem\Abstractions\IClassProvider.cs`
- Criar: `src\XForge.DesignSystem\Abstractions\IStyleProvider.cs`
- Criar: `src\XForge.DesignSystem\Abstractions\IClassDefinitions.cs`
- Criar: `src\XForge.DesignSystem\Providers\Tailwind\TailwindClassProvider.cs`
- Criar: `src\XForge.DesignSystem\Providers\Tailwind\TailwindStyleProvider.cs`
- Criar: `src\XForge.DesignSystem\Providers\Tailwind\TailwindClassDefinitions.cs`
- Modificar: `src\XForge.DesignSystem\Base\XForgeComponentBase.cs`
- Modificar: `src\XForge.DesignSystem\Components\Button\XFButton.razor.cs`
- Modificar: `src\XForge.DesignSystem\Components\Input\XFInput.razor.cs`
- Modificar: `src\XForge.DesignSystem\Extensions\ServiceCollectionExtensions.cs`
- Criar: `src\XForge.DesignSystem.Tests\Providers\TailwindClassProviderTests.cs`
- Criar: `docs\providers.md`

## Critérios de Aceite
- [ ] `IClassProvider` e `IStyleProvider` são interfaces públicas com XML docs
- [ ] `TailwindClassProvider` retorna classes Tailwind v4 corretas para Button, Input, Alert
- [ ] `XFButton` renderiza idêntico antes/depois da refatoração (snapshot test)
- [ ] DI registration funciona: `AddXForgeDesignSystem().UseTailwindProvider()`
- [ ] Testes bUnit cobrem ≥ 80% do `TailwindClassProvider`
- [ ] Documentação `docs\providers.md` mostra como criar novo provider (Bootstrap, Material)
- [ ] Build clean: 0 errors, 0 warnings

## Notas
- **Origem**: Blazorise (Source/Blazorise/Base/BaseComponent.cs linhas 564-569)
- **Inspiração direta**: `ClassProvider` property com `[Inject]`, contrato `string Class(Color)`, etc.
- **Não-objetivo v2.0.0**: Implementar providers para Bootstrap/Material. Só a abstração + 1 implementação.
- **ROI alto**: Fundação para multi-CSS futuro, vira feature de marketing
- **Cuidado**: NÃO quebrar API atual; refatorar internamente
- **Inspirado em**: Blazorise tem 7 implementações em produção; XForge v2.0.0 começa com 1
