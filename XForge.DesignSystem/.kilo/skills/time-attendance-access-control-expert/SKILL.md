---
name: time-attendance-access-control-expert
description: Expert em controle de jornada e acesso: ponto eletronico, REP, escalas, banco de horas e catracas.
metadata:
  version: "1.0.0"
  xforge-category: "domain-expert"
---

# time-attendance-access-control-expert

## Objetivo

Implementar e gerenciar controle de jornada e acesso físico.

## Controle de Jornada

### Equipamentos (Portaria 671)
| Tipo | Descrição | Obrigatório |
|------|-----------|:-----------:|
| REP-C | Registro eletrônico com certificado | Sim |
| REP-P | Registro eletrônico de ponto | Sim |
| REP-A | Acessório de registro | Não |

### Dados Obrigatórios no Ponto
- Identificação do empregado
- Data e hora do registro
- Tipo de registro (entrada, saída, intervalo)
- Número de série do REP

### Escalas de Trabalho

| Escala | Descanso | Legal |
|--------|----------|-------|
| 12x36 | 36h consecutivas | Art. 59-A CLT |
| 6x1 | 24h semanais | Art. 7º, XIV CF |
| 5x2 | 48h semanais | Art. 7º, XIII CF |
| 4x3 | Variável | Negociado |

### Banco de Horas
```
Saldo = Horas trabalhadas - Horas devidas
Se positivo: credor (empresa deve)
Se negativo: devedor (empregado deve)
```
- Compensação em até 6 meses (acordo individual)
- Compensação em até 1 ano (acordo coletivo)
- Horas extras pagas com adicional mínimo 50%

### Adicionais

| Adicional | Mínimo | Quando |
|-----------|:------:|--------|
| Hora extra | 50% | Acima de 8h/dia |
| Noturno | 20% | 22h-5h |
| Domingo | 100% | Trabalho em domingo |
| Feriado | 100% | Trabalho em feriado |
| Insalubridade | 10-40% | Agentes nocivos |
| Periculosidade | 30% | Risco de vida |

## Controle de Acesso

### Tecnologias
| Tecnologia | Vantagem | Desvantagem |
|------------|----------|-------------|
| Biometria | Alta segurança | Custo, LGPD |
| RFID | Rápido | Clone possível |
| QR Code | Barato | Compartilhável |
| Face Recognition | Sem contato | Custo alto |

### Integração Ponto ↔ Acesso
1. Registro de acesso gera registro de ponto
2. Conciliação automática
3. Alertas de divergência

## Procedimento

1. Configurar REP (certificado, horários)
2. Cadastrar funcionários e escalas
3. Configurar regras de banco de horas
4. Integrar com folha de pagamento
5. Monitorar divergências
6. Gerar relatórios mensais

## Regras

- REP-C obrigatório para empresas > 20 empregados
- Banco de horas por acordo (individual ou coletivo)
- Hora extra com adicional mínimo 50%
- Ponto eletrônico não pode ser desativado
- Backup dos registros diário
