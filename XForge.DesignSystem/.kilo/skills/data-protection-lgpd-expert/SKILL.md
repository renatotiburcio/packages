---
name: data-protection-lgpd-expert
description: Expert em proteção de dados, LGPD, criptografia at rest/in transit, pseudonimização, consentimento e direitos do titular.
metadata:
  version: "37.0.0"
  xforge-category: "continuous-security"
---

# data-protection-lgpd-expert

## Objetivo

Garantir conformidade com LGPD e boas práticas de proteção de dados.

## Checklist LGPD

### Classificação de Dados
- [ ] Dados pessoais mapeados em todos os fluxos
- [ ] Dados sensíveis identificados (art. 5, II)
- [ ] Base legal definida para cada tratamento
- [ ] Dados classificados: público / interno / confidencial / restrito

### Criptografia
- [ ] Em trânsito: TLS 1.2+ obrigatório
- [ ] Em repouso: AES-256 ou equivalente
- [ ] Keys gerenciadas em vault (NUNCA no código)
- [ ] Rotação de keys programada

### Consentimento
- [ ] Consentimento explícito antes de coleta
- [ ] Granular (finalidades separadas)
- [ ] Registro de consentimento com timestamp
- [ ] Facilidade de revogação

### Direitos do Titular
- [ ] Acesso aos dados (art. 18, I)
- [ ] Correção (art. 18, II)
- [ ] Anonimização/bloqueio/exclusão (art. 18, III)
- [ ] Portabilidade (art. 18, V)
- [ ] Revogação de consentimento (art. 18, IX)

### Pseudonimização
- [ ] Dados sensíveis pseudonimizados em dev/test
- [ ] Tokens irrevogáveis para dados de treino
- [ ] Mapeamento token→real em vault separado

## Procedimento

1. Mapear fluxo de dados do projeto
2. Identificar dados pessoais e sensíveis
3. Verificar base legal para cada tratamento
4. Aplicar checklist LGPD
5. Classificar achados por risco
6. EXIGIR validação humana para questões legais
7. Documentar em .xforge/decisions/
