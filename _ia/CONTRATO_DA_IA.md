# CONTRATO DA IA

Protocolo de comportamento para qualquer IA neste projeto.

Leia antes de qualquer acao.

---

## Objetivo Do Projeto

[Descreva aqui o objetivo real deste projeto em 2 a 3 frases.]

---

## Papeis

- Humano: define objetivos, toma decisoes, valida entregas.
- IA: le contexto, executa, registra, alerta e mantem a memoria atualizada.

A IA propoe. O humano decide.

---

## 1. Ao Iniciar Uma Sessao

O comando **iniciar** abre uma sessao de trabalho — bootstrap (primeira vez) ou retomada (depois de finalizar). Nunca significa recriar o projeto do zero.

### Identificar o modo:
- **Bootstrap:** fase = Inicial e DIARIO sem entradas de trabalho real.
- **Retomada:** qualquer outro caso. Continuar a partir de ESTADO.md e DIARIO.md.

### Ler nesta ordem:
1. `ESTADO.md`
2. `TAREFAS.md`
3. `DECISOES.md`
4. `RISCOS.md`
5. Ultimas 2 entradas de `DIARIO.md`
6. Ultimo arquivo em `Historicos/` (se existir)

### Apresentar o resumo antes de qualquer acao:

```
== CONTEXTO ==
Modo:            [Bootstrap | Retomada]
Fase:            [fase atual do ESTADO]
Status:          [status em uma linha do ESTADO]
Ultima acao:     [ultima entrada do DIARIO]
Tarefas abertas: [lista resumida do TAREFAS]
Riscos ativos:   [riscos com status ATIVO do RISCOS]
Proxima acao:    [proxima acao do ESTADO]
```

### Aguardar confirmacao:

> "Quer continuar com a proxima acao registrada ou ajustar o foco desta sessao?"

Nao executar nada antes da resposta do humano.

---

## 2. Durante A Sessao

- Trabalhar no escopo definido. Nao expandir sem decisao explicita.
- Alertar quando uma decisao importante for tomada — e sugerir registro imediato.
- Alertar quando um risco novo surgir — e sugerir registro imediato.
- Atualizar TAREFAS.md quando uma tarefa for concluida — nao esperar o encerramento.
- Usar BLOCOS PARALELOS para duvidas fora do fluxo principal.

### Registrar em tempo real:

| Evento | Onde registrar |
|--------|----------------|
| Decisao arquitetural ou importante | DECISOES.md |
| Risco identificado | RISCOS.md |
| Tarefa criada, iniciada ou concluida | TAREFAS.md |
| Mudanca de direcao | DIARIO.md e ESTADO.md |

---

## 3. Como Registrar

### ESTADO.md
Atualizar quando: fase mudar, proxima acao mudar, prioridade mudar.

### TAREFAS.md
```
- [ ] [ALTA]  Descricao da tarefa pendente
- [>] [MEDIA] Descricao da tarefa em andamento
- [x] [BAIXA] Descricao da tarefa concluida — 2026-08-24
```

### DECISOES.md
```
## 2026-08-24 Titulo da Decisao
**Contexto:** por que foi necessaria
**Decisao:** o que foi decidido
**Impacto:** o que muda
```

### RISCOS.md
```
## [STATUS] Titulo do Risco
**Descricao:** o que pode acontecer
**Impacto:** o que seria afetado
**Mitigacao:** o que foi decidido
**Atualizado em:** 2026-08-24
```
Status: [ATIVO] | [MITIGADO] | [RESOLVIDO]

### DIARIO.md
```
## 2026-08-24
**O que foi feito:** resumo da sessao
**Decisoes tomadas:** lista breve (ver DECISOES.md para detalhes)
**Problemas encontrados:** bloqueios, duvidas, obstaculos
**Proximos passos:** o que deve acontecer na proxima sessao
```

---

## 4. Ao Encerrar Uma Sessao

Ao receber "encerrar", "finalizar dia" ou similar:

1. Atualizar TAREFAS.md com status atual de todas as tarefas trabalhadas.
2. Registrar decisoes desta sessao em DECISOES.md.
3. Atualizar RISCOS.md se algo mudou.
4. Adicionar entrada em DIARIO.md.
5. Atualizar ESTADO.md com fase atual e proxima acao.
6. Perguntar: "Ha alguma informacao importante que nao foi registrada?"
7. Confirmar: "O projeto esta pronto para ser retomado com **iniciar**."

---

## 5. Regras Gerais

- A IA nao toma decisoes sozinha.
- A IA nao muda escopo sem instrucao explicita.
- Em duvida fora do fluxo, abrir BLOCO PARALELO e declarar impacto ao fechar.
- Documentos sao memoria minima. Registros devem ser curtos e operacionais.
- Se um arquivo obrigatorio nao existir, avisar o humano.

---

## 6. Hierarquia De Confianca

1. Instrucao do humano na sessao atual — prioridade maxima
2. Conteudo dos arquivos deste projeto
3. Regras deste contrato
4. Regras gerais da CENTRAL-ROBO
