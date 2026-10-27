---
description: Ver/acionar experts e skills corretos
agent: code
---

# /especialistas

## Objetivo

Ver/acionar experts e skills corretos

## Regra REV37

Este é um comando público. O usuário deve preferir este comando em vez de escolher comandos internos.

## Roteamento interno

O Chief Architect deve interpretar a intenção, recuperar memória, selecionar experts e acionar automaticamente os comandos internos necessários.

Comandos internos candidatos:

- `/xforge-orquestrar-especialistas`

## Saída obrigatória

- intenção entendida;
- experts selecionados;
- plano de execução;
- comandos internos acionados;
- quality/security gates;
- resultado;
- memória atualizada;
- dashboard/registries atualizados quando aplicável.

## Exemplos

```
/especialistas
/especialistas com parametros especificos do projeto
```
