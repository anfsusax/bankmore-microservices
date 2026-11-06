# DECISOES

Registro de decisoes importantes tomadas neste projeto.

Registrar no momento em que a decisao for tomada, nao ao encerrar a sessao.

---

## Quando Registrar

Registre toda decisao que afete arquitetura, escopo, tecnologia, fluxo de trabalho
ou qualquer coisa que impacte o futuro do projeto.

---

## Formato

```
## 2026-08-24 Titulo da Decisao
**Contexto:** por que foi necessaria
**Decisao:** o que foi decidido
**Impacto:** o que muda
```

---

## Registro De Decisoes

### 2026-08-29 — Transferência Financeira Atômica

**Contexto:** a transferência persistia débito, crédito, transferência e idempotência em chamadas separadas, permitindo inconsistência em caso de falha ou concorrência.
**Decisao:** concentrar a operação em uma porta transacional (`ITransferenciaFinanceira`) implementada por EF Core com isolamento serializável. O débito condicional e o crédito, movimentos, transferência e idempotência são confirmados em uma única transação.
**Impacto:** mensagens duplicadas retornam conflito sem nova cobrança; uma falha reverte todas as gravações. A validação de carga real permanece pendente.

### 2026-08-25 — Reformulação Visual com Paleta Suave de Banco Real
**Contexto:** o tema escuro anterior apresentava contraste excessivo e pouca aderência estética aos padrões de bancos digitais e corporativos reais do dia a dia.
**Decisao:** remodelar todo o design system no `app.css` e componentes Blazor para utilizar superfícies claras em tons suaves (Slate/Soft Pearl `#f8fafc` e `#f1f5f9`), cartões brancos com sombras suaves, detalhes em Royal Blue (`#2563eb`), cartão digital azul-marinho com chip metálico e tabelas financeiras limpas.
**Impacto:** a interface do BankMore agora oferece alta legibilidade, conforto visual e aparência idêntica a bancos digitais modernos (como Nubank, Inter, BTG e Revolut).

### 2026-08-24 — Criação da Interface Blazor Web App (BankMore.Web)

**Contexto:** necessidade do usuário por uma interface de banco digital intuitiva, moderna e responsiva para operar contas, depósitos, saques e saldos.
**Decisao:** criar o projeto `BankMore.Web` em Blazor (.NET 8) com suporte a componentes interativos (`InteractiveServer`), visual Glassmorphic com tema escuro e integração HTTP via `BankMoreApiClient`.
**Impacto:** os usuários agora possuem um portal bancário web completo na porta `5001` integrado ao Docker.

### 2026-08-24 — Migração de MySQL para PostgreSQL

**Contexto:** solicitação explícita do usuário para alterar a tecnologia de banco de dados do projeto de MySQL para PostgreSQL.
**Decisao:** substituir os pacotes MySQL por `Npgsql.EntityFrameworkCore.PostgreSQL` no .NET 8, atualizar `Program.cs` para suporte ao Npgsql e alterar a infraestrutura Docker para usar o container `postgres:16-alpine`.
**Impacto:** toda a persistência via Docker e EF Core agora utiliza PostgreSQL com o script de inicialização `sql/create.sql` convertido para a sintaxe Postgres.

### 2026-08-24 — Criacao Do Projeto

**Contexto:** projeto criado a partir do template da CENTRAL-ROBO.
**Decisao:** adotar a estrutura de memoria operacional padrao.
**Impacto:** todos os registros seguirao os formatos definidos em CONTRATO_DA_IA.md.

## 2026-11-06 Arquitetura híbrida: núcleo modular + microsserviços nas bordas
**Contexto:** o laboratório precisa ensinar a migrar E a não migrar, como numa empresa real.
**Decisão:** Contas, ledger e transferência interna ficam num monólito modular; Identity, Tarifas, Notificações, PIX, SPI Simulado e Antifraude viram microsserviços, via Strangler Fig. Ver docs/adr/ADR-001.
**Impacto:** a solution passa a ter os dois estilos; cada escolha é explicada no Livro do Laboratório (/livro no BankMore.Web).

## 2026-11-06 Conta vinculada ao titular por ID, sem FK
**Contexto:** a conta era localizada pelo nome do usuário e o ID do usuário era usado como ID da conta.
**Decisão:** coluna contacorrente.idusuario com índice único e sem FK (Identity sairá com banco próprio). Ver docs/adr/ADR-002.
**Impacto:** contas legadas sem idusuario ficam inacessíveis pela API até serem vinculadas.

## 2026-11-06 Fase 0: operações financeiras atômicas com READ COMMITTED + ordem de lock
**Contexto:** saque tinha race condition (double-spend); transferências cruzadas simultâneas geravam deadlock (24 de 40 com HTTP 500 no teste).
**Decisão:** UPDATE condicional + movimento + idempotência na mesma transação; READ COMMITTED no lugar de SERIALIZABLE; EnableRetryOnFailure; linhas travadas sempre em ordem crescente de Id.
**Impacto:** k6 com 50 saques simultâneos aceita exatamente 10 e o saldo fecha em zero; 40/40 transferências cruzadas aceitas sem erro.

## 2026-11-06 Migração para .NET 10
**Contexto:** alinhar com o laboratório microservico-senior e com a versão atual.
**Decisão:** todos os projetos em net10.0; Swashbuckle 10 (Microsoft.OpenApi 2); pin de System.Security.Cryptography.Pkcs 10.0.14 por vulnerabilidade transitiva.
**Impacto:** é preciso o SDK .NET 10 para compilar; as imagens Docker passaram para 10.0.
