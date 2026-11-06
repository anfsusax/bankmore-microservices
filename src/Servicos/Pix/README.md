# PIX — capítulo 10

**Estilo:** microsserviço · **Status:** planejado

- **Papel:** orquestra a saga do PIX externo: Bloquear saldo → Antifraude → Liquidar no SPI → Efetivar ou Estornar.
- **Por que saga aqui (e não na transferência interna):** a liquidação depende de outro sistema; não existe transação ACID que cubra os dois.
- **Estado persistido:** `INICIADA, BLOQUEADA, EM_ANALISE, LIQUIDADA, ESTORNADA, FALHA_COMPENSACAO`.
- **Idempotência:** EndToEndId.
- **Critério de pronto:** teste de cada caminho de compensação + reconciliação de `FALHA_COMPENSACAO`.
