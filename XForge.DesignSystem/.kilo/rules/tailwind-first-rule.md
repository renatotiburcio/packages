# tailwind-first-rule

## Contexto
O XForge DesignSystem é um kit de componentes Blazor que usa **Tailwind v4** como fonte primária de estilização. Esta regra define como o CSS deve ser escrito e o que fazer quando o Tailwind não atender.

## Objetivo
Garantir que **100% dos componentes** sejam construídos com classes Tailwind, reservando CSS customizado apenas para casos em que o Tailwind é estruturalmente incapaz de resolver.

## Regra Fundamental

**TAILWIND FIRST — SEMPRE.**

- Cada componente deve funcionar **apenas com classes Tailwind** para layout, espaçamento, cores, tipografia, estados e responsividade.
- Classes de variante/size/state geradas em C# devem ser **tokens Tailwind válidos** (ex: `bg-primary`, `text-xs`, `rounded-md`), não classes inventadas sem definição.
- O design deve ser **tema-agnostic**: usar apenas tokens CSS customizados que existem no tema (`var(--color-primary)`, `var(--color-bg-surface)`, etc.).

## Arquitetura de Tokens (OBRIGATÓRIO)

O DesignSystem é 100% auto-contido. O consumidor só precisa referenciar `xf-designsystem.css`.

| Arquivo | Responsabilidade |
|---------|------------------|
| `XForge.DesignSystem/wwwroot/css/xf-designsystem.css` | Tema completo: light + dark + 8 brand overrides. Single source of truth para cores. |
| `app.css` (projeto consumidor) | `@theme` do Tailwind gera utilitários (`bg-success`, `text-danger`) |

### Data Attributes
| Atributo | Efeito |
|----------|--------|
| nenhum | light + primária padrão #6366f1 |
| `data-theme="dark"` | dark mode |
| `data-brand="brand-XXX"` | **só** sobrescreve `--color-primary-*` |
| `data-theme="dark"` `data-brand="brand-forms"` | combined |

### Brand Strategy
- Brand override **nunca** duplica todos os tokens — apenas primária muda
- Cores semânticas (success, warning, danger, info, surfaces, borders, text) são **idênticas** entre todos os brands
- Usuário pode sobrescrever CSS vars manualmente para custom total


---

## Quando usar CSS Customizado (Únicas Exceções)

Quando o Tailwind v4 **não conseguir** gerar o estilo necessário:

1. **Tokens sem classe utilitária equivalente** (ex: `--color-primary-contrast` não tem utilitário tailwind)
2. **CSS custom properties que fazem blended colors em runtime** (`color-mix(in oklab, var(--x) Y%, transparent)`)
3. **Keyframes / animações complexas** que `@theme` não cobre (ex: `progress-stripe`, `skeleton-shimmer`)
4. **Estilos estruturais não-JS-dependentes** (ex: `appearance: base-select`, scrollbar thin)
5. **Pseudo-elementos complexos** (ex: `::picker(select)`, `::checkmark`)
6. **Reset de terceiro** (ex: scrollbar global, SVG display block)

---

## Hierarquia de Decisão

```
Preciso de estilo
    │
    ├─ Tailwind resolve? → Usar classe tailwind diretamente
    │   ├─ bg-primary, text-xs, rounded-lg, shadow-*, ring-*
    │   ├─ hover:bg-[var(--color-primary-hover)]  (arbitrary value OK)
    │   └─ data-[state=active]:*  (data attribute variant OK)
    │
    ├─ Tailwind NÃO resolve, mas é cor tokenizada?
    │   └─ Usar arbitrary value: bg-[var(--color-success)]
    │
    ├─ É animação / keyframe / pseudo-elemento / reset global?
    │   └─ CSS customizado em xf-designsystem.css
    │
    └─ É atômico + reutilizável + combina com tema?
        └─ Criar @utility no xf-designsystem.css
```

---

## Obrigatório

1. **Todo componente começa Tailwind-first**: montar o markup com classes utilitárias ANTES de considerar CSS customizado.
2. **Arbitrary values são permitidos** para tokens CSS: `bg-[var(--color-primary)]`, `text-[var(--color-text-primary)]`, `pl-[calc(var(--depth)*1rem)]`.
3. **Hover/focus/active padrão** sempre com variantes Tailwind: `hover:bg-primary-hover`, `focus-visible:ring-2 focus-visible:ring-border-focus`.
4. **Arbitrary properties** para valores dinâmicos: `bg-[var(--tone)]` quando vem de parâmetro de componente.
5. **Variantes custom** com `@variant` ou `&:where()` em xf-designsystem.css para encadeamento complexo.
6. **Tokens no @theme** do app.css são fonte canônica de cores/tipografia; tokens em xf-designsystem.css estendem o tema.
7. **Sem classes �rfãs**: toda classe `xf-*` ou `xf-component-X` usada em C# deve existir ou como `@utility` ou como classe base com estilo mínimo (layout/positioning) — mas os estilos visuais vêm do Tailwind.

---

## Proibido

- ❌ Tailwind utility classes hard-coded no C# com valores fixos que deviam ser tokens (ex: `bg-[#6366f1]` ao invés de `bg-primary`)
- ❌ Inventar classes sem utilitário ou definição CSS correspondente (ex: `xf-card--elevated` sem estilo)
- ❌ Usar `style="..."` inline inline inline inline inline inline inline com valores que podem ser Tailwind (ex: style="margin-top: 8px" ao invés de `mt-2`)
- ❌ BaseClass definido sem estilo mínimo para resetar/user-agent-styles (ex: `xf-button` precisa de `border-0`, `bg-transparent` ou similar)
- ❌ Duplicar tokens: se `--color-primary` existe, não usar `--xf-color-acept` para a mesma coisa
- ❌ Cor fixa `#hex` em xf-designsystem.css (exceto tokens em `@theme`)
- ❌ Animação inline no C# keyframe sem definição CSS no xf-designsystem.css

---

## Exemplo: Aplicação Correta

**ANTES (errado — color-mix hard-coded em C#):**
```csharp
// NÃO FAÇA ISSO
XFButtonColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger"
```

**DEPOIS (correto — @utility + token Tailwind):**
```css
/* Em xf-designsystem.css */
@utility xf-tone-solid {
  background-color: var(--color-primary);
  color: var(--color-primary-contrast);
  border-color: var(--color-primary);
}
@utility xf-tone-soft {
  background-color: color-mix(in oklab, var(--color-primary) 10%, transparent);
  color: var(--color-primary);
}
```
```csharp
// Em XFCard.razor.cs
Color == XFCardColor.Danger
  ? "xf-tone-solid bg-danger border-danger text-white"
  : "xf-tone-soft bg-primary/10 text-primary"
```

---

## Exceção: XFButton e XFBadge
Estes componentes usam `color-mix()` em C# inline. **Migrar** gradualmente para `@utility` representa conformidade total com esta regra.

---

## Manutenção
- Sempre que um novo componente for criado, testá-lo **sem xf-designsystem.css** — se quebrar visualmente, algo não é Tailwind puro
- xf-designsystem.css deve ser o mais fino possível
- Aumento em xf-designsystem.css = falha em usar Tailwind correctly

---

## Quando Usar Esta Regra
- criar ou modificar componentes Blazor
- no xf-designsystem.css
- revisar markup razor para classes Tailwind
- projetar tokens de tema (@theme em app.css vs CSS vars em xf-designsystem.css)

## Quando NÃO Usar
- tela legada que usa Bootstrap/Material (rever separadamente)
- CSS reset de browser (ex: scrollbar, picker comportamento nativo)
- Classes puramente estruturais JS-dependentes (ex: `data-open`)
