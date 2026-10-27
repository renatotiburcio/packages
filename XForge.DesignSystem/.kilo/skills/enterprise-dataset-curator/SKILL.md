---
name: enterprise-dataset-curator
description: Especialista em datasets internos: approved patterns, anti-patterns, examples e support cases.
metadata:
  version: "28.0.0"
  xforge-category: "knowledge-first"
---

# enterprise-dataset-curator

## Objetivo

Curator e manter datasets de conhecimento internos para treinamento, validação e melhoria contínua.

## Tipos de Dataset

| Tipo | Conteúdo | Uso |
|------|----------|-----|
| **Approved Patterns** | Código aprovado em code review | Referência positiva |
| **Anti-Patterns** | Código rejeitado com motivo | O que NÃO fazer |
| **Examples** | Soluções completas para problemas comuns | Templates reutilizáveis |
| **Support Cases** | Tickets de suporte resolvidos | Padrões de resolução |
| **Decisions** | ADRs e decisões arquiteturais | Contexto histórico |

## Estrutura de Entrada

```json
{
  "id": "dataset-001",
  "type": "approved-pattern",
  "domain": "authentication",
  "title": "JWT with refresh token rotation",
  "content": "...",
  "source": "PR #234",
  "trustScore": 90,
  "createdAt": "2024-01-15",
  "lastValidated": "2024-01-20",
  "tags": ["security", "auth", "jwt"]
}
```

## Procedimento

### 1. Coletar
- Extrair de PRs aprovados
- Extrair de tickets resolvidos
- Extrair de decisões documentadas
- Extrair de incidentes (o que funcionou)

### 2. Classificar
- Tipo (pattern/anti-pattern/example/case)
- Domínio (security/architecture/domain/etc)
- Confiança (trust-score)
- Tags para busca

### 3. Validar
- Verificar se ainda está atualizado
- Verificar se contradiz conhecimento mais recente
- Atualizar trust-score
- Marcar deprecated se obsoleto

### 4. Curadoria
- Deduplicar entradas similares
- Comprimir entradas redundantes
- Mover para cold storage se antigo
- Promover padrões fortes

### 5. Indexar
- Adicionar ao RAG index
- Atualizar knowledge graph
- Criar cross-references
- Gerar estatísticas

## Métricas

| Métrica | Target |
|---------|--------|
| Entradas ativas | > 50 |
| Trust score médio | > 75 |
| Atualização mensal | > 10 entradas |
| Uso em tarefas | > 30% das tarefas |

## Regras

- Toda entrada deve ter fonte
- Trust score ≥ 50 para uso
- Revalidar trimestralmente
- Deduplicar antes de adicionar
