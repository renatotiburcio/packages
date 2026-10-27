---
name: autonomy-levels
description: Select and enforce the correct autonomy level for each task.
metadata:
  version: "7.0.0"
  xforge-category: "strategic-intelligence"
---

# autonomy-levels

## Objetivo

Selecionar o nível de autonomia correto para cada tarefa, equilibrando segurança e eficiência.

## Níveis de Autonomia

| Nível | Descrição | Permissão | Exemplo |
|:-----:|-----------|-----------|---------|
| **L0** | Assistido | Precisa de aprovação para cada ação | Migration de banco, LGPD |
| **L1** | Supervisionado | Pode executar, reporta resultados | Refactor, nova feature |
| **L2** | Autônomo | Executa sem aprovação, reporta ao final | Bug fix, documentação |
| **L3** | Independente | Executa e decide sem reportar | Configuração, formatação |

## Regras de Seleção

### Sempre L0 (Assistido)
- Migração de banco de dados
- Alteração de dados LGPD
- Modificação de RBAC/governança
- Release para produção
- Alteração de security policies

### Sempre L1 (Supervisionado)
- Nova feature com impacto > 3 arquivos
- Refactor estrutural
- Adição de dependência nova
- Alteração de API pública

### Pode L2 (Autônomo)
- Bug fix simples
- Documentação
- Testes
- Formatação/lint
- Configuração de dev

### Pode L3 (Independente)
- Leitura de código
- Busca de informação
- Análise sem modificação
- Organização de arquivos

## Procedimento

1. Identificar a tarefa
2. Avaliar risco (score 0-19)
3. Selecionar nível mínimo adequado
4. Verificar se há restrição (política, legal)
5. Aplicar nível com permissões correspondentes
6. Registrar no audit trail

## Decisão

```json
{
  "task": "descrição",
  "riskScore": 8,
  "autonomyLevel": "L1",
  "reason": "refactor com impacto em 5 arquivos",
  "overrides": []
}
```
