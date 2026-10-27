---
description: Valida SOLID e SRP, garantindo que cada arquivo tenha uma única responsabilidade.
agent: code
---

# /validar-solid-srp

## Objetivo

Auditar o projeto em busca de violações SOLID/SRP.

## Deve detectar

- classes grandes demais;
- arquivos com múltiplas responsabilidades;
- handlers fazendo persistência, validação e mapping juntos;
- services genéricos demais;
- Program.cs poluído;
- duplicação de regra de negócio.

## Saída

- relatório de violações;
- plano de refatoração;
- backlog;
- arquivos sugeridos;
- testes necessários.

## Exemplos

```
/validar-solid-srp
/validar-solid-srp conformidade com padroes do projeto
```
