---
id: T015
title: Multi-targeting net8.0;net9.0;net10.0 no csproj
priority: P1
status: pending
estimated_time: 2h
dependencies: nenhuma
---

# T015: Multi-targeting para net8/9/10

## Objetivo
Permitir que o XForge.DesignSystem seja consumido em projetos .NET 8 (LTS), .NET 9 e .NET 10, ampliando compatibilidade.

## Passos
1. Alterar `<TargetFramework>net10.0</TargetFramework>` para `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`
2. Adicionar ItemGroups condicionais para package versions por TFM (Components 8.0.12, 9.0.1, 10.0.1)
3. Testar build em cada TFM
4. Validar pack em cada TFM
5. Adicionar pipeline matrix GitHub Actions

## Arquivos
- Modificar: `src/XForge.DesignSystem/XForge.DesignSystem.csproj`
- Modificar: `.github/workflows/build.yml` (adicionar matrix)

## Criterios de Aceite
- [ ] Build limpo em net8.0, net9.0, net10.0
- [ ] Pack produz 3 tfm-specific nupkgs
- [ ] CI matrix roda os 3 builds

## Notas
- MudBlazor multi-target assim
- ALC (AssemblyLoadContext) issues raros com MudBlazor em net8 (cuidado)
