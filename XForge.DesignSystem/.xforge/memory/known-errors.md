# Memória - Erros Conhecidos

## EF Core + SQLite em testes

**Data de descoberta:** 2026-06-08
**Status:** documented
**Severidade:** medium

### Sintoma
Migration falha com "SQLite does not support this migration operation"

### Causa Raiz
SQLite não suporta todas as operações de migration do EF Core (ex: `AlterColumn`, `DropColumn`).

### Solução
- Usar InMemory database para testes de unidade simples
- Usar Testcontainers com PostgreSQL para testes de integração com migrations
- Ou usar `ExcludeFromMigrations()` e gerar script SQL para SQLite

### Prevenção
- Não assumir que migrations EF Core funcionam em SQLite
- Usar o mesmo engine de banco em testes e produção

### Fonte
https://learn.microsoft.com/ef/core/providers/sqlite/limitations

---

## RAG não encontra informação após adicionar arquivo

**Data de descoberta:** 2026-06-08
**Status:** documented
**Severidade:** low

### Sintoma
Arquivo adicionado mas query não retorna resultados

### Causa Raiz
RAG não foi reindexado após adição do arquivo.

### Solução
```powershell
.\.kilo\automation\scripts\rag\index-local.ps1
```

### Prevenção
- Sempre reindexar após adicionar conhecimento
- Verificar com `report-index.ps1` se o documento foi indexado
