---
id: T077
title: HxSmart AI components
priority: P2
status: pending
estimated_time: 16h
dependencies: T060
origin: Havit
---

# T077: HxSmart AI components

## Objetivo
Implementar 3 HxSmart AI components: HxSmartComboBox (semantic search suggestions), HxSmartPasteButton (paste structured data into form), HxSmartTextArea (autocomplete). Pattern HxSmart* Havit wrapping SmartComponents MS.

## Passos
1. Criar projeto XForge.DesignSystem.AI.Smart/ (separate package)
2. Adicionar pacote SmartComponents.AspNetCore 0.1.0-preview10148 (central managed)
3. Criar XFSmartComboBox<T> : XFInputBaseWithInputGroups<string>
4. XFSmartComboBox: [Parameter, EditorRequired] string Url (semantic search endpoint)
5. XFSmartComboBox: [Parameter] int MaxSuggestions = 10
6. XFSmartComboBox: [Parameter] float SimilarityThreshold = 0.5f
7. Renderizar SmartComboBox MS internamente com AddAttribute(Url, MaxSuggestions, etc)
8. Criar XFSmartPasteButton (button que analisa clipboard + popula form fields)
9. Criar XFSmartTextArea (text area com AI autocomplete)
10. Pattern: BuildRenderInputCore abre componente MS com builder.AddComponent<MS_Smart_Comp>()
11. Herdar IInputWithPlaceholder, IInputWithSize, IInputWithLabelType (T066)
12. Adicionar TryParseValueFromString = throw new NotSupportedException (value via MS internamente)

## Arquivos
- Criar: src/XForge.DesignSystem.AI.Smart/XFSmartComboBox.cs
- Criar: src/XForge.DesignSystem.AI.Smart/XFSmartPasteButton.cs
- Criar: src/XForge.DesignSystem.AI.Smart/XFSmartTextArea.cs
- Criar: src/XForge.DesignSystem.AI.Smart/XForge.DesignSystem.AI.Smart.csproj
- Modificar: src/XForge.DesignSystem/XForge.DesignSystem.csproj (add SmartComponents ref if needed)

## Criterios de Aceite
- [ ] XFSmartComboBox renderiza SmartComboBox MS com Hx bootstrap styling
- [ ] XFSmartPasteButton invoca AI endpoint on click
- [ ] XFSmartTextArea mostra autocomplete suggestions
- [ ] 3 componentes herdam de XFInputBaseWithInputGroups
- [ ] Build 0 warnings (SmartComponents preview nao dispara warning)
