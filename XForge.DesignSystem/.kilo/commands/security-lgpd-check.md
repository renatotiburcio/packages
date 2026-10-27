---
description: Executa o scanner LGPD automatizado, verifica dados pessoais, criptografia, endpoints de direitos do titular e consentimento.
agent: code
---

# /security-lgpd-check

## Objetivo

Executa o scanner LGPD automatizado (.xforge/automation/scripts/lgpd_scanner.py) para verificar conformidade com a Lei Geral de Protecao de Dados.

## Procedimento

1. O scanner percorre o projeto analisando arquivos `.cs`, `.ts`, `.js`, `.py`, `.md`, `.json`
2. Identifica campos de dados pessoais (CPF, RG, email, telefone, endereco, CEP, data de nascimento, genero, raca, saude, biometria, religiao)
3. Verifica se campos sensiveis possuem criptografia ou protecao
4. Checa se os comandos `.kilo/commands/` incluem endpoints de direitos do titular (consulta, exclusao, alteracao, consentimento, portabilidade)
5. Verifica se DTOs/Requests/Commands com dados pessoais possuem mecanismo de consentimento

## Saida

Relatorio JSON contendo:
- **summary**: contagem de achados e score geral (PASS / WARN / FAIL)
- **findings**: dados pessoais encontrados, problemas de criptografia, endpoints ausentes, inconsistencias de consentimento
- **recommendations**: acoes corretivas sugeridas

## Execucao manual

```powershell
python .xforge\automation\scripts\lgpd_scanner.py .
```

## Exit codes

- 0: PASS — nenhum problema critico
- 1: WARN — dados pessoais encontrados, revisao recomendada
- 2: FAIL — falha critica de conformidade

## Bloqueia release

- FAIL ou WARN nao resolvido
- Dado pessoal sensivel sem criptografia
- Endpoint de direito do titular ausente
- DTO com dado pessoal sem consentimento

## Exemplos

```
/security-lgpd-check
/security-lgpd-check --scan-only
```
