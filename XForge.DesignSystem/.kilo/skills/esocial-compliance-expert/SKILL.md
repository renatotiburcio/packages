---
name: esocial-compliance-expert
description: Expert em eSocial: eventos, validacoes, prazos, correcoes, SST e integracao com folha de pagamento.
metadata:
  version: "1.0.0"
  xforge-category: "domain-expert"
---

# esocial-compliance-expert

## Objetivo

Garantir conformidade completa com o eSocial, incluindo eventos, validações e prazos.

## Arquitetura do eSocial

```
Empregador → Evento → eSocial → Validacao → Retorno
                                    ↓
                            Erros → Correcao → Reenvio
```

## Eventos

### Iniciais (S-1000 a S-1080)
| Evento | Descrição | Prazo |
|--------|-----------|-------|
| S-1000 | Dados do empregador | Antes do 1º evento |
| S-1005 | Tabelas de rubricas | Antes do 1º evento |
| S-1010 | Tabelas de lotação | Antes do 1º evento |
| S-1020 | Tabelas de cargos | Antes do 1º evento |
| S-1070 | Tabelas de processos | Antes do 1º evento |

### Não Periódicos (S-2200 a S-2400)
| Evento | Descrição | Prazo |
|--------|-----------|-------|
| S-2200 | Cadastramento inicial/admissão | Até dia anterior ao início |
| S-2205 | Alteração cadastral | Até 16 dias após mudança |
| S-2206 | Alteração contratual | Até 16 dias após mudança |
| S-2230 | Afastamento temporário | Até 16 dias após início |
| S-2299 | Desligamento | Até 10 dias após desligamento |
| S-2399 | Trabalhador sem vínculo | Até 16 dias após término |

### Periódicos (S-1200 a S-1299)
| Evento | Descrição | Prazo |
|--------|-----------|-------|
| S-1200 | Remuneração | Até dia 7 do mês seguinte |
| S-1210 | Pagamentos | Até dia 7 do mês seguinte |
| S-1260 | Comercialização produção | Até dia 7 do mês seguinte |
| S-1299 | Fechamento dos eventos | Até dia 7 do mês seguinte |

### SST (S-2210 a S-2240)
| Evento | Descrição | Prazo |
|--------|-----------|-------|
| S-2210 | Comunicação de acidente | Até dia seguinte ao acidente |
| S-2220 | Monitoramento da saúde | Até 15 dias após exame |
| S-2240 | Condições ambientais | Até 15 dias após avaliação |

## Validações Comuns

| Erro | Causa | Correção |
|------|-------|----------|
| CPRB incorreta | Base de cálculo errada | Recalcular sobre receita bruta |
| FGTS divergente | Rubrica não mapeada | Verificar tabela de rubricas |
| IRRF incorreto | Dependentes não cadastrados | Atualizar S-2205 |
| eSocial LOTACAO inválida | Lotação não existe | Cadastrar S-1010 |

## Procedimento

1. Verificar tabelas iniciais (S-1000 a S-1080)
2. Processar eventos não periódicos (admissões, afastamentos)
3. Calcular remuneração (S-1200)
4. Processar pagamentos (S-1210)
5. Fechar período (S-1299)
6. Enviar SST (S-2210, S-2220, S-2240)
7. Validar retorno
8. Corrigir erros se houver

## Regras

- Eventos iniciais ANTES de qualquer outro evento
- Prazos são rígidos (multas por atraso)
- Correção via evento específico (não exclusão)
- Backup de todos os envios
- Validação antes de transmitir
