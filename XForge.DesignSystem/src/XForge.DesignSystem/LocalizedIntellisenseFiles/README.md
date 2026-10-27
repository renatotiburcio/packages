# Localized Intellisense (T005)

XForge.DesignSystem embeds XML docs in 6 cultures for localized IntelliSense tooltips.

## Cultures supported
- en (default)
- pt-BR
- es
- zh
- ja
- ru

## Build behavior
`XForge.targets` copies `LocalizedIntellisenseFiles/<culture>/*.xml` to `bin/<config>/<tfm>/LocalizedIntellisenseFiles/<culture>/`.

## Why antd-blazor chose this pattern
- Allows Visual Studio to show localized tooltips based on user culture
- Zero consumer configuration required
- Package size impact is minimal (<2MB for 6 langs)

## Reference
- T005 in antd-blazor-comparison BACKLOG
- antd-blazor: 6 langs (en, ja-JP, ko-KR, ru, zh-CN, zh-HK, zh-TW)
- XForge: 6 langs (en, pt-BR, es, zh, ja, ru) - aligned with LATAM market