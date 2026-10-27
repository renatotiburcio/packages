# Knowledge Engine v1

## Objetivo

Transformar arquivos, URLs, repositórios, concorrentes, fontes oficiais, legados e incidentes em conhecimento organizado.

## Fontes suportadas

- arquivos locais;
- PDFs;
- DOCX;
- XLSX;
- CSV;
- Markdown;
- URLs;
- GitHub;
- GitLab;
- Azure DevOps;
- sistemas legados;
- Engineers antigos;
- tickets de suporte;
- incidentes de produção;
- concorrentes.

## Estrutura

```text
.xforge/knowledge/
├── official/
├── competitors/
├── local/
├── repositories/
├── fiscal/
├── accounting/
├── commercial/
├── hr/
├── architecture/
├── ux/
├── security/
├── indexed/
└── packs/
```

## Fluxo

```text
fonte
→ classificação
→ trust score
→ resumo operacional
→ regras extraídas
→ edge cases
→ pitfalls
→ experts impactados
→ knowledge pack
→ memory update
```
