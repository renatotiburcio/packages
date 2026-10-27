# Guia: Como adicionar seu proprio icone (T096)

Este guia mostra como adicionar um icone custom ao XForge Design System.

## 1. Estrutura do icon system

XForge organiza icones em tres dimensoes:

- **Variant**: Regular, Filled, Light, Color
- **Size**: 10, 12, 16, 20, 24, 28, 32, 48 pixels
- **Keywords**: 3-5 palavras-chave para busca

Cada icone e identificado pela tripla (Name, Variant, Size).

## 2. Onde colocar o SVG

Coloque em `tools/XForge.AssetsGenerator/Icon.svgs/<Variant>/<Size>/`.

## 3. Convencoes de naming

`{name}_{size}_{variant}.svg`

Exemplos: `add_20_Regular.svg`, `home_24_Filled.svg`.

## 4. Como usar o AssetsGenerator CLI

```bash
xforge-assets-gen --assets ./Icon.svgs --target ./Generated --library icon --names "mylogo,home" --sizes 20,24
```

Saida: arquivos `Regular_20.cs` e `Regular_24.cs` em `./Generated/`.

## 5. Adicionar keywords e category (metadata.json)

Crie `Icon.svgs/metadata.json` com:

```json
{ "mylogo": { "keywords": ["logo", "brand"], "category": "branding" } }
```

## 6. Usar em componente

```razor
<XFFluentIcon Value="@Icons.Custom.MyLogo" Variant="Regular" Size="Size20" />
```

## 7. Custom registry (DI override)

```csharp
services.AddXFIconRegistry();
services.PostConfigure<IXFIconRegistry>(reg => reg.Register(...));
```

## 8. Color tokens (var(--xf-color-*))

Use Color="Custom" + CustomColor para cor livre.

## 9. Best practices

- SVG deve ter viewBox definido.
- Remova metadados comerciais (SH-013 / SvgContentSanitizer).
- Use <title> no SVG para acessibilidade.
- Otimize com svgo (T097) para reduzir ate 60% do tamanho.

## 10. Troubleshooting

- **Build error CS0103**: icone nao registrado. Rode o AssetsGenerator.
- **AOT warning RequiresUnreferencedCode**: use IXFIconRegistry.Get.
- **Bundle muito grande**: ative o trimmer (T087).

---

Tarefas relacionadas: T078, T080, T084, T087.