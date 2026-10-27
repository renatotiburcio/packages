---
id: T074
title: Resources .resx per-component pattern
priority: P1
status: pending
estimated_time: 8h
dependencies: T005
origin: Havit
---

# T074: Resources .resx per-component pattern

## Objetivo
Implementar pattern de Resources .resx per-component (Havit: 14 components x 6 langs = 84 files) para localizacao. Adaptar para 3 linguas LATAM (pt-BR/en/es).

## Passos
1. Criar pasta src/XForge.DesignSystem/Components/Forms/Resources/
2. Criar XFInputText.pt-BR.resx, XFInputText.en.resx, XFInputText.es.resx (placeholder strings)
3. Criar XFInputDate.pt-BR.resx, etc para top 5 inputs (Text, Number, Date, Select, MultiSelect)
4. Criar global Resources/Strings.pt-BR.resx com 30+ strings comuns
5. Configurar .csproj: <EmbeddedResource> para todos *.resx
6. Adicionar public static class Strings com 30+ ResourceManager.GetString() helpers
7. Padronizar acesso: [Required, Display(Name = nameof(Strings.SomeString))] nos models

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Forms/Resources/XFInputText.pt-BR.resx (+ en, es)
- Criar: src/XForge.DesignSystem/Components/Forms/Resources/XFInputDate.pt-BR.resx (+ en, es)
- Criar: src/XForge.DesignSystem/Components/Forms/Resources/XFInputNumber.pt-BR.resx (+ en, es)
- Criar: src/XForge.DesignSystem/Components/Forms/Resources/XFSelect.pt-BR.resx (+ en, es)
- Criar: src/XForge.DesignSystem/Components/Forms/Resources/XFMultiSelect.pt-BR.resx (+ en, es)
- Criar: src/XForge.DesignSystem/Resources/Strings.pt-BR.resx (+ en, es)
- Criar: src/XForge.DesignSystem/Resources/Strings.Designer.cs
- Modificar: src/XForge.DesignSystem/XForge.DesignSystem.csproj (EmbeddedResource)

## Criterios de Aceite
- [ ] 15+ .resx files (5 components x 3 langs) gerados
- [ ] Strings.Designer.cs com type-safe access
- [ ] Build 0 warnings, .resx embedded como resources
- [ ] Documentacao: como adicionar nova string em 3 linguas
- [ ] Testes: troca de culture mostra strings corretas
