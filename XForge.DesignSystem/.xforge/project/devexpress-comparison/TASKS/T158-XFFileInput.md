---
id: T158
title: XFFileInput
priority: P1
status: pending
estimated_time: 8h
dependencies: T001
---

# T158: XFFileInput

## Objetivo
File input com preview, drag-drop, multi-file, validacao e upload progress.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFFileInput oferece selecao de arquivos com preview (image thumbnail, file icon por extensao), drag-drop area, multi-file, validacao (tipo/tamanho/extensao), upload progress e integracao com IBrowserFile (Blazor inputfile nativo).

## Passos
1. Estudar API DevExpress FileInput no docs.devexpress.com
2. Definir modelo XForge (XFFileInput, FileValidationRule, UploadProgress)
3. Implementar drag-drop handlers em JS interop
4. Renderizar preview via data URI (image) ou icone (outros)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T158/XFFileInput.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T158/FileValidationRule.cs
- Criar: src/XForge.DesignSystem/Components/T158/UploadProgress.cs
- Criar: src/XForge.DesignSystem/Components/T158/wwwroot/fileInput.js
- Criar: src/XForge.DesignSystem.Tests/T158FileInputTests.cs
- Criar: docs/components/t158-file-input/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- LGPD: validar MIME type server-side (NAO confiar em extensao)
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap