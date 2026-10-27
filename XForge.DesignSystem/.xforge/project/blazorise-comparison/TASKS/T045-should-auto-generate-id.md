---
id: T045
title: ShouldAutoGenerateId em base + Attributes CaptureUnmatchedValues
priority: P0
status: pending
estimated_time: 4h
dependencies: nenhuma
---

# T045: ShouldAutoGenerateId em base + Attributes CaptureUnmatchedValues

## Objetivo
Adotar o padrão do Blazorise para geração automática de IDs e captura de atributos HTML não-matched. Reduz boilerplate em todo componente e padroniza a11y/forms.

## Contexto (Blazorise)
- **Origem**: `Source/Blazorise/Base/BaseComponent.cs:486` (`protected virtual bool ShouldAutoGenerateId => false;`)
- **Origem**: `Source/Blazorise/Base/BaseComponent.cs:559` (`[Inject] protected IIdGenerator IdGenerator { get; set; }`)
- **Origem**: `Source/Blazorise/Base/BaseComponent.cs:1094-1095` (`[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> Attributes { get; set; }`)
- **Pattern**: Base gera ID se `ShouldAutoGenerateId = true` e `ElementId` é null
- **XForge atual**: Cada componente tem `private string _id` + `OnInitialized` override; verboso

## Passos
1. Adicionar `protected virtual bool ShouldAutoGenerateId => false;` em `XForgeComponentBase`
2. Adicionar `public string ElementId { get; protected set; }` em `XForgeComponentBase`
3. Adicionar `OnInitialized` que gera ID se `ShouldAutoGenerateId` e `ElementId == null`
4. Adicionar `[Inject] protected IIdGenerator IdGenerator { get; set; }`
5. Adicionar `[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> Attributes { get; set; }`
6. Criar `Services\IdGenerator.cs` (singleton, retorna `xf-{guid:N}.Substring(0,8)}`)
7. Registrar `IIdGenerator` no DI
8. Refatorar `XFInput`, `XFButton`, `XFCard` para `protected override bool ShouldAutoGenerateId => true;`
9. Remover `_id` field privado + `OnInitialized` override desses componentes
10. Adicionar testes verificando que ID é gerado no formato `xf-xxxxxxxx`

## Arquivos
- Modificar: `src\XForge.DesignSystem\Base\XForgeComponentBase.cs`
- Criar: `src\XForge.DesignSystem\Services\IdGenerator.cs`
- Criar: `src\XForge.DesignSystem\Abstractions\IIdGenerator.cs`
- Modificar: `src\XForge.DesignSystem\Components\Input\XFInput.razor.cs` (remover _id field)
- Modificar: `src\XForge.DesignSystem\Components\Button\XFButton.razor.cs`
- Modificar: `src\XForge.DesignSystem\Components\Card\XFCard.razor.cs`
- Modificar: `src\XForge.DesignSystem\Extensions\ServiceCollectionExtensions.cs`
- Criar: `src\XForge.DesignSystem.Tests\Services\IdGeneratorTests.cs`

## Critérios de Aceite
- [ ] `IIdGenerator` interface com método `string Generate()`
- [ ] `IdGenerator` retorna formato `xf-{8-char-guid}` (lowercase, sem hífens)
- [ ] `XForgeComponentBase.ShouldAutoGenerateId` é `false` por default; `true` em componentes que precisam
- [ ] `ElementId` é auto-gerado no `OnInitialized` se `ShouldAutoGenerateId` e `ElementId == null`
- [ ] `Attributes` captura todos os HTML attributes não-recognized
- [ ] Componentes refatorados continuam funcionando (snapshot tests passam)
- [ ] Testes cobrem `IdGenerator` 100% (determinístico com seed)
- [ ] Build clean: 0 errors, 0 warnings

## Notas
- **Origem**: Blazorise BaseComponent.cs
- **Padrão**: Virtual + override opt-in; cada componente decide
- **Benefício**: Remove ~10 linhas de boilerplate por componente (152 componentes = 1520 linhas removidas)
- **Cuidado**: NÃO quebrar componentes que override `OnInitialized` para outras razões
- **Inspirado em**: Blazorise `protected virtual bool ShouldAutoGenerateId => false;` (linha 486)
- **Cuidado com `Attributes`**: Conflita com `width`/`height` HTML attributes (workaround do Blazorise em SetParametersAsync)
