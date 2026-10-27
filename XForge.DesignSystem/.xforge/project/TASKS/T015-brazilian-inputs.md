---
id: T015
title: Inputs BR — Cnpj, Cpf, Cep, Phone máscara e validação
priority: P1
status: pending
estimated_time: 2h
dependencies: T001
---

# T015: Inputs BR — Cnpj, Cpf, Cep, Phone máscara e validação

## Objetivo
Testes para inputs específicos do Brasil.

## Passos
1. Testar CnpjInput: máscara (00.000.000/0000-00), validação de CNPJ válido/inválido
2. Testar CpfInput: máscara (000.000.000-00), validação de CPF válido/inválido
3. Testar CepInput: máscara (00000-000), busca via API
4. Testar PhoneInput: máscara ((00) 00000-0000), validação

## Arquivos
- Criar: `src/XForge.DesignSystem.Tests/Components/XFBrazilianInputsFunctionalTests.cs`

## Critérios de Aceite
- [ ] Cada input: 4+ testes
- [ ] Máscara aplicada corretamente
- [ ] Validação de documento válido/inválida
- [ ] 0 failures
