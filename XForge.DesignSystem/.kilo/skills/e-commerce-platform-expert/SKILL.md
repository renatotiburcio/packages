---
name: e-commerce-platform-expert
description: Expert em plataformas e-commerce: Shopify, WooCommerce, Nuvemshop, Tray, VTEX, Magento e integracao com ERP.
metadata:
  version: "1.0.0"
  xforge-category: "domain-expert"
---

# e-commerce-platform-expert

## Objetivo

Integrar e gerenciar plataformas e-commerce com ERP para sincronização completa.

## Plataformas

| Plataforma | API | WooCommerce | CMS | Host |
|------------|-----|:-----------:|-----|------|
| Shopify | GraphQL/REST | ✅ | Sim | Cloud |
| WooCommerce | REST | - | WordPress | Self/Cloud |
| Nuvemshop | REST | - | Próprio | Cloud |
| Tray | REST | - | Próprio | Cloud |
| VTEX | REST | - | Próprio | Cloud |
| Magento | REST/GraphQL | - | Próprio | Self/Cloud |

## Fluxo de Integração

```
E-commerce → Webhook → Integração → ERP
                                      ↓
ERP → Estoque/Preço/NF → API → E-commerce
```

### Sincronização de Produtos
1. Cadastro no ERP é fonte da verdade
2. Mapeamento: SKU ERP ↔ SKU e-commerce
3. Sincronização: nome, descrição, preço, estoque, fotos
4. Categorias mapeadas

### Processamento de Pedidos
1. Pedido criado no e-commerce
2. Webhook notifica integração
3. Cria pedido no ERP
4. Reserva estoque
5. Gera NF-e
6. Atualiza status no e-commerce
7. Envia rastreio

### Exemplo Shopify API
```graphql
mutation orderCreate($input: OrderInput!) {
  orderCreate(input: $input) {
    order {
      id
      name
      totalPriceSet { shopMoney { amount currencyCode } }
    }
    userErrors { field message }
  }
}
```

### Exemplo WooCommerce API
```javascript
const order = await wc.post('orders', {
  payment_method: 'bacs',
  line_items: [{ product_id: 123, quantity: 2 }],
  shipping_lines: [{ method_id: 'flat_rate', total: '10.00' }]
});
```

## Procedimento

1. Configurar credenciais da plataforma
2. Mapear categorias e produtos
3. Configurar webhooks
4. Testar sincronização estoque
5. Testar fluxo completo pedido → NF
6. Monitorar erros

## Regras

- ERP é fonte da verdade para estoque/preço
- NF-e sempre gerada antes do envio
- Webhooks processados idempotentemente
- Log completo de sincronizações
