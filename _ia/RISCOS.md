# RISCOS

Registro de riscos identificados neste projeto.

Atualizar quando um risco for identificado, mitigado ou resolvido.
Nao remova riscos resolvidos — mova-os para a secao correta.

---

## Formato

```
## [STATUS] Titulo do Risco
**Descricao:** o que pode acontecer
**Impacto:** o que seria afetado
**Mitigacao:** o que foi decidido
**Atualizado em:** 2026-08-24
```

Status: [ATIVO] | [MITIGADO] | [RESOLVIDO]

---

## Riscos Ativos

### [ATIVO] Objetivo Do Projeto Nao Definido

**Descricao:** o projeto ainda nao tem objetivo real declarado.
**Impacto:** qualquer IA que abra o projeto nao consegue entender o que construir.
**Mitigacao:** definir objetivo antes de qualquer execucao tecnica.
**Atualizado em:** 2026-08-24

### [ATIVO] Atomicidade Ainda Não Exercitada Em Banco Real Sob Concorrência

**Descricao:** a implementação compila e possui testes unitários de orquestração, mas ainda não foi executada contra PostgreSQL ou SQL Server com várias transferências simultâneas.
**Impacto:** diferenças de lock, serialização ou tratamento de colisão podem aparecer somente em integração.
**Mitigacao:** criar cenário de integração com saldo finito, chaves duplicadas e falhas injetadas antes de usar o fluxo como base do laboratório de pagamentos.
**Atualizado em:** 2026-08-29

---

## Riscos Mitigados

### [MITIGADO] Transferência Parcial ou Saldo Divergente

**Descricao:** débito, crédito, transferência e chave de idempotência eram persistidos separadamente; transferências não atualizavam a projeção de saldo.
**Impacto:** falhas ou concorrência podiam deixar dados financeiros inconsistentes.
**Mitigacao:** `TransferenciaFinanceiraEfCore` executa débito condicional, crédito, movimentos, transferência e idempotência em uma única transação serializável.
**Atualizado em:** 2026-08-29

---

## Riscos Resolvidos

Nenhum.
