---
description: Criar aplicação Blazor Server/WASM com Tailwind e design system.
agent: code
---

# /criar-blazor

## Categoria

produção

## Objetivo

Scaffold de aplicação Blazor profissional com Tailwind CSS.

## Procedimento

### 1. Definir Stack
- Blazor Server (ou WASM)
- Tailwind CSS
- .NET 10
- PostgreSQL + EF Core

### 2. Criar Estrutura
```
src/
├── Components/
│   ├── Layout/
│   ├── Pages/
│   └── Shared/
├── Services/
├── Models/
└── wwwroot/
    ├── css/
    └── js/
```

### 3. Configurar
- Tailwind CSS via CDN ou npm
- Authentication com Identity
- Navigation com routing
- State management com CascadingValues

### 4. Componentes Base
- Layout responsivo
- Sidebar de navegação
- Tabela paginada
- Formulário com validação
- Modal de confirmação
- Loading spinner
- Toast notifications

### 5. Páginas Essenciais
- Dashboard
- Login/Register
- Settings
- CRUD genérico

## Exemplo de Uso

```
/criar-blazor Dashboard administrativo com: sidebar, tabelas paginadas, gráficos, e autenticação
```

```
/criar-blazor App de gestão de estoque com: busca, filtros, inventário, e relatórios
```
