---
description: Desenvolver feature, bugfix, refactor ou qualquer tarefa de código.
agent: code
---

# /desenvolver

## Categoria

produção

## Objetivo

Executar tarefas de desenvolvimento de forma estruturada.

## Sub-comandos

| Sub-comando | Tipo |
|-------------|------|
| `/desenvolver feature <desc>` | Nova funcionalidade |
| `/desenvolver bugfix <desc>` | Correção de bug |
| `/desenvolver refactor <desc>` | Refatoração |
| `/desenvolver hotfix <desc>` | Correção urgente |

## Procedimento

### 1. Entender
- Ler requisitos/issue
- Verificar se já existe similar
- Identificar módulos afetados
- Recuperar memória relevante

### 2. Planejar
- Arquivos a criar/modificar
- Dependências necessárias
- Testes a escrever
- Estimativa de esforço

### 3. Implementar
- Criar branch (se necessário)
- Seguir Clean Architecture
- XForge.MediatR para CQRS
- AutoMapper para mapping
- FluentValidation para inputs
- Extension methods para Program.cs

### 4. Testar
- Unit tests para lógica
- Integration tests para endpoints
- Arrange-Act-Assert
- Coverage ≥ 85%

### 5. Validar
- Rodar `/qualidade`
- Rodar `/seguranca` (se aplicável)
- Verificar SOLID/SRP
- Sem preview packages

### 6. Finalizar
- Commit com mensagem descritiva
- Atualizar memória se decisão significativa
- Criar PR se branch

## Golden Rules

- 1 arquivo = 1 responsabilidade
- Separação: modelo ≠ service ≠ handler ≠ DTO
- NUNCA misturar regra de negócio com persistência
- Program.cs limpo via extensions
- Pacotes sempre versão estável

## Exemplos

```
/desenvolver
/desenvolver feature autenticação JWT
/desenvolver bugfix cálculo de impostos
/desenvolver refactor módulo de pagamentos
```
