# XForge Icon & Emoji Catalog Improvement Plan

**Phase**: 6 (FluentUI Blazor analysis)
**Foco**: Refatorar catalog de icones + criar catalog de emojis
**Data**: 2026-06-13
**Tasks**: T078-T094 (17 tasks) + T095-T101 (7 tasks) = 24 total

## 1. Diagnostico do Estado Atual (XForge)

### Icon Catalog (atual)
- Arquivo: `src/XForge.DesignSystem/Components/Icon/XFSvgIcons.cs`
- Tamanho: **3.87 MB**, **4013 linhas**
- Estrutura: 26 buckets alfabeticos (Dictionary<string,string> por letra)
- Total: **3698 icones Font Awesome Pro 7.1.0** + 1 custom (agro-business)
- 0 emojis (nao existe catalog de emojis)

### Problemas Identificados
1. Flat Dictionary (sem variant)
2. Sem size variants (8 sizes FluentUI vs 1 FA)
3. Sem keywords[] (search impossivel)
4. Sem category (filtro impossivel)
5. Sem metadata (group, style)
6. 1 arquivo monolitico (4k linhas)
7. Font Awesome Pro 7.1.0 (licensing restrito)
8. Sem bundle trimming (3.87 MB sempre)
9. Sem emoji catalog (gap vs FluentUI)
10. Sem emoji picker
11. Sem shortcode parser
12. Sem skin tones (a11y gap)

## 2. Referencia: FluentUI Blazor
- **21.034 icones** × 4 variants × 8 sizes
- **7.885 emojis** × 3 styles × 5 skintones × 3 sizes
- **45 projects** total
- **Custom tool**: AssetsGenerator (CLI .NET, SVG → C#)
- **GZip compression** para emojis
- ❌ Reflection lookup (quebra AOT)
- ❌ Bundle completo (sem trim)

## 3. Plano de Migracao XForge (4 fases)

### Fase F1: Icon Catalog Refactor (S3) - 52h
- T078 SvgIcon refactor (variant+size+keyword) 16h
- T080 Icons Generator CLI .NET 16h
- T081 XFFluentIcon.razor generic 8h
- T083 Color tokens 8h
- T093 FluentUI→Tailwind mapping 4h

### Fase F2: Emoji Catalog (S4) - 50h
- T079 Emoji catalog GZip 12h
- T082 XFFluentEmoji.razor generic 6h
- T084 DI registry (no AOT) 12h
- T085 Icon picker UI 8h
- T086 Emoji shortcode parser 4h
- T087 Per-component registry + trimmable 6h
- T094 Emoji as data URI inline 2h

### Fase F3: UI + Dev Tools (S5) - 18h
- T088 Icon cache LRU 4h
- T089 [FluentUIComponent] discovery SG 4h
- T090 Icon color picker UI 4h
- T092 Auto-classify icons 4h
- T095 Icon inspector demo page 4h
- T096 Docs: como adicionar icone 2h

### Fase F4: Advanced (S6) - 28h
- T091 Skin tone picker 4h
- T097 SVG optimizer svgo 4h
- T098 Animated icons CSS/SMIL 6h
- T099 Brand badge icons 4h
- T100 Icon usage analytics 2h
- T101 Emoji as Bot chat UI 8h

## 4. Distribuicao per-componente (vs full-bundle)

### FluentUI (❌ bundle huge ~30MB)
- Bundle: Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.dll
- Lookup: reflection (quebra AOT)

### XForge Proposto (✅ bundle minimo)
- Bundle: XForge.DesignSystem.Icons.Regular.dll (~500KB media)
- Lookup: DI registry, AOT-compatible
- Atributo: [FluentUIComponent(UsesIcons = [...])]
- Trimmer: drops icones nao usados

## 5. Naming Conventions SVG

Pattern FluentUI = Pattern XForge:
```
{name}_{size}_{variant}.svg
accessibility_32_filled.svg
home_24_regular.svg
```

Estrutura de pastas:
```
tools/XForge.AssetsGenerator/Icon.svgs/
├── Regular/Size{10,12,16,20,24,28,32,48}/
├── Filled/Size{...}/
├── Light/Size{...}/
└── Color/Size{...}/
```

## 6. CI/CD Pipeline (auto-generation)

```
1. Designer updates SVG in Figma
2. Figma Sync exports SVG → tools/Icon.svgs/
3. dotnet run --project tools/XForge.AssetsGenerator -- --assets ./Icon.svgs --target ./src/.../Icon
4. 3698 classes generated in src/.../Icon/{Variant}{Size}.cs
5. Roslyn SG XFIconRegistryGenerator emits registry (T087)
6. dotnet build + tests
7. Trimmer drops unused icons (WasmPublish)
8. Final bundle: 100-500 KB (vs 3.87 MB flat)
```

## 7. Emojis LATAM-Focused (200+ curated)

| Grupo | Top 5 |
|-------|-------|
| SmileysEmotion | 😀 😃 😄 😁 😆 😅 🤣 😂 🙂 🙃 😉 😊 |
| PeopleBody | 👍 👎 👌 ✌️ 🤞 🤝 👋 🙌 👐 🤲 |
| Symbols | ❤️ 🧡 💛 💚 💙 💜 🖤 🤍 🤎 💔 |
| FoodDrink | 🍎 🍊 🍋 🍌 🍉 🍇 🍓 🍈 🍒 🍑 |
| Activities | ⚽ 🏀 🏈 ⚾ 🥎 🎾 🏐 🏉 🎱 🪀 |
| TravelPlaces | 🚗 🚕 🚙 🚌 🚎 🏎 🚓 🚑 🚒 🚐 |
| Objects | 📱 💻 ⌨️ 🖥 🖨 🖱 🖲 🕹 🗜 💽 |
| Nature | 🌲 🌳 🌴 🌵 🌾 🌿 ☘️ 🍀 🍁 🍂 |
| Flags | 🇧🇷 🇦🇷 🇲🇽 🇨🇱 🇨🇴 🇵🇪 🇻🇪 🇺🇾 🇵🇾 🇧🇴 |

## 8. Shortcodes GitHub-style (500+)
:rocket: → 🚀 | :fire: → 🔥 | :heart: → ❤️ | :+1: → 👍 | :tada: → 🎉
:warning: → ⚠️ | :white_check_mark: → ✅ | :x: → ❌ | :bug: → 🐛 | :sparkles: → ✨

## 9. Backward Compatibility

```csharp
// v1.x (manter funcionando)
var svg = XFSvgIcons._A["agro-business"];

// v2.0.0 (recommended)
var icon = XFIconCatalog.Get("agro-business", XFIconVariant.Regular, XFIconSize.Size24);
var svg = icon.Content;

// Migration shim em XFSvgIcons.cs
public static class XFSvgIcons {
    public static string Get(string name) => XFIconCatalog.Get(name, XFIconVariant.Regular, XFIconSize.Size24)?.Content;
}
```

## 10. Performance Budget

| Metrica | Antes | Depois | Status |
|---------|------:|-------:|--------|
| Icon catalog file size | 3.87 MB | 500 KB (avg) | -87% |
| Bundle size (WASM) | 3.87 MB | 100-500 KB | -87-97% |
| Lookup time | O(1) Dict | O(1) Dict (AOT) | OK |
| AOT-compatible | N/A | YES (DI registry) | NEW |
| Build time | N/A | +5s (SG) | acceptable |

## 11. A11y

- aria-label automatico
- aria-hidden para decorativos
- Focusable (tabindex=0, role=button)
- Keyboard Enter/Space
- prefers-reduced-motion pauses animations
- 5 skin tones Unicode CLDR
- HighContrast style (FluentUI pattern)
- aria-live picker search

## 12. Documentation Plan

| Doc | Path |
|-----|------|
| Guia usuario | docs/guides/add-custom-icon.md (T096) |
| API reference | docs/api/icon-catalog.md |
| Emoji reference | docs/api/emoji-catalog.md |
| Design tokens | docs/design-tokens/icon-colors.md |
| Architecture | docs/architecture/icon-system.md |
| Performance | docs/performance/icon-bundle.md |
| A11y | docs/a11y/icons-emojis.md |

## 13. Riscos & Mitigacoes

| Risco | Prob | Impacto | Mitigacao |
|-------|------|---------|-----------|
| FA licensing | Media | Alto | Migrar para FluentUI System Icons (MIT) |
| Bundle regression | Media | Alto | Roslyn SG trim (T087) |
| AOT break | Baixa | Alto | DI registry (T084) |
| Build regression | Baixa | Medio | IIncrementalGenerator (T087) |
| Designer adoption | Alta | Baixo | Docs + samples |
| Backward compat | Media | Alto | XFSvgIcons shim |
| Emoji curation | Baixa | Baixo | A/B test |
| Skin tone | Baixa | Baixo | Default = Default |

## 14. Decisoes Finais

| Decision | Escolha | Motivo |
|----------|---------|--------|
| Icon source | FluentUI System Icons (MIT) | Sem licensing |
| Emoji source | FluentUI Emojis (MIT) | 7885 emojis, GZip |
| Variants | 4 (R/F/L/C) | FluentUI parity |
| Sizes | 8 (10/12/16/20/24/28/32/48) | Toolbar/Body/Card/Hero |
| Lookup | DI registry (T084) | AOT-friendly |
| Generator | CLI .NET (T080) | Fork do FluentUI.AssetsGenerator |
| Compression | GZip byte[] | 70-80% reducao |
| Trimming | [FluentUIComponent] SG | Bundle 10x menor |
| Colors | 6 (Accent/Lightweight/Success/Warning/Error/Important) | FluentUI parity |
| Backward compat | XFSvgIcons shim | v1.x → v2.0.0 smooth |
| Docs | Guia oficial (T096) | Self-service |


## 12. License Hygiene: SVG Comment Strip (CRITICO — D039)

### Contexto Legal
Os SVGs do **Font Awesome Pro 7.1.0** vem com comentario embutido:
```xml
<!--! Font Awesome Pro 7.1.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2025 Fonticons, Inc. -->
```

**XForge e MIT (open source). Font Awesome Pro 7.1.0 e Commercial (proprietario).**

Vazar essa metadata no bundle XForge == **violacao de license** (FA Pro probe redistribuicao sem授权授权 + exposicao indevida de copyright de terceiro).

### Regra Obrigatoria
> Todo SVG distribuido pelo XForge NAO pode conter comentario HTML/XML (`<!--...-->`).
> Elementos `<title>`/`<desc>` DEVEM ser preservados como elementos (NAO em comentarios) para acessibilidade W3C.

### Implementacao
- **Local primario**: T078 passo 6 + helper `SvgContentSanitizer.cs`
- **Local secundario**: T097 (SVG optimizer) ja remove comments como parte do svgo pipeline
- **Local defensivo**: SH-013 self-healing rule (auto-strip em qualquer `.svg` no repo)
- **Validacao**: testes `Assert_No_SVG_Comments` + `Assert_No_External_License_Traces` em IconCatalogTests

### Regex
```csharp
// CUIDADO: non-greedy + Singleline (multi-line)
private static readonly Regex SvgCommentRegex = new(
    @"<!--.*?-->",
    RegexOptions.Compiled | RegexOptions.Singleline);

// Edge case: nao conflitar com `<` em path data
// Edge case: preservar <?xml ... ?> processing instructions (NAO e comentario)
```

### Exceptions Permitidas
- `<title>` (W3C SVG accessibility — converte de comentario para elemento)
- `<desc>` (W3C SVG description)
- `<?xml ... ?>` (XML processing instruction, NAO e comentario HTML)

### Checklist por SVG
- [ ] Zero matches de `<!--.*?-->`
- [ ] Zero matches de `Font Awesome`, `@fontawesome`, `Fonticons`, `Commercial License`, `Copyright 20\d\d`, `fontawesome\.com`
- [ ] `<title>`/`<desc>` preservados como elementos (se aplicavel)
- [ ] Bundle final < 1MB para 3698 icones (vs 3.87 MB atual)
