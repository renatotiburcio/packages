---
name: state-machine
description: Use when tracking or validating Engineer workflow states such as recognizing, learning, planning, developing, testing, reviewing, memorizing, indexing, failed, or done.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# state-machine

## Objetivo

Controlar transições de estado do workflow garantindo consistência.

## Estados

```
recognizing → planning → developing → testing → reviewing → memorizing → indexing → done
                         ↓                ↓           ↓
                       failed           failed      failed
```

## Transições Válidas

| De | Para | Trigger |
|----|------|---------|
| recognizing | planning | Reconhecimento completo |
| planning | developing | Plano aprovado |
| developing | testando | Implementação completa |
| testing | reviewing | Testes passando |
| reviewing | memorizing | Review aprovado |
| memorizing | indexing | Memória salva |
| indexing | done | Indexação completa |
| qualquer | failed | Erro crítico |
| failed | qualquer | Retry manual |

## Regras

- NUNCA pular estados
- NUNCA voltar de done para developing
- Sempre documentar motivo de failed
- Timeout: 30 min por estado (avisar se exceder)

## Procedimento

1. Verificar estado atual
2. Validar se transição é permitida
3. Executar ação do estado
4. Registrar transição com timestamp
5. Atualizar .xforge/engineer/runtime/state.json
