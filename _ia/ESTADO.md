# ESTADO

Estado atual deste projeto.

Atualizar ao encerrar cada sessao ou quando fase, prioridade ou proxima acao mudarem.
Nao use para registrar o que foi feito — isso vai no DIARIO.md.

---

## Nome Do Projeto

BankMore

## Fase Atual

Fase 0 concluída (estabilizar antes de migrar). Próximo: cap. 01–02 do livro e reorganização do núcleo em monólito modular.

## Prioridade

Alta

## Status Em Uma Linha

.NET 10; rotas financeiras autenticadas; saldo com fonte única; depósito, saque e transferência atômicos com idempotência (replay/conflito); Livro do Laboratório em /livro com o capítulo 00 escrito.

## Proxima Acao

Escrever os capítulos 01 (por que migrar) e 02 (mapa de bounded contexts) e iniciar o cap. 03: separar o núcleo em módulos Contas e Transferências.

## Ferramenta Recomendada

Cowork / Cursor para código; k6 para provas de concorrência (tests/carga).

## Ultimo Historico Registrado

Historicos/HISTORICO_001.md

## O Que Nao Fazer Agora

- Nao criar projetos vazios para os microsserviços: cada um nasce no seu capítulo, com teste.
- Nao transformar a transferência interna em saga (ADR-001).
- Nao integrar pagamentos externos ou dinheiro real; o BankMore permanece uma simulação educacional.
