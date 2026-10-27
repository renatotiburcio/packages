---
id: T032
title: Bundle analyzer — tamanho final + tree-shaking
priority: P3
status: pending
estimated_time: 1h
dependencies: T001
---

# T032: Bundle analyzer

## Objetivo
Analisar tamanho do bundle final e oportunidades de otimização.

## Passos
1. Executar `dotnet publish -c Release -p:PublishAot=true`
2. Analisar tamanho dos DLLs
3. Identificar componentes não usados (tree-shaking)
4. Gerar report

## Arquivos
- Criar: `.xforge/project/bundle-report.md`

## Critérios de Aceite
- [ ] Tamanho do bundle medido
- [ ] Componentes não usados identificados
- [ ] Report salvo
