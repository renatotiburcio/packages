---
description: Valida que AutoMapper está sendo usado e Mapster não está sendo usado.
agent: code
---

# /validar-automapper

## Objetivo

Garantir padrão de mapping XForge.

## Deve fazer

1. Procurar pacotes Mapster.
2. Procurar using Mapster.
3. Procurar configuração Mapster.
4. Procurar AutoMapper.
5. Validar profiles.
6. Criar plano de migração se houver Mapster.
7. Atualizar memória.

## Regra

Mapster é proibido. AutoMapper é obrigatório.

## Exemplos

```
/validar-automapper
/validar-automapper conformidade com padroes do projeto
```
