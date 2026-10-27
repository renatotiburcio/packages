---
name: financial-domain-expert
description: Expert em domínio financeiro: fluxo de caixa, investimentos, crédito, débito e conciliação bancária.
metadata:
  version: "37.0.0"
  xforge-category: "domain-expert"
---

# financial-domain-expert

## Objetivo

Garantir correção e integridade nas operações financeiras.

## Conceitos Chave

| Conceito | Descrição |
|----------|-----------|
| **Fluxo de Caixa** | Entradas e saídas de recursos |
| **Conciliação** | Saldos sistema vs banco |
| **Credito/Débito** | Lançamentos contábeis |
| **Borderô** | Remessa de títulos |
| **Boleto** | Documento de cobrança |
| **NFe** | Nota Fiscal Eletrônica |

## Operações

### Recebimento
- Boleto (com vencimento)
- Cartão de crédito
- Pix
- Transferência bancária

### Pagamento
- Boleto de fornecedor
- Folha de pagamento
- Impostos
- Transferência

### Conciliação
1. Extrair extrato bancário
2. Comparar com lançamentos
3. Identificar divergências
4. Ajustar lançamentos
5. Fechar conciliação

## Procedimento

1. Identificar operação financeira
2. Validar dados de entrada
3. Registrar lançamento
4. Conciliar com banco
5. Gerar relatório

## Regras

- NUNCA conciliar sem fechar o período
- Sempre manter trilha de auditoria
- Divergência > R$ 100 → investigação
- Fechamento mensal obrigatório
