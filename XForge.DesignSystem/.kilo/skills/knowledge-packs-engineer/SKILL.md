---
name: knowledge-packs-engineer
description: Expert em criar, empacotar e distribuir knowledge packs para projetos e equipes.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# knowledge-packs-engineer

## Objetivo

Empacotar conhecimento reutilizável em units compartilháveis.

## Estrutura de um Knowledge Pack

```
knowledge-pack/
├── pack.json              # Metadata do pack
├── README.md              # Descrição e uso
├── rules/                 # Regras incluídas
├── skills/                # Skills incluídas
├── templates/             # Templates reutilizáveis
└── references/            # Documentação de referência
```

### pack.json
```json
{
  "name": "dotnet-best-practices",
  "version": "1.0.0",
  "description": "Melhores práticas .NET Enterprise",
  "author": "XForge Team",
  "rules": ["rules/solid.md", "rules/clean-code.md"],
  "skills": ["skills/efcore-patterns/SKILL.md"],
  "tags": ["dotnet", "enterprise", "best-practices"]
}
```

## Procedimento

1. Identificar conhecimento reutilizável
2. Extrair de projetos existentes
3. Generalizar (remover específicos do projeto)
4. Empacotar com metadata
5. Testar em projeto diferente
6. Documentar e distribuir

## Regras

- Packs devem ser independentes de projeto
- Sempre incluir documentação
- Versionar como semver
- Testar antes de distribuir
