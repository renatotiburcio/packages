---
name: cnab-payment-expert
description: Expert em CNAB (Comunicacao Bancaria): remessa, retorno, boleto, pix, layout 240/400 e integracao bancaria.
metadata:
  version: "1.0.0"
  xforge-category: "domain-expert"
---

# cnab-payment-expert

## Objetivo

Implementar e gerenciar integrações bancárias via CNAB, boletos e pagamentos digitais.

## Tipos de CNAB

### CNAB 240 (Spefin)
- Padrão FEBRABAN para remessa/retorno
- Usado para: títulos, folha, DARF, GPS
- Layout: header, detalhe (segmentos A-J), trailer

### CNAB 400 (Legado)
- Padrão antigo ainda usado por alguns bancos
- Layout: header, registro tipo 1 e 2, trailer

### CNAB SIGCB (Caixa)
- Padrão específico para boletos Caixa
- Geração online com nosso número

## Fluxo de Boleto

```
Emissão → Remessa (CNAB) → Banco → Registro
                                        ↓
Pagamento ← Retorno (CNAB) ← Banco ← Confirmação
```

### Emissão
1. Gerar dados do boleto (sacado, valor, vencimento)
2. Calcular nosso número
3. Gerar linha digitável e código de barras
4. Enviar para impressão/PDF

### Remessa
1. Montar arquivo CNAB (header + detalhes + trailer)
2. Validar: hash, sequencial, totais
3. Enviar ao banco via API ou arquivo
4. Aguardar retorno

### Retorno
1. Ler arquivo de retorno do banco
2. Processar cada registro:
   - Liquidado → baixar título
   - Protestado → gerar evento
   - Desconhecido → investigar
3. Atualizar situações
4. Gerar relatório

## Layout CNAB 240 - Segmento A (Título)

| Posição | Tamanho | Campo |
|---------|:-------:|-------|
| 001-003 | 3 | Código do banco |
| 004-006 | 3 | Lote de serviço |
| 007-009 | 3 | Conta de movimento |
| 014-016 | 3 | Número do documento |
| 038-050 | 13 | Valor do título |
| 051-053 | 3 | Banco cobrador |
| 058-058 | 1 | Moeda |
| 073-083 | 11 | Nosso número |

## Pix via CNAB

- Chave PIX (CNPJ, email, telefone, aleatória)
- QR Code estático ou dinâmico
- Confirmação instantânea
- Conciliação automática

## Integrações Bancárias

| Banco | API | CNAB |
|-------|-----|------|
| Itaú | Open Banking | 240/400 |
| Bradesco | Extranet | 240/400 |
| BB | Open Banking | 240 |
| Caixa | SIGCB | 240 |
| Santander | Extranet | 240 |
| Inter | API | 240 |
| Sicredi | API | 240 |

## Procedimento

1. Identificar tipo de pagamento (boleto, Pix, TED, folha)
2. Montar layout CNAB correto
3. Validar campos obrigatórios
4. Gerar arquivo de remessa
5. Enviar ao banco
6. Processar retorno
7. Conciliar com contas a pagar/receber

## Regras

- Nosso número sempre sequencial e único
- Hash de validação obrigatório
- Retorno processado em até 24h
- Divergências geram alerta
- Backup de todos os arquivos CNAB
