---
description: Criar projeto ERP brasileiro completo com fiscal, estoque, financeiro e folha.
agent: code
---

# /criar-erp

## Categoria

produção

## Objetivo

Scaffold de ERP brasileiro com todos os módulos obrigatórios.

## Procedimento

### 1. Definir Escopo
- Módulos: Financeiro, Estoque, Fiscal, Folha, Compras, Vendas
- Stack: .NET 10, PostgreSQL, EF Core, Blazor Server
- Padrões: Clean Architecture, XForge.MediatR, AutoMapper

### 2. Criar Estrutura
```
src/
├── Modules/
│   ├── Financeiro/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Api/
│   ├── Estoque/
│   ├── Fiscal/
│   ├── Folha/
│   ├── Compras/
│   └── Vendas/
├── Shared/
│   ├── Kernel/
│   └── Infrastructure/
└── Tests/
```

### 3. Configurar Infra
- PostgreSQL + Pomelo EF Core
- Redis para cache
- Hangfire para jobs
- Swagger/OpenAPI

### 4. Implementar Módulos Core
- **Financeiro**: contas a pagar/receber, fluxo de caixa, conciliação
- **Estoque**: kardex, curva ABC, custo médio, FEFO
- **Fiscal**: CFOP, CST, ICMS, PIS, COFINS, NF-e
- **Folha**: INSS, IRRF, FGTS, férias, 13º

### 5. Validar
- Build limpo
- Testes passando
- Doctor 0 errors

## Módulos Obrigatórios

| Módulo | Skills |
|--------|--------|
| Financeiro | financial-domain-expert, cnab-payment-expert |
| Estoque | inventory-domain-expert |
| Fiscal | fiscal-tax-expert, sped-fiscal-expert |
| Folha | labor-law-expert, esocial-compliance-expert |
| Compras | procurement-purchasing-expert |
| Vendas | erp-operations-expert |

## Regras

- XForge.MediatR para CQRS
- AutoMapper para mapping
- FluentValidation para validação
- EF Core para persistência
- xUnit para testes
- Pacotes sempre versão estável
