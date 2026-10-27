---
description: Segurança contínua: audit, LGPD, OWASP, release gate, secrets scan.
agent: code
---

# /seguranca

## Categoria

validação

## Objetivo

Auditar e garantir segurança do código, dados e infraestrutura.

## Sub-comandos

| Sub-comando | Foco |
|-------------|------|
| `/seguranca audit` | Auditoria completa |
| `/seguranca lgpd` | Conformidade LGPD |
| `/seguranca api` | Segurança de endpoints |
| `/seguranca auth` | Autenticação/autorização |
| `/seguranca deps` | Dependências vulneráveis |
| `/seguranca release` | Release gate de segurança |

## Procedimento

### 1. Secrets Scan
- Verificar: sem API keys, passwords, tokens no código
- Verificar: sem connection strings hardcoded
- Verificar: .env não commitado

### 2. Autenticação
- JWT: expiry ≤ 15 min, signing ≥ 256 bits
- MFA: TOTP implementado
- Cookie: HttpOnly, Secure, SameSite=Strict
- Brute force: lockout após 5 tentativas

### 3. Autorização
- RBAC configurado
- Deny by padrão
- Audit trail de tentativas negadas

### 4. API Security
- Rate limiting configurado
- Input validation em todos os endpoints
- CORS: origins explícitas (NUNCA *)
- Headers de segurança (X-Content-Type-Options, CSP, etc.)

### 5. LGPD
- Dados pessoais mapeados
- Criptografia: TLS 1.2+ (trânsito), AES-256 (repouso)
- Consentimento documentado
- Direitos do titular implementados

### 6. Dependências
```powershell
dotnet list package --vulnerable
npm audit
```

## Saída

```json
{
  "secrets": "clean",
  "auth": "pass",
  "api": "pass",
  "lgpd": "pass",
  "deps": "clean",
  "score": 92,
  "blockers": []
}
```

## Bloqueio

CRITICAL ou HIGH → bloquear release. MEDIUM → documentar e criar issue.

## Exemplos

```
/seguranca
/seguranca audit completo da API
/seguranca verificar conformidade LGPD
/seguranca analisar vulnerabilidades no endpoint de auth
```
