---
description: Criar componente Blazor com Tailwind CSS seguindo padrões XForge.
agent: code
---

# /criar-componente-blazor

## Categoria

produção

## Objetivo

Scaffolding de componente Blazor com UI responsiva e padrões XForge.

## Procedimento

### 1. Definir
- Nome do componente
- Tipo: Page / Component / Layout
- Estilo: Tailwind CSS
- Interações: form, list, modal, etc.

### 2. Criar
```razor
@* /Components/Pages/FeatureName.razor *@

@page "/feature"
@rendermode InteractiveServer

<PageTitle>Feature</PageTitle>

<div class="container mx-auto p-4">
    <!-- Conteúdo -->
</div>

@code {
    // Lógica
}
```

### 3. Padrões
- Tailwind CSS para estilos (NUNCA CSS inline)
- Componentes pequenos (< 200 linhas)
- Separar: UI, lógica, services
- @code-behind para lógica complexa
- Parameter para dados de entrada
- EventCallback para ações

### 4. Validar
- Build limpo
- Responsivo (mobile-first)
- Acessível (ARIA labels)
- Testes de componente

## Regras

- Tailwind CSS sempre
- Componentes < 200 linhas
- Sem lógica no .razor (usar .razor.cs)
- Parameter/EventCallback para interfaces

## Exemplos

```
/criar-componente-blazor
/criar-componente-blazor formulário de pagamento
/criar-componente-blazor lista de produtos com paginação
/criar-componente-blazor modal de confirmação
```
