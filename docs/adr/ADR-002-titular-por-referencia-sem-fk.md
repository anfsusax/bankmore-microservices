# ADR-002 — Conta referencia o titular por ID, sem chave estrangeira

- **Status:** Aceito
- **Data:** 2026-11-06

## Contexto
A conta corrente não tinha vínculo com o usuário. O login achava a conta **pelo nome** do usuário
(dois homônimos enxergavam a conta um do outro) e os handlers tratavam o ID do usuário como ID da conta.

## Decisão
- `contacorrente.idusuario` (UUID, nulo só para contas legadas) com índice **único**: uma conta por titular.
- **Sem FK** para `usuarios`, mesmo estando no mesmo banco hoje.
- A Application recebe o usuário autenticado por `IUsuarioAtual`; a conta é sempre resolvida a partir dele.

## Por que sem FK
O Identity será extraído como microsserviço com banco próprio (cap. 07). FK entre contextos é
acoplamento de dados: impede a separação. A integridade passa a ser garantida pela aplicação
(o titular vem de um token válido emitido pelo Identity).

## Consequências
- (+) Fecha a falha de acesso a conta de terceiros (BOLA).
- (+) Prepara a extração do Identity sem migração de dados.
- (−) Contas legadas (sem `idusuario`) ficam inacessíveis pela API até serem vinculadas manualmente.
- (−) Sem FK, um usuário apagado deixa conta órfã: precisará de evento `UsuarioEncerrado` (cap. 08).
