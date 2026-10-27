# Architecture Testing Standard

## Objetivo

Garantir que Clean Architecture, DDD, CQRS, Endpoints e dependências sigam as regras definidas.

## Ferramentas sugeridas

- ArchUnitNET
- NetArchTest

## Regras que devem ser testadas

- Domain não depende de Infrastructure;
- Application não depende de Presentation;
- Endpoints não acessam DbContext diretamente;
- Handlers não dependem de endpoints;
- Commands não retornam entidades de domínio diretamente;
- DTOs não substituem entidades;
- MediatR original não deve ser usado;
- Mapster não deve ser usado;
- XForge.MediatR deve ser usado;
- AutoMapper deve ser usado.

## Quality Gate

Falha de arquitetura deve bloquear PR.
