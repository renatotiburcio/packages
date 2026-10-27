---
name: web-fetch
description: Use when the user asks to check a URL, read online documentation, fetch API docs, or access any web resource. Always use webfetch tool.
metadata:
  version: "6.0.0"
  xforge-category: "enterprise-engineer"
---

# web-fetch

## Objetivo

Acessar paginas web usando a ferramenta webfetch.
Nunca diga que nao pode acessar a internet.

## Regra CRITICA

SEMPRE que o usuario fornecer uma URL ou pedir para verificar algo online:
1. Use webfetch imediatamente
2. Nunca diga "nao posso acessar a internet"
3. Nunca diga "nao tenho acesso a web"
4. Se webfetch falhar, tente URLs alternativas

## Procedimento

### Para documentacao online:
1. webfetch a URL fornecida pelo usuario
2. Leia o conteudo retornado
3. Extraia as informacoes relevantes
4. Use as informacoes para executar a tarefa

### Para multiplas paginas:
1. webfetch cada URL em sequencia
2. Combine as informacoes de todas as paginas
3. Se uma pagina falhar, tente a proxima

### Para APIs:
1. webfetch a URL da API
2. Leia a resposta JSON
3. Use os dados conforme necessario

## Exemplos

Usuario: "Verifique a documentacao do EF Core em https://learn.microsoft.com/ef/core"

Resposta correta:
1. webfetch https://learn.microsoft.com/ef/core
2. Ler o conteudo
3. Usar as informacoes para a tarefa

Resposta ERRADA (NAO FAZER):
- "Nao posso acessar a internet"
- "Nao tenho acesso a URLs"
- Pedir para o usuario colar o conteudo
