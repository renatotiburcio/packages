---
name: accessibility-a11y-expert
description: Expert em acessibilidade web: WCAG 2.1, ARIA, navegação por teclado, screen readers.
metadata:
  version: "7.0.0"
  xforge-category: "testing"
---

# accessibility-a11y-expert

## Objetivo

Garantir acessibilidade WCAG 2.1 AA em interfaces web.

## Níveis WCAG

| Nível | Critério | Obrigatório |
|:-----:|----------|:-----------:|
| A | Mínimo | Sim |
| AA | Aceitável | Sim (padrão) |
| AAA | Superior | Opcional |

## Checklist

### Perceptível
- [ ] Texto alternativo para imagens
- [ ] Legendas para áudio/vídeo
- [ ] Contraste ≥ 4.5:1 (texto), ≥ 3:1 (grande)
- [ ] Conteúdo não depende apenas de cor

### Operável
- [ ] Navegação por teclado (Tab, Enter, Escape)
- [ ] Skip links
- [ ] Focus visible
- [ ] Sem trap de foco
- [ ] Tempo ajustável

### Compreensível
- [ ] Lang attribute definido
- [ ] Labels em todos os inputs
- [ ] Erros descritivos
- [ ] Consistente de navegação

### Robusto
- [ ] HTML válido
- [ ] ARIA roles corretos
- [ ] Names/labels para componentes

## Ferramentas

| Ferramenta | Uso |
|------------|-----|
| Axe-core | Auditoria automática |
| Lighthouse | Score de acessibilidade |
| NVDA/VoiceOver | Teste com screen reader |

## Procedimento

1. Rodar axe-core
2. Corrigir issues críticas
3. Testar com teclado
4. Testar com screen reader
5. Verificar contraste
6. Documentar exceções
