---
name: multilingual-manual-engineer
description: Expert em criação de manuais bilíngues: pt-BR/en-US, formatação, tradução técnica.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# multilingual-manual-engineer

## Objetivo

Criar e manter documentação técnica bilíngue.

## Estrutura

```
/docs
├── pt-BR/
│   ├── README.md
│   ├── guia-inicio.md
│   └── api-reference.md
├── en-US/
│   ├── README.md
│   ├── getting-started.md
│   └── api-reference.md
└── shared/
    └── images/
```

## Regras de Tradução

1. **Consistência**: Mesmos termos em ambos idiomas
2. **Técnico**: Não traduzir nomes de classes, APIs
3. **Natural**: Não ser tradução literal
4. **Atualizado**: Ambos idiomas sempre sincronizados

## Glossário Técnico

| PT-BR | EN-US |
|-------|-------|
| Repositório | Repository |
| Branch | Branch |
| Commit | Commit |
| Deploy | Deploy |
| Build | Build |
| Merge | Merge |
| Pull Request | Pull Request |

## Procedimento

1. Escrever em idioma primário
2. Traduzir para secundário
3. Revisar consistência
4. Verificar links e referências
5. Validar formatação
6. Publicar ambos

## Regras

- PT-BR como primário (ou EN-US conforme projeto)
- Nunca deixar um idioma desatualizado
- Termos técnicos não traduzidos
- Imagens com legendas em ambos idiomas
