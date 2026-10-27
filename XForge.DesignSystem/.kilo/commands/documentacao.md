---
description: Gerar e manter documentação: manuais, docs HTML, changelog, ADRs.
agent: code
---

# /documentacao

## Categoria

produção

## Objetivo

Criar, atualizar e manter documentação do projeto.

## Sub-comandos

| Sub-comando | Foco |
|-------------|------|
| `/documentacao api` | Docs de API (OpenAPI/Swagger) |
| `/documentacao readme` | README do projeto |
| `/documentacao changelog` | CHANGELOG de versão |
| `/documentacao adr` | Architecture Decision Record |
| `/documentacao manual` | Manual do usuário |
| `/documentacao setup` | Guia de setup |

## Procedimento

### API Docs
1. Verificar OpenAPI/Swagger configurado
2. Gerar specs a partir de endpoints
3. Incluir exemplos de request/response
4. Documentar erros comuns

### README
1. Nome + descrição do projeto
2. Pré-requisitos
3. Setup (passo-a-passo)
4. Uso (exemplos)
5. Contribuição
6. Licença

### CHANGELOG
1. Seguir Keep a Changelog
2. Categorias: Added, Changed, Fixed, Removed
3. Versão semântica (semver)

### ADR
1. Contexto: por que a decisão
2. Decisão: o que foi decidido
3. Consequências: o que muda
4. Status: proposto / aceito / deprecado

## Regras

- Documentação deve ser concisa e acionável
- Código > docs (código é a fonte da verdade)
- Docs desatualizadas são pior que nenhuma doc
- Sempre incluir exemplos de uso

## Exemplos

```
/documentacao
/documentacao gerar docs da API de pagamentos
/documentacao criar ADR para migração de banco
/documentacao atualizar CHANGELOG da v2.1.0
/documentacao criar guia de setup do projeto
```
