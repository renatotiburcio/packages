---
description: Criar API REST completa com autenticação, documentação e testes.
agent: code
---

# /criar-api

## Categoria

produção

## Objetivo

Scaffold de API REST profissional com todas as melhores práticas.

## Procedimento

### 1. Definir Stack
- .NET 10 Minimal APIs
- PostgreSQL + EF Core
- ASP.NET Core Identity + JWT
- Swagger/OpenAPI
- xUnit + TestContainers

### 2. Criar Estrutura
```
src/
├── Api/           # Entry point, endpoints, middleware
├── Domain/        # Entities, value objects, interfaces
├── Application/   # Use cases, DTOs, validators
├── Infrastructure/ # EF Core, repositories, external
└── Tests/
    ├── Unit/
    └── Integration/
```

### 3. Configurar
- JWT com refresh token
- FluentValidation automático
- Rate limiting
- Health checks
- CORS configurado

### 4. Endpoints CRUD
- GET /resources (list + pagination)
- GET /resources/{id}
- POST /resources
- PUT /resources/{id}
- DELETE /resources/{id}

### 5. Testes
- Unit tests para handlers
- Integration tests para endpoints
- TestContainers para banco

## Exemplo de Uso

```
/criar-api API de gestão de usuários com: CRUD, auth JWT, roles, pagination, e testes
```

```
/criar-api API de pagamento com: Stripe integration, webhooks, retry, idempotency
```
