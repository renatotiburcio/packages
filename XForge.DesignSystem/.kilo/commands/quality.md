---
description: Executa quality gates: build, testes, coverage, SOLID/SRP, lint, documentação.
agent: code
---

# /qualidade

## Categoria

validação

## Objetivo

Validar que o código atende padrões de qualidade antes de commit/PR/release.

## Procedimento

### 1. Build
```powershell
dotnet build --no-restore 2>&1
```
- Zero errors obrigatório
- Warnings documentados ou corrigidos

### 2. Testes
```powershell
dotnet test --no-build --verbosity normal
```
- Todos passando
- Coverage ≥ 85%

### 3. SOLID/SRP
- Verificar: 1 arquivo = 1 responsabilidade
- Verificar: sem services gigantes
- Verificar: handlers sem mistura de regra/persistência
- Verificar: Program.cs limpo (extensions)

### 4. Código
- Sem `TODO` sem issue associada
- Sem `Console.WriteLine` em produção
- Sem commented-out code
- Complexidade ciclomática < 10
- Naming consistente (PascalCase classes, camelCase vars)

### 5. Pacotes
- Nenhum preview/alpha/beta/rc
- Última versão estável
- Sem warnings de deprecated

### 6. Documentação
- README atualizado (se mudou setup/uso)
- CHANGELOG entry (se feature nova)
- XML docs em APIs públicas

## Saída

```json
{
  "build": "pass",
  "tests": "pass",
  "coverage": 87,
  "solid": "pass",
  "packages": "pass",
  "docs": "pass",
  "ready": true,
  "blockers": []
}
```

## Bloqueio

Se qualquer gate falhar → NÃO finalizar. Reportar bloqueio.

## Exemplos

```
/qualidade
/qualidade verificar cobertura do módulo FolhaPagamento
/qualidade auditar SOLID no projeto atual
```
