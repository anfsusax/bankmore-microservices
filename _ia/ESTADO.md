# ESTADO

Estado atual deste projeto.

Atualizar ao encerrar cada sessao ou quando fase, prioridade ou proxima acao mudarem.
Nao use para registrar o que foi feito — isso vai no DIARIO.md.

---

## Nome Do Projeto

BankMore

## Fase Atual

Desenvolvimento Frontend Blazor WebApp

## Prioridade

Alta

## Status Em Uma Linha

Aplicação .NET 8 com API, Blazor e operações bancárias; transferências agora usam uma transação única para saldo, movimentos, registro da transferência e idempotência.

## Proxima Acao

Executar teste de integração concorrente no PostgreSQL ou SQL Server para confirmar saldo, idempotência e rollback de transferências sob carga.

## Ferramenta Recomendada

Cursor / Antigravity para testes e refinamentos visuais.

## Ultimo Historico Registrado

Historicos/HISTORICO_001.md

## O Que Nao Fazer Agora

- Nao alterar rotas da API sem necessidade.
- Nao integrar pagamentos externos ou dinheiro real; o BankMore permanece uma simulação educacional.
