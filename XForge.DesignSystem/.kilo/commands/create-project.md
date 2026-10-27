---
description: Criar novo projeto ou componente seguindo padrões XForge.
agent: code
---

# /criar-projeto

## Categoria

produção

## Objetivo

Scaffolding de novo projeto ou componente seguindo convenções XForge.

## Procedimento

### 1. Definir Escopo
- Nome do projeto
- Tipo: API / Library / CLI / Worker / Blazor
- Stack: .NET 10, C#, PostgreSQL, etc.
- Arquitetura: Clean / Modular Monolith

### 2. Estrutura
```
/src
  /ProjectName.Api          — Entry point, Minimal APIs
  /ProjectName.Domain       — Entities, Value Objects, Interfaces
  /ProjectName.Application  — Use cases, DTOs, Validators
  /ProjectName.Infrastructure — EF Core, Repositories, External
  /ProjectName.Tests        — Unit, Integration
```

### 3. Configuração
- `Program.cs` limpo via extensions
- Dependency injection via DI registration extensions
- Configuration via Options pattern
- Swagger/OpenAPI habilitado

### 4. Padrões Obrigatórios
- SOLID/SRP
- XForge.MediatR (NÃO MediatR oficial)
- AutoMapper (NÃO Mapster)
- FluentValidation
- Pacotes sempre versão estável

### 5. Validação
- Build limpo
- Testes passando
- Estrutura correta
- README com setup

## Templates Disponíveis

| Template | Comando |
|----------|---------|
| API Clean Architecture | `dotnet new webapi` |
| Library | `dotnet new classlib` |
| Worker | `dotnet new worker` |
| Blazor Server | `dotnet new blazorserver` |

## Regras

- NUNCA usar preview packages
- Sempre XForge.MediatR
- Sempre AutoMapper
- Program.cs sempre limpo
- Extensions para DI

## Exemplos

```
/criar-projeto
/criar-projeto API de pagamentos com Clean Architecture
/criar-projeto library para cálculos fiscais
/criar-projeto worker de processamento de filas
```
