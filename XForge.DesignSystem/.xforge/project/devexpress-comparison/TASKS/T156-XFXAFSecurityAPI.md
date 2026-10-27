---
id: T156
title: XFXAFSecurityAPI
priority: P0
status: pending
estimated_time: 24h
dependencies: T001
---

# T156: XFXAFSecurityAPI

## Objetivo
Re-implementar padrao XAF Security API: User, Role, Permission, Member, ObjectSpace.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial
- Resolve CRIT-01 (risco de dependencia comercial)

## Descricao
XFXAFSecurityAPI oferece modelo de seguranca completo: User, Role, TypePermission, ObjectPermission, MemberPermission, com ObjectSpace gerenciando permissao em tempo de execucao. Resolve CRIT-01 ao eliminar dependencia do pacote comercial DevExpress.ExpressApp.Security.

## Passos
1. Estudar XAF Security System no docs.devexpress.com
2. Definir modelo XForge (User, Role, Permission, TypePermission, ObjectPermission, MemberPermission)
3. Implementar SecurityStrategy, ObjectSpace, SecuritySystemUser, SecuritySystemRole
4. Implementar permission checker (CanRead/CanWrite/CanDelete/CanNavigate)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T156/Security/XFSecuritySystem.cs
- Criar: src/XForge.DesignSystem/Components/T156/Security/XFSecurityStrategy.cs
- Criar: src/XForge.DesignSystem/Components/T156/Security/XFObjectSpace.cs
- Criar: src/XForge.DesignSystem/Components/T156/Model/XFUser.cs
- Criar: src/XForge.DesignSystem/Components/T156/Model/XFRole.cs
- Criar: src/XForge.DesignSystem/Components/T156/Model/XFPermission.cs
- Criar: src/XForge.DesignSystem/Components/T156/Model/XFTypePermission.cs
- Criar: src/XForge.DesignSystem/Components/T156/Model/XFObjectPermission.cs
- Criar: src/XForge.DesignSystem/Components/T156/Model/XFMemberPermission.cs
- Criar: src/XForge.DesignSystem.Tests/T156XAFSecurityTests.cs
- Criar: docs/components/t156-xaf-security/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10
- [ ] CRIT-01 resolvido: zero dependencia comercial

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Re-implementa PADRAO XAF, NAO usa pacote DevExpress.ExpressApp.Security
- Inspirar-se no PADRAO/API DevExpress XAF
- 9-way consolidation: 9a fonte do roadmap