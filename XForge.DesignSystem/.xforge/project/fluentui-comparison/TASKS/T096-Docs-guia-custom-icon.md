---
id: T096
title: Docs: guia "Como adicionar seu próprio ícone"
priority: P2
status: pending
estimated_time: 2h
dependencies: T078, T080
---

# T096: Docs: guia "Como adicionar seu próprio ícone"

## Objetivo
Documentacao completa passo-a-passo para usuarios adicionarem icones custom ao XForge Design System. Inclui Icon Catalog, AssetsGenerator, e exemplos de uso.

## Passos
1. Criar `docs/guides/add-custom-icon.md`
2. Secao 1: Estrutura do icon system (variant×size×keywords)
3. Secao 2: Onde colocar SVG (tools/XForge.AssetsGenerator/Icon.svgs/)
4. Secao 3: Convencoes de naming (`{name}_{size}_{variant}.svg`)
5. Secao 4: Como usar o AssetsGenerator CLI
6. Secao 5: Adicionar keywords e category (metadata.json)
7. Secao 6: Usar em componente (`<XFFluentIcon Value="@Icons.Custom.MyLogo">`)
8. Secao 7: Custom registry (DI override)
9. Secao 8: Color tokens (var(--xf-color-*))
10. Secao 9: Best practices (SVG optimization, accessibility, viewBox)
11. Secao 10: Troubleshooting (build errors, AOT, bundle size)
12. Exemplos: Hero icon, brand logo, custom illustration

## Arquivos
- Criar: `docs/guides/add-custom-icon.md`
- Criar: `docs/guides/add-custom-icon-assets/metadata.json` (sample)
- Criar: `docs/guides/add-custom-icon-assets/mylogo_24_regular.svg` (sample)
- Modificar: `docs/index.html` (add link to guide)

## Criterios de Aceite
- [ ] Guia passo-a-passo completo (10 secoes)
- [ ] 3 exemplos praticos (hero, brand, custom)
- [ ] CLI usage com output esperado
- [ ] Best practices section
- [ ] Troubleshooting guide
- [ ] Sample assets bundled
- [ ] Link no index.md
- [ ] Build: 0 erros, 0 warnings

## Notas
- Guia oficial para usuarios extenderem o icon system
- Demonstra T078, T080, T084
- Reduz tickets de suporte ("como adicionar icone?")
