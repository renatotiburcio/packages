---
id: T143
title: XFFormLayout-Engine
priority: P0
status: pending
estimated_time: 24h
dependencies: T001
---

# T143: XFFormLayout-Engine

## Objetivo
Implementar FormLayout com 17 templates de editor e validacao integrada, replicando o padrao DevExpress.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFFormLayout oferece LayoutGroups aninhados, LayoutTabPages com TabPage, alinhamento de labels (horizontal/vertical), validacao DataAnnotations/FluentValidation integrada e 17 item templates (TextBox, ComboBox, DateEdit, SpinEdit, CheckBox, Radio, Memo, Mask, DropDown, ListBox, TagBox, ColorPalette, SearchBox, TimeEdit, FileInput, RichText, HTMLEditor).

## Passos
1. Estudar API DevExpress FormLayout no docs.devexpress.com
2. Definir modelo XForge (XFFormLayout, XFFormLayoutItem, XFLayoutGroup, XFLayoutTabPages, XFTabPage)
3. Implementar classe base Razor com cascaded parameters
4. Renderizar 17 templates via RenderFragment (TTemplate<T>)
5. Integrar EditContext/ValidationMessageStore
6. Criar XFDemo page com 5+ exemplos
7. Escrever testes xUnit (10+ tests)
8. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T143/XFFormLayout.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T143/XFFormLayoutItem.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T143/XFLayoutGroup.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T143/XFLayoutTabPages.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T143/XFTabPage.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T143/Templates/*.razor (17 templates)
- Criar: src/XForge.DesignSystem.Tests/T143FormLayoutTests.cs
- Criar: docs/components/t143-form-layout/index.md

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
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap