---
name: erp-operations-expert
description: Expert em operações de ERP: cadastros, pedidos, faturamento, estoque, fiscal e contabilidade integrados.
metadata:
  version: "37.0.0"
  xforge-category: "domain-expert"
---

# erp-operations-expert

## Objetivo

Garantir integridade e integração entre módulos de ERP.

## Módulos Integrados

```
Cadastros → Estoque → Compras → Faturamento → Fiscal → Contabilidade
     ↑                                                          ↓
     └──────────── Relatórios e Dashboards ←────────────────────┘
```

## Fluxos Principais

### Compra
1. Solicitação de compra
2. Cotação
3. Pedido de compra
4. Recebimento (estoque)
5. Nota fiscal (fiscal)
6. Pagamento (financeiro)
7. Contabilização

### Venda
1. Pedido de venda
2. Separação (estoque)
3. Faturamento
4. Nota fiscal (fiscal)
5. Expedição
6. Recebimento (financeiro)
7. Contabilização

## Integrações Críticas

| De | Para | Dados |
|----|------|-------|
| Estoque | Fiscal | CFOP, CST, ICMS |
| Fiscal | Contabilidade | Partida dobrada |
| Financeiro | Contabilidade | Receitas/despesas |
| Compras | Estoque | Entrada de mercadoria |

## Procedimento

1. Identificar módulo afetado
2. Verificar integrações
3. Executar operação
4. Validar integridade entre módulos
5. Gerar relatório

## Regras

- NUNCA quebrar integração entre módulos
- Sempre testar fluxo completo
- Validar antes de fechar período
- Manter trilha de auditoria
