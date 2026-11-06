# SPI Simulado — capítulo 09

**Estilo:** microsserviço · **Status:** planejado

- **Papel:** simula a liquidação do PIX no Banco Central, com falhas e lentidão configuráveis (chaos).
- **Por que existe:** dependência externa que falha de forma independente: é onde o circuit breaker se justifica.
- **Decisão de negócio pendente:** circuito aberto → PIX rejeitado ou `PENDENTE_LIQUIDACAO` com saldo bloqueado?
