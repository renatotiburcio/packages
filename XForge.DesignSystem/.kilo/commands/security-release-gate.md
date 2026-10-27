---
description: Gate de segurança para liberação de release/deploy com critérios objetivos.
agent: code
---

# /security-release-gate

## Objetivo

Determina se a release está apta para publicação com base em critérios objetivos de segurança.
Avalia bloqueadores, sign-offs obrigatórios e condições de aprovação por severidade.

## Escopo

Release candidate atual. Não executa verificações de código — consome resultados de
`security-golden-rule-check` e `security-audit` como insumos para decisão.

## Referências

- `.xforge/engineer/security/release-gates/SECURITY-RELEASE-GATE.md`
- `.xforge/engineer/security/release-gates/security-release-checklist.json`
- `.xforge/engineer/security/release-gates/SECURITY-RELEASE-GATE-APPROVAL-WORKFLOW.md` (se existir)

## Procedimento

1. Executar `security-release-checklist.json` item por item
2. Para cada item de blocking criteria:
   - Avaliar condição → PASS ou FAIL
   - Se FAIL e `blocks_release: true` → registrar como blocker
3. Verificar sign-offs obrigatórios (security lead, QA lead, PO)
4. Verificar deadline de aprovação (expirou? reassign automático?)
5. Consolidar resultado: APPROVED / BLOCKED / CONDITIONAL
6. Gerar parecer com lista de blockers e responsáveis

## Critérios de Bloqueio

| Severidade | Condição | Impacto |
|---|---|---|
| CRITICAL | Qualquer blocker FAIL | BLOQUEIA release |
| HIGH | Blocker FAIL sem exceção aprovada | BLOQUEIA release |
| MEDIUM | Blocker FAIL sem plano de ação | CONDITIONAL (aprova com ressalvas) |
| LOW | Blocker FAIL | Não bloqueia, registra |

## Sign-offs Obrigatórios

| Papel | Obrigatório para | Prazo |
|---|---|---|
| Security Lead | Releases com blockers CRITICAL/HIGH | 48h úteis |
| QA Lead | Releases com blockers MEDIUM | 24h úteis |
| Product Owner | Releases condicionais | 72h úteis |

## Saída Esperada

JSON com resultado da decisão:

```json
{
  "release": "v2.5.1",
  "status": "BLOCKED",
  "blockers": [
    {
      "item": "SQ001 - Dependency scan",
      "severity": "CRITICAL",
      "detail": "CVE-2025-1234 em Newtonsoft.Json 12.0.3",
      "owner": "dev-team",
      "exception_granted": false
    }
  ],
  "sign_offs": {
    "security_lead": "pending",
    "qa_lead": "approved",
    "po": "approved"
  },
  "decision": "BLOCKED",
  "justification": "Blocker CRITICAL sem exceção: CVE-2025-1234 sem patch disponível.",
  "timestamp": "2026-06-10T04:59:53-03:00"
}
```

## Bloqueia Release

- QUALQUER CRITICAL sem exceção → BLOQUEIA
- HIGH blocker sem sign-off do Security Lead → BLOQUEIA
- CONDITIONAL sem sign-off do PO → BLOQUEIA

## Exemplos

```
/security-release-gate
/security-release-gate --verbose
/security-release-gate --checklist custom-checklist.json
/security-release-gate --output parecer-release.json
```
