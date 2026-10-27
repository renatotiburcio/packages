---
id: T005
title: Localizar XML docs em 6 idiomas (en/pt-BR/es/zh/ja/ru)
priority: P0
status: pending
estimated_time: 24h
dependencies: nenhuma
---

# T005: Localizar XML docs em 6 idiomas

## Objetivo
Embarcar XML docs (IntelliSense tooltips) em 6 idiomas no NuGet package, copiando o padrao do antd-blazor.

## Contexto
antd-blazor embarca `LocalizedIntellisenseFiles/` em 6 culturas (en, ja-JP, ko-KR, ru, zh-CN, zh-HK, zh-TW). Quando o consumer usa VS em outra cultura, ele ve a doc traduzida.

## Passos
1. Criar `src/XForge.DesignSystem/LocalizedIntellisenseFiles/en/*.xml` (copia do obj/Debug)
2. Criar `src/XForge.DesignSystem/LocalizedIntellisenseFiles/pt-BR/*.xml` (traduzir)
3. Criar `src/XForge.DesignSystem/LocalizedIntellisenseFiles/es/*.xml` (traduzir)
4. Criar `src/XForge.DesignSystem/LocalizedIntellisenseFiles/zh/*.xml` (traduzir)
5. Criar `src/XForge.DesignSystem/LocalizedIntellisenseFiles/ja/*.xml` (traduzir)
6. Criar `src/XForge.DesignSystem/LocalizedIntellisenseFiles/ru/*.xml` (traduzir)
7. Modificar csproj: `<DocumentationFile>` aponta para en version; `<Content Include>` embute os outros
8. Criar `src/XForge.DesignSystem/XForge.targets` (copia AntDesign.targets)
9. Criar pipeline de traducao automatica (pt-en, en-es) usando IA
10. Documentar processo em `.xforge/knowledge/domains/design-system/i18n-docs.md`

## Arquivos
- Criar: 6 pastas em `src/XForge.DesignSystem/LocalizedIntellisenseFiles/<culture>/`
- Criar: ~2500 XML files (152 components x 6 langs, ~16 lines cada)
- Modificar: `XForge.DesignSystem.csproj`
- Criar: `src/XForge.DesignSystem/XForge.targets`

## Criterios de Aceite
- [ ] 6 culturas com XML docs completos
- [ ] Package embute todas as culturas em /content/LocalizedIntellisenseFiles/
- [ ] XForge.targets copia para net8/net9/net10
- [ ] VS em pt-BR mostra docs em pt-BR
- [ ] Pipeline de traducao documentada

## Notas
- Padrao vem de antd-blazor (6 langs; maximo)
- Para XForge, foco em pt-BR (mercado alvo)
- Processo de traducao pode usar IA + revisao humana
- Custo estimado: 24h = 4h/lingua (automatizado) + 8h revisao humana
- Fator limitante: manter docs em sincronia com codigo
