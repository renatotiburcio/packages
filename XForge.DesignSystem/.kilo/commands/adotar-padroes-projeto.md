---
description: Analisa padrões atuais de um projeto existente e permite assumir alguns como padrão oficial do projeto.
agent: code
---

# /adotar-padroes-projeto

## Objetivo

Permitir que o usuário diga quais padrões existentes devem virar padrão oficial do projeto.

## Fluxo

1. Reconhecer projeto.
2. Mapear padrões encontrados.
3. Separar padrões bons, ruins e incertos.
4. Perguntar interativamente quais adotar.
5. Registrar decisão em PROJECT-DNA.
6. Atualizar memória do projeto.
7. Criar ADR se impactar arquitetura.

## Perguntas ao usuário

- Quais padrões atuais devem ser mantidos?
- Algum padrão deve ser substituído?
- A estrutura de pastas atual é oficial?
- DTOs/Requests/Responses atuais devem ser padrão?
- Endpoints atuais devem ser padrão?
- Testes atuais devem ser padrão?

## Exemplos

```
/adotar-padroes-projeto
/adotar-padroes-projeto com parametros especificos do projeto
```
