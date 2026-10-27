---
description: Valida configuração de providers de IA, base URL, API key env, modelos, privacidade, fallback e roteamento.
agent: code
---

# /validar-provedores-ia

## Objetivo

Validar configuração dos providers de IA.

## Deve verificar

- baseUrl preenchida;
- apiKeyEnv definida quando necessário;
- modelo padrão definido;
- providerType válido;
- fallback order;
- privacyLevel;
- costTier;
- providers externos bloqueados para dados sensíveis;
- `.env` real não versionado.

## Saída

- relatório de providers;
- problemas;
- riscos;
- correções sugeridas.

## Exemplos

```
/validar-provedores-ia
/validar-provedores-ia conformidade com padroes do projeto
```
