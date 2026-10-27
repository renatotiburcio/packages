---
name: prompt-engineering-expert
description: Expert em prompt engineering: estruturação de prompts, few-shot, chain-of-thought, system prompts.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# prompt-engineering-expert

## Objetivo

Criar prompts eficazes para obter respostas de alta qualidade de modelos de IA.

## Técnicas

### 1. System Prompt
```
Você é um especialista em [domínio].
Siga estas regras:
- Regra 1
- Regra 2
```

### 2. Few-Shot
```
Exemplo 1: entrada → saída esperada
Exemplo 2: entrada → saída esperada
Agora: nova entrada →
```

### 3. Chain-of-Thought
```
Pense passo a passo:
1. Primeiro, analise...
2. Depois, considere...
3. Por fim, conclua...
```

### 4. Structured Output
```
Responda em JSON:
{
  "campo1": "...",
  "campo2": "..."
}
```

## Regras de Ouro

1. **Seja específico**: "Refatore o método X para Y" > "Melhore o código"
2. **Dê contexto**: Incluir arquivos relevantes, histórico
3. **Defina formato**: JSON, markdown, lista
4. **Limite escopo**: Uma tarefa por prompt
5. **Inclua exemplos**: Mostrar o esperado

## Anti-Padrões

- ❌ Prompts vagos: "Faça isso ficar bom"
- ❌ Múltiplas tarefas em um prompt
- ❌ Sem formato de saída definido
- ❌ Sem contexto do projeto
- ❌ Confiança cega em AI para decisões críticas

## Procedimento

1. Entender o objetivo
2. Coletar contexto
3. Estruturar prompt
4. Testar e iterar
5. Documentar prompt eficaz

## Regras

- Documentar prompts que funcionam
- Versionar system prompts
- Testar com múltiplos modelos
- Nunca expor secrets em prompts
