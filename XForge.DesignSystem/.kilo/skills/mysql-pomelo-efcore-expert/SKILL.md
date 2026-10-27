---
name: mysql-pomelo-efcore-expert
description: Expert em MySQL com Pomelo EF Core: migrations, queries, performance e charset UTF-8.
metadata:
  version: "7.0.0"
  xforge-category: "technical-expert"
---

# mysql-pomelo-efcore-expert

## Objetivo

Otimizar e configurar MySQL/MariaDB com Pomelo EF Core.

## Configuração

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySql =>
    {
        mySql.MigrationsAssembly("Infrastructure");
        mySql.EnableRetryOnFailure(3);
        mySql.CommandTimeout(30);
    }));
```

## Best Practices

### Charset
```csharp
// UTF-8 obrigatório
entity.Property(e => e.Name).HasMaxLength(255).HasColumnType("varchar(255) CHARACTER SET utf8mb4");
```

### Migrations
- Naming: `YYYYMMDDHHMMSS_Add_Table`
- Sempre down migration
- Testar em banco limpo

### Queries
- Use `.AsNoTracking()` para reads
- Evite subqueries complexas (use JOINs)
- Use indexes para WHERE e ORDER BY

### Performance
```csharp
// Connection pooling
"Server=localhost;Database=mydb;Uid=user;Pwd=pass;MinimumPoolSize=5;MaximumPoolSize=100;"
```

## Procedimento

1. Configurar Pomelo
2. Criar migrations
3. Otimizar queries
4. Adicionar indexes
5. Configurar charset
6. Testar performance

## Regras

- charset=utf8mb4 sempre
- NUNCA use raw SQL sem parameterização
- Connection pooling configurado
- Index para foreign keys
