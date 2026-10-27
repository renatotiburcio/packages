# manual-sync-rules

Toda mudanca significativa no Engineer DEVE ser verificada contra o manual.

## Gatilhos

Apos QUALQUER uma destas acoes, verificar se o manual precisa de atualizacao:

- Novo skill adicionado ou enriquecido
- Novo agent criado ou modificado
- Novo command criado ou modificado
- Regra adicionada ou alterada
- Configuracao mudada (kilo.jsonc, xforge-engineer.config.json)
- Estrutura de diretorios alterada
- Script de automacao modificado ou criado
- Provider adicionado ou removido
- Feature de seguranca implementada
- Bug fix que afeta comportamento documentado
- Migracao ou upgrade realizada
- Novo caso de uso descoberto

## Procedimento

1. Identificar o que mudou
2. Ler `docs/index.html` e verificar se a secao afetada existe
3. Se a secao existe -> atualizar com a mudanca
4. Se a secao nao existe -> criar nova secao
5. Verificar se a mudanca afeta multiplas secoes (atualizar todas)
6. Manter consistencia: mesmos exemplos, mesmos formatos, mesma terminologia

## Regras de Formatacao do Manual

- Cada secao DEVE ter titulo claro e descricao
- Cada componente DEVE ter "quando usar" + "exemplo"
- Cada comando DEVE ter prompt + resultado esperado
- Cada caso de uso DEVE ter: cenario, prompt, o que acontece, resultado
- Usar `code` blocks para comandos e prompts
- Manter sidebar sincronizada com secoes

## O que NAO documentar

- Alteracoes internas de implementacao sem impacto no usuario
- Fixes de encoding ou BOM
- Renomeacoes de scripts internos
- Mudancas em testes

## Validacao

O doctor.ps1 ja verifica se `docs/index.html` existe. Apos atualizar o manual, rodar:

```powershell
.\.kilo\automation\scripts\doctor.ps1
```
