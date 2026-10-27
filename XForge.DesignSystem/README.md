# XForge.DesignSystem

Biblioteca de componentes **Blazor/Razor** — a única coisa que este pacote precisa ser:
componentes, tokens e temas, com testes.

```
src/
├── XForge.DesignSystem/          componentes, tokens e temas (Razor, net10.0)
├── XForge.DesignSystem.Shared/   código compartilhado do núcleo (única dependência dos componentes)
└── XForge.DesignSystem.Tests/    testes bUnit dos componentes
docs/                             guias e análises de design
```

## Uso

```csharp
// Program.cs do app Blazor
builder.Services.AddXForgeDesignSystem();
```

```razor
@* _Imports.razor *@
@using XForge.DesignSystem.Components
```

## Desenvolvimento

```bash
dotnet build src/XForge.DesignSystem          # build do núcleo
dotnet test  src/XForge.DesignSystem.Tests    # testes dos componentes (bUnit)
```

## Histórico da reorganização (2026-10-27)

O design system veio de um repo maior (migrado de `temp/xforge-design-system`) com
**15 projetos + tooling de raiz** (servidores MCP, geradores de assets/docs, site de
documentação, pipeline Bun, E2E, analisador, benchmarks, configs de agentes). A análise de
dependências mostrou que o núcleo de componentes depende **apenas** de `Shared` — todo o
resto era ferramenta satélite que consome os componentes, mas não é necessária para eles.

A reorganização removeu 9 projetos satélite, o E2E e todo o tooling de raiz, mantendo o que
importa para a biblioteca de componentes. O registro completo está em
[`MIGRACAO.md`](MIGRACAO.md).
