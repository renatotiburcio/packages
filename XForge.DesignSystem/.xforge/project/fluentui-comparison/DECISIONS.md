# Decisões Técnicas — FluentUI Blazor (6-way)

## D026: Icon Catalog Strategy (1 class per SVG)
- Contexto: 3698 Font Awesome icons como 1 Dictionary flat; precisa variant×size
- Opcoes: (a) 1 Dictionary<string,string> flat (atual), (b) Dictionary<(variant,size),string>, (c) 1 class tipada por SVG (FluentUI)
- Escolha: (c) 1 class tipada
- Rationale: AOT-friendly (sem reflection), IntelliSense, trimmable, type-safe
- Tradeoffs: Bundle size maior (mas truncable), 3698 arquivos (mas gerados)

## D027: Icon Variant/Size Model
- Contexto: XForge tem 1 size fixo (Font Awesome viewBox 0 0 X 512); FluentUI tem 4 variants × 8 sizes
- Opcoes: (a) Manter FA único viewBox, (b) Adotar variants Regular/Filled/Light/Color, (c) Size variants 16/20/24/32
- Escolha: (b) + (c) — 4 variants × 8 sizes = 32 combinacoes
- Rationale: Necessario para product polish (Filled em hover, Color para success/error)
- Tradeoffs: Aumento significativo de icones (3698 → ~14792 classes), requer Generator

## D028: Icon Distribution Strategy
- Contexto: XForge tem 1 arquivo gigante (3.87 MB, 4013 linhas)
- Opcoes: (a) Single dictionary, (b) One class per file (FluentUI), (c) Embedded resource (.json/.resx)
- Escolha: (b) — 1 class por SVG
- Rationale: Generator tool (AssetsGenerator) gera arquivos, versionados, AOT-friendly
- Tradeoffs: Files no repo (mas auto-generated), complexidade de build

## D029: Emoji Compression
- Contexto: Emojis SVG são 5-20 KB; bundle cresceria muito
- Opcoes: (a) SVG inline (sem compressao), (b) GZip byte[] (FluentUI), (c) WebP image, (d) Web font
- Escolha: (b) GZip byte[]
- Rationale: Reduz 70-80% do tamanho, lazy decode nativo, Blazor-friendly
- Tradeoffs: GZip overhead (mas cached), requer helper (EmojiCompress)

## D030: Icon Lookup Mechanism
- Contexto: FluentUI usa reflection (quebra AOT); precisa ser AOT-friendly no XForge
- Opcoes: (a) Reflection (FluentUI), (b) DI registry (T084), (c) Source Generator (T089), (d) Static Dictionary global
- Escolha: (c) Source Generator [FluentUIComponent] attribute + class discovery
- Rationale: AOT-friendly zero reflection, build-time, IntelliSense preserved
- Tradeoffs: SG complexity, build time increase, requires more tooling

## D031: Color Token Strategy
- Contexto: FluentUI usa Figma tokens; XForge tem 15 design token enums
- Opcoes: (a) Manter enums (atual), (b) Figma JSON + Roslyn SG, (c) CSS custom properties + JS interop
- Escolha: (b) Figma JSON + Roslyn SG (DesignToken.Generator)
- Rationale: Single source of truth (Figma), auto-generated tokens, no drift
- Tradeoffs: Build dependency on Figma sync, SG complexity

## D032: Icon Color Customization
- Contexto: FluentUI Icon tem Color enum (Accent/Lightweight/Custom); XForge usa fill="currentColor" simples
- Opcoes: (a) currentColor apenas, (b) FluentColor enum, (c) Custom hex string
- Escolha: (b) + (c) — enum padrao + custom
- Rationale: Consistencia visual (Accent/Success/Warning/Error/Important), custom para temas
- Tradeoffs: 5 cores enum + custom prop, mais codigo

## D033: IconSize Strategy
- Contexto: FluentUI tem 8 sizes (10/12/16/20/24/28/32/48); XForge tem viewBox fixo 0 0 X 512
- Opcoes: (a) Manter 1 size, (b) Adotar 8 sizes, (c) Custom size
- Escolha: (b) + (c) — 8 sizes + custom
- Rationale: Toolbar(16), Body(20), Card(24), Hero(32), AppBar(48) - todos os contextos
- Tradeoffs: 8x mais icones (mas cada um é um SVG path), generators mais complexos

## D034: Emoji Skintone Support
- Contexto: Unicode CLDR tem 5 skin tones para PeopleBody
- Opcoes: (a) Apenas Default, (b) 6 skintones (Default + Light/MediumLight/Medium/MediumDark/Dark)
- Escolha: (b) 6 skintones
- Rationale: Acessibilidade + representatividade (requisito LATAM)
- Tradeoffs: 5x mais emojis para PeopleBody (~750 vs 150)

## D035: Generator Tool Reusability
- Contexto: AssetsGenerator é CLI que parseia SVG → C#; pode reusar para XForge
- Opcoes: (a) Fork completo, (b) Adaptar para nosso namespace XForge, (c) Re-implementar
- Escolha: (b) Adaptar (criar fork interno)
- Rationale: Microsoft.Extensions.Configuration + CLI args pattern é generico, copy-paste com namespace change
- Tradeoffs: Manter 2 codebases (mas isoladas), ou fork MIT-licensed (permissivel)

## D036: Icon Picker UI Component
- Contexto: XForge demo precisa de Icon picker (search + variant + size)
- Opcoes: (a) XFSelector generico, (b) XFIconPicker dedicado, (c) MudBlazor/Mud equivalent
- Escolha: (b) XFIconPicker dedicado (criar)
- Rationale: UX otimizada para icones (grid + search + variant switch), reusavel
- Tradeoffs: 1 componente novo (~300 linhas), valor alto

## D037: Emoji as Bot Variant
- Contexto: AI components (T060, T077) podem usar emojis
- Opcoes: (a) Emoji picker padrao, (b) Emoji shortcode parser (:rocket: → 🚀)
- Escolha: (b) Shortcode parser + emoji picker hibrido
- Rationale: UX natural (typing :rocket: inline), GitHub-like
- Tradeoffs: Parser ~50 linhas, dicionario ~500 shortcodes (inline)

## D038: Icon Bundle Trimming
- Contexto: 21034 icones × 4 variants × 8 sizes = bundle enorme
- Opcoes: (a) Bundle all (FluentUI), (b) Per-component registry (T087), (c) Just-in-time load
- Escolha: (b) Per-component registry via [FluentUIComponent] attribute
- Rationale: Compilador sabe quais icones cada componente usa, gera registro minimo
- Tradeoffs: SG complexidade, build time, mas bundle 10x menor


## D039: SVG Comment Stripping (License Hygiene)
- Contexto: SVGs do Font Awesome Pro 7.1.0 vem com comentario <!--! Font Awesome Pro 7.1.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2025 Fonticons, Inc. --> embutido. Esse comentario NAO pode vazar no bundle distribuido do XForge (XForge e MIT, FA Pro e Commercial).
- Opcoes: (a) Manter comentario (vaza license FA Pro), (b) Strip todos comentarios <!--.*?--> (perde acessibilidade se houver <title> em comentario), (c) Strip comentarios MAS preservar <title>/<desc> como elementos
- Escolha: (c) Strip comentarios HTML/XML MAS preservar <title>/<desc> como elementos proprios (W3C SVG accessibility pattern)
- Rationale:
  - Compliance: MIT bundle NAO pode conter referencia a Commercial License (Font Awesome Pro 7.1.0)
  - Compliance: Font Awesome Pro 7.1.0 e restrito; embedar metadata viola license
  - Acessibilidade: SVG W3C recomenda <title>/<desc> como elementos (NAO comentarios) para screen readers
  - Legal: Copyright 2025 Fonticons, Inc. nao pode aparecer em derivado MIT
  - Tamnho: cada comentario FA = ~200-300 bytes × 3698 = ~1MB economizado
- Tradeoffs:
  - Regex precisa ser cuidadosa (multi-line, non-greedy, nao conflitar com < no path data)
  - Validacao automatica necessaria: Assert_No_SVG_Comments + Assert_No_External_License_Traces
  - Padrao self-healing: SH-013 aplica strip automaticamente em qualquer .svg no repo
- Implementacao:
  - Local: T078 passo 5a (refactor IconCatalog)
  - Validador: T078 criterios de aceite (novos)
  - Generic: T097 SVG optimizer (svgo integration) ja cobre
  - Self-healing: SH-013 (rules/self-healing-rules.md)
- Referencias:
  - FA example: <!--! Font Awesome Pro 7.1.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2025 Fonticons, Inc. -->
  - W3C SVG Accessibility: https://www.w3.org/TR/SVG-access/
  - XForge license: MIT (CONFIRMAR: NAO pode conter FA Pro metadata)
