---
description: Transforma uma intenção do usuário em comando Kilo, prompt, workflow e checklist operacional.
agent: code
---

# /gerar-comando-prompt

## Objetivo

O usuário informa o que quer fazer e o Engineer gera:

- comando Kilo recomendado;
- prompt pronto;
- workflow relacionado;
- skill acionada;
- provider recomendado;
- thinking mode;
- checklist;
- saída esperada;
- riscos;
- próximos passos.

## Exemplos

Entrada:

```text
Quero criar um app financeiro em Blazor com Tailwind e API Minimal
```

Saída esperada:

- comando: `/criar-projeto-dotnet`
- skill: `dotnet-standards`, `blazor-tailwind`
- provider: server/local
- prompt completo;
- checklist de criação.
