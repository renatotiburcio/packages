# Testcontainers Standard

## Objetivo

Garantir testes de integração com dependências reais, descartáveis e reproduzíveis via Docker.

## Quando usar

- PostgreSQL;
- MySQL;
- SQL Server;
- Redis;
- RabbitMQ;
- Kafka;
- serviços externos containerizados;
- integrações que não devem usar banco compartilhado.

## Valor real

Evita testes frágeis que dependem de ambiente manual.

Cada dev ou pipeline sobe dependências temporárias e limpas.

## Regras

- não usar banco de produção;
- containers devem ser descartáveis;
- dados de teste devem ser isolados;
- migrations devem rodar no container;
- testes devem ser determinísticos;
- limpar recursos no final.

## Exemplo conceitual

```csharp
var postgres = new PostgreSqlBuilder()
    .WithImage("postgres:16")
    .WithDatabase("xforge_tests")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .Build();

await postgres.StartAsync();
```
