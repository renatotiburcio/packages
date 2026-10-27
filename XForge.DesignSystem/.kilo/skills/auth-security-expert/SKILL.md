---
name: auth-security-expert
description: Expert em autenticação, MFA, JWT, cookies, sessão e brute force.
metadata:
  version: "37.0.0"
  xforge-category: "continuous-security"
---

# auth-security-expert

## Objetivo

Auditar e implementar mecanismos de autenticação seguros.

## Checklist de Autenticação

### JWT
- [ ] Token expiry ≤ 15 min (access), 7 dias (refresh)
- [ ] Signing key ≥ 256 bits
- [ ] Claims mínimas: sub, exp, iat, iss
- [ ] Refresh token rotação implementada
- [ ] Token blacklist para logout

### MFA
- [ ] TOTP (Google Authenticator) como mínimo
- [ ] SMS como fallback apenas
- [ ] Recovery codes criptografados
- [ ] Rate limit em tentativas de verificação

### Sessão
- [ ] Cookie: HttpOnly, Secure, SameSite=Strict
- [ ] Session timeout ≤ 30 min inatividade
- [ ] Regeneração de session ID após login
- [ ] Invalidação completa no logout

### Brute Force
- [ ] Lockout após 5 tentativas
- [ ] Progressivo: 1min → 5min → 15min → lock
- [ ] CAPTCHA após 3 tentativas
- [ ] Log de tentativas suspeitas

## Procedimento

1. Mapear endpoints de autenticação
2. Aplicar checklist acima
3. Classificar achados por severidade
4. Gerar código de correção se aplicável
5. Bloquear release se crítico/alto
6. Atualizar security audit trail
