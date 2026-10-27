---
id: T105
title: Document Processing (PDF+Word)
priority: P0
status: done
estimated_time: 40h
dependencies: [T102]
---

## Objetivo
Lib para gerar PDF e Word (DOCX) nativamente.

## Passos
1. QuestPDF para PDF
2. DocumentFormat.OpenXml para DOCX
3. API fluent
4. Streaming para arquivos grandes

## Criterios de Aceite
- PDF >= 10MB sem OOM
- DOCX roundtrip Word
- Fluent API