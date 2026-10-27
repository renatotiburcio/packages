---
description: Valida se todos os pacotes NuGet/npm usam versões estáveis e não preview.
agent: code
---

# /validar-pacotes-estaveis

## Objetivo

Verificar pacotes do projeto.

## Deve fazer

1. Listar pacotes NuGet.
2. Listar pacotes npm.
3. Detectar preview/beta/rc/alpha/nightly.
4. Verificar última versão estável disponível.
5. Sugerir atualização quando seguro.
6. Validar compatibilidade.
7. Gerar relatório.
8. Atualizar memória.

## Proibido

- aceitar pacote preview;
- suprimir warnings;
- atualizar sem validar build/testes.

## Exemplos

```
/validar-pacotes-estaveis
/validar-pacotes-estaveis conformidade com padroes do projeto
```
