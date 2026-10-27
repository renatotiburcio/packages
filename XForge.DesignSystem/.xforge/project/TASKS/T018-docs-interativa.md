---
id: T018
title: Documentação interativa — live demo em cada doc page
priority: P1
status: pending
estimated_time: 4h
dependencies: T010-T017
---

# T018: Documentação interativa — live demo em cada doc page

## Objetivo
Cada doc page deve ter um exemplo interativo editável com preview ao vivo.

## Passos
1. Identificar páginas sem exemplo interativo (XFCode editável)
2. Adicionar `<XFCode>` com `Copyable="true"` + preview render
3. Garantir que exemplos compilam e executam
4. Adicionar seção "Experimente" em cada doc

## Arquivos
- Modificar: `src/XForge.DesignSystem.Docs/Components/Pages/*.razor`
- Padrão: cada doc com `<XFCode Text="..." Language="razor" Copyable="true" />` + preview

## Critérios de Aceite
- [ ] 100% dos docs com exemplo interativo
- [ ] Exemplos compilam e executam
- [ ] Botão "Copiar" funcional
- [ ] Preview renderiza corretamente

## Notas
- Usar `XFCode` component existente
- Exemplos precisam ser self-contained (funcionam isoladamente)
