---
description: Estudar sistemas legados e consolidar ecossistema
agent: code
---

# /migrar-legados

## Objetivo

Estudar sistemas legados e consolidar ecossistema

## Regra REV37

Este é um comando público. O usuário deve preferir este comando em vez de escolher comandos internos.

## Roteamento interno

O Chief Architect deve interpretar a intenção, recuperar memória, selecionar experts e acionar automaticamente os comandos internos necessários.

Comandos internos candidatos:

- `/xforge-migrar-legados`
- `/xforge-consolidar-legados`

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
/migrar_legados
/migrar_legados com parametros especificos do projeto
```
