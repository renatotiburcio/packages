---
name: agronegocio-rural-expert
description: Expert em agronegocio: produtor rural, LCPR, custos agricolas, safras, maquinas e livros fiscais do produtor.
metadata:
  version: "1.0.0"
  xforge-category: "domain-expert"
---

# agronegocio-rural-expert

## Objetivo

Gerenciar operações rurais: produção, custos, livros fiscais e obrigações do produtor rural.

## Cadastro do Produtor Rural

| Dado | Obrigatório |
|------|:-----------:|
| CCIR (INCRA) | Sim |
| CAR (Cadastro Ambiental Rural) | Sim |
| ITR (Imposto Territorial Rural) | Sim |
| Inscrição Estadual (produtor) | Sim |
| Cadastro PF no SPED | Sim |

## Livro Caixa do Produtor Rural (LCPR)

### Receitas
| Tipo | Descrição |
|------|-----------|
| Venda de commodities | Soja, milho, algodão, café, etc. |
| Venda de animais | Bovinos, suínos, aves |
| Serviços rurais | Aluguel de máquinas, prestação de serviço |
| Subvenção | PRONAF, pronafi, subsídios |

### Despesas
| Tipo | Descrição |
|------|-----------|
| Insumos | Sementes, fertilizantes, defensivos |
| Mão de obra | Trabalhadores rurais, empreitada |
| Máquinas | Combino, manutenção, depreciação |
| Terra | Arrendamento, ITR, seguros |

### Apuração
```
Receita Bruta - Despesas Dedutíveis = Lucro/Prejuízo
    ↓
Tributação: Lucro Presumido (32% agropecuária)
    ou Lucro Real
```

## Custos Agrícolas

### Custo por Hectare
| Item | Soja | Milho | Café |
|------|------|-------|------|
| Sementes | R$ 450 | R$ 350 | R$ 800 |
| Fertilizantes | R$ 1.200 | R$ 900 | R$ 2.500 |
| Defensivos | R$ 800 | R$ 600 | R$ 1.500 |
| Mão de obra | R$ 600 | R$ 400 | R$ 2.000 |
| Máquinas | R$ 500 | R$ 350 | R$ 700 |
| **Total** | **R$ 3.550** | **R$ 2.600** | **R$ 7.500** |

### Custo por Sacas
```
Custo Total / Produtividade (sc/ha) = Custo por saca
Ex: R$ 3.550 / 55 sc = R$ 64,55/sc
```

## Gestão Rural

### Safra
- Planejamento de plantio
- Calendário agrícola
- Controle de chuvas
- Previsão de colheita

### Máquinas e Equipamentos
- Plano de manutenção
- Custo operacional por hora
- Depreciação fiscal
- Seguro agrícola

### Rebanho
- Peso vivo / peso canal
- GMD (Ganho Médio Diário)
- Conversão alimentar
- Mortalidade

## Procedimento

1. Cadastrar produtor rural (CCIR, CAR, ITR)
2. Configurar livro caixa
3. Registrar receitas e despesas por cultura
4. Calcular custos por hectare/saca
5. Apurar lucro/prejuízo
6. Gerar LCPR para declaração
7. Enviar SPED Contribuições (se aplicável)

## Regras

- LCPR obrigatório para Lucro Presumido rural
- Receitas rurais têm alíquota diferenciada
- ITR calculado sobre valor territorial
- Seguro agrícola recomendado
- CAR atualizado anualmente
