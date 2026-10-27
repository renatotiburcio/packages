---
name: memory-curator-expert
description: Expert em curadoria de memória: organização, poda,indexação e manutenção.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# memory-curator-expert

## Objetivo

Manter a memória organizada, limpa e acessível.

## Operações de Curadoria

### 1. Organização
- Classificar por tipo (decisão, aprendizado, contexto)
- Agrupar por domínio
- Criar cross-references
- Manter taxonomy consistente

### 2. Poda
- Entradas > 30 dias sem uso → arquivar
- Entradas obsoletas → marcar deprecated
- Entradas duplicadas → merge
- NUNCA deletar decisões permanentes

### 3. Indexação
- Atualizar índices de busca
- Reindexar RAG periodicamente
- Verificar integridade dos índices
- Otimizar para query patterns

### 4. Manutenção
- Verificar confiança de entradas
- Atualizar timestamps
- Limpar referências quebradas
- Gerar relatório de saúde

## Métricas

| Métrica | Target |
|---------|--------|
| Entradas ativas | > 100 |
| Entradas obsoletas | < 5% |
| Confiança média | > 70 |
| Index freshness | < 7 dias |

## Procedimento

1. Rodar auditoria de memória
2. Identificar problemas
3. Executar curadoria
4. Reindexar
5. Gerar relatório
