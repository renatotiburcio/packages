# Backlog — FluentUI Blazor vs XForge Design System (6-way)

Gerado em: 2026-06-13
Fontes analisadas: 6 (MudBlazor + antd-blazor + Blazorise + Radzen + Havit + FluentUI Blazor)
Foco especial: `Microsoft.FluentUI.AspNetCore.Components.Icons` + `Microsoft.FluentUI.AspNetCore.Components.Emoji`

## P0 — Crítico (fazer primeiro)
| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T078 | done | XF SvgIcon IconCatalog refactor (variant+size+keyword) | 16h | nenhuma |
| T079 | done | XF Emoji catalog com compressão GZip | 12h | T078 | done |
| T080 | done | XF Icons Generator (CLI .NET tools/AssetsGenerator) | 16h | T078 | done |
| T081 | done | XFFluentIcon.razor<TIcon> generic component (variant+size) | 8h | T078 | done |

## P1 — Importante
| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T082 | XFFluentEmoji.razor<TGroup,TStyle,TSkintone,Tone> generic | 6h | T079 | done |
| T083 | done | Color tokens em ICONS (TokenStyle-aware CSS) | 8h | T078, T001 |
| T084 | done | Lookup via DI registry + Json catalog (no AOT) | 12h | T078 | done |
| T085 | done | Icon picker UI component (XFSelector com search) | 8h | T078, T020 |
| T086 | done | Emoji shortcode parser (:rocket: → 🚀) | 4h | T079 | done |
| T087 | done | Per-component icon registry + bundle trimmable | 6h | T078 | done |
| T088 | done | Icon cache + LRU por assembly | 4h | T078 | done |
| T089 | done | [FluentUIComponent] attribute → class discovery | 4h | T080 | done |

## P2 — Desejável
| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T090 | done | Icon color picker (accent/lightweight/custom) | 4h | T081 | done |
| T091 | done | Skin tone picker para emojis de PeopleBody | 4h | T082 |
| T092 | done | Auto-classify icons por keyword (Category inferred) | 4h | T078 | done |
| T093 | done | FluentUI → Tailwind class mapping tokens | 4h | T083 | done |
| T094 | done | Emoji as inline icon (data URI inline) | 2h | T079 | done |
| T095 | done | Icon inspector demo page (com search) | 4h | T085 | done |
| T096 | done | Docs: guia "Como adicionar seu próprio ícone" | 2h | T078, T080 |

## P3 — Baixa prioridade
| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T097 | done | SVG optimizer (svgo integration) | 4h | T080 | done |
| T098 | done | Animated icons (CSS/SMIL) | 6h | T078 | done |
| T099 | done | Brand badge icons (8 marcas comuns) | 4h | T078 | done |
| T100 | done | Icon usage analytics in demo | 2h | T085 | done |
| T101 | done | Emoji as Bot (chat UI) variant | 8h | T082, T060 |

## Resumo
- **Total: 24 tarefas** (P0: 4, P1: 8, P2: 7, P3: 5)
- **Estimativa total: ~160h** (P0: 52h, P1: 50h, P2: 26h, P3: 24h)
- **Tarefas sem dependência**: 4 (T078–T081)
- **Foco crítico**: T078–T084 (icon/emoji catalog) + T085 (picker UI)

## Foco Especial: Catálogo de Ícones

### Estado atual XForge
- 1 arquivo `XFSvgIcons.cs` (3.87 MB, 4013 linhas)
- 26 buckets alfabéticos (Dictionary<string,string>)
- 3698 ícones Font Awesome Pro 7.1.0 + 1 custom (agro-business)
- Sem variant (estilo), sem size fixo, sem cor semântica
- Sem categoria/palavras-chave (search impossível)
- Sem emoji catalog
- Sem emoji picker

### Estado FluentUI (referência)
- 21.034 ícones × 4 variants × 8 sizes = 32 combinações
- 7.885 emojis × 3 styles × 5 skintones × 3 sizes = 9 combinações
- Generator tool que parseia SVG e gera C# tipado
- 1 SVG = 1 class (AOT-friendly em teoria, mas lookup reflection quebra AOT)
- Compressão GZip para emojis (byte[] lazy decode)
- Color variants mantêm cor original (não monochrome)
- Skin tone para emojis de pessoas

### Plano de migração XForge → FluentUI-like

| Fase | Tarefa | Output |
|------|--------|--------|
| **F1 (S3)** | T078, T080, T081 | IconCatalog com variant×size, XFIcon picker |
| **F2 (S4)** | T079, T082, T086 | Emoji catalog comprimido, shortcode parser |
| **F3 (S5)** | T085, T090, T095 | UI components (pickers, inspector) |
| **F4 (S6)** | T097–T101 | Otimizações, animações, brand icons |

