---
name: dotnet-standards
description: Use when creating new .NET projects, defining .NET project standards, applying Clean Architecture, CQRS, Result Pattern, Repository, UnitOfWork, Swagger/OpenAPI .NET 10, tests, build rules, or adopting standards in existing projects.
metadata:
  version: "12.0.0"
  xforge-category: "dotnet-architecture"
---

# dotnet-standards

## Objective

Apply XForge standard .NET architecture and conventions.

## Sources

Primary reference:

```text
engineer/dotnet-standards/studied/xforge-enterprise-generic-architecture-reference-v2
```

## Procedure for new projects

1. Ask project type.
2. Ask architecture style.
3. Ask selected features.
4. Ask database.
5. Ask frontend type.
6. Ask documentation/Swagger/auth requirements.
7. Generate structure.
8. Generate PROJECT-DNA.
9. Generate ADRs.
10. Generate initial backlog and roadmap.

## Procedure for existing projects

1. Recognize project first.
2. Map current standards.
3. Compare against XForge .NET Standards.
4. Ask what should be adopted.
5. Save standards to PROJECT-DNA.
6. Create ADR for major choices.
7. Update memory.
