# XForge Golden Rules

## Regra de ouro 1 — SOLID e SRP

Todo código deve respeitar SOLID.

SRP é obrigatório:

```text
cada arquivo deve ter uma única responsabilidade
```

## Proibido

- arquivo fazendo múltiplas responsabilidades;
- services gigantes;
- handlers com regra, persistência, validação e mapping misturados;
- Program.cs poluído;
- classes utilitárias genéricas sem coesão;
- duplicação de regra de negócio.

## Obrigatório

- separar responsabilidades;
- usar extensions para manter Program.cs limpo;
- separar validação, regra, persistência, mapping, endpoint e resposta;
- criar arquivos pequenos, coesos e testáveis;
- aplicar Clean Code.

---

## Regra de ouro 2 — Pacotes estáveis

Nunca usar versão preview, beta, rc, alpha, nightly ou experimental em projeto produtivo.

## Obrigatório

Antes de instalar ou atualizar pacotes:

1. verificar a última versão estável;
2. ignorar versões preview;
3. registrar a versão escolhida;
4. justificar caso uma versão não seja a última estável;
5. validar compatibilidade;
6. compilar sem warnings.

## Aplica-se também a pacotes fixados

Mesmo pacotes que pareçam já definidos devem ser verificados:

- EF Core;
- Pomelo.EntityFrameworkCore.MySql;
- Swashbuckle;
- QuestPDF;
- OpenTelemetry;
- Serilog;
- Tailwind;
- qualquer pacote NuGet ou npm.

---

## Regra de ouro 3 — XForge.MediatR obrigatório

Não usar MediatR.

Sempre usar:

```text
XForge.MediatR
```

Referência oficial:

```text
https://github.com/renatotiburcio/XForge.MediatR
```

## Proibido

- instalar MediatR;
- sugerir MediatR como alternativa;
- criar documentação com MediatR;
- usar patterns dependentes do MediatR oficial quando o projeto exige XForge.MediatR.

## Obrigatório

- usar XForge.MediatR para CQRS/mediator;
- documentar uso do XForge.MediatR;
- criar examples usando XForge.MediatR;
- adaptar handlers, commands, queries e pipelines para XForge.MediatR.


## Regra de ouro — AutoMapper obrigatório

- Não usar Mapster.
- Sempre usar AutoMapper.
- Validar última versão estável do AutoMapper.
- Mapping não deve conter regra de negócio.
- Profiles devem ser coesos e testáveis.
- Projetos existentes com Mapster devem gerar plano de migração para AutoMapper.
