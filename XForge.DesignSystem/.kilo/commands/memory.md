---
description: Salvar, recuperar, auditar e versionar memória do projeto.
agent: code
---

# /memoria

## Categoria

gestão

## Objetivo

Gerenciar a memória persistente do projeto: decisões, aprendizados, contexto.

## Sub-comandos

| Sub-comando | Ação |
|-------------|------|
| `/memoria salvar <conteúdo>` | Criar/atualizar entrada |
| `/memoria buscar <keyword>` | Buscar por palavra-chave |
| `/memoria listar` | Listar entradas recentes |
| `/memoria auditar` | Verificar integridade |
| `/memoria limpar` | Poda de entradas obsoletas |

## Procedimento

### Salvar
1. Identificar tipo: decisão / aprendizado / contexto
2. Verificar se já existe similar
3. Criar com timestamp + fonte
4. Indexar para busca

### Buscar
1. Usar BM25 (memory search)
2. Filtrar por tipo e scope
3. Ordenar por relevância
4. Retornar top-5

### Auditar
1. Verificar todas as entradas têm fonte
2. Verificar nenhuma contém secrets
3. Verificar confiança ≥ 50
4. Gerar relatório de saúde

### Limpar
1. Entradas > 30 dias sem uso → arquivar
2. Contraditórias → marcar deprecated
3. NUNCA deletar decisões permanentes

## Onde Salvar

| Tipo | Caminho |
|------|---------|
| Decisão | `.xforge/decisions/` |
| Sessão | `.xforge/memory/sessions/` |
| Global | `.xforge/memory/global/` |
| Aprendizado | `.xforge/learning/` |

## Regras

- NUNCA salvar passwords, keys, tokens
- Sempre registrar fonte
- Dados sensíveis → pseudonimizar
- Decisões arquiteturais → sempre salvar

## Exemplos

```
/memoria
/memoria salvar: usamos PostgreSQL por requisitos de ACID
/memoria buscar decisões sobre autenticação
/memoria auditar integridade da memória
/memoria listar decisões arquiteturais
```
