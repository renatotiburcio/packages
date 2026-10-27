---
description: Auditoria completa de segurança do projeto (arquitetura, dados, processos).
agent: code
---

# /security-audit

## Objetivo

Auditoria geral de segurança com visão holística do projeto. Avalia arquitetura, modelo de permissões,
proteção de dados, logging, monitoramento, resposta a incidentes e conformidade regulatória.

## Escopo

Todo o projeto: código, infraestrutura como código, pipelines, configurações de runtime,
políticas de acesso, dependências, documentação de segurança.

## Referências

- `.xforge/engineer/security/release-gates/SECURITY-RELEASE-GATE.md`
- `.xforge/engineer/security/release-gates/security-release-checklist.json`
- `.kilo/rules/security-rules.md`
- `.xforge/engineer/security/frequency/SECURITY-FREQUENCY-MATRIX.md`

## Procedimento

1. Revisar arquitetura do sistema (diagramas, fluxos de dados, trust boundaries)
2. Mapear árvore de dependências e identificar bibliotecas não mantidas
3. Auditar modelo de permissões (RBAC, claims,最小权限)
4. Verificar proteção de dados em repouso e em trânsito (criptografia, TLS)
5. Inspecionar logging e monitoramento (logs sem dados sensíveis, alertas configurados)
6. Revisar plano de resposta a incidentes (runbooks, contatos, SLAs)
7. Avaliar conformidade com requisitos regulatórios (LGPD, SOX, PCI)
8. Consolidar achados em matriz de risco com recomendações

## Critérios de Avaliação

| Domínio | O que verifica | Peso |
|---|---|---|
| Arquitetura | Trust boundaries, privilégios, segmentação | ALTO |
| Dependências | Bibliotecas obsoletas, CVEs, licenças | ALTO |
| Permissões | RBAC, privilégios mínimos, expire | ALTO |
| Proteção de Dados | Criptografia, masking, retention | CRÍTICO |
| Logging | Sensitive data leak, retention, alertas | MÉDIO |
| Monitoramento | Dashboards, uptime, detecção | MÉDIO |
| Incident Response | Runbook, contatos, SLA, testes | ALTO |
| LGPD | Consentimento, DPO, direitos do titular | CRÍTICO |

## Saída Esperada

JSON com matriz de risco e recomendações:

```json
{
  "summary": {
    "critical": 1,
    "high": 3,
    "medium": 2,
    "low": 1
  },
  "domains": [
    {
      "domain": "arquitetura",
      "risk": "MEDIUM",
      "findings": ["Usuário admin com acesso irrestrito a todas as tabelas"],
      "recommendation": "Implementar RBAC por camada com claims específicas"
    }
  ],
  "blocking": true,
  "report_url": "audit-report-2026-06-10.md"
}
```

## Bloqueia Release

- CRÍTICO em qualquer domínio → bloqueia
- LGPD violation → bloqueia imediatamente
- Alta dependência sem patch disponível → bloqueia
- Ausência de runbook de incidentes → bloqueia

## Exemplos

```
/security-audit
/security-audit --deep
/security-audit --domain lgpd
/security-audit --domain dependencies
/security-audit --output relatorio-seguranca.md
```
