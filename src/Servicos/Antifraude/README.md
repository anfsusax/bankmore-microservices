# Antifraude — capítulo 10

**Estilo:** microsserviço · **Status:** planejado

- **Papel:** analisa o PIX (valor, horário, conta nova) e aprova, recusa ou pede revisão.
- **Decisão de negócio pendente:** se o antifraude cair, fail-closed (bloqueia tudo) ou fail-open até um limite de valor?
