# Tarifas — capítulo 08

**Estilo:** microsserviço (worker) · **Status:** planejado

- **Papel:** cobra tarifa de transferência/saque conforme regras do pacote do cliente.
- **Por que é microsserviço:** reage a evento; se cair, a transferência não pode falhar junto.
- **Consome:** `TransferenciaRealizada`, `SaqueRealizado` (via Outbox → Kafka).
- **Garantia:** entrega at-least-once → inbox para deduplicar por id do evento.
- **Critério de pronto:** derrubar o relay e a tarifa ser cobrada exatamente uma vez.
