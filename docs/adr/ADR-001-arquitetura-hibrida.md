# ADR-001 — Arquitetura híbrida: núcleo modular + microsserviços nas bordas

- **Status:** Aceito
- **Data:** 2026-11-06

## Contexto
O BankMore é um monólito (`BankMore.Auth.API`) com usuários, contas, movimentos e transferências no mesmo
deploy e no mesmo banco. O objetivo do laboratório é saber responder, como numa empresa real:
*"migramos para microsserviços ou não?"* — e defender as duas respostas.

## Decisão
1. **Contas, ledger e transferência interna ficam juntos, num monólito modular.**
   A transferência interna debita, credita, grava movimentos e idempotência numa **única transação ACID**.
   Separar Contas de Transferências em serviços com bancos próprios trocaria essa garantia por uma saga
   (consistência eventual + compensação) sem ganho de negócio.
2. **Viram microsserviços apenas as capacidades com motivo concreto:**
   - Identity: fronteira clara e ritmo de mudança diferente;
   - Tarifas/Notificações: reagem a eventos e não podem travar o fluxo principal;
   - PIX/SPI/Antifraude: dependem de sistema externo que falha de forma independente.
3. A migração é incremental (**Strangler Fig**), com um gateway na frente desde o início.

## Consequências
- (+) Sem saga onde uma transação local resolve; menos dinheiro "no limbo".
- (+) O laboratório demonstra os dois estilos e o critério de escolha.
- (−) O núcleo continua sendo um deploy único; escalar só "Transferências" não é possível.
- (−) Operação mais complexa (Kafka, gateway, vários serviços) nas bordas.

## Alternativas descartadas
- **Tudo em microsserviços:** saga em transferência interna; mais falhas parciais para tratar.
- **Tudo no monólito:** não exercita integração externa, resiliência nem mensageria, que o mercado cobra.
