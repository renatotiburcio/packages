---
description: Verifica regras de ouro de segurança no código-fonte (nível código).
agent: code
---

# /security-golden-rule-check

## Objetivo

Avalia o código contra as regras de ouro de segurança. Foco estrito em violações de codificação:
validação de entrada, secrets hardcoded, senhas em texto plano, injeção SQL, XSS, dependências vulneráveis.

## Escopo

Apenas código-fonte (arquivos .cs, .js, .ts, .py, .sql, .json). Não cobre infraestrutura,
configuração de deploy ou políticas organizacionais.

## Referências

- `.xforge/engineer/security/golden-rules/CONTINUOUS-SECURITY-GOLDEN-RULE.md`
- `.xforge/engineer/security/golden-rules/continuous-security-checklist.json`

## Procedimento

1. Carregar `continuous-security-checklist.json` com a lista de regras de ouro
2. Para cada regra, escanear o código-fonte do diretório atual
3. Reportar PASS ou FAIL por regra, incluindo arquivo:linha e trecho do código
4. Classificar severidade: CRITICAL / HIGH / MEDIUM / LOW
5. Tentar correção automática quando `auto_fixable: true` e flag `--fix` ativa
6. Gerar saída JSON estruturada

## Critérios de Avaliação

| Regra de Ouro | O que verifica | Severidade padrão |
|---|---|---|
| Input Validation | Parâmetros não sanitizados | CRITICAL |
| Secrets in Code | Tokens, chaves, connection strings hardcoded | CRITICAL |
| Hardcoded Passwords | Literais de senha no código | CRITICAL |
| SQL Injection | Concatenação de strings em queries | CRITICAL |
| XSS Prevention | Saída não escapada em HTML/JS | HIGH |
| Dependency Vulns | Pacotes com CVEs conhecidas | HIGH |

## Saída Esperada

JSON array com a estrutura:

```json
[
  {
    "rule": "input-validation",
    "status": "FAIL",
    "severity": "CRITICAL",
    "files": [
      {"file": "src/Controllers/Login.cs", "line": 42, "snippet": "Request.QueryString["user"]"}
    ],
    "auto_fixable": false
  }
]
```

## Bloqueia Release

- CRITICAL FAIL → bloqueia automaticamente
- HIGH FAIL > 3 → bloqueia
- MEDIUM FAIL sem plano de correção → bloqueia

## Exemplos

```
/security-golden-rule-check
/security-golden-rule-check --category secrets
/security-golden-rule-check --fix
/security-golden-rule-check --output golden-report.json
```
