---
id: T014
title: FileInput/Upload — progresso, drag-drop, cancelamento
priority: P1
status: pending
estimated_time: 2h
dependencies: T001
---

# T014: FileInput/Upload — progresso, drag-drop, cancelamento

## Objetivo
Testes para upload de arquivos com cenários reais.

## Passos
1. Testar FileInput: file selection, accept types, multiple
2. Testar Upload: progress callback, cancel, error handling
3. Testar DragDropZone: drag over, drop, invalid files
4. Validar com mock IBrowserFile

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFFileInputFunctionalTests.cs`
- Criar: `src/XForge.DesignSystem.Tests/Components/XFUploadFunctionalTests.cs`

## Critérios de Aceite
- [ ] FileInput: 5+ testes
- [ ] Upload: 6+ testes
- [ ] Progress testado
- [ ] Cancel testado
- [ ] 0 failures
