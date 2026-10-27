---
description: Auditoria final completa antes de produção
agent: code
---

# /auditoria-final

## Objetivo

Auditoria final completa antes de produção

## Regra REV37

Este é um comando público. O usuário deve preferir este comando em vez de escolher comandos internos.

## Roteamento interno

O Chief Architect deve interpretar a intenção, recuperar memória, selecionar experts e acionar automaticamente os comandos internos necessários.

Comandos internos candidatos:

- `/xforge-auditoria-final`
- `/xforge-hard-premium-finalizar`
- `/security-release-gate`

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
/auditoria_final
/auditoria_final com parametros especificos do projeto
```
