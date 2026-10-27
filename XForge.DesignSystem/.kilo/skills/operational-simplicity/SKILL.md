---
name: operational-simplicity
description: Use when evaluating complexity, removing unnecessary abstractions, or simplifying existing code and processes.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# operational-simplicity

## Objetivo

Manter o sistema o mais simples possível sem sacrificar funcionalidade.

## Princípios

1. **YAGNI**: Não implementar o que não é necessário agora
2. **KISS**: Solução mais simples que funciona
3. **Zero-config**: Funcionar com configuração mínima
4. **Convention over configuration**: Preferir padrões a configuração

## Sinais de Complexidade Desnecessária

- [ ] Abstrações que ninguém usa
- [ ] Configurações que ninguém muda
- [ ] Camadas extras sem benefício
- [ ] Genéricos onde específicos bastam
- [ ] Patterns aplicados por aplicar, não por necessidade

## Procedimento

1. Identificar o que pode ser removido
2. Verificar se há uso real
3. Se não usado → marcar como deprecated
4. Se deprecated > 30 dias → remover
5. Documentar decisão de remoção

## Regras

- Preferir código explícito a mágico
- Preferir 3 linhas claras a 1 linha obscura
- Preferir 1 arquivo coeso a 5 arquivos genéricos
- Preferir padrão do framework a custom pattern
