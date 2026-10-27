---
name: authorization-access-control-expert
description: Expert em autorização, RBAC, ABAC, policies, role hierarchy e permission boundaries.
metadata:
  version: "37.0.0"
  xforge-category: "continuous-security"
---

# authorization-access-control-expert

## Objetivo

Auditar e implementar controle de acesso robusto.

## Modelos de Controle

| Modelo | Quando Usar | Exemplo |
|--------|-------------|---------|
| **RBAC** | Papéis fixos | Admin, Developer, Viewer |
| **ABAC** | Atributos dinâmicos | "só vê dados da própria empresa" |
| **ReBAC** | Relacionamentos | "só vê projetos que participa" |

## Checklist

### RBAC
- [ ] Roles definidas e documentadas
- [ ] Hierarquia de roles clara
- [ ] Princípio do menor privilégio
- [ ] Sem roles genéricas ("admin" para tudo)
- [ ] Audit log de mudanças de role

### ABAC
- [ ] Attributes definidas (user, resource, environment)
- [ ] Policies escritas em linguagem clara
- [ ] Testes para cada policy
- [ ] Performance da evaliação OK (< 10ms)

### Geral
- [ ] Authorization check em TODO endpoint
- [ ] Não confiar apenas no frontend
- [ ] Rate limiting por role
- [ ] Deny por padrão (whitelist)
- [ ] Audit trail de tentativas negadas

## Procedimento

1. Mapear endpoints e recursos
2. Definir who can do what
3. Implementar checks no backend
4. Testar com usuários de cada role
5. Verificar denegation logging
6. Documentar em .xforge/decisions/
