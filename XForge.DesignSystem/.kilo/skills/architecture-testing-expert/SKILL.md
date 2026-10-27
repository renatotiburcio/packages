---
name: architecture-testing-expert
description: Expert em testes arquiteturais: ArchUnit, dependências, layer violations, naming conventions.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# architecture-testing-expert

## Objetivo

Validar que a arquitetura do código está correta e consistente.

## Regras Arquiteturais

### Clean Architecture
```csharp
// Domain não depende de ninguém
// Application depende de Domain
// Infrastructure depende de Application + Domain
// API depende de todos
```

### Naming Conventions
```csharp
// Services: *Service
// Handlers: *Handler
// Validators: *Validator
// DTOs: *Dto ou *Request/*Response
// Repositories: I*Repository
```

### Layer Violations
```csharp
// ❌ Domain não pode usar EF Core
// ❌ Application não pode usar HttpContext
// ❌ Infrastructure não pode conter regra de negócio
```

## Ferramentas

| Ferramenta | Uso |
|------------|-----|
| ArchUnitNET | Regras de arquitetura em C# |
| NetArchTest | Alternativa ao ArchUnit |
| SonarQube | Análise estática |
| NDepend | Métricas de dependência |

## Procedimento

1. Definir regras arquiteturais
2. Criar testes para cada regra
3. Rodar testes
4. Identificar violações
5. Corrigir ou documentar exceções

## Regras

- Todo teste arquitetural deve ser automático
- Violação deve ser justificada
- Documentar exceções em ADR
- Rodar em toda build
