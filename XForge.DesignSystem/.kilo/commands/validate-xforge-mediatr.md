---
description: Verifica se o projeto não usa MediatR e usa XForge.MediatR como padrão obrigatório.
agent: code
---

# /validar-xforge-mediatr

## Objetivo

Garantir que o projeto use XForge.MediatR.

## Deve fazer

1. Procurar referências a MediatR.
2. Procurar pacotes MediatR instalados.
3. Procurar using MediatR.
4. Verificar uso de XForge.MediatR.
5. Gerar plano de substituição se necessário.
6. Atualizar documentação.

## Regra

MediatR é proibido. XForge.MediatR é obrigatório.

## Exemplos

```
/validar-xforge-mediatr
/validar-xforge-mediatr conformidade com padroes do projeto
```
