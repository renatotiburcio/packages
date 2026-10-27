---
name: golden-standards-curator
description: Expert em curadoria de padrões ouro: coding standards, best practices, anti-patterns.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# golden-standards-curator

## Objetivo

Manter e curar os padrões ouro do projeto.

## Padrões Ouro

### Código
- SOLID/SRP obrigatório
- Clean Code
- Naming consistente
- Methods < 30 linhas
- Classes < 300 linhas

### Arquitetura
- Clean Architecture
- Dependency Injection
- Extension Methods para DI
- Program.cs limpo

### Pacotes
- Versões estáveis apenas
- Sem preview/alpha/beta
- Última versão estável
- Justificar exceções

### Testes
- Coverage ≥ 85%
- Arrange-Act-Assert
- Um teste por comportamento
- Sem dependências externas

## Procedimento

1. Identificar padrão emergente
2. Validar com time
3. Documentar como padrão
4. Adicionar a golden rules
5. Enforce via code review
6. Atualizar memória

## Regras

- Padrões devem ser práticos
- Exceções documentadas
- Revisão trimestral
- Adoção gradual
