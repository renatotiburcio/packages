---
id: T046
title: Snippets.generated.cs (executable code samples)
priority: P1
status: pending
estimated_time: 24h
dependencies: T021
---

# T046: Snippets.generated.cs (executable code samples)

## Objetivo
Adotar o padrão do Blazorise.Docs.Compiler: code samples em docs são .razor files reais dentro de `Documentation/Blazorise.Docs/Examples/`, escaneados e compilados em `Snippets.generated.cs` (classe partial com `public partial class ButtonExample { ... }`). Garante que exemplos em docs SEMPRE compilam.

## Contexto (Blazorise)
- **Origem**: `Documentation/Blazorise.Docs.Compiler/CodeSnippets.cs` + `CodeExamplesMarkup.cs`
- **Origem**: `Documentation/Blazorise.Docs/Models/Snippets.generated.cs` (gerado, NÃO editar)
- **Origem**: `Documentation/Blazorise.Docs/ApiDocs/Blazorise.ApiDocs.cs` (gerado)
- **Pattern**:
  1. Dev escreve `ButtonExample.razor` em `Documentation/XForge.Docs/Examples/Button/`
  2. `XForge.Docs.Compiler` (CLI) escaneia `Examples/**/*.razor`
  3. Gera `Snippets.generated.cs` com `public partial class ButtonExample { ... }`
  4. Gera `Examples\Button\ButtonExample.razor` markup pre-rendered
  5. Docs site consome ambos
- **Benefício**: Se exemplo quebrar, build falha

## Passos
1. Criar `src\XForge.DesignSystem.Docs.Compiler\` (CLI, net10.0, depende de `Microsoft.CodeAnalysis.CSharp`)
2. Criar `CodeSnippets.cs` que escaneia `Documentation/XForge.Docs/Examples/**/*.razor`
3. Parsear .razor files com Roslyn para extrair `@code` block + markup
4. Gerar `Snippets.generated.cs` no projeto `XForge.DesignSystem.Docs`
5. Criar `CodeExamplesMarkup.cs` que gera markup HTML pre-rendered
6. Configurar como `<Target Name="CompileDocs" BeforeTargets="Build">` no csproj
7. Refatorar 10+ exemplos existentes (`XFButton.razor`, `XFInput.razor`, `XFDataGrid.razor`, etc.) para usar `XButtonExample` class
8. Adicionar teste que verifica `Snippets.generated.cs` é gerado e compila
9. Documentar em `docs\contributing-docs.md` como adicionar novo exemplo

## Arquivos
- Criar: `src\XForge.DesignSystem.Docs.Compiler\XForge.DesignSystem.Docs.Compiler.csproj`
- Criar: `src\XForge.DesignSystem.Docs.Compiler\Program.cs`
- Criar: `src\XForge.DesignSystem.Docs.Compiler\CodeSnippets.cs`
- Criar: `src\XForge.DesignSystem.Docs.Compiler\CodeExamplesMarkup.cs`
- Criar: `src\XForge.DesignSystem.Docs.Compiler\Paths.cs`
- Criar: `src\XForge.DesignSystem.Docs.Compiler\StringExtensions.cs`
- Criar: `src\XForge.DesignSystem.Docs\XForge.DesignSystem.Docs.csproj` (consome Snippets.generated.cs)
- Criar: `src\XForge.DesignSystem.Docs\Examples\Button\XButtonExample.razor`
- Criar: `src\XForge.DesignSystem.Docs\Examples\Input\XInputExample.razor`
- Criar: `src\XForge.DesignSystem.Docs\Examples\Card\XCardExample.razor`
- Modificar: `src\XForge.DesignSystem\XForge.DesignSystem.csproj` (Target CompileDocs)
- Criar: `docs\contributing-docs.md`

## Critérios de Aceite
- [ ] `XForge.DesignSystem.Docs.Compiler` CLI gera `Snippets.generated.cs` válido
- [ ] `Snippets.generated.cs` contém `public partial class XButtonExample { ... }` com código real
- [ ] `XForge.DesignSystem.Docs` builda limpo após consumer do generated file
- [ ] Pelo menos 10 exemplos refatorados e funcionando
- [ ] Adicionar exemplo novo é trivial: criar .razor em `Examples/`, build gera classe
- [ ] Build falha se exemplo .razor tem erro de sintaxe
- [ ] Documentação `contributing-docs.md` mostra fluxo completo

## Notas
- **Origem**: Blazorise (Documentation/Blazorise.Docs.Compiler/CodeSnippets.cs)
- **Padrão**: Roslyn scanning + Roslyn parsing → generated C# file
- **Inspirado em**: MudBlazor `UnitTests.Docs.Generator` (gerar `nameof(ExampleClass)` em test methods)
- **Benefício**: Docs SEMPRE compilam; PR review verifica exemplos automaticamente
- **Cuidado**: Não editar `Snippets.generated.cs` (gerado); CI deve falhar se for commitado
- **Futuro v2.1**: Adicionar screenshot generation (Playwright) para visual regression
