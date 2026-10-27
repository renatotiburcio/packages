---
name: blazor-tailwind-v4-expert
description: Expert em Blazor com Tailwind CSS v4: configuração, componentes, responsividade e design system.
metadata:
  version: "7.0.0"
  xforge-category: "technical-expert"
---

# blazor-twind-v4-expert

## Objetivo

Criar interfaces Blazor modernas com Tailwind CSS v4.

## Configuração

### Tailwind v4
```css
/* app.css */
@import "tailwindcss";
```

### Package
```xml
<PackageReference Include="TailwindCSS" Version="4.*" />
```

## Padrões de Componente

### Componente Base
```razor
<div class="@CssClass">
    @ChildContent
</div>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string CssClass { get; set; } = "";
}
```

### Formulário
```razor
<div class="space-y-4">
    <div>
        <label class="block text-sm font-medium text-gray-700">Nome</label>
        <input type="text" @bind="Model.Name"
               class="mt-1 block w-full rounded-md border-gray-300 shadow-sm
                      focus:border-blue-500 focus:ring-blue-500" />
    </div>
</div>
```

### Tabela
```razor
<table class="min-w-full divide-y divide-gray-200">
    <thead class="bg-gray-50">
        <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Nome</th>
        </tr>
    </thead>
    <tbody class="bg-white divide-y divide-gray-200">
        @foreach (var item in Items)
        {
            <tr>
                <td class="px-6 py-4 whitespace-nowrap">@item.Name</td>
            </tr>
        }
    </tbody>
</table>
```

## Responsividade

| Breakpoint | Prefixo | Exemplo |
|------------|---------|---------|
| Mobile | (none) | `grid-cols-1` |
| Tablet | `sm:` | `sm:grid-cols-2` |
| Desktop | `md:` | `md:grid-cols-3` |
| Large | `lg:` | `lg:grid-cols-4` |

## Regras

- Mobile-first sempre
- Componentes < 200 linhas
- Sem CSS inline
- Design system consistente
- Acessibilidade (ARIA labels)
