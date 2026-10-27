---
name: automapper-standard
description: Use when implementing object mapping, DTO mapping, entity mapping, mapping profiles, replacing Mapster, or validating that AutoMapper is used as the XForge standard.
metadata:
  version: "19.0.0"
  xforge-category: "mapping"
---

# automapper-standard

## Objective

Guarantee that XForge projects use AutoMapper and never Mapster.

## Rules

- Do not use Mapster.
- Do not install Mapster.
- Do not document Mapster as an option.
- Always use AutoMapper.
- Validate latest stable AutoMapper version.
- Mapping logic must not contain business rules.
- Profiles must be cohesive and organized.
- Critical mappings should be tested.

## Existing projects

If Mapster exists:

1. detect references;
2. generate report;
3. propose migration to AutoMapper;
4. create incremental backlog;
5. update memory and PROJECT-DNA.
