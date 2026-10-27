---
name: banking-open-finance-expert
description: Expert em integracao bancaria: Open Finance, Open Banking, PIX, APIs bancarias e conciliacao automatica.
metadata:
  version: "1.0.0"
  xforge-category: "domain-expert"
---

# banking-open-finance-expert

## Objetivo

Integrar com bancos via Open Finance, PIX e APIs para automação financeira.

## Open Finance Brasil

### Fases
| Fase | Escopo | Status |
|------|--------|--------|
| 1 | Dados de contas e cartões | Em execução |
| 2 | Meios de pagamento | Em execução |
| 3 | Crédito | Planejado |
| 4 | Seguros | Planejado |

### API Open Finance
```http
GET /accounts/{accountId}/balances
Authorization: Bearer {consent_token}

Response:
{
  "balances": [
    {
      "type": "AVAILABLE",
      "amount": { "value": 15000.00, "currency": "BRL" }
    }
  ]
}
```

## PIX

### Tipos de Chave
| Tipo | Exemplo | Uso |
|------|---------|-----|
| CNPJ | 12.345.678/0001-90 | Empresas |
| Email | contato@empresa.com | Pessoas |
| Telefone | +5511999999999 | Pessoas |
| Aleatória | abc123... | Qualquer |

### Fluxo PIX
```
Pagador → App banco → SPI (Banco Central) → Recebedor
                          ↓
                    Confirmação instantânea
                          ↓
                    Comprovante gerado
```

### Webhook PIX
```json
{
  "event": "PAYMENT_RECEIVED",
  "pix": {
    "endToEndId": "E12345678901234567890123456789012",
    "value": 150.00,
    "key": "12345678000190",
    "description": "Pedido #1234"
  }
}
```

## APIs Bancárias

| Banco | API | Documentação |
|-------|-----|-------------|
| Itaú | Open Banking | developers.itau.com.br |
| Bradesco | Open Banking | developers.bradesco.com.br |
| BB | Open Banking | developers.bb.com.br |
| Santander | Open Banking | developers.santander.com.br |
| Inter | API | developers.inter.co |
| Sicredi | API | developer.sicredi.com.br |

## Conciliação Automática

1. Extrair extrato bancário (OFX/API)
2. Comparar com lançamentos do ERP
3. Match automático: valor + data + documento
4. Divergências → fila de análise
5. Conciliação manual para não match

## Procedimento

1. Configurar credenciais Open Finance
2. Implementar fluxo PIX (estático + dinâmico)
3. Configurar webhooks de confirmação
4. Integrar com contas a pagar/receber
5. Automatizar conciliação bancária
6. Monitorar transações

## Regras

- PIX: confirmação sempre via webhook
- Open Finance: consentimento obrigatório
- Logs de todas as transações
- Reconciliação diária
- Backup de comprovantes
