# PROJECT-DNA — XForge Design System

## Identidade

| Propriedade | Valor |
|---|---|
| **Nome** | XForge Design System |
| **Versão** | 1.0.0 |
| **Tipo** | Blazor Component Library (RCL) + AI Extensions |
| **Framework** | .NET 10.0 |
| **Linguagem** | C# + Razor + TypeScript (JS interop) |
| **Frontend** | Blazor WebAssembly + Tailwind CSS v4 |
| **Licença** | MIT (Core) / Commercial (AI) |
| **Repositório** | https://github.com/xforge/designsystem |

## Estrutura de Produtos

```
XForge Design System
├── Core (Open Source — MIT)
│   ├── XForge.DesignSystem          → RCL com ~100 componentes Blazor
│   │   ├── Components/              → Button, Forms, DataDisplay, Navigation, etc.
│   │   ├── Core/                    → Base classes + Primitives (enums)
│   │   ├── Services/               → Brand, NavigationState, NavGuard
│   │   └── wwwroot/                 → CSS, JS interop, vendor (ECharts, Quill)
│   └── XForge.DesignSystem.Shared   → Ecosystem boundaries (enums)
│
├── AI (Commercial)
│   ├── XForge.DesignSystem.AI       → RCL com ~50 componentes AI
│   │   ├── Components/AI/           → 21 sub-domínios (Accounting, Fiscal, etc.)
│   │   ├── Services/AI/             → 11 providers + interfaces
│   │   └── Extensions/              → DI registration (AddXFAi)
│   └── XForge.DesignSystem.AI.Tests → bUnit tests (~350+ métodos)
│
├── Documentação
│   ├── XForge.DesignSystem.Docs              → Blazor WASM app (Core docs)
│   ├── XForge.DesignSystem.Docs.AI           → Blazor WASM app (AI docs)
│   └── XForge.DesignSystem.Docs.Shared       → Shared docs shell
│
└── Testes
    └── XForge.DesignSystem.Tests             → bUnit + Stryker mutation testing
```

## Stack Técnico

| Camada | Tecnologia |
|---|---|
| **Runtime** | .NET 10.0 |
| **UI Framework** | Blazor WebAssembly + Razor Components |
| **CSS** | Tailwind CSS v4 (via @tailwindcss/cli) |
| **JS Interop** | 25 módulos JS isolados (xf-*.js) |
| **Charts** | ECharts 6.0.0 (via npm) |
| **Rich Text** | Quill (via wwwroot/vendor) |
| **Testes** | xUnit 2.9.2 + bUnit 2.3.4 + Stryker |
| **AI Providers** | OpenAI, Ollama, Gemini, Anthropic, Azure OpenAI, DeepSeek, OpenRouter, Local LLM, InMemory, Generic |
| **Build** | MSBuild + npm (Tailwind) |
| **Package Manager** | NuGet |

## Convenções de Código

| Convenção | Padrão |
|---|---|
| **Namespace raiz** | `XForge.DesignSystem` |
| **Componentes** | Prefixo `XF` (ex: `XFButton`, `XFDataGrid`) |
| **Arquivos** | `{Nome}.razor` + `{Nome}.razor.cs` + `{Nome}.types.cs` |
| **Enums** | Sem sufixo, PascalCase (ex: `XFSize`, `XFSemanticColor`) |
| **CSS** | Tailwind utility-first, via `XFFormCss` centralizador |
| **JS** | Módulos ES isolados em `wwwroot/js/xf-*.js` |
| **Validação** | Fluent validators por componente (XFRequiredValidator, etc.) |
| **Acessibilidade** | WCAG 2.1 AA — ARIA roles, keyboard nav, live regions |

## Métricas do Projeto

| Métrica | Valor |
|---|---|
| **Projetos** | 8 |
| **Componentes Core** | ~100 |
| **Componentes AI** | ~52 |
| **Páginas de Documentação** | 163 (99 Core + 64 AI) |
| **Arquivos de Teste** | ~200+ |
| **Métodos de Teste** | ~350+ |
| **AI Providers** | 11 |
| **Primitivos (enums)** | 15 |
| **Arquivos JS** | 25 |
| **Linhas de Código (estimada)** | ~50.000+ |
