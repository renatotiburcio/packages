---
description: Migrar dados, schemas, legados ou sistemas entre versões/ambientes.
agent: code
---

# /migrar

## Categoria

produção

## Objetivo

Executar migrações de dados, schemas ou sistemas de forma segura.

## Sub-comandos

| Sub-comando | Tipo |
|-------------|------|
| `/migrar db` | Migração de banco (EF Core) |
| `/migrar dados` | Migração de dados entre schemas |
| `/migrar legado` | Importação de sistema legado |
| `/migrar ambiente` | Migração entre ambientes |

## Procedimento

### 1. Planejar
- Identificar origem e destino
- Mapear dados/schemas
- Estimar volume e tempo
- Definir janela de manutenção
- Planejar rollback

### 2. Preparar
- Criar snapshot/backup
- Testar em staging
- Validar integridade
- Comunicar aos stakeholders

### 3. Executar
- Rodar migration
- Monitorar progresso
- Validar resultados
- Verificar integridade

### 4. Validar
- Comparar origem vs destino
- Rodar testes pós-migração
- Verificar performance
- Documentar diferenças

### 5. Limpar
- Remover dados temporários
- Atualizar documentação
- Atualizar memória

## Regras

- NUNCA migrar sem backup
- Sempre testar em staging primeiro
- Sempre ter rollback planejado
- Documentar cada passo
- Validar integridade pós-migração

## Exemplos

```
/migrar
/migrar db adicionar coluna UserId
/migrar dados de SQL Server para PostgreSQL
/migrar legado do sistema X para novo formato
```
